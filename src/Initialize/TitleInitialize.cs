using UnityEngine;

namespace RaruLib
{
    [RequireComponent(typeof(SoundCommand))]
    public class TitleInitialize : MonoBehaviour
    {
        private SoundCommand _soundCommand;
        [SerializeField] private string _bgmName = "BGM1";

        private void Start()
        {
            _soundCommand = GetComponent<SoundCommand>();
            _soundCommand.CallPlayBGM(_bgmName);
        }
    }
}
