using MRubyCS.Serializer;
using VitalRouter;

namespace RaruLib
{
    /// <summary>Rubyから受け取る画像コマンドの値だけを保持する。</summary>
    [MRubyObject]
    public partial struct StandCommand : ICommand
    {
        public int Id; // 管理番号
        public string Path; // 画像パス
        public string Position; // 表示位置
        public string Effect; // 演出名
        public float Duration; // 演出秒数
    }
}
