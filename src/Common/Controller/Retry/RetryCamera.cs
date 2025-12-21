using UnityEngine;

[RequireComponent(typeof(Camera))]
public class RetryCamera : RetryEntity
{
    private Camera _camera;
    [SerializeField] private float[] retryCameraSize = new float[(int)SaveSpotKind.MAX];

    private void Start()
    {
        base.Start();

        _camera = GetComponent<Camera>();

        this.MoveResetSpot();
    }

    private void MoveResetSpot()
    {
        _camera.orthographicSize = retryCameraSize[(int)GameData.instance.saveSpot];
    }
}
