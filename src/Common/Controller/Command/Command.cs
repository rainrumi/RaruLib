using Cysharp.Threading.Tasks;
using RaruLib;
using System;
using UnityEngine;

namespace RaruLib
{
    public class Command : MonoBehaviour
    {
        protected void Start()
        {

        }
        public virtual void LogOutForGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }
        public virtual void CallPanelChange_SendMain()
        {
            if (ScenePanel.instance == null)
            {
                Debug.Log("ƒpƒlƒ‹‘JˆÚŽ¸”s", gameObject);
                return;
            }
            ScenePanel.instance.DataCallOpenPanel(PanelKind.Main);
        }
        public virtual void CallPanelChange_SendMenu()
        {
            if (ScenePanel.instance == null)
            {
                Debug.Log("ƒpƒlƒ‹‘JˆÚŽ¸”s", gameObject);
                return;
            }
            ScenePanel.instance.DataCallOpenPanel(PanelKind.Menu);
        }
    }
}