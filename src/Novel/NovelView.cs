using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;

// 参考ソース：https://github.com/void2610/arenani

public enum TalkVisibility
{
    Appear,
    Disappear,
}

public class NovelView : MonoBehaviour
{
    protected const float TYPE_INTERVAL = 0.05f;
    protected const float CANVAS_FADE_DURATION = 0.3f;

    /******************************************************************/
    // 各キャラのアニメーター

    private Dictionary<TalkCharacter, Animator> _portraitAnimators;
    public Dictionary<TalkCharacter, Animator> PortraitAnimators => _portraitAnimators;

    /******************************************************************/

    protected CancellationTokenSource _animationCts;
    public bool IsVisible { get; private set; }

    /******************************************************************/

    [SerializeField] protected TextMeshProUGUI messageText;
    [SerializeField] protected CanvasGroup _rootCanvasGroup;
    [SerializeField] protected CanvasGroup _textCanvasGroup;

    /******************************************************************/
    protected virtual void Awake()
    {

    }

    /******************************************************************/

    public void HideMessage()
    {
        _rootCanvasGroup.alpha = 0f;
        _rootCanvasGroup.interactable = false;
        _rootCanvasGroup.blocksRaycasts = false;
    }

    public async UniTask FadeInAsync()
    {
        _textCanvasGroup.alpha = 0;
        messageText.text = "";
        _rootCanvasGroup.interactable = true;
        _rootCanvasGroup.blocksRaycasts = true;
        try
        {
            await _rootCanvasGroup.DOFade(1, CANVAS_FADE_DURATION).SetEase(Ease.OutCubic)
                  .SetLink(gameObject, LinkBehaviour.KillOnDestroy)
                  .AsyncWaitForCompletion(this.GetCancellationTokenOnDestroy());
            _rootCanvasGroup.alpha = 1.0f;
            IsVisible = true;
        }
        catch (OperationCanceledException)
        {
            
        }
    }

    public async UniTask FadeOutAsync()
    {
        try
        {
            if (_rootCanvasGroup != null)
            {
                await _rootCanvasGroup.DOFade(0, CANVAS_FADE_DURATION).SetEase(Ease.OutCubic)
                      .SetLink(gameObject, LinkBehaviour.KillOnDestroy)
                      .AsyncWaitForCompletion(this.GetCancellationTokenOnDestroy());

                _rootCanvasGroup.alpha = 0.0f;
                _rootCanvasGroup.interactable = false;
                _rootCanvasGroup.blocksRaycasts = false;
            }
            IsVisible = false;
        }
        catch (OperationCanceledException)
        {
            
        }

    }

    public void SetVisibleCanvas(bool set)
    {
        _textCanvasGroup.alpha = set ? 1 : 0;
        messageText.text = "";
        _rootCanvasGroup.interactable = set;
        _rootCanvasGroup.blocksRaycasts = set;
        IsVisible = set;
    }

    protected void OnDestroy()
    {
        _animationCts?.Cancel();
        _animationCts?.Dispose();
        _animationCts = null;
    }

}
