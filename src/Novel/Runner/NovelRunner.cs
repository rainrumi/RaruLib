using System;
using System.Threading;
using MRubyCS;
using Cysharp.Threading.Tasks;
using VitalRouter;
using VitalRouter.MRuby;

namespace RaruLib
{
    public sealed class NovelRunner : INovelRunner, IDisposable
    {
        private readonly Router _router;
        private readonly INovelLoader _loader;

        private readonly MRubyState _mrb;
        private readonly SemaphoreSlim _playGate = new(1, 1);
        private readonly object _lifetimeLock = new();

        private bool _preambleLoaded;
        private int _activePlays;
        private bool _isDisposed;
        private bool _resourcesDisposed;

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
            lock (_lifetimeLock)
            {
                if (_isDisposed)
                {
                    throw new ObjectDisposedException(nameof(NovelRunner));
                }
                _activePlays++;
            }

            bool hasGate = false;
            try
            {
                await _playGate.WaitAsync(ct);
                hasGate = true;
                EnsurePreamble();
                byte[] bytecode = _loader.LoadScenario(key);
                var irep = _mrb.ParseBytecode(bytecode);
                await _mrb.ExecuteAsync(_router, irep, ct);
            }
            finally
            {
                if (hasGate)
                {
                    _playGate.Release();
                }
                lock (_lifetimeLock)
                {
                    _activePlays--;
                    TryDisposeResources();
                }
            }
        }

        /// <summary>
        /// 新しい再生受付を終了し、所有するVMとgateを解放する。
        /// 受付済み再生はcallerの取消で終了させ、待機を含む全再生の決着まで解放を遅延する。
        /// </summary>
        public void Dispose()
        {
            lock (_lifetimeLock)
            {
                _isDisposed = true;
                TryDisposeResources();
            }
        }

        // 全再生が終了してから所有資源を解放
        private void TryDisposeResources()
        {
            if (!_isDisposed || _activePlays != 0 || _resourcesDisposed)
            {
                return;
            }
            _resourcesDisposed = true;
            try
            {
                _mrb.Dispose();
            }
            finally
            {
                _playGate.Dispose();
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
