using System;

namespace Ricave.Core
{
    public static class Noise
    {
        static Noise()
        {
            Noise.noise.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
        }

        public static float PerlinNoise(float x, float y)
        {
            x *= 100f;
            y *= 100f;
            return (Noise.noise.GetNoise(x, y) + 1f) / 2f;
        }

        public static float PerlinNoiseMinusOneToOne(float x, float y)
        {
            return Noise.PerlinNoise(x, y) * 2f - 1f;
        }

        private static FastNoiseLite noise = new FastNoiseLite(736489314);
    }
}