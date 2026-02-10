using TMPro;
using UnityEngine;

namespace RaruLib
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class LineBoilTMP : MonoBehaviour
    {
        private TextMeshProUGUI tmp;
        private Material material;

        [SerializeField] private float amount = 0.02f;
        [SerializeField] private float factor = 0.05f;
        [SerializeField] private float fps = 10f;

        private int _amountId = Shader.PropertyToID("_Amount");
        private int _factorId = Shader.PropertyToID("_Factor");
        private int _fpsId = Shader.PropertyToID("_fps");

        private void Start()
        {
            tmp = GetComponent<TextMeshProUGUI>();
            UpdateMaterial();
        }

        protected virtual void UpdateMaterial()
        {
            Shader s = Shader.Find("RaruLib/LineBoil/LineBoilTMP");
            material = new Material(tmp.fontSharedMaterial);
            material.shader = s;
            material.hideFlags = HideFlags.HideAndDontSave;

            // プロパティのセット
            material.SetFloat(_amountId, amount);
            material.SetFloat(_factorId, factor);
            material.SetFloat(_fpsId, fps);

            tmp.fontSharedMaterial = material;
        }
    }
}