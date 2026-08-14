using System;
using UnityEngine;

namespace RaruLib
{
    public class NovelLoader : INovelLoader
    {
        // マクロをロード
        public byte[] LoadPreamble()
        {
            return LoadMrb("Novel/Preamble");
        }

        // シナリオをロード
        public byte[] LoadScenario(string key)
        {
            return LoadMrb($"Novel/Scenarios/{key}");
        }

        // mrb拡張子データをロード
        private static byte[] LoadMrb(string path)
        {
            var assets = Resources.LoadAll<TextAsset>(path);

            foreach (var asset in assets)
            {
                if (asset.name.EndsWith(".mrb", StringComparison.Ordinal))
                {
                    // mrbファイルを返す
                    return asset.bytes;
                }
            }

            // 失敗時はエラー
            throw new InvalidOperationException($"MRuby bytecode not found: {path}");
        }
    }
}