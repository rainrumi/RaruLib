
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
    }
}