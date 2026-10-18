
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

        // ’Êíƒ_ƒCƒAƒƒO
        public async UniTask On(DialogCommand command, CancellationToken ct)
        {
            await _view.ShowDialogAsync(command.Name, command.Text, ct);
        }

        // ‘I‘ğˆ
        public async UniTask On(ChoiceCommand command, PublishContext context)
        {
            var index = await _view.ShowChoiceAsync(command.Options, context.CancellationToken);

            context.MRubySharedVariables()!.Set(command.StateKey, index);
        }

        // ”wŒi•\¦‚ğˆÏ÷
        public async UniTask On(BgCommand command, CancellationToken ct)
        {
            await _view.ShowBackgroundAsync(command.Path, command.FadeOut, command.FadeIn, ct);
        }

        // ”Ô†‚Ì—§‚¿ŠG‚ğ•\¦
        public async UniTask On(StandCommand command, CancellationToken ct)
        {
            await _view.ShowStandAsync(command.Id, command.Path, command.Position, command.Effect, command.Duration, ct);
        }

        // ”Ô†‚Ì—§‚¿ŠG‚ğ‘Şê
        public async UniTask On(StandHideCommand command, CancellationToken ct)
        {
            await _view.HideStandAsync(command.Id, command.Duration, ct);
        }

        // ”Ô†‚Ì—§‚¿ŠG‚ğ“®ì
        public async UniTask On(StandEffectCommand command, CancellationToken ct)
        {
            await _view.PlayStandEffectAsync(command.Id, command.Effect, command.Duration, ct);
        }
    }
}