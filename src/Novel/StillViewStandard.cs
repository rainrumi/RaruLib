using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using System;
using UnityEngine;
using UnityEngine.UI;

//public class StillViewStandard : StillView
//{
//    private UniTaskCompletionSource _waitForClick;
//    private bool _isAnimating;

//    public async UniTask SetSpriteAsync(Sprite sprite, CancellationToken ct, float fadeTime = 0.3f)
//    {
//        _animationCts = CancellationTokenSource.CreateLinkedTokenSource(ct);

//        try
//        {
//            try
//            {
//                _image1.sprite = sprite;
//                await FadeInAsync(_animationCts.Token, duration: fadeTime);
//            }
//            catch (OperationCanceledException)
//            {

//            }

//            _isAnimating = false;

//            // クリック待ち
//            // await WaitForClickAsync();
//        }
//        catch (OperationCanceledException)
//        {
//            _waitForClick?.TrySetCanceled();   // タスクを強制完了
//            SetVisibleCanvas(false);    // カンバスを即座に非表示にする
//        }
//    }

//    public async UniTask HideSpriteAsync(CancellationToken ct, float fadeTime = 0.3f)
//    {
//        _animationCts = CancellationTokenSource.CreateLinkedTokenSource(ct);

//        try
//        {
//            try
//            {
//                await FadeOutAsync(_animationCts.Token, duration: fadeTime);
//            }
//            catch (OperationCanceledException)
//            {

//            }

//            _isAnimating = false;
//        }
//        catch (OperationCanceledException)
//        {
//            _waitForClick?.TrySetCanceled();   // タスクを強制完了
//            SetVisibleCanvas(false);    // カンバスを即座に非表示にする
//        }
//    }

//    private async UniTask WaitForClickAsync()
//    {
//        _waitForClick = new UniTaskCompletionSource();
//        await _waitForClick.Task;
//    }

//    private void Update()
//    {
//        if (!Input.GetMouseButtonDown(0)) return;

//        if (_isAnimating)
//        {
//            // アニメーション中はスキップ
//            _animationCts?.Cancel();
//        }
//        else
//        {
//            // アニメーション完了後は次へ
//            _waitForClick?.TrySetResult();
//        }
//    }
//}
