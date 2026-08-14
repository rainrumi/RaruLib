using System.Collections.Generic;
using MRubyCS.Serializer;
using VitalRouter;

namespace RaruLib
{
    // 選択肢
    [MRubyObject]
    public partial struct ChoiceCommand : ICommand
    {
        public IReadOnlyList<string> Options;
        public string StateKey;
    }
}