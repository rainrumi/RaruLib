using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class ButtonEventHandle : ButtonEventView
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
