using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Text;
using RaruLib;
using R3;

public enum NovelLiveState
{
    Accept,
    Reject
}

[RequireComponent(typeof(SoundCommand))]
public class NovelViewLive : NovelView
{
//    private const float AUTO_CHAR_INTERVAL = 0.1f;
//    private const float AUTO_CONST_INTERVAL = 3.0f;
//    private const float COOL_TIME = 2.0f;

//    private Queue<DialogueEntry2> _logQueue = new();
//    private StringBuilder _currentText = new();
//    private DialogueEntry2 _cachedDialog = new DialogueEntry2();
//    private SoundCommand _soundCommand;
//    private NovelLiveState _state;

//    // ログが更新されたときのイベント
//    private Subject<int> OnEnqueueLogSubject = new();
//    public Observable<int> OnEnqueueLog => OnEnqueueLogSubject.AsObservable();

//    public async UniTask SetMessage(DialogueEntry2 data, CancellationToken ct)
//    {
//        if(_state != NovelLiveState.Accept) return;
//        EnqueueLog(data);

//        if(_animationCts == null)
//        {
//            // このctがキャンセルされるとき->クリックがいるノベルが始まったとき
//            _animationCts = CancellationTokenSource.CreateLinkedTokenSource(ct);

//            try
//            {
//                await FadeInAsync();

//                if(_textCanvasGroup!=null) _textCanvasGroup.alpha = 1;                 // テキストボックスを表示する

//                await TypeLoopAsync(_animationCts.Token);

//                if (_textCanvasGroup != null) _textCanvasGroup.alpha = 0;                 // テキストボックスを非表示にする
//                _currentText.Clear();                       // テキストスタックを削除

//                await FadeOutAsync();
//            }
//            catch (System.OperationCanceledException)
//            {
//                _currentText.Clear();                               // テキストスタックを削除
//                _logQueue.Clear();                                  // ログの削除
//                PlayDisappearAnimation(_cachedDialog.Character);    // 出場キャラ強制退場
//                SetVisibleCanvas(false);
//            }
//        }
//    }

//    public void CallRetro(DialogueEntry2 data)
//    {
//        _logQueue.Clear();
//        SetMessage(data, _animationCts.Token).Forget();
//        CoolTime(_animationCts.Token).Forget();
//    }

//    private void EnqueueLog(DialogueEntry2 data)
//    {
//        _logQueue.Enqueue(data);
//        OnEnqueueLogSubject.OnNext(_logQueue.Count);
//    }

//    private async UniTask CoolTime(CancellationToken ct)
//    {
//        _state = NovelLiveState.Reject;
//        try
//        {
//            await UniTask.WaitForSeconds(COOL_TIME, cancellationToken: ct);
//        }
//        catch (System.OperationCanceledException) 
//        {
//            _state = NovelLiveState.Accept;
//        }
//        _state = NovelLiveState.Accept;
//    }

//    private async UniTask TypeLoopAsync(CancellationToken ct)
//    {
//        while (_logQueue.Count > 0)
//        {
//            var next = _logQueue.Dequeue();

//            // 今回喋る情報をキャッシュ
//            _cachedDialog = next;

//	    /* ここにテキストをTypeAsyncへ送る処理 */

//            if (_logQueue.Count <= 0)
//            {
//                // TypeCompleteWaitAsyncで一定時間後に_animationCtsをキャンセルする処理を起動
//                var viewCt = CancellationTokenSource.CreateLinkedTokenSource(ct);
//                TypeCompleteWaitAsync(next.Text.Length, viewCt.Token).Forget();

//                try
//                {
//                    // TypeCompleteWaitAsyncで_animationCtsがキャンセルされるまでに
//                    // キューが０以上になったらTypeCompleteWaitAsyncの待機をキャンセルしてwhile文に復帰する
//                    await UniTask.WaitUntil(() => _logQueue.Count > 0, cancellationToken: ct);
//                    viewCt?.Cancel();
//                    viewCt?.Dispose();
//                }
//                catch (System.OperationCanceledException)
//                {
//                    // TypeCompleteWaitAsyncで_animationCtsがキャンセルされるまでに
//                    // キューが０以上にならなかったらwhile文から抜ける
//                    break;
//                }
//            }
//        }
//    }

//    private async UniTask TypeAsync(string text, CancellationToken ct)
//    {
//        string baseText = messageText.text;

//        try
//        {
//            for (int i = 0; i < text.Length; i++)
//            {
//                _soundCommand.CallPlaySE("popopo");
//                _currentText.Append(text[i]);
//                messageText.text = _currentText.ToString();
//                await UniTask.Delay(TimeSpan.FromSeconds(TYPE_INTERVAL), cancellationToken: ct);
//            }
//        }
//        catch (System.OperationCanceledException)
//        {
//            // キャンセルされた場合は全文表示
//            messageText.text = baseText + text;
//        }
//    }

//    private async UniTaskVoid TypeCompleteWaitAsync(int charLength, CancellationToken ct)
//    {
//        try
//        {
//            var viewInterval = charLength * AUTO_CHAR_INTERVAL + AUTO_CONST_INTERVAL;
//            await UniTask.WaitForSeconds(viewInterval, cancellationToken: ct);

//            // 時間が経過したらアニメーションを全て終了させる
//            _animationCts?.Cancel();
//            _animationCts?.Dispose();
//            _animationCts = null;
//        }
//        catch (System.OperationCanceledException)
//        {
//            // キャンセル想定のためエラーは丸め込む
//        }
//    }

//    protected override void Awake()
//    {
//        base.Awake();
//        _soundCommand = gameObject.GetComponent<SoundCommand>();
//        _state = NovelLiveState.Accept;
//    }
}