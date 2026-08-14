using UnityEngine;

namespace RaruLib
{
    [CreateAssetMenu(fileName = nameof(NovelInitializeInfo), menuName = "Game/Novel/" + nameof(NovelInitializeInfo))]
    public class NovelInitializeInfo : ScriptableObject, INovelInitializeStatus
    {
        [SerializeField, Tooltip("文字再生速度"), Range(0.0f, 1.0f)] private float _charSpeed;
        public float CharSpeed => _charSpeed;

        [SerializeField, Tooltip("文字間停止最大時間")] private float _maxCharWait;
        public float MaxCharWait => _maxCharWait;

        [SerializeField, Tooltip("オート停止時間")] private float _autoWait;
        public float AutoWait => _autoWait;

        [SerializeField, Tooltip("オート文字数追加停止時間")] private float _autoWaitPerChar;
        public float AutoWaitPerChar => _autoWaitPerChar;

        [SerializeField, Tooltip("初期オート状態")] private bool _isAuto;
        public bool IsAuto => _isAuto;

        [SerializeField, Tooltip("再生時の音名（※未使用なら空文字）")] private string _popopo;
        public string Popopo => _popopo;
    }
}