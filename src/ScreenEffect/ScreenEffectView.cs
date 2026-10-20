using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ScreenEffectView : MonoBehaviour, IScreenEffectView
{
    [SerializeField] private Image _image;
    [SerializeField] private CanvasGroup _canvasGroup; // 表示制御

    /// <summary>
    /// フェードイン
    /// </summary>
    public async UniTask FadeInAsync(float duration, CancellationToken _ct)
    {
        var color = _image.color;
        color.a = 1.0f;
        _image.color = color;
        _image.enabled = true;
        _image.raycastTarget = true;
        _canvasGroup.DOComplete();
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.alpha = 0.0f;
        await _canvasGroup.DOFade(1f, duration).SetEase(Ease.OutQuad).WithCancellation(_ct);
    }

    /// <summary>
    /// フェードアウト
    /// </summary>
    public async UniTask FadeOutAsync(float duration, CancellationToken _ct)
    {
        var color = _image.color;
        color.a = 1.0f;
        _image.color = color;
        _image.enabled = true;
        _image.raycastTarget = true;
        _canvasGroup.DOComplete();
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;
        await _canvasGroup.DOFade(0f, duration).SetEase(Ease.OutQuad).WithCancellation(_ct);
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
        _image.raycastTarget = false;
    }
}
