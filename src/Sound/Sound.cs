using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

/***************************************************************
* 
* 使い方
* 前提１,このクラスはサウンド全般の管理を行います。
* 
* １,クラスをオブジェにアタッチ
* ２,soundGroupsの要素を音のカテゴリの数だけ作成（例：カテゴリをBGMとSEに分ける場合は２）
* ３,soundDatasの要素をカテゴリに設定する音の数だけ作成
* ４,音量変更、再生、停止したい音に設定したstringの名前をメソッドで呼び出す
* 
* 追記.何故これを書くのかは書きませんが、同じコンポーネントはひとつのオブジェに複数個アタッチできます。
* 
* ************************************************************/

public enum SoundKind
{
    BGM = 0,
    SE = 1,
    Master = 2
}

namespace RaruLib
{
    public class Sound : MonoBehaviour
    {
        public static Sound instance;
        public bool dontDestroy;    // 本スクリプトの非破壊化

        private bool _finishedInit = false; // 初期化が終了した
        public bool finishedInit => _finishedInit;

        [SerializeField] SoundGroup[] soundGroups;          // 音のカテゴリ
        private Dictionary<string, SoundGroup> groupDict;
        private float _masterVolume = 0.5f;  // マスターボリューム
        private readonly Dictionary<AudioSource, float> _fadeFactors = new(); // 音ごとのフェード係数
        private readonly Dictionary<AudioSource, Tween> _fadeTweens = new(); // 自身の音量Tween

        [Serializable]
        public class SoundGroup
        {
            public string groupName;        // カテゴリの名称
            [Range(0, 1)] public float volume = 0.5f;     // カテゴリの音量
            public SoundData[] soundDatas;  // カテゴリに入れる音
            public Dictionary<string, AudioSource> soundDict;

            [Serializable]
            public class SoundData
            {
                public string soundName;        // 音の名称
                public AudioSource audioSource; // 音のソース
            }
        }

        private void Awake()
        {
            //Debug.Log(instance);
            if (instance == null)
            {
                instance = this;
                if (dontDestroy)
                {
                    DontDestroyOnLoad(this);
                }
            }
            else
            {
#if UNITY_EDITOR                
                Debug.Log($"重複した{instance}を削除します");
#endif

                Destroy(instance.gameObject);
            }

            InitSoundGroup();

            foreach (var data in soundGroups)   // 音量の初期化
            { ChangeVolume(data.groupName, data.volume); }

            _finishedInit = true;   // 
        }

        // 全ての辞書の初期化と挿入
        private void InitSoundGroup()
        {
            groupDict = new Dictionary<string, SoundGroup>();

            foreach (var data in soundGroups)
            {
                groupDict.Add(data.groupName, data);

                data.soundDict = new Dictionary<string, AudioSource>();
                foreach (var dataSet in data.soundDatas)
                {
                    data.soundDict.Add(dataSet.soundName, dataSet.audioSource);
                    _fadeFactors[dataSet.audioSource] = 1.0f;
                }
            }
        }

        /***************************************************************
        * 音量取得
        ***************************************************************/
        /// <param name="groupName">カテゴリ</param>
        public float GetVolume(string groupName)
        {
            if (!groupDict.ContainsKey(groupName))
            {
#if UNITY_EDITOR
                Debug.Log($"{groupName}は存在しないカテゴリです");
#endif
                return 0; 
            }

            return groupDict[groupName].volume;
        }

        /***************************************************************
        * 音量変更
        ***************************************************************/
        /// <param name="groupName">カテゴリ</param>
        public void ChangeVolume(string groupName, float newVolume)
        {
            if (!groupDict.ContainsKey(groupName))
            { 
#if UNITY_EDITOR
                Debug.Log($"{groupName}は存在しないカテゴリです"); 
#endif                
                return; 
            }

            groupDict[groupName].volume = newVolume;
            foreach (var dataSet in groupDict[groupName].soundDict)
            { ApplyVolume(groupDict[groupName], dataSet.Value); }
        }

        /***************************************************************
        * 開始位置変更
        ***************************************************************/
        /// <param name="groupName">カテゴリ</param>
        /// <param name="soundName">音の名称</param>
        public void ChangeStartTime(string groupName, string soundName, float time)
        {
            if (!groupDict.ContainsKey(groupName))
            { 
#if UNITY_EDITOR
                Debug.Log($"{groupName}は存在しないカテゴリです"); 
#endif                
                return; 
            }
            if (!groupDict[groupName].soundDict.ContainsKey(soundName))
            { 
#if UNITY_EDITOR
                Debug.Log($"{soundName}は存在しない音源名です"); 
#endif                
                return; 
            }
            groupDict[groupName].soundDict[soundName].time = time;
        }

        /***************************************************************
        * 再生
        ***************************************************************/
        /// <param name="groupName">カテゴリ</param>
        /// <param name="soundName">音の名称</param>
        public void Play(string groupName, string soundName)
        {
            if (!groupDict.ContainsKey(groupName))
            {
#if UNITY_EDITOR                
                Debug.Log($"{groupName}は存在しないカテゴリです"); 
#endif                
                return; 
            }
            if (!groupDict[groupName].soundDict.ContainsKey(soundName))
            {
#if UNITY_EDITOR
                Debug.Log($"{soundName}は存在しない音源名です"); 
#endif                
                return; 
            }
            // フェードを解除して再生
            SoundGroup group = groupDict[groupName];
            AudioSource source = group.soundDict[soundName];
            CancelVolumeFade(source);
            _fadeFactors[source] = 1.0f;
            ApplyVolume(group, source);
            source.Play();
        }

