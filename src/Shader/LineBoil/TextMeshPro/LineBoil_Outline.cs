using TMPro;
using UnityEngine;

namespace RaruLib
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class LineBoil_Outline : MonoBehaviour
    {
        private TextMeshProUGUI tmp;
        private Material material;

        [SerializeField] private float amount = 0.02f;
        [SerializeField] private float factor = 0.05f;
        [SerializeField] private float fps = 10f;
        [SerializeField] private Color outlineColor = Color.black;
        [SerializeField] private float outlineWidth = 1.0f;

        private int _amountId = Shader.PropertyToID("_Amount");
        private int _factorId = Shader.PropertyToID("_Factor");
        private int _fpsId = Shader.PropertyToID("_fps");
        private int _faceColorId = Shader.PropertyToID("_FaceColor");
        private int _outlineColorId = Shader.PropertyToID("_OutlineColor");
        private int _outlineWidthId = Shader.PropertyToID("_OutlineWidth");

        private void Start()
        {
            tmp = GetComponent<TextMeshProUGUI>();
            UpdateMaterial();
        }

        protected virtual void UpdateMaterial()
        {
            Shader s = Shader.Find("RaruLib/LineBoil/LineBoil_Outline");
            material = new Material(tmp.fontSharedMaterial);
            material.shader = s;
            material.hideFlags = HideFlags.HideAndDontSave;

            // プロパティのセット
            material.SetFloat(_amountId, amount);
            material.SetFloat(_factorId, factor);
            material.SetFloat(_fpsId, fps);
            // TMPのvertexColorをそのままベース色にするため、マテリアル側は中立色に固定する。
            material.SetColor(_faceColorId, Color.white);
            material.SetColor(_outlineColorId, outlineColor);
            material.SetFloat(_outlineWidthId, outlineWidth);

            tmp.fontSharedMaterial = material;
        }
    }
}
