using Cysharp.Threading.Tasks;
using DG.Tweening;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace RaruLib
{
    public enum ViewKind
    {
        View_ScaleIn,
        Hide_ScaleOut,
        View_FadeIn,
        Hide_FadeOut,
        View_SlideInFromTop_FadeIn,
        Hide_SlideOutToTop_FadeOut
    }

    [RequireComponent(typeof(CanvasGroup))]
    public class UiViewAsync : MonoBehaviour
    {
        private CanvasGroup _canvasGroup;
        private RectTransform _rectTransform;
        private Vector3 _initPosition;
        private Vector3 _initScale;
        private float _initAlpha;

        [SerializeField] private string memo = "";    // 何するかのメモ（分からなくなるため、インスペクタ用）
        [SerializeField] private ViewKind viewKind;
        [SerializeField] private float duration = 1f;
        [SerializeField, Range(0, 1)] private float amountScale = 0.5f;
        //[SerializeField] private float amountFade = 0.5f;

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
                case ViewKind.View_ScaleIn:
                    SetTrigger(true);
                    _rectTransform.localScale =
                        new Vector3(amountScale, amountScale, amountScale);

                    await _rectTransform
                        .DOScale(_initScale, duration)
                        .SetEase(Ease.OutCubic)
                        .SetLink(gameObject, LinkBehaviour.KillOnDestroy)
                        .AsyncWaitForCompletion();
                    break;

                case ViewKind.Hide_ScaleOut:
                    _rectTransform.localScale = _initScale;

                    await _rectTransform
                        .DOScale(amountScale, duration)
                        .SetEase(Ease.OutCubic)
                        .SetLink(gameObject, LinkBehaviour.KillOnDestroy)
                        .AsyncWaitForCompletion();

                    SetTrigger(false);
                    break;

                case ViewKind.View_FadeIn:
                    SetTrigger(true);
                    _canvasGroup.alpha = 0;

                    await _canvasGroup
                        .DOFade(1, duration)
                        .SetEase(Ease.OutCubic)
                        .SetLink(gameObject, LinkBehaviour.KillOnDestroy)
                        .AsyncWaitForCompletion();
                    break;

                case ViewKind.Hide_FadeOut:
                    _canvasGroup.alpha = 1;

                    await _canvasGroup
                        .DOFade(0, duration)
                        .SetEase(Ease.OutCubic)
                        .SetLink(gameObject, LinkBehaviour.KillOnDestroy)
                        .AsyncWaitForCompletion();

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

        private async UniTaskVoid FadeIn()
        {
            SetTrigger(true);
            _canvasGroup.alpha = 0;

            await _canvasGroup
                .DOFade(1, duration)
                .SetEase(Ease.OutCubic)
                .SetLink(gameObject, LinkBehaviour.KillOnDestroy)
                .AsyncWaitForCompletion();
        }

        private void OnDestroy()
        {
            _canvasGroup.DOKill();
        }
    }
}