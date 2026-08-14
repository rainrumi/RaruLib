using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using VContainer;
using UnityEngine.InputSystem;
using RaruLib;

namespace RaruLib
{
    public class NovelData
    {
        public float _charSpeed;
        public float _maxCharWait;
        public float _autoWait;
        public float _autoWaitPerChar;
        public bool _isAuto;
        public string _popopo;

        public float CharWait => _maxCharWait * (1 - System.MathF.Min(1, _charSpeed));

        public NovelData(
            float charSpeed, float maxCharWait, float autoWait, float autoWaitPerChar, bool isAuto, string popopo)
        {
            _charSpeed = charSpeed;
            _maxCharWait = maxCharWait;
            _autoWait = autoWait;
            _autoWaitPerChar = autoWaitPerChar;
            _isAuto = isAuto;
            _popopo = popopo;
        }
    }

    public class NovelView : MonoBehaviour, INovelView
    {
        // ベースUI
        [SerializeField] private TMP_Text nameTxt;
        [SerializeField] private TMP_Text bodyTxt;
        // 音用コマンド
        [SerializeField] private SoundCommand soundCmd;

        private NovelData _data;
        private bool isTyping;
        private CancellationTokenSource typingCts;

        [Inject]
        public void Construct(INovelInitializeStatus initializeStatus)
        {
            _data = new NovelData(
                initializeStatus.CharSpeed,
                initializeStatus.MaxCharWait,
                initializeStatus.AutoWait,
                initializeStatus.AutoWaitPerChar,
                initializeStatus.IsAuto,
                initializeStatus.Popopo);
        }

        private void Start()
        {
            isTyping = false;
        }

        private void Update()
        {
            if (Pointer.current?.press.wasPressedThisFrame != true &&
                    Mouse.current?.rightButton.wasPressedThisFrame != true)
            {
                return;
            }

            if (isTyping)
            {
                typingCts?.Cancel();
            }
        }

        // ダイアログ表示
        public async UniTask ShowDialogAsync(string name, string text, CancellationToken _ct)
        {
            using CancellationTokenSource linkedTokenSource =
                CancellationTokenSource.CreateLinkedTokenSource(
                    _ct,
                    this.GetCancellationTokenOnDestroy());
            CancellationToken cancellationToken = linkedTokenSource.Token;
            cancellationToken.ThrowIfCancellationRequested();

            nameTxt.text = "";
            bodyTxt.text = "";

            nameTxt.gameObject.SetActive(true);
            bodyTxt.gameObject.SetActive(true);

            nameTxt.text = name;

            await TypeText(text, cancellationToken);

            if (!_data._isAuto)
            {
                await UniTask.WaitUntil(() =>
                    Pointer.current?.press.wasPressedThisFrame == true ||
                    Mouse.current?.rightButton.wasPressedThisFrame == true
                    , cancellationToken: cancellationToken);
            }
            else
            {
                // オート時の待機時間
                float autoWait = _data._autoWait + text.Length * _data._autoWaitPerChar;
                // 通常条件に加えて、オート時の待機時間終了でも進行
                await UniTask.WhenAny(
                    UniTask.WaitUntil(() =>
                        Pointer.current?.press.wasPressedThisFrame == true ||
                        Mouse.current?.rightButton.wasPressedThisFrame == true
                        , cancellationToken: cancellationToken),
                    UniTask.Delay(TimeSpan.FromSeconds(autoWait), cancellationToken: cancellationToken));
            }

            nameTxt.gameObject.SetActive(false);
            bodyTxt.gameObject.SetActive(false);
        }

        // 選択肢表示
        public async UniTask<int> ShowChoiceAsync(IReadOnlyList<string> options, CancellationToken _ct)
        {
            using CancellationTokenSource linkedTokenSource =
                CancellationTokenSource.CreateLinkedTokenSource(
                    _ct,
                    this.GetCancellationTokenOnDestroy());
            CancellationToken cancellationToken = linkedTokenSource.Token;

            Debug.Log("未設定。今度やる");
            await UniTask.WaitForSeconds(2.0f, cancellationToken: cancellationToken);
            throw new NotImplementedException("未実装～Choice UIは次に実装");
        }

        // タイプライター
        private async UniTask TypeText(string text, CancellationToken _ct)
        {
            // 実行中に新しいリクエストが来たら止める
            await UniTask.WaitUntil(() => !isTyping, cancellationToken: _ct);

            isTyping = true;
            using CancellationTokenSource currentTypingCts =
                CancellationTokenSource.CreateLinkedTokenSource(_ct);
            typingCts = currentTypingCts;

            bodyTxt.text = "";

            try
            {
                foreach (char _text in text)
                {
                    // 効果音があれば鳴らす
                    if (soundCmd != null && _data._popopo.Length > 0) soundCmd.CallPlaySE(_data._popopo);
                    // テキストの追加
                    bodyTxt.text += _text;
                    await UniTask.WaitForSeconds(_data.CharWait, cancellationToken: currentTypingCts.Token);
                }
            }
            catch (OperationCanceledException)
                when (!_ct.IsCancellationRequested && currentTypingCts.IsCancellationRequested)
            {
                bodyTxt.text = text;
            }
            finally
            {
                isTyping = false;
                if (ReferenceEquals(typingCts, currentTypingCts))
                {
                    typingCts = null;
                }
            }

            _ct.ThrowIfCancellationRequested();
        }
    }
}