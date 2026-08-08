using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ScreenEffectView : MonoBehaviour, IScreenEffectView
{
    [SerializeField] private Image _image;

    /// <summary>
    /// フェードイン
    /// </summary>
    public async UniTask FadeInAsync(float duration, CancellationToken _ct)
    {
        var color = _image.color;
        color.a = 0;
        _image.enabled = true;
        _image.raycastTarget = true;
        _image.DOComplete();
        await _image.DOFade(1f, duration).SetEase(Ease.OutQuad).WithCancellation(_ct);
    }

    /// <summary>
    /// フェードアウト
    /// </summary>
    public async UniTask FadeOutAsync(float duration, CancellationToken _ct)
    {
        var color = _image.color;
        _image.enabled = true;
        _image.raycastTarget = true;
        _image.DOComplete();
        await _image.DOFade(0f, duration).SetEase(Ease.OutQuad).WithCancellation(_ct);
        _image.enabled = false;
        _image.raycastTarget = false;
    }
}
