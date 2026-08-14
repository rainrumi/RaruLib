using Cysharp.Threading.Tasks;
using System.Threading;

namespace RaruLib
{
    public interface INovelRunner
    {
        UniTask PlayAsync(string key, CancellationToken ct);
    }
}