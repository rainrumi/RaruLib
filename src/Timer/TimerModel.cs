using R3;
using UnityEngine;

namespace RaruLib
{
    public enum TimerKind
    {
        Active,
        InActive
    }

    public class TimerModel : MonoBehaviour
    {
        public const float MAX_TIMER = 40;          // 最大制限時間
        public const float MAX_ENDLESS_TIMER = 21;          // エンドレス最大制限時間

        private Subject<float> OnUpdateTimeSubject = new();     // タイマーの更新
        public Observable<float> OnUpdateTime => OnUpdateTimeSubject.AsObservable();

        private Subject<Unit> OnTimeUpSubject = new();          // タイマーのカウントアップ
        public Observable<Unit> OnTimeUp => OnTimeUpSubject.AsObservable();

        private TimerKind _timerKind = TimerKind.InActive;               // タイマーの状態
        public TimerKind TimerKind { get => _timerKind; set { _timerKind = value; } }

        private float _nowTimer;

        public float NowTimer                       // 現在の制限時間
        {
            get => _nowTimer;
            set
            {
                _nowTimer = value;
            }
        }

        public void Update()
        {
            if (_timerKind == TimerKind.Active)
            {
                Debug.LogError("コメントアウトを解除してください");
                /*
                NowTimer = Mathf.Max(0f, NowTimer - Time.deltaTime * GameSceneData.Instance.GameSpeed);
                OnUpdateTimeSubject.OnNext(NowTimer);

                if (_nowTimer <= 0)
                {
                    TimerKind = TimerKind.InActive;
                    OnTimeUpSubject.OnNext(Unit.Default);
                }
                */
            }
        }
    }
}