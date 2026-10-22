using MRubyCS.Serializer;
using VitalRouter;

namespace RaruLib
{
    /// <summary>Rubyが指定したメッセージボックスの表示・文字消去要求を運ぶ。</summary>
    [MRubyObject]
    public partial struct MessageBoxCommand : ICommand
    {
        public bool IsVisible; // 表示指定
        public bool ClearText; // 文字消去指定
    }
}
