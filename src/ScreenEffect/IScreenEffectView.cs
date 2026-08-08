using Cysharp.Threading.Tasks;
using System.Threading;

public interface IScreenEffectView
{
    UniTask FadeInAsync(float duration, CancellationToken _ct);
    UniTask FadeOutAsync(float duration, CancellationToken _ct);
}
