using System;
using System.Collections.Generic;
using UnityEngine;

namespace RaruLib
{
    /// <summary>一人分の共通素体と、キーで切り替える差分セットを保持する。</summary>
    [CreateAssetMenu(fileName = "NovelStandInfo", menuName = "Game/Novel/NovelStandInfo")]
    public sealed class NovelStandInfo : ScriptableObject
    {
        [SerializeField, Tooltip("シナリオの立ち絵コマンドで指定するキャラクター名")]
        private string _characterKey; // キャラクター名
        [SerializeField, Tooltip("元の立ち絵PNGの幅と高さ")]
        private Vector2Int _canvasSize; // 元の画像サイズ
        [SerializeField, Tooltip("差分を配置する領域を透過にした共通素体")]
        private Sprite _body; // 共通素体
        [SerializeField, Tooltip("全部の差分セットで共通の配置。配列順に描画する")]
        private NovelStandPartData[] _parts = Array.Empty<NovelStandPartData>(); // 部位一覧
        [SerializeField, Tooltip("キーと部位ごとのSpriteの組み合わせ")]
        private NovelStandTextureSetData[] _textureSets = Array.Empty<NovelStandTextureSetData>(); // 差分セット

        public string CharacterKey => _characterKey;
        public Vector2Int CanvasSize => _canvasSize;
        public Sprite Body => _body;
        public IReadOnlyList<NovelStandPartData> Parts => _parts;
        public IReadOnlyList<NovelStandTextureSetData> TextureSets => _textureSets;

        /// <summary>指定されたセットキーに対応する差分を取得する。</summary>
        public bool TryGetTextureSet(string key, out NovelStandTextureSetData textureSet)
        {
            foreach (NovelStandTextureSetData candidate in _textureSets)
            {
                if (string.Equals(candidate.Key, key, StringComparison.Ordinal))
                {
                    textureSet = candidate;
                    return true;
                }
            }

            textureSet = default;
            return false;
        }
    }
}
