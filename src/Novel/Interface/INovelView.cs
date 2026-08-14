using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using VitalRouter;

namespace RaruLib
{
    // ノベルの表示
    public interface INovelView
    {
        // ダイアログ表示
        UniTask ShowDialogAsync(string name, string text, CancellationToken ct);

        // 選択肢表示
        UniTask<int> ShowChoiceAsync(IReadOnlyList<string> options, CancellationToken ct);
    }
}