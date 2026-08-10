using UnityEngine;
using UnityEngine.UI;

namespace RaruLib
{
    public class LineBoil : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer sprite;
        [SerializeField] private Image image;
        [SerializeField] private float amount = 0.01f;
        [SerializeField] private float factor = 10.0f;
        [SerializeField] private float fps = 10f;
        [SerializeField] private Vector2 noiseVector = new Vector2(2,2);

        [SerializeField] private bool useRandomNoise = true;

        private Material material;

        private int _mainTexId = Shader.PropertyToID("_MainTex");
        private int _amountId = Shader.PropertyToID("_Amount");
        private int _factorId = Shader.PropertyToID("_Factor");
        private int _fpsId = Shader.PropertyToID("_fps");
        private int _noiseVecId = Shader.PropertyToID("_NoiseVector");

        private void Start()
        {
            UpdateMaterial();
        }

        protected virtual void UpdateMaterial()
        {
            Shader s = Shader.Find("RaruLib/LineBoil/LineBoil");
            if (s == null)
            {
                Debug.LogError($"{name}: LineBoil shader was not found.", this);
                return;
            }

            Texture targetTexture;
            if (sprite != null)
            {
                if (sprite.sprite == null)
                {
                    Debug.LogError($"{name}: The assigned SpriteRenderer has no Sprite.", this);
                    return;
                }

                targetTexture = sprite.sprite.texture;
            }
            else if (image != null)
            {
                if (image.sprite == null)
                {
                    Debug.LogError($"{name}: The assigned Image has no Sprite.", this);
                    return;
                }

                targetTexture = image.sprite.texture;
            }
            else
            {
                Debug.LogError($"{name}: Assign either a SpriteRenderer or Image to LineBoil.", this);
                return;
            }

            material = new Material(s);
            material.hideFlags = HideFlags.HideAndDontSave;

            Vector2 _noiseVector = noiseVector;
            // ランダムノイズ有効時のみランダムノイズ座標の生成
            if (useRandomNoise)
            {
                var x = Random.Range(1f, 100f);
                var y = Random.Range(1f, 100f);
                _noiseVector = new Vector2(x, y);
            }

            material.SetTexture(_mainTexId, targetTexture);
            material.SetFloat(_amountId, amount);
            material.SetFloat(_factorId, factor);
            material.SetFloat(_fpsId, fps);
            material.SetVector(_noiseVecId, _noiseVector);

            if (sprite != null)
            {
                sprite.sharedMaterial = material;
            }
            else
            {
                image.material = material;
            }
        }

        private void OnDestroy()
        {
            if (material != null)
            {
                Destroy(material);
            }
        }
    }
}
