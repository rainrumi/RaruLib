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

        /// <summary>背景の切替完了まで待つ。</summary>
        UniTask ShowBackgroundAsync(string path, float fadeOut, float fadeIn, CancellationToken ct);

        /// <summary>管理番号の立ち絵を表示し、登場完了まで待つ。</summary>
        UniTask ShowStandAsync(int id, string path, string position, string effect, float duration, CancellationToken ct);

        /// <summary>管理番号の立ち絵を非表示にする。</summary>
        UniTask HideStandAsync(int id, float duration, CancellationToken ct);

        /// <summary>管理番号の立ち絵の動作完了まで待つ。</summary>
        UniTask PlayStandEffectAsync(int id, string effect, float duration, CancellationToken ct);
    }
}