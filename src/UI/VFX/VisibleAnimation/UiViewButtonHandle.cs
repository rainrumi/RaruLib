using Cysharp.Threading.Tasks;
using DG.Tweening;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace RaruLib
{
    [RequireComponent(typeof(CanvasGroup))]
    public class UiViewButtonHandle : UiViewAsync
    {
        [SerializeField] private ButtonEvent buttonEvent;

        protected override void Start()
        {
            base.Start();

            if (buttonEvent != null)
            {
                buttonEvent.OnClickEvent
                    .Subscribe(_ =>
                    {
                        ViewEventAsync().Forget();
                    }).AddTo(this);
            }
        }
    }
}