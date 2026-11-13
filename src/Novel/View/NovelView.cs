using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        [SerializeField, Tooltip("全文表示後のクリック待ち案内")]
        private GameObject _clickWaitGlyph; // クリック待ち表示
        // 音用コマンド
        [SerializeField] private SoundCommand soundCmd;

        [SerializeField, Tooltip("背景の黒下地と画像をまとめた常設オブジェクト")]
        private GameObject _backgroundRoot; // 背景全体
        [SerializeField, Tooltip("全画面の背景画像")]
        private UnityEngine.UI.Image _backgroundImage; // 背景画像
        [SerializeField, Tooltip("管理番号0・1・2の順に設定する立ち絵画像")]
        private UnityEngine.UI.Image[] _standImages = new UnityEngine.UI.Image[3]; // 立ち絵枠
        [SerializeField, Tooltip("中央下の立ち絵表示位置")]
        private RectTransform _standCenterDownAnchor; // 中央下の位置
        [SerializeField, Tooltip("キャラクターごとの立ち絵設定")]
        private NovelStandInfo[] _standInfos = Array.Empty<NovelStandInfo>(); // 立ち絵の設定
        [SerializeField, Tooltip("立ち絵ID順のフェード用CanvasGroup")]
        private CanvasGroup[] _standFadeGroups = Array.Empty<CanvasGroup>(); // フェード
        [SerializeField, Tooltip("立ち絵ID順に3枚ずつ並べた差分Image")]
        private UnityEngine.UI.Image[] _standPartImages = Array.Empty<UnityEngine.UI.Image>(); // 差分表示枠
        private const int StandPartCapacity = 3;
        private readonly NovelStandInfo[] _activeStandInfos = new NovelStandInfo[3]; // 現在の素体
        private INovelInitializeStatus _initializeStatus; // 演出設定
        private readonly Vector2[] _standOrigins = new Vector2[3]; // 定位置
        private readonly Vector2[] _standPositions = new Vector2[4]; // 左中央右
        private readonly Vector3[] _standScales = new Vector3[3]; // 基準倍率

        [SerializeField, Tooltip("既読シナリオのスキップ操作")]
        private UnityEngine.UI.Button _skipButton; // スキップ操作

        [SerializeField, Tooltip("本文送りを止める画面のCanvasGroup。空なら除外なし")]
        private CanvasGroup[] _advanceInputBlockingGroups = Array.Empty<CanvasGroup>(); // 本文入力の停止UI
        [SerializeField, Tooltip("本文送りに使わないScreen Space Overlayの領域。空なら除外なし")]
        private RectTransform[] _advanceInputExcludedAreas = Array.Empty<RectTransform>(); // 本文入力の除外領域

        /// <summary>シナリオ全体のスキップ要求を接続するButton。</summary>
        public UnityEngine.UI.Button SkipButton => _skipButton;

        /// <summary>スキップ操作の表示と受付を切り替える。既読判定は呼出し側が行う。</summary>
        // スキップ表示切替
        public void ChangeSkipVisible(bool isVisible)
        {
            _skipButton.interactable = isVisible;
            _skipButton.gameObject.SetActive(isVisible);
        }

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
            _standPositions[3] = _standCenterDownAnchor.anchoredPosition;
        }

        private void Start()
        {
            isTyping = false;
            _clickWaitGlyph.SetActive(false);
        }

        // 本文入力を判定
        private bool IsAdvanceInputPressed()
        {
            // 押下した入力元
            bool isPointerPressed = Pointer.current?.press.wasPressedThisFrame == true;
            bool isRightButtonPressed = Mouse.current?.rightButton.wasPressedThisFrame == true;
            if (!isPointerPressed && !isRightButtonPressed)
            {
                return false;
            }

            // 表示中の設定を優先
            foreach (CanvasGroup group in _advanceInputBlockingGroups)
            {
                if (group.isActiveAndEnabled && group.alpha > 0.0f && group.blocksRaycasts)
                {
                    return false;
                }
            }

            // 押下元の画面位置
            Vector2 screenPosition = isPointerPressed
                ? Pointer.current.position.ReadValue()
                : Mouse.current.position.ReadValue();
            // 設定ボタンを除外
            foreach (RectTransform area in _advanceInputExcludedAreas)
            {
                if (area.gameObject.activeInHierarchy
                    && RectTransformUtility.RectangleContainsScreenPoint(area, screenPosition))
                {
                    return false;
                }
            }

            return true;
        }

        private void Update()
        {
            if (!IsAdvanceInputPressed())
            {
                return;
            }

            if (isTyping)
            {
                typingCts?.Cancel();
            }
        }

        /// <summary>Rubyの明示命令でメッセージボックスと文章欄を表示する。</summary>
        // ボックスを表示
        public void ShowMessageBox()
        {
            // 既存の表示親
            GameObject textBoxRoot = nameTxt.transform.parent.gameObject;
            nameTxt.gameObject.SetActive(true);
            bodyTxt.gameObject.SetActive(true);
            textBoxRoot.SetActive(true);
        }

        /// <summary>Rubyの明示命令でメッセージボックスだけを非表示にする。</summary>
        // ボックスを隠す
        public void HideMessageBox()
        {
            _clickWaitGlyph.SetActive(false);
            nameTxt.transform.parent.gameObject.SetActive(false);
        }

        /// <summary>背景や立ち絵、ボックスの表示状態を変えず、名前と本文を消す。</summary>
        // 名前と本文を消去
        public void ClearMessageBox()
        {
            _clickWaitGlyph.SetActive(false);
            nameTxt.text = "";
            bodyTxt.text = "";
        }

        // ダイアログ表示
        public async UniTask ShowDialogAsync(string name, string text, CancellationToken _ct)
        {
            CancellationToken destroyToken = this.GetCancellationTokenOnDestroy();
            using CancellationTokenSource linkedTokenSource =
                CancellationTokenSource.CreateLinkedTokenSource(_ct, destroyToken);
            CancellationToken cancellationToken = linkedTokenSource.Token;
            cancellationToken.ThrowIfCancellationRequested();

            _clickWaitGlyph.SetActive(false);
            try
            {
                nameTxt.text = "";
                bodyTxt.text = "";

                nameTxt.text = name;

                await TypeText(text, cancellationToken);

                // 全文表示後だけ案内
                _clickWaitGlyph.SetActive(nameTxt.transform.parent.gameObject.activeInHierarchy);
                // 全文表示の入力と分離
                await UniTask.NextFrame(PlayerLoopTiming.EarlyUpdate, cancellationToken: cancellationToken);
                if (!_data._isAuto)
                {
                    await UniTask.WaitUntil(IsAdvanceInputPressed,
                        cancellationToken: cancellationToken);
                }
                else
                {
                    // オート時の待機時間
                    float autoWait = _data._autoWait + text.Length * _data._autoWaitPerChar;
                    // 通常条件に加えて、オート時の待機時間終了でも進行
                    await UniTask.WhenAny(
                        UniTask.WaitUntil(IsAdvanceInputPressed,
                            cancellationToken: cancellationToken),
                        UniTask.Delay(TimeSpan.FromSeconds(autoWait), cancellationToken: cancellationToken));
                }
            }
            finally
            {
                // 破棄後の参照を避ける
                if (!destroyToken.IsCancellationRequested)
                {
                    _clickWaitGlyph.SetActive(false);
                }
            }
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
            bool useSpaceWordWrapping = _initializeStatus.UseSpaceWordWrapping; // 表示方法

            try
            {
                // 前の文章の表示制限を解除
                bodyTxt.maxVisibleCharacters = int.MaxValue;
                bodyTxt.text = "";

                if (useSpaceWordWrapping)
                {
                    // 全文で配置を確定してから文字を見せる
                    bodyTxt.maxVisibleCharacters = 0;
                    bodyTxt.text = BuildSpaceWordWrappingText(text);
                    bodyTxt.ForceMeshUpdate(true, true);
                    int characterCount = bodyTxt.textInfo.characterCount; // 表示文字数

                    for (int index = 0; index < characterCount; index++)
                    {
                        // タグを数えずに文字を再生
                        if (soundCmd != null && _data._popopo.Length > 0) soundCmd.CallPlaySE(_data._popopo);
                        bodyTxt.maxVisibleCharacters = index + 1;
                        await UniTask.WaitForSeconds(_data.CharWait, cancellationToken: currentTypingCts.Token);
                    }
                }
                else
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
            }
            catch (OperationCanceledException)
                when (!_ct.IsCancellationRequested && currentTypingCts.IsCancellationRequested)
            {
                if (useSpaceWordWrapping)
                {
                    // 折り返しを保ったまま全文を表示
                    bodyTxt.maxVisibleCharacters = int.MaxValue;
                }
                else
                {
                    bodyTxt.text = text;
                }
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

        /// <summary>空白区切りの塊を途中で折り返さない表示用文字列へ整える。</summary>
        // 空白区切りで表示用タグを付加
        public static string BuildSpaceWordWrappingText(string text)
        {
            // 明示的な改行制御とタグ無効化を優先
            if (text.IndexOf("<nobr", StringComparison.OrdinalIgnoreCase) >= 0
                || text.IndexOf("<noparse", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return text;
            }

            StringBuilder result = new StringBuilder(text.Length); // 表示用全文
            StringBuilder word = new StringBuilder(); // 表示する塊

            for (int index = 0; index < text.Length; index++)
            {
                char character = text[index]; // 現在の文字
                if (character == '<')
                {
                    int tagEnd = text.IndexOf('>', index + 1); // タグの末尾
                    if (tagEnd >= 0)
                    {
                        string tag = text.Substring(index, tagEnd - index + 1); // 既存のタグ
                        if (tag.Equals("<br>", StringComparison.OrdinalIgnoreCase)
                            || tag.Equals("<br/>", StringComparison.OrdinalIgnoreCase)
                            || tag.Equals("<br />", StringComparison.OrdinalIgnoreCase))
                        {
                            AppendWrappedWord(result, word);
                            result.Append(tag);
                        }
                        else
                        {
                            word.Append(tag);
                        }

                        index = tagEnd;
                        continue;
                    }
                }

                // 空白・明示改行は元のまま残す
                if (character == ' ' || character == '\u3000'
                    || character == '\r' || character == '\n' || character == '\t')
                {
                    AppendWrappedWord(result, word);
                    result.Append(character);
                    continue;
                }

                word.Append(character);
            }

            AppendWrappedWord(result, word);
            return result.ToString();
        }

        // 途中で折り返さない塊を追加
        private static void AppendWrappedWord(StringBuilder result, StringBuilder word)
        {
            if (word.Length == 0)
            {
                return;
            }

            result.Append("<nobr>").Append(word).Append("</nobr>");
            word.Clear();
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
        public async UniTask ShowStandAsync(int id, string path, float width, float height, string position, string effect, float duration, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            UnityEngine.UI.Image image = _standImages[id];
            await ShowStandAsync(id, image, LoadSprite(path), null, default, width, height, position, effect, duration, ct);
        }

        /// <summary>共通素体と指定キーの差分セットを表示する。</summary>
        public UniTask ShowLayeredStandAsync(int id, string characterKey, string textureSetKey, float width, float height, string position, string effect, float duration, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            if (id < 0 || id >= _standImages.Length)
            {
                Debug.LogError($"立ち絵のIDが範囲外です: {id}", this);
                return UniTask.CompletedTask;
            }

            NovelStandInfo info = Array.Find(_standInfos, item => item != null && item.CharacterKey == characterKey);
            if (info == null || !info.TryGetTextureSet(textureSetKey, out NovelStandTextureSetData textureSet))
            {
                Debug.LogError($"立ち絵の差分セットが見つかりません: {characterKey}/{textureSetKey}", this);
                return UniTask.CompletedTask;
            }

            if (info.Body == null || info.CanvasSize.x <= 0 || info.CanvasSize.y <= 0 ||
                info.Parts.Count > StandPartCapacity || textureSet.Sprites == null || textureSet.Sprites.Count != info.Parts.Count ||
                _standFadeGroups.Length <= id || _standPartImages.Length < (id + 1) * StandPartCapacity)
            {
                Debug.LogError($"立ち絵の素材または表示枠の設定が不正です: {characterKey}/{textureSetKey}", this);
                return UniTask.CompletedTask;
            }

            return ShowStandAsync(id, _standImages[id], info.Body, info, textureSet, width, height, position, effect, duration, ct);
        }

        private async UniTask ShowStandAsync(int id, UnityEngine.UI.Image image, Sprite sprite, NovelStandInfo info,
            NovelStandTextureSetData textureSet, float width, float height, string position, string effect, float duration, CancellationToken ct)
        {
            // 表示先の寿命を連結
            CancellationToken destroyToken = this.GetCancellationTokenOnDestroy();
            using CancellationTokenSource linked = CancellationTokenSource.CreateLinkedTokenSource(ct, destroyToken);
            linked.Token.ThrowIfCancellationRequested();
            RectTransform rect = image.rectTransform; // 表示姿勢
            Vector2 origin = GetStandPosition(id, position);
            bool keepMotion = info != null && _activeStandInfos[id] == info && image.gameObject.activeSelf &&
                (duration <= 0.0f || effect == "none") && _standOrigins[id] == origin &&
                Mathf.Approximately(rect.rect.width, width) && Mathf.Approximately(rect.rect.height, height);
            _standOrigins[id] = origin;
            // 指定した表示寸法
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            if (image.sprite != sprite)
            {
                image.sprite = sprite;
            }
            image.gameObject.SetActive(true);
            ApplyStandParts(id, info, textureSet);
            if (keepMotion)
            {
                return;
            }

            rect.anchoredPosition = _standOrigins[id];
            rect.localScale = _standScales[id];
            SetStandAlpha(id, 1.0f);
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
                        SetStandAlpha(id, 0.0f);
                        tween = FadeStand(id, 1.0f, duration);
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
                    SetStandAlpha(id, 1.0f);
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
                    await AwaitImageTweenAsync(FadeStand(id, 0.0f, duration), linked.Token);
                }
            }
            finally
            {
                // 次の登場に透明度を持ち越さない
                if (!destroyToken.IsCancellationRequested)
                {
                    image.gameObject.SetActive(false);
                    SetStandAlpha(id, 1.0f);
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

        // 共通素体に差分を配置する
        private void ApplyStandParts(int id, NovelStandInfo info, NovelStandTextureSetData textureSet)
        {
            if (_activeStandInfos[id] != null && info == null)
            {
                _standFadeGroups[id].alpha = 1.0f;
            }
            _activeStandInfos[id] = info;

            int start = id * StandPartCapacity;
            for (int partIndex = 0; partIndex < StandPartCapacity && start + partIndex < _standPartImages.Length; partIndex++)
            {
                UnityEngine.UI.Image image = _standPartImages[start + partIndex];
                Sprite sprite = info != null && partIndex < info.Parts.Count ? textureSet.Sprites[partIndex] : null;
                if (image.sprite != sprite)
                {
                    image.sprite = sprite;
                }
                image.gameObject.SetActive(sprite != null);
                if (sprite == null)
                {
                    continue;
                }

                PositionStandPart(_standImages[id], image.rectTransform, info.Parts[partIndex].PlacementRect, info.CanvasSize);
            }
        }

        // 素体と同じ縦横比と余白を部位にも適用する
        private static void PositionStandPart(UnityEngine.UI.Image body, RectTransform part, RectInt placement, Vector2Int canvas)
        {
            Rect rect = body.GetPixelAdjustedRect();
            if (body.preserveAspect && rect.width > 0.0f && rect.height > 0.0f)
            {
                float aspect = body.sprite.rect.width / body.sprite.rect.height;
                Vector2 pivot = body.rectTransform.pivot;
                if (aspect > rect.width / rect.height)
                {
                    float height = rect.width / aspect;
                    rect.y += (rect.height - height) * pivot.y;
                    rect.height = height;
                }
                else
                {
                    float width = rect.height * aspect;
                    rect.x += (rect.width - width) * pivot.x;
                    rect.width = width;
                }
            }

            part.sizeDelta = new Vector2(rect.width * placement.width / canvas.x, rect.height * placement.height / canvas.y);
            Rect parent = body.rectTransform.rect;
            part.anchoredPosition = new Vector2(
                rect.x - parent.x + rect.width * placement.x / canvas.x,
                rect.y - parent.y + rect.height * (canvas.y - placement.yMax) / canvas.y);
        }

        // 差分付き立ち絵は全体を一緒にフェードする
        private Tween FadeStand(int id, float alpha, float duration)
        {
            Tween tween = _activeStandInfos[id] != null
                ? _standFadeGroups[id].DOFade(alpha, duration)
                : _standImages[id].DOFade(alpha, duration);
            return tween.SetEase(Ease.Linear);
        }

        private void SetStandAlpha(int id, float alpha)
        {
            if (_activeStandInfos[id] != null)
            {
                SetImageAlpha(_standImages[id], 1.0f);
                _standFadeGroups[id].alpha = alpha;
                return;
            }

            SetImageAlpha(_standImages[id], alpha);
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
                "center_down" => _standPositions[3],
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