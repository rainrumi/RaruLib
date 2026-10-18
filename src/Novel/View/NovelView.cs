using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
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

        [SerializeField, Tooltip("背景の黒下地と画像をまとめた常設オブジェクト")]
        private GameObject _backgroundRoot; // 背景全体
        [SerializeField, Tooltip("全画面の背景画像")]
        private UnityEngine.UI.Image _backgroundImage; // 背景画像
        [SerializeField, Tooltip("管理番号0・1・2の順に設定する立ち絵画像")]
        private UnityEngine.UI.Image[] _standImages = new UnityEngine.UI.Image[3]; // 立ち絵枠
        private INovelInitializeStatus _initializeStatus; // 演出設定
        private readonly Vector2[] _standOrigins = new Vector2[3]; // 定位置
        private readonly Vector2[] _standPositions = new Vector2[3]; // 左中央右
        private readonly Vector3[] _standScales = new Vector3[3]; // 基準倍率

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
            _initializeStatus = initializeStatus;
            // Prefabの左中央右と基準倍率を保持
            for (int id = 0; id < _standImages.Length; id++)
            {
                _standPositions[id] = _standImages[id].rectTransform.anchoredPosition;
                _standOrigins[id] = _standPositions[id];
                _standScales[id] = _standImages[id].rectTransform.localScale;
            }
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

            GameObject textBoxRoot = nameTxt.transform.parent.gameObject;
            textBoxRoot.SetActive(true);
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

            textBoxRoot.SetActive(false);
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

        /// <summary>旧背景を消して画像を差し替え、新背景の表示完了を待つ。</summary>
        // 背景を切替
        public async UniTask ShowBackgroundAsync(string path, float fadeOut, float fadeIn, CancellationToken ct)
        {
            // 表示先の寿命を連結
            CancellationToken destroyToken = this.GetCancellationTokenOnDestroy();
            using CancellationTokenSource linked = CancellationTokenSource.CreateLinkedTokenSource(ct, destroyToken);
            linked.Token.ThrowIfCancellationRequested();
            Sprite sprite = LoadSprite(path); // 次の画像
            bool hasBackground = _backgroundRoot.activeSelf; // 初回判定
            _backgroundRoot.SetActive(true);
            try
            {
                if (hasBackground && fadeOut > 0.0f)
                {
                    await AwaitImageTweenAsync(_backgroundImage.DOFade(0.0f, fadeOut).SetEase(Ease.Linear), linked.Token);
                }
                _backgroundImage.sprite = sprite;
                SetImageAlpha(_backgroundImage, fadeIn > 0.0f ? 0.0f : 1.0f);
                if (fadeIn > 0.0f)
                {
                    await AwaitImageTweenAsync(_backgroundImage.DOFade(1.0f, fadeIn).SetEase(Ease.Linear), linked.Token);
                }
            }
            finally
            {
                // 取消時も中間の透明度を残さない
                if (!destroyToken.IsCancellationRequested)
                {
                    _backgroundImage.sprite = sprite;
                    SetImageAlpha(_backgroundImage, 1.0f);
                }
            }
        }

        /// <summary>指定枠の画像を差し替え、登場演出の完了を待つ。</summary>
        // 立ち絵を表示
        public async UniTask ShowStandAsync(int id, string path, string position, string effect, float duration, CancellationToken ct)
        {
            // 表示先の寿命を連結
            CancellationToken destroyToken = this.GetCancellationTokenOnDestroy();
            using CancellationTokenSource linked = CancellationTokenSource.CreateLinkedTokenSource(ct, destroyToken);
            linked.Token.ThrowIfCancellationRequested();
            UnityEngine.UI.Image image = _standImages[id]; // 対象画像
            RectTransform rect = image.rectTransform; // 表示姿勢
            Sprite sprite = LoadSprite(path); // 指定差分
            _standOrigins[id] = GetStandPosition(id, position);
            image.sprite = sprite;
            image.gameObject.SetActive(true);
            rect.anchoredPosition = _standOrigins[id];
            rect.localScale = _standScales[id];
            SetImageAlpha(image, 1.0f);
            try
            {
                if (duration <= 0.0f || effect == "none")
                {
                    return;
                }
                Tween tween; // 登場演出
                switch (effect)
                {
                    case "fade":
                        SetImageAlpha(image, 0.0f);
                        tween = image.DOFade(1.0f, duration).SetEase(Ease.Linear);
                        break;
                    case "slide":
                        // 表示位置に応じた方向から登場
                        Vector2 direction = _standOrigins[id].x < _standPositions[1].x ? Vector2.left :
                            _standOrigins[id].x > _standPositions[1].x ? Vector2.right : Vector2.down;
                        rect.anchoredPosition += direction * _initializeStatus.StandSlideDistance;
                        tween = rect.DOAnchorPos(_standOrigins[id], duration).SetEase(Ease.OutQuad);
                        break;
                    case "pop":
                        rect.localScale = Vector3.zero;
                        tween = rect.DOScale(_standScales[id], duration).SetEase(Ease.OutBack);
                        break;
                    default:
                        throw new NotSupportedException($"立ち絵の表示演出に未対応です: {effect}");
                }
                await AwaitImageTweenAsync(tween, linked.Token);
            }
            finally
            {
                // 取消時も指定差分を定位置へ確定
                if (!destroyToken.IsCancellationRequested)
                {
                    rect.anchoredPosition = _standOrigins[id];
                    rect.localScale = _standScales[id];
                    SetImageAlpha(image, 1.0f);
                }
            }
        }

        /// <summary>指定枠をフェードアウトし、非表示まで待つ。</summary>
        // 立ち絵を退場
        public async UniTask HideStandAsync(int id, float duration, CancellationToken ct)
        {
            // 表示先の寿命を連結
            CancellationToken destroyToken = this.GetCancellationTokenOnDestroy();
            using CancellationTokenSource linked = CancellationTokenSource.CreateLinkedTokenSource(ct, destroyToken);
            linked.Token.ThrowIfCancellationRequested();
            UnityEngine.UI.Image image = _standImages[id]; // 対象画像
            if (!image.gameObject.activeSelf)
            {
                return;
            }
            try
            {
                if (duration > 0.0f)
                {
                    await AwaitImageTweenAsync(image.DOFade(0.0f, duration).SetEase(Ease.Linear), linked.Token);
                }
            }
            finally
            {
                // 次の登場に透明度を持ち越さない
                if (!destroyToken.IsCancellationRequested)
                {
                    image.gameObject.SetActive(false);
                    SetImageAlpha(image, 1.0f);
                }
            }
        }

        /// <summary>表示中の立ち絵へ短い動作を加え、終了時に定位置へ戻す。</summary>
        // 立ち絵を動作
        public async UniTask PlayStandEffectAsync(int id, string effect, float duration, CancellationToken ct)
        {
            // 表示先の寿命を連結
            CancellationToken destroyToken = this.GetCancellationTokenOnDestroy();
            using CancellationTokenSource linked = CancellationTokenSource.CreateLinkedTokenSource(ct, destroyToken);
            linked.Token.ThrowIfCancellationRequested();
            UnityEngine.UI.Image image = _standImages[id]; // 対象画像
            if (!image.gameObject.activeSelf || duration <= 0.0f)
            {
                return;
            }
            RectTransform rect = image.rectTransform; // 表示姿勢
            Vector2 origin = _standOrigins[id]; // 復帰位置
            try
            {
                // 時間を往復の各区間へ均等配分
                Tween tween = effect switch
                {
                    "jump" => DOTween.Sequence()
                        .Append(rect.DOAnchorPos(origin + Vector2.up * _initializeStatus.StandJumpHeight, duration / 2.0f).SetEase(Ease.OutQuad))
                        .Append(rect.DOAnchorPos(origin, duration / 2.0f).SetEase(Ease.InQuad)),
                    "shake" => DOTween.Sequence()
                        .Append(rect.DOAnchorPos(origin + Vector2.right * _initializeStatus.StandShakeDistance, duration / 4.0f).SetEase(Ease.Linear))
                        .Append(rect.DOAnchorPos(origin + Vector2.left * _initializeStatus.StandShakeDistance, duration / 4.0f).SetEase(Ease.Linear))
                        .Append(rect.DOAnchorPos(origin + Vector2.right * _initializeStatus.StandShakeDistance, duration / 4.0f).SetEase(Ease.Linear))
                        .Append(rect.DOAnchorPos(origin, duration / 4.0f).SetEase(Ease.Linear)),
                    _ => throw new NotSupportedException($"立ち絵の動作に未対応です: {effect}")
                };
                await AwaitImageTweenAsync(tween, linked.Token);
            }
            finally
            {
                // 取消でも定位置へ復帰
                if (!destroyToken.IsCancellationRequested)
                {
                    rect.anchoredPosition = origin;
                }
            }
        }

        // 表示枠を解決
        private Vector2 GetStandPosition(int id, string position)
        {
            return position switch
            {
                "keep" => _standOrigins[id],
                "left" => _standPositions[0],
                "center" => _standPositions[1],
                "right" => _standPositions[2],
                _ => throw new NotSupportedException($"立ち絵の表示位置に未対応です: {position}")
            };
        }

        // 単一Spriteを取得
        private static Sprite LoadSprite(string path)
        {
            return Resources.LoadAll<Sprite>(path).Single();
        }

        // 透明度だけを更新
        private static void SetImageAlpha(UnityEngine.UI.Image image, float alpha)
        {
            Color color = image.color; // 元の色
            color.a = alpha;
            image.color = color;
        }

        // 停止責務を待機へ委譲
        private async UniTask AwaitImageTweenAsync(Tween tween, CancellationToken ct)
        {
            await tween.SetUpdate(true)
                .SetAutoKill(true)
                .SetRecyclable(false)
                .SetTarget(this)
                .SetLink(gameObject, LinkBehaviour.KillOnDestroy)
                .ToUniTask(TweenCancelBehaviour.KillAndCancelAwait, ct);
        }

    }
}