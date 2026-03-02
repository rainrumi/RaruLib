using System;
using Cysharp.Threading.Tasks;
using TMPro;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace RaruLib
{
    [Serializable]
    public class TalkDataSet
    {
        public UiViewAsync uiViewAsync_View;
        public UiViewAsync uiViewAsync_Hide;
        public Image textBoxImg;
        public TextMeshProUGUI textBox;
    }

    public class TypingNovel : MonoBehaviour
    {
        [SerializeField] TalkDataSet[] talkDataSet;
        [SerializeField] float textDuration = 0.05f;
        [Header("音いるなら")]
        [SerializeField] private SoundCommand soundCommand;
        [SerializeField] private string soundName = "popopo";

        private Subject<Unit> WaitNextSubject = new Subject<Unit>();
        public Observable<Unit> OnWaitNext => WaitNextSubject;

        private bool isTyping;

        public void SetTalkData(TextBoxKind kind, string _text)
        {
            SetViewTalkData(kind, true);
            SetTypingText(kind, _text).Forget();
        }

        private void SetViewTalkData(TextBoxKind kind, bool isSet)
        {
            if (!isSet && talkDataSet[(int)kind].uiViewAsync_View != null) talkDataSet[(int)kind].uiViewAsync_View.ViewEventAsync().Forget();
            else if (isSet && talkDataSet[(int)kind].uiViewAsync_Hide != null) talkDataSet[(int)kind].uiViewAsync_Hide.ViewEventAsync().Forget();
        }

        private async UniTask SetTypingText(TextBoxKind kind, string _text)
        {
            var token = this.GetCancellationTokenOnDestroy();

            // タイプしてる時に新しいのが流れてきたら止めておく
            await UniTask.WaitUntil(() => !isTyping, cancellationToken: token);

            isTyping = true;
            talkDataSet[(int)kind].textBox.text = "";

            foreach (var text in _text)
            {
                // 後々クリックで飛ばせるようにします！（2/14 22:40）
                await UniTask.WaitForSeconds(textDuration, cancellationToken: token);
                if (soundCommand != null) soundCommand.CallPlaySE(soundName);
                talkDataSet[(int)kind].textBox.text += text;
            }

            isTyping = false;
            WaitNextSubject.OnNext(Unit.Default);
        }

        public void HideTextBox(TextBoxKind kind)
        {
            talkDataSet[(int)kind].textBox.text = "";
            SetViewTalkData(kind, false);
        }
    }
}