using UnityEngine;

namespace RaruLib
{
    [CreateAssetMenu(fileName = nameof(NovelInitializeInfo), menuName = "Game/Novel/" + nameof(NovelInitializeInfo))]
    public class NovelInitializeInfo : ScriptableObject, INovelInitializeStatus
    {
        [SerializeField, Tooltip("•¶ŽšÄ¶‘¬“x"), Range(0.0f, 1.0f)] private float _charSpeed;
        public float CharSpeed => _charSpeed;

        [SerializeField, Tooltip("•¶ŽšŠÔ’âŽ~Å‘åŽžŠÔ")] private float _maxCharWait;
        public float MaxCharWait => _maxCharWait;

        [SerializeField, Tooltip("ƒI[ƒg’âŽ~ŽžŠÔ")] private float _autoWait;
        public float AutoWait => _autoWait;

        [SerializeField, Tooltip("ƒI[ƒg•¶Žš”’Ç‰Á’âŽ~ŽžŠÔ")] private float _autoWaitPerChar;
        public float AutoWaitPerChar => _autoWaitPerChar;

        [SerializeField, Tooltip("‰ŠúƒI[ƒgó‘Ô")] private bool _isAuto;
        public bool IsAuto => _isAuto;

        [SerializeField, Tooltip("Ä¶Žž‚Ì‰¹–¼i¦–¢Žg—p‚È‚ç‹ó•¶Žšj")] private string _popopo;
        public string Popopo => _popopo;

        [SerializeField, Tooltip("—§‚¿ŠG‚ª“oêŽž‚ÉˆÚ“®‚·‚éCanvasã‚Ì‹——£"), Min(0.0f)]
        private float _standSlideDistance = 160.0f; // “oê‚ÌˆÚ“®‹——£
        public float StandSlideDistance => _standSlideDistance;

        [SerializeField, Tooltip("—§‚¿ŠG‚ªƒWƒƒƒ“ƒv‚·‚éCanvasã‚Ì‚‚³"), Min(0.0f)]
        private float _standJumpHeight = 48.0f; // ƒWƒƒƒ“ƒv‚Ì‚‚³
        public float StandJumpHeight => _standJumpHeight;

        [SerializeField, Tooltip("—§‚¿ŠG‚ª¶‰E‚É—h‚ê‚éCanvasã‚Ì‹——£"), Min(0.0f)]
        private float _standShakeDistance = 24.0f; // —h‚ê‚Ì‹——£
        public float StandShakeDistance => _standShakeDistance;
    }
}