        /***************************************************************
        * フェード再生
        ***************************************************************/
        /// <param name="groupName">カテゴリ</param>
        /// <param name="soundName">音の名称</param>
        public async UniTaskVoid Play(string groupName, string soundName, float duration)
        {
            if (!groupDict.ContainsKey(groupName))
            {
#if UNITY_EDITOR
                Debug.Log($"{groupName}は存在しないカテゴリです"); 
#endif                
                return; 
            }
            if (!groupDict[groupName].soundDict.ContainsKey(soundName))
            { 
#if UNITY_EDITOR
                Debug.Log($"{soundName}は存在しない音源名です"); 
#endif                
                return; 
            }
            if (groupDict[groupName].soundDict[soundName].isPlaying)
            { 
#if UNITY_EDITOR
                Debug.Log($"既に再生中です");
#endif
                return; 
            }

            // 音量係数をゼロから戻す
            SoundGroup group = groupDict[groupName];
            AudioSource source = group.soundDict[soundName];
            CancelVolumeFade(source);
            _fadeFactors[source] = 0.0f;
            ApplyVolume(group, source);
            CancellationToken destroyToken = this.GetCancellationTokenOnDestroy();
            try
            {
                await UniTask.Yield(cancellationToken: destroyToken);
            }
            catch (OperationCanceledException) when (destroyToken.IsCancellationRequested)
            {
                return;
            }
            if (this == null || source == null)
            {
                return;
            }
            source.Play();
            StartVolumeFade(group, source, 1.0f, duration);
        }

        /***************************************************************
        * 停止
        ***************************************************************/
        /// <param name="groupName">カテゴリ</param>
        /// <param name="soundName">音の名称</param>
        public void Stop(string groupName, string soundName)
        {
            if (!groupDict.ContainsKey(groupName))
            { 
#if UNITY_EDITOR
                Debug.Log($"{groupName}は存在しないカテゴリです"); 
#endif                
                return; }
            if (!groupDict[groupName].soundDict.ContainsKey(soundName))
            { 
#if UNITY_EDITOR
                Debug.Log($"{soundName}は存在しない音源名です"); 
#endif                
                return; 
            }
            if (this == null) return;

            // 停止後の音量を元へ戻す
            SoundGroup group = groupDict[groupName];
            AudioSource source = group.soundDict[soundName];
            CancelVolumeFade(source);
            source.Stop();
            _fadeFactors[source] = 1.0f;
            ApplyVolume(group, source);
        }

        /***************************************************************
        * フェード停止
        ***************************************************************/
        /// <param name="groupName">カテゴリ</param>
        /// <param name="soundName">音の名称</param>
        public async UniTaskVoid Stop(string groupName, string soundName, float duration)
        {
            if (!groupDict.ContainsKey(groupName))
            { 
#if UNITY_EDITOR
                Debug.Log($"{groupName}は存在しないカテゴリです"); 
#endif                
                return; }
            if (!groupDict[groupName].soundDict.ContainsKey(soundName))
            { 
#if UNITY_EDITOR
                Debug.Log($"{soundName}は存在しない音源名です"); 
#endif                
                return; 
            }

            // フェード中も現在の設定を使う
            SoundGroup group = groupDict[groupName];
            AudioSource source = group.soundDict[soundName];
            Tween fadeTween = StartVolumeFade(group, source, 0.0f, duration);
            CancellationToken destroyToken = this.GetCancellationTokenOnDestroy();
            try
            {
                await UniTask.WaitForSeconds(duration, cancellationToken: destroyToken);
            }
            catch (OperationCanceledException) when (destroyToken.IsCancellationRequested)
            {
                return;
            }
            if (this == null || source == null)
            {
                return;
            }
            // 新しい要求の再生は止めない
            if (!_fadeTweens.TryGetValue(source, out Tween currentTween) || currentTween != fadeTween)
            {
                return;
            }
            CancelVolumeFade(source);
            source.Stop();
            _fadeFactors[source] = 1.0f;
            ApplyVolume(group, source);
        }


        /// <summary>グループ音量とは独立したマスター音量を返す。</summary>
        public float GetMasterVolume() => _masterVolume;

        /// <summary>マスター音量を変更し、全ての音へ即時に反映する。</summary>
        public void ChangeMasterVolume(float value)
        {
            _masterVolume = Mathf.Clamp01(value);
            // 生のグループ音量は保持する
            foreach (SoundGroup group in groupDict.Values)
            {
                foreach (AudioSource source in group.soundDict.Values)
                {
                    ApplyVolume(group, source);
                }
            }
        }

        // 設定値と係数を合成
        private void ApplyVolume(SoundGroup group, AudioSource source)
        {
            source.volume = group.volume * _masterVolume * _fadeFactors[source];
        }

        // 自身の古いTweenを取消
        private void CancelVolumeFade(AudioSource source)
        {
            if (_fadeTweens.TryGetValue(source, out Tween tween))
            {
                tween.Kill();
                _fadeTweens.Remove(source);
            }
        }

        // 音ごとの係数をイージング
        private Tween StartVolumeFade(SoundGroup group, AudioSource source, float target, float duration)
        {
            CancelVolumeFade(source);
            Tween tween = DOTween.To(() => _fadeFactors[source], factor =>
            {
                if (this == null || source == null)
                {
                    return;
                }
                _fadeFactors[source] = factor;
                ApplyVolume(group, source);
            }, target, duration).SetTarget(source).SetLink(source.gameObject, LinkBehaviour.KillOnDestroy);
            _fadeTweens[source] = tween;
            return tween;
        }

        // 所有する音量Tweenを解放
        private void OnDestroy()
        {
            foreach (Tween tween in _fadeTweens.Values)
            {
                tween.Kill();
            }
            _fadeTweens.Clear();
            _fadeFactors.Clear();
        }

    }
}