using UnityEngine;

namespace RaruLib
{
    public class SoundSliderCommand : MonoBehaviour
    {
        private bool activeSound => Sound.instance != null;

        public void ChangeBgmVol(float value)
        {
            if (!activeSound) { Debug.Log("Soundがありません", gameObject); return; }
            Sound.instance.ChangeVolume("BGM", value);
        }

        public void ChangeSeVol(float value)
        {
            if (!activeSound) { Debug.Log("Soundがありません", gameObject); return; }
            Sound.instance.ChangeVolume("SE", value);
        }

        /// <summary>設定画面のマスター音量を全体へ反映する。</summary>
        public void ChangeMasterVol(float value)
        {
            if (!activeSound)
            {
#if UNITY_EDITOR
                Debug.Log("Soundがありません", gameObject);
#endif
                return;
            }
            Sound.instance.ChangeMasterVolume(value);
        }

    }
}