using UnityEngine;
using UnityEngine.Events;

namespace RaruLib
{
    public class CommandTrigger : Command
    {
        [SerializeField] private UnityEvent m_event;
        public override void LogOutForGame()
        { base.LogOutForGame(); }
        public override void CallPanelChange_SendMain()
        { base.CallPanelChange_SendMain(); }
        public override void CallPanelChange_SendMenu()
        { base.CallPanelChange_SendMenu(); }
    }
}