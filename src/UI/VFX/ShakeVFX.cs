using DG.Tweening;
using UnityEngine;

namespace RaruLib
{
    public class ShakeVFX : MonoBehaviour
    {
        [SerializeField] float duration = 1.5f;
        [SerializeField] float strength = 0.1f;
        [SerializeField] int vibrato = 10;
        [SerializeField] float randomness = 90f;
        [SerializeField] bool onAwake = false;
        [SerializeField] bool isLoop = true;
        [SerializeField] bool onFadeout = false;

        Tween shakeTween;

        private void Awake()
        {
            if(onAwake)
            {
                shakeTween?.Kill();
                shakeTween = transform.DOShakePosition(
                        duration,
                        strength,
                        vibrato,
                        randomness,
                        fadeOut: onFadeout
                    )
                    .SetEase(Ease.Linear);
                if (isLoop)
                {
                    shakeTween.SetLoops(-1, LoopType.Restart);
                }
            }
        }

        public void ShakePlay()
        {
            shakeTween?.Kill();
            shakeTween = transform.DOShakePosition(
                    duration,
                    strength,
                    vibrato,
                    randomness,
                    fadeOut: onFadeout
                )
                .SetEase(Ease.Linear);
            if (isLoop)
            {
                shakeTween.SetLoops(-1, LoopType.Restart);
            }
        }

        public void ShakeStop()
        {
            shakeTween?.Kill();
        }

        void OnDestroy()
        {
            shakeTween?.Kill();
        }
    }
}