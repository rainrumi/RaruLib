using UnityEngine;

namespace RaruLib
{
    public static class DegreesConvert
    {
        /// <summary>
        /// Degrees‚©‚çRadian‚É•ÏŠ·
        /// </summary>
        public static float DegToRadConvert(float degrees)
        {
            return degrees * (float)(Mathf.PI / 180.0);
        }

        /// <summary>
        /// Radian‚©‚çDegrees‚É•ÏŠ·
        /// </summary>
        public static float RadToDegConvert(float radians)
        {
            return radians * (float)(180.0 / Mathf.PI);
        }
    }
}