using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Graphic))]
public class LoopMoveUi : MonoBehaviour
{
    private Graphic graphic;
    private Vector3 _cachedPosition;

    private bool finishPosX => graphic.rectTransform.position.x < warpEntrance.position.x && graphic.rectTransform.position.x < _cachedPosition.x 
        || graphic.rectTransform.position.x > warpEntrance.position.x && graphic.rectTransform.position.x > _cachedPosition.x;
    private bool finishPosY => graphic.rectTransform.position.y < warpEntrance.position.y && graphic.rectTransform.position.y < _cachedPosition.y 
        || graphic.rectTransform.position.y > warpEntrance.position.y && graphic.rectTransform.position.y > _cachedPosition.y;
    private bool finishPosZ => graphic.rectTransform.position.z < warpEntrance.position.z && graphic.rectTransform.position.z < _cachedPosition.z 
        || graphic.rectTransform.position.z > warpEntrance.position.z && graphic.rectTransform.position.z > _cachedPosition.z;

    [SerializeField] protected Vector3 moveSpeed = new Vector3(0, 10.0f, 0);
    [SerializeField] private RectTransform warpEntrance;
    [SerializeField] private RectTransform warpExit;

    private void Awake()
    {
        graphic = GetComponent<Graphic>();
    }

    private void Update()
    {
        var addVector = moveSpeed * Time.deltaTime;
        graphic.rectTransform.position += addVector;
        Debug.Log($"{addVector}‰ÁŽZ");

        if (finishPosX || finishPosY || finishPosZ)
        {
            WarpToPosition();
        }

        _cachedPosition = graphic.rectTransform.position;
    }

    private void WarpToPosition()
    {
        graphic.rectTransform.position = warpExit.position;
    }
}
