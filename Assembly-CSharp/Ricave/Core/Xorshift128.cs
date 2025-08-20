using System;

namespace Ricave.Core
{
    public static class Xorshift128
    {
        public static ValueTuple<uint, uint, uint, uint> State
        {
            get
            {
                return new ValueTuple<uint, uint, uint, uint>(Xorshift128.x, Xorshift128.y, Xorshift128.z, Xorshift128.w);
            }
            set
            {
                Xorshift128.x = value.Item1;
                Xorshift128.y = value.Item2;
                Xorshift128.z = value.Item3;
                Xorshift128.w = value.Item4;
            }
        }

        public static void InitStateWithSeed(int seed)
        {
            Xorshift128.x = (uint)seed;
            Xorshift128.y = Xorshift128.x * 1812433253U + 1U;
            Xorshift128.z = Xorshift128.y * 1812433253U + 1U;
            Xorshift128.w = Xorshift128.z * 1812433253U + 1U;
        }

        public static uint NextUint()
        {
            uint num = Xorshift128.x ^ (Xorshift128.x << 11);
            Xorshift128.x = Xorshift128.y;
            Xorshift128.y = Xorshift128.z;
            Xorshift128.z = Xorshift128.w;
            Xorshift128.w = Xorshift128.w ^ (Xorshift128.w >> 19) ^ num ^ (num >> 8);
            return Xorshift128.w;
        }

        public static uint NextUint(uint maxExclusive)
        {
            if (maxExclusive == 0U)
            {
                return 0U;
            }
            return Xorshift128.NextUint() % maxExclusive;
        }

        public static uint NextUint(uint minInclusive, uint maxExclusive)
        {
            if (maxExclusive - minInclusive == 0U)
            {
                return minInclusive;
            }
            if (maxExclusive < minInclusive)
            {
                return minInclusive - Xorshift128.NextUint() % (maxExclusive + minInclusive);
            }
            return minInclusive + Xorshift128.NextUint() % (maxExclusive - minInclusive);
        }

        public static int NextInt()
        {
            return (int)(Xorshift128.NextUint() % 2147483647U);
        }

        public static int NextInt(int maxExclusive)
        {
            return Xorshift128.NextInt() % maxExclusive;
        }

        public static int NextInt(int minInclusive, int maxExclusive)
        {
            if (maxExclusive - minInclusive == 0)
            {
                return minInclusive;
            }
            long num = (long)minInclusive;
            long num2 = (long)maxExclusive;
            long num3 = (long)((ulong)Xorshift128.NextUint());
            if (maxExclusive < minInclusive)
            {
                return (int)(num - num3 % (num2 - num));
            }
            return (int)(num + num3 % (num2 - num));
        }

        public static float NextFloat()
        {
            return 1f - Xorshift128.NextFloat(0f, 1f);
        }

        public static float NextFloat(float min, float max)
        {
            return (min - max) * ((Xorshift128.NextUint() << 9) / 4.2949673E+09f) + max;
        }

        private static uint x;

        private static uint y;

        private static uint z;

        private static uint w;

        private const uint InitializationMultiplierMT19937 = 1812433253U;
    }
}