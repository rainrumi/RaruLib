using System;
using System.Collections.Generic;
using UnityEngine;

namespace RaruLib
{
    /// <summary>シナリオで選ぶキーと、共通部位の順に並んだ差分Spriteを保持する。</summary>
    [Serializable]
    public struct NovelStandTextureSetData
    {
        [SerializeField, Tooltip("シナリオから指定するセットキー")]
        private string _key; // セットキー
        [SerializeField, Tooltip("共通部位と同じ順に並べた差分Sprite")]
        private Sprite[] _sprites; // 差分Sprite

        public string Key => _key;
        public IReadOnlyList<Sprite> Sprites => _sprites;
    }
}
