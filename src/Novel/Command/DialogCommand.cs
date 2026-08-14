using MRubyCS.Serializer;
using VitalRouter;

namespace RaruLib
{
    // ダイアログ
    [MRubyObject]
    public partial struct DialogCommand : ICommand
    {
        public string Name;
        public string Text;
    }
}