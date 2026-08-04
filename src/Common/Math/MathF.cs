using UnityEngine;

namespace RaruLib
{
    public static class MathF
    {
        /// <summary>
        /// しきい値aに対するx（範囲内=0(true), 範囲外=1(false)）
        /// </summary>
        public static bool Step(float x, float a)
        {
            return a < x ? true : false;
        }
    }
}