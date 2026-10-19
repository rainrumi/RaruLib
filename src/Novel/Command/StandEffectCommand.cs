using MRubyCS.Serializer;
using VitalRouter;

namespace RaruLib
{
    /// <summary>Rubyから受け取る画像コマンドの値だけを保持する。</summary>
    [MRubyObject]
    public partial struct StandEffectCommand : ICommand
    {
        public int Id; // 管理番号
        public string Effect; // 演出名
        public float Duration; // 演出秒数
    }
}
