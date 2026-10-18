using MRubyCS.Serializer;
using VitalRouter;

namespace RaruLib
{
    /// <summary>Rubyから受け取る画像コマンドの値だけを保持する。</summary>
    [MRubyObject]
    public partial struct BgCommand : ICommand
    {
        public string Path; // 画像パス
        public float FadeOut; // 退場秒数
        public float FadeIn; // 登場秒数
    }
}
