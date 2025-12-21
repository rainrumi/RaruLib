using RaruLib;
using UnityEngine;
using UniRx;

public class RetryEntity : MonoBehaviour
{
    protected Retry retry;

    [SerializeField] protected Vector3[] retrySpot = new Vector3[(int)SaveSpotKind.MAX];

    protected void Start()
    {
        if (Retry.instance != null)
        {
            retry = Retry.instance;
        }
        else
        {
            Debug.Log("小川：Retryが存在しません", gameObject);
        }

        MoveResetSpot();    // セーブされている初期位置に移動

        retry.OnRetry
            .Subscribe(kind => {
                MoveResetSpot();
            })
            .AddTo(this); ;
    }

    // 位置をリセット
    protected void MoveResetSpot()
    {
        if (GameData.instance == null)
        {
            Debug.Log("プレイヤー位置リセット失敗。ゲームデータがない", gameObject);
        }

        transform.position = retrySpot[(int)GameData.instance.saveSpot];
    }
}
