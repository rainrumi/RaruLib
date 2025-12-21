using System;
using UniRx;
using UnityEngine;

namespace RaruLib
{
    public class Retry : MonoBehaviour
    {
        public static Retry instance;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(this);
            }
        }

        private Subject<Unit> retrySubject = new Subject<Unit>();

        // リトライイベント
        public IObservable<Unit> OnRetry
        {
            get
            {
                return retrySubject;
            }
        }

        // リトライを呼ぶ
        public void CallRetry()
        {
            if (SceneController.instance == null)
            {
                Debug.Log("リトライ失敗。シーン遷移用スクリプトがない", gameObject);
                return;
            }

            SceneController.instance.SceneReLoad();
            retrySubject.OnNext(Unit.Default);
        }
    }
}