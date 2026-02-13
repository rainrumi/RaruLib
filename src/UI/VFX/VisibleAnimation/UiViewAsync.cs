using Cysharp.Threading.Tasks;
using DG.Tweening;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace RaruLib
{
    public enum ViewKind
    {
        View,
        Hide
    }

    [RequireComponent(typeof(CanvasGroup))]
    public class UiViewAsync : MonoBehaviour
    {
        private CanvasGroup _canvasGroup;
        private RectTransform _rectTransform;
        private Vector3 _initPosition;
        private Vector3 _initScale;
        private float _initAlpha;
        private Vector2 _resoltion => new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
        private Vector2 amountSlide => new Vector2(_resoltion.x * viewStartPosFactorX, _resoltion.y * viewStartPosFactorY);

        [SerializeField] private string memo = "";    // 何するかのメモ（分からなくなるため、インスペクタ用）
        [SerializeField] private Ease ease = Ease.OutCubic;
        [Header("アニメーションの種類")]
        [SerializeField] private ViewKind viewKind = ViewKind.View;
        [Header("所要時間")]
        [SerializeField] private float duration = 0.3f;
        [Header("Alpha")]
        [SerializeField, Range(0, 1)] private float viewStartAlpha = 0f;
        [Header("拡大率")]
        [SerializeField, Range(0, 1)] private float viewStartScale = 0.5f;
        [Header("位置変化率")]
        [SerializeField, Range(-1, 1)] private float viewStartPosFactorX = 0f;
        [SerializeField, Range(-1, 1)] private float viewStartPosFactorY = 0f;

        protected virtual void Start()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _rectTransform = GetComponent<RectTransform>();
            _initPosition = _rectTransform.localPosition;
            _initScale = _rectTransform.localScale;
            _initAlpha = _canvasGroup.alpha;
        }

        public async UniTask ViewEventAsync()
        {
            if (!this) return;

            _rectTransform.DOKill();
            _canvasGroup.DOKill();

            switch (viewKind)
            {
                case ViewKind.View:
                    SetTrigger(true);
                    await UniTask.WhenAll(ScaleIn(), SlideIn(), FadeIn());
                    break;

                case ViewKind.Hide:
                    await UniTask.WhenAll(ScaleOut(), SlideOut(), FadeOut());
                    SetTrigger(false);
                    break;
            }
        }

        private void SetTrigger(bool set)
        {
            if (this == null) return;
            _canvasGroup.blocksRaycasts = set;
            _canvasGroup.alpha = set ? 1 : 0;
        }

        private async UniTask ScaleIn()
        {
            _rectTransform.localScale =
                new Vector3(viewStartScale, viewStartScale, 0);

            await _rectTransform
                .DOScale(_initScale, duration)
                .SetEase(ease)
                .SetLink(gameObject, LinkBehaviour.KillOnDestroy)
                .AsyncWaitForCompletion();
        }

        private async UniTask ScaleOut()
        {
            _rectTransform.localScale = _initScale;

            await _rectTransform
                .DOScale(viewStartScale, duration)
                .SetEase(ease)
                .SetLink(gameObject, LinkBehaviour.KillOnDestroy)
                .AsyncWaitForCompletion();
        }

        private async UniTask FadeIn()
        {
            _canvasGroup.alpha = viewStartAlpha;

            await _canvasGroup
                .DOFade(1, duration)
                .SetEase(ease)
                .SetLink(gameObject, LinkBehaviour.KillOnDestroy)
                .AsyncWaitForCompletion();
        }

        private async UniTask FadeOut()
        {
            _canvasGroup.alpha = 1;

            await _canvasGroup
                .DOFade(0, duration)
                .SetEase(ease)
                .SetLink(gameObject, LinkBehaviour.KillOnDestroy)
                .AsyncWaitForCompletion();
        }

        private async UniTask SlideIn()
        {
            _rectTransform.localPosition = amountSlide;

            await _rectTransform
                .DOAnchorPos(_initPosition, duration)
                .SetEase(ease)
                .SetLink(gameObject, LinkBehaviour.KillOnDestroy)
                .AsyncWaitForCompletion();
        }

        private async UniTask SlideOut()
        {
            _rectTransform.localPosition = _initPosition;

            await _rectTransform
                .DOAnchorPos(amountSlide, duration)
                .SetEase(ease)
                .SetLink(gameObject, LinkBehaviour.KillOnDestroy)
                .AsyncWaitForCompletion();
        }

        private void OnDestroy()
        {
            _canvasGroup.DOKill();
        }
    }
}