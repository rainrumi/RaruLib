using MRubyCS.Serializer;
using VitalRouter;

namespace RaruLib
{
    /// <summary>Rubyが指定したメッセージボックスの表示状態を運ぶ。</summary>
    [MRubyObject]
    public partial struct MessageBoxCommand : ICommand
    {
        public bool IsVisible; // 表示指定
    }
}
