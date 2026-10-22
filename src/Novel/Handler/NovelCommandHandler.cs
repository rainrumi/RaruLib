
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

        public NovelCommandHandler(INovelView view)
        {
            _view = view;
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
    }
}