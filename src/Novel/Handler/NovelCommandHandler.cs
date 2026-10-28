using System;

using System.Threading;
using Cysharp.Threading.Tasks;
using VitalRouter;
using VitalRouter.MRuby;
namespace RaruLib
{
    [Routes]
    public partial class NovelCommandHandler
    {
        private readonly INovelView _view;
        private readonly ISoundCommand _soundCommand; // 音声の呼出先

        public NovelCommandHandler(INovelView view, ISoundCommand soundCommand)
        {
            _view = view;
            _soundCommand = soundCommand;
        }

        /// <summary>Rubyのボックス操作を、取消済みでない場合だけViewへ反映する。</summary>
        // 表示命令を転送
        public UniTask On(MessageBoxCommand command, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            if (command.ClearText)
            {
                _view.ClearMessageBox();
                return UniTask.CompletedTask;
            }

            if (command.IsVisible)
            {
                _view.ShowMessageBox();
            }
            else
            {
                _view.HideMessageBox();
            }
            return UniTask.CompletedTask;
        }

        // 通常ダイアログ
        public async UniTask On(DialogCommand command, CancellationToken ct)
        {
            await _view.ShowDialogAsync(command.Name, command.Text, ct);
        }

        // 選択肢
        public async UniTask On(ChoiceCommand command, PublishContext context)
        {
            var index = await _view.ShowChoiceAsync(command.Options, context.CancellationToken);

            context.MRubySharedVariables()!.Set(command.StateKey, index);
        }

        // 背景表示を委譲
        public async UniTask On(BgCommand command, CancellationToken ct)
        {
            await _view.ShowBackgroundAsync(command.Path, command.FadeOut, command.FadeIn, ct);
        }

        // 番号の立ち絵を表示
        public async UniTask On(StandCommand command, CancellationToken ct)
        {
            await _view.ShowStandAsync(command.Id, command.Path, command.Width, command.Height, command.Position, command.Effect, command.Duration, ct);
        }

        // 番号の立ち絵を退場
        public async UniTask On(StandHideCommand command, CancellationToken ct)
        {
            await _view.HideStandAsync(command.Id, command.Duration, ct);
        }

        // 番号の立ち絵を動作
        public async UniTask On(StandEffectCommand command, CancellationToken ct)
        {
            await _view.PlayStandEffectAsync(command.Id, command.Effect, command.Duration, ct);
        }

        /// <summary>指定秒数の待機を、ノベル再生の取消と接続する。</summary>
        // 次の命令まで待機
        public UniTask On(WaitCommand command, CancellationToken ct)
        {
            return UniTask.Delay(TimeSpan.FromSeconds(command.Seconds),
                ignoreTimeScale: true, cancellationToken: ct);
        }

        /// <summary>SEの開始だけを指示し、音の終了は待たない。</summary>
        // 音声の要求を転送
        public UniTask On(SePlayCommand command, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            _soundCommand.CallPlaySE(command.SoundName);
            return UniTask.CompletedTask;
        }

        /// <summary>指定したBGMだけを再生し、音の終了は待たない。</summary>
        // 音声の要求を転送
        public UniTask On(BgmPlayCommand command, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            _soundCommand.CallPlayBGM(command.SoundName);
            return UniTask.CompletedTask;
        }

        /// <summary>指定したBGMだけを、既存フェードも含めて停止する。</summary>
        // 音声の要求を転送
        public UniTask On(BgmStopCommand command, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            _soundCommand.CallStopBGM(command.SoundName);
            return UniTask.CompletedTask;
        }
    }
}