using System;
using UnityEngine;

namespace RaruLib
{
    /// <summary>元のキャンバスで共通の部位名と切り出し範囲を保持する。</summary>
    [Serializable]
    public struct NovelStandPartData
    {
        [SerializeField, Tooltip("差分画像のファイル名に使う部位名")]
        private string _name; // 部位名
        [SerializeField, Tooltip("元画像の左上を原点とするピクセル座標とサイズ")]
        private RectInt _placementRect; // 元画像内の範囲

        public string Name => _name;
        public RectInt PlacementRect => _placementRect;
    }
}
