using UnityEngine;
using UnityEngine.UI;

namespace RaruLib
{
    [RequireComponent(typeof(Graphic))]
    public class KeepWorldRotation : MonoBehaviour
    {
        private Graphic _graphics;

        private void Awake()
        {
            _graphics = GetComponent<Graphic>();
        }

        private void Update()
        {
            _graphics.rectTransform.rotation = Quaternion.identity;
        }
    }
}