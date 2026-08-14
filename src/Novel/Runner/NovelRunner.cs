using System.Threading;
using MRubyCS;
using Cysharp.Threading.Tasks;
using VitalRouter;
using VitalRouter.MRuby;

namespace RaruLib
{
    public sealed class NovelRunner : INovelRunner
    {
        private readonly Router _router;
        private readonly INovelLoader _loader;

        private readonly MRubyState _mrb;
        private readonly SemaphoreSlim _playGate = new(1, 1);

        private bool _preambleLoaded;

        public NovelRunner(Router router, INovelLoader loder)
        {
            _router = router;
            _loader = loder;
            _mrb = MRubyState.Create();
            _mrb.DefineVitalRouter(x =>
            {
                x.AddCommand<DialogCommand>("dialog");
                x.AddCommand<ChoiceCommand>("choice");
            });
        }

        // 再生
        public async UniTask PlayAsync(string key, CancellationToken ct)
        {
            await _playGate.WaitAsync(ct);
            try
            {
                EnsurePreamble();
                byte[] bytecode = _loader.LoadScenario(key);
                var irep = _mrb.ParseBytecode(bytecode);
                await _mrb.ExecuteAsync(_router, irep, ct);
            }
            finally
            {
                _playGate.Release();
            }
        }

        // マクロの内容を取得
        private void EnsurePreamble()
        {
            if (_preambleLoaded) return;

            // マクロ取得
            byte[] bytecode = _loader.LoadPreamble();
            // マクロを渡す
            _mrb.LoadBytecode(bytecode);

            _preambleLoaded = true;
        }
    }
}