using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using System;
using UnityEngine;
using UnityEngine.UI;
using RaruLib;

public class DailogueItem
{
    //[serializeField] private string text;
    //public string Text => text;
    
    //[serializeField] private Sprite sprite;
    //public Sprite Sprite => sprite;
}


public class DailogueEntry
{
    //[serializeField] private List<DailogueItem> data;
    //public List<DailogueItem> Data => data;
}

[RequireComponent(typeof(SoundCommand))]
public class NovelViewStandard : NovelView
{
    //private UniTaskCompletionSource _waitForClick;
    //private bool _isAnimating;
    //private SoundCommand _soundCommand;

    //[SerializeField] private HorizontalLayoutGroup _textLayoutGroup;

    //public async UniTask ShowMessageAsync(DailogueEntry data, CancellationToken ct)
    //{
    //    try
    //    {
    //        await FadeInAsync();

    //        for (int d = 0; d < data.Count; d++)
    //        {
    //            // タイプライターアニメーション
    //            _isAnimating = true;
    //            _animationCts = CancellationTokenSource.CreateLinkedTokenSource(ct);

    //            try
    //            {
    //                await PlayTypewriterAsync(data[d].Text, _animationCts.Token);
    //                if(_animationCts!=null) await UniTask.Delay(200, cancellationToken: _animationCts.Token);
    //            }
    //            catch (System.OperationCanceledException)
    //            {
    //                // スキップされた場合は全文表示
    //                messageText.text = data[d].Text;
    //                if (_textLayoutGroup != null)
    //                {
    //                    _textLayoutGroup.SetLayoutHorizontal();
    //                    _textLayoutGroup.SetLayoutVertical();
    //                    LayoutRebuilder.ForceRebuildLayoutImmediate(_textLayoutGroup.GetComponent<RectTransform>());
    //                }
    //            }

    //            _isAnimating = false;

    //            // クリック待ち
    //            await WaitForClickAsync();
    //        }

    //        await FadeOutAsync();
    //    }
    //    catch (OperationCanceledException)
    //    {
    //        _waitForClick?.TrySetCanceled();   // タスクを強制完了
    //        SetVisibleCanvas(false);    // カンバスを即座に非表示にする
    //    }
    //}

    ///******************************************************************/
    //private async UniTask PlayTypewriterAsync(string text, CancellationToken ct)
    //{
    //    await TypeAsync(text, ct);
    //}

    //private async UniTask TypeAsync(string text, CancellationToken ct)
    //{
    //    var sb = new System.Text.StringBuilder(text.Length);
    //    _textCanvasGroup.alpha = 1; // テキストボックスを表示する

    //    try
    //    {
    //        for (int i = 0; i < text.Length; i++)
    //        {
    //            _soundCommand.CallPlaySE("popopo");
    //            sb.Append(text[i]);
    //            messageText.text = sb.ToString();
    //            await UniTask.Delay(TimeSpan.FromSeconds(TYPE_INTERVAL), cancellationToken: ct);
    //        }
    //    }
    //    catch (System.OperationCanceledException)
    //    {

    //    }
    //}

    //private async UniTask WaitForClickAsync()
    //{
    //    _waitForClick = new UniTaskCompletionSource();
    //    await _waitForClick.Task;
    //}

    //protected override void Awake()
    //{
    //    base.Awake();
    //    _soundCommand = gameObject.GetComponent<SoundCommand>();
    //}

    //private void Update()
    //{
    //    if (!Input.GetMouseButtonDown(0)) return;

    //    if (_isAnimating)
    //    {
    //        // アニメーション中はスキップ
    //        _animationCts?.Cancel();
    //    }
    //    else
    //    {
    //        // アニメーション完了後は次へ
    //        _waitForClick?.TrySetResult();
    //    }
    //}
}
