using UnityEngine;

namespace RaruLib
{
    public class DegreesConvert
    {
        public static float DegToRadConvert(float degrees)
        {
            return degrees * (float)(Mathf.PI / 180.0);
        }

        public static float RadToDegConvert(float radians)
        {
            return radians * (float)(180.0 / Mathf.PI);
        }
    }
}