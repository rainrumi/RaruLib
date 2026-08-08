using System;
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

        public static float Clamp(float value, float min,float max)
        {
            if (min > max)
                throw new ArgumentException("min must be less than or equal to max.");
            return Min(Max(value, min), max);
        }

        public static float Min(float a, float b)
        {
            return (a > b) ? b : a;
        }

        public static float Max(float a, float b)
        {
            return (a > b) ? a : b;
        }
    }
}