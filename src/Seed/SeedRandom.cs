using UnityEngine;

namespace RaruLib
{
    public class SeedRandom
    {
        // ƒL[
        public class Key
        {
            private uint _key;

            public uint key
            {
                get
                {
                    _key += 1;
                    return _key;
                }
            }
            public Key(uint key)
            {
                this._key = key;
            }
        }

        // *************************************************************

        private Key _key;
        public Key key => _key;

        private readonly ulong _seed;

        // *************************************************************

        public SeedRandom(ulong seed, uint key = 0)
        {
            _key = new Key(key);
            _seed = seed;
        }

        // *************************************************************

        // uint‹^Ž——”¶¬
        public uint RandomUInt()
        { return (uint)(Mix(_seed ^ key.key) >> 32); }

        // int‹^Ž——”¶¬
        public int RandomInt(int minimum, int maximum)
        {
            uint value = RandomUInt();
            return minimum + (int)(value % (uint)(maximum - minimum));
        }

        // float‹^Ž——”¶¬
        public float RandomFloat(float minimum, float maximum)
        {
            uint value = RandomUInt();
            float normalized = (value >> 8) * (1f / 16_777_216f);
            return minimum + (maximum - minimum) * normalized;
        }

        // *************************************************************

        // ‹¤’Ê‹^Ž——”
        public static ulong Mix(ulong value)
        {
            value += 0x9E3779B97F4A7C15UL;

            value = (value ^ (value >> 30))
                * 0xBF58476D1CE4E5B9UL;

            value = (value ^ (value >> 27))
                * 0x94D049BB133111EBUL;

            return value ^ (value >> 31);
        }

    }
}