using System;
using System.Runtime.CompilerServices;

public class FastNoiseLite
{
    public FastNoiseLite(int seed = 1337)
    {
        this.SetSeed(seed);
    }

    public void SetSeed(int seed)
    {
        this.mSeed = seed;
    }

    public void SetFrequency(float frequency)
    {
        this.mFrequency = frequency;
    }

    public void SetNoiseType(FastNoiseLite.NoiseType noiseType)
    {
        this.mNoiseType = noiseType;
        this.UpdateTransformType3D();
    }

    public void SetRotationType3D(FastNoiseLite.RotationType3D rotationType3D)
    {
        this.mRotationType3D = rotationType3D;
        this.UpdateTransformType3D();
        this.UpdateWarpTransformType3D();
    }

    public void SetFractalType(FastNoiseLite.FractalType fractalType)
    {
        this.mFractalType = fractalType;
    }

    public void SetFractalOctaves(int octaves)
    {
        this.mOctaves = octaves;
        this.CalculateFractalBounding();
    }

    public void SetFractalLacunarity(float lacunarity)
    {
        this.mLacunarity = lacunarity;
    }

    public void SetFractalGain(float gain)
    {
        this.mGain = gain;
        this.CalculateFractalBounding();
    }

    public void SetFractalWeightedStrength(float weightedStrength)
    {
        this.mWeightedStrength = weightedStrength;
    }

    public void SetFractalPingPongStrength(float pingPongStrength)
    {
        this.mPingPongStrength = pingPongStrength;
    }

    public void SetCellularDistanceFunction(FastNoiseLite.CellularDistanceFunction cellularDistanceFunction)
    {
        this.mCellularDistanceFunction = cellularDistanceFunction;
    }

    public void SetCellularReturnType(FastNoiseLite.CellularReturnType cellularReturnType)
    {
        this.mCellularReturnType = cellularReturnType;
    }

    public void SetCellularJitter(float cellularJitter)
    {
        this.mCellularJitterModifier = cellularJitter;
    }

    public void SetDomainWarpType(FastNoiseLite.DomainWarpType domainWarpType)
    {
        this.mDomainWarpType = domainWarpType;
        this.UpdateWarpTransformType3D();
    }

    public void SetDomainWarpAmp(float domainWarpAmp)
    {
        this.mDomainWarpAmp = domainWarpAmp;
    }

    [MethodImpl((MethodImplOptions)512)]
    public float GetNoise(float x, float y)
    {
        this.TransformNoiseCoordinate(ref x, ref y);
        switch (this.mFractalType)
        {
            case FastNoiseLite.FractalType.FBm:
                return this.GenFractalFBm(x, y);
            case FastNoiseLite.FractalType.Ridged:
                return this.GenFractalRidged(x, y);
            case FastNoiseLite.FractalType.PingPong:
                return this.GenFractalPingPong(x, y);
            default:
                return this.GenNoiseSingle(this.mSeed, x, y);
        }
    }

    [MethodImpl((MethodImplOptions)512)]
    public float GetNoise(float x, float y, float z)
    {
        this.TransformNoiseCoordinate(ref x, ref y, ref z);
        switch (this.mFractalType)
        {
            case FastNoiseLite.FractalType.FBm:
                return this.GenFractalFBm(x, y, z);
            case FastNoiseLite.FractalType.Ridged:
                return this.GenFractalRidged(x, y, z);
            case FastNoiseLite.FractalType.PingPong:
                return this.GenFractalPingPong(x, y, z);
            default:
                return this.GenNoiseSingle(this.mSeed, x, y, z);
        }
    }

    [MethodImpl((MethodImplOptions)512)]
    public void DomainWarp(ref float x, ref float y)
    {
        FastNoiseLite.FractalType fractalType = this.mFractalType;
        if (fractalType == FastNoiseLite.FractalType.DomainWarpProgressive)
        {
            this.DomainWarpFractalProgressive(ref x, ref y);
            return;
        }
        if (fractalType != FastNoiseLite.FractalType.DomainWarpIndependent)
        {
            this.DomainWarpSingle(ref x, ref y);
            return;
        }
        this.DomainWarpFractalIndependent(ref x, ref y);
    }

    [MethodImpl((MethodImplOptions)512)]
    public void DomainWarp(ref float x, ref float y, ref float z)
    {
        FastNoiseLite.FractalType fractalType = this.mFractalType;
        if (fractalType == FastNoiseLite.FractalType.DomainWarpProgressive)
        {
            this.DomainWarpFractalProgressive(ref x, ref y, ref z);
            return;
        }
        if (fractalType != FastNoiseLite.FractalType.DomainWarpIndependent)
        {
            this.DomainWarpSingle(ref x, ref y, ref z);
            return;
        }
        this.DomainWarpFractalIndependent(ref x, ref y, ref z);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static float FastMin(float a, float b)
    {
        if (a >= b)
        {
            return b;
        }
        return a;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static float FastMax(float a, float b)
    {
        if (a <= b)
        {
            return b;
        }
        return a;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static float FastAbs(float f)
    {
        if (f >= 0f)
        {
            return f;
        }
        return -f;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static float FastSqrt(float f)
    {
        return (float)Math.Sqrt((double)f);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int FastFloor(float f)
    {
        if (f < 0f)
        {
            return (int)f - 1;
        }
        return (int)f;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int FastRound(float f)
    {
        if (f < 0f)
        {
            return (int)(f - 0.5f);
        }
        return (int)(f + 0.5f);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static float Lerp(float a, float b, float t)
    {
        return a + t * (b - a);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static float InterpHermite(float t)
    {
        return t * t * (3f - 2f * t);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static float InterpQuintic(float t)
    {
        return t * t * t * (t * (t * 6f - 15f) + 10f);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static float CubicLerp(float a, float b, float c, float d, float t)
    {
        float num = d - c - (a - b);
        return t * t * t * num + t * t * (a - b - num) + t * (c - a) + b;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static float PingPong(float t)
    {
        t -= (float)((int)(t * 0.5f) * 2);
        if (t >= 1f)
        {
            return 2f - t;
        }
        return t;
    }

    private void CalculateFractalBounding()
    {
        float num = FastNoiseLite.FastAbs(this.mGain);
        float num2 = num;
        float num3 = 1f;
        for (int i = 1; i < this.mOctaves; i++)
        {
            num3 += num2;
            num2 *= num;
        }
        this.mFractalBounding = 1f / num3;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int Hash(int seed, int xPrimed, int yPrimed)
    {
        return (seed ^ xPrimed ^ yPrimed) * 668265261;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int Hash(int seed, int xPrimed, int yPrimed, int zPrimed)
    {
        return (seed ^ xPrimed ^ yPrimed ^ zPrimed) * 668265261;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static float ValCoord(int seed, int xPrimed, int yPrimed)
    {
        int num = FastNoiseLite.Hash(seed, xPrimed, yPrimed);
        int num2 = num * num;
        return (float)(num2 ^ (num2 << 19)) * 4.656613E-10f;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static float ValCoord(int seed, int xPrimed, int yPrimed, int zPrimed)
    {
        int num = FastNoiseLite.Hash(seed, xPrimed, yPrimed, zPrimed);
        int num2 = num * num;
        return (float)(num2 ^ (num2 << 19)) * 4.656613E-10f;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static float GradCoord(int seed, int xPrimed, int yPrimed, float xd, float yd)
    {
        int num = FastNoiseLite.Hash(seed, xPrimed, yPrimed);
        num ^= num >> 15;
        num &= 254;
        float num2 = FastNoiseLite.Gradients2D[num];
        float num3 = FastNoiseLite.Gradients2D[num | 1];
        return xd * num2 + yd * num3;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static float GradCoord(int seed, int xPrimed, int yPrimed, int zPrimed, float xd, float yd, float zd)
    {
        int num = FastNoiseLite.Hash(seed, xPrimed, yPrimed, zPrimed);
        num ^= num >> 15;
        num &= 252;
        float num2 = FastNoiseLite.Gradients3D[num];
        float num3 = FastNoiseLite.Gradients3D[num | 1];
        float num4 = FastNoiseLite.Gradients3D[num | 2];
        return xd * num2 + yd * num3 + zd * num4;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void GradCoordOut(int seed, int xPrimed, int yPrimed, out float xo, out float yo)
    {
        int num = FastNoiseLite.Hash(seed, xPrimed, yPrimed) & 510;
        xo = FastNoiseLite.RandVecs2D[num];
        yo = FastNoiseLite.RandVecs2D[num | 1];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void GradCoordOut(int seed, int xPrimed, int yPrimed, int zPrimed, out float xo, out float yo, out float zo)
    {
        int num = FastNoiseLite.Hash(seed, xPrimed, yPrimed, zPrimed) & 1020;
        xo = FastNoiseLite.RandVecs3D[num];
        yo = FastNoiseLite.RandVecs3D[num | 1];
        zo = FastNoiseLite.RandVecs3D[num | 2];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void GradCoordDual(int seed, int xPrimed, int yPrimed, float xd, float yd, out float xo, out float yo)
    {
        int num = FastNoiseLite.Hash(seed, xPrimed, yPrimed);
        int num2 = num & 254;
        int num3 = (num >> 7) & 510;
        float num4 = FastNoiseLite.Gradients2D[num2];
        float num5 = FastNoiseLite.Gradients2D[num2 | 1];
        float num6 = xd * num4 + yd * num5;
        float num7 = FastNoiseLite.RandVecs2D[num3];
        float num8 = FastNoiseLite.RandVecs2D[num3 | 1];
        xo = num6 * num7;
        yo = num6 * num8;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void GradCoordDual(int seed, int xPrimed, int yPrimed, int zPrimed, float xd, float yd, float zd, out float xo, out float yo, out float zo)
    {
        int num = FastNoiseLite.Hash(seed, xPrimed, yPrimed, zPrimed);
        int num2 = num & 252;
        int num3 = (num >> 6) & 1020;
        float num4 = FastNoiseLite.Gradients3D[num2];
        float num5 = FastNoiseLite.Gradients3D[num2 | 1];
        float num6 = FastNoiseLite.Gradients3D[num2 | 2];
        float num7 = xd * num4 + yd * num5 + zd * num6;
        float num8 = FastNoiseLite.RandVecs3D[num3];
        float num9 = FastNoiseLite.RandVecs3D[num3 | 1];
        float num10 = FastNoiseLite.RandVecs3D[num3 | 2];
        xo = num7 * num8;
        yo = num7 * num9;
        zo = num7 * num10;
    }

    private float GenNoiseSingle(int seed, float x, float y)
    {
        switch (this.mNoiseType)
        {
            case FastNoiseLite.NoiseType.OpenSimplex2:
                return this.SingleSimplex(seed, x, y);
            case FastNoiseLite.NoiseType.OpenSimplex2S:
                return this.SingleOpenSimplex2S(seed, x, y);
            case FastNoiseLite.NoiseType.Cellular:
                return this.SingleCellular(seed, x, y);
            case FastNoiseLite.NoiseType.Perlin:
                return this.SinglePerlin(seed, x, y);
            case FastNoiseLite.NoiseType.ValueCubic:
                return this.SingleValueCubic(seed, x, y);
            case FastNoiseLite.NoiseType.Value:
                return this.SingleValue(seed, x, y);
            default:
                return 0f;
        }
    }

    private float GenNoiseSingle(int seed, float x, float y, float z)
    {
        switch (this.mNoiseType)
        {
            case FastNoiseLite.NoiseType.OpenSimplex2:
                return this.SingleOpenSimplex2(seed, x, y, z);
            case FastNoiseLite.NoiseType.OpenSimplex2S:
                return this.SingleOpenSimplex2S(seed, x, y, z);
            case FastNoiseLite.NoiseType.Cellular:
                return this.SingleCellular(seed, x, y, z);
            case FastNoiseLite.NoiseType.Perlin:
                return this.SinglePerlin(seed, x, y, z);
            case FastNoiseLite.NoiseType.ValueCubic:
                return this.SingleValueCubic(seed, x, y, z);
            case FastNoiseLite.NoiseType.Value:
                return this.SingleValue(seed, x, y, z);
            default:
                return 0f;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void TransformNoiseCoordinate(ref float x, ref float y)
    {
        x *= this.mFrequency;
        y *= this.mFrequency;
        FastNoiseLite.NoiseType noiseType = this.mNoiseType;
        if (noiseType <= FastNoiseLite.NoiseType.OpenSimplex2S)
        {
            float num = (x + y) * 0.3660254f;
            x += num;
            y += num;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void TransformNoiseCoordinate(ref float x, ref float y, ref float z)
    {
        x *= this.mFrequency;
        y *= this.mFrequency;
        z *= this.mFrequency;
        switch (this.mTransformType3D)
        {
            case FastNoiseLite.TransformType3D.ImproveXYPlanes:
                {
                    float num = x + y;
                    float num2 = num * -0.21132487f;
                    z *= 0.57735026f;
                    x += num2 - z;
                    y = y + num2 - z;
                    z += num * 0.57735026f;
                    return;
                }
            case FastNoiseLite.TransformType3D.ImproveXZPlanes:
                {
                    float num3 = x + z;
                    float num4 = num3 * -0.21132487f;
                    y *= 0.57735026f;
                    x += num4 - y;
                    z += num4 - y;
                    y += num3 * 0.57735026f;
                    return;
                }
            case FastNoiseLite.TransformType3D.DefaultOpenSimplex2:
                {
                    float num5 = (x + y + z) * 0.6666667f;
                    x = num5 - x;
                    y = num5 - y;
                    z = num5 - z;
                    return;
                }
            default:
                return;
        }
    }

    private void UpdateTransformType3D()
    {
        FastNoiseLite.RotationType3D rotationType3D = this.mRotationType3D;
        if (rotationType3D == FastNoiseLite.RotationType3D.ImproveXYPlanes)
        {
            this.mTransformType3D = FastNoiseLite.TransformType3D.ImproveXYPlanes;
            return;
        }
        if (rotationType3D == FastNoiseLite.RotationType3D.ImproveXZPlanes)
        {
            this.mTransformType3D = FastNoiseLite.TransformType3D.ImproveXZPlanes;
            return;
        }
        FastNoiseLite.NoiseType noiseType = this.mNoiseType;
        if (noiseType <= FastNoiseLite.NoiseType.OpenSimplex2S)
        {
            this.mTransformType3D = FastNoiseLite.TransformType3D.DefaultOpenSimplex2;
            return;
        }
        this.mTransformType3D = FastNoiseLite.TransformType3D.None;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void TransformDomainWarpCoordinate(ref float x, ref float y)
    {
        FastNoiseLite.DomainWarpType domainWarpType = this.mDomainWarpType;
        if (domainWarpType <= FastNoiseLite.DomainWarpType.OpenSimplex2Reduced)
        {
            float num = (x + y) * 0.3660254f;
            x += num;
            y += num;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void TransformDomainWarpCoordinate(ref float x, ref float y, ref float z)
    {
        switch (this.mWarpTransformType3D)
        {
            case FastNoiseLite.TransformType3D.ImproveXYPlanes:
                {
                    float num = x + y;
                    float num2 = num * -0.21132487f;
                    z *= 0.57735026f;
                    x += num2 - z;
                    y = y + num2 - z;
                    z += num * 0.57735026f;
                    return;
                }
            case FastNoiseLite.TransformType3D.ImproveXZPlanes:
                {
                    float num3 = x + z;
                    float num4 = num3 * -0.21132487f;
                    y *= 0.57735026f;
                    x += num4 - y;
                    z += num4 - y;
                    y += num3 * 0.57735026f;
                    return;
                }
            case FastNoiseLite.TransformType3D.DefaultOpenSimplex2:
                {
                    float num5 = (x + y + z) * 0.6666667f;
                    x = num5 - x;
                    y = num5 - y;
                    z = num5 - z;
                    return;
                }
            default:
                return;
        }
    }

    private void UpdateWarpTransformType3D()
    {
        FastNoiseLite.RotationType3D rotationType3D = this.mRotationType3D;
        if (rotationType3D == FastNoiseLite.RotationType3D.ImproveXYPlanes)
        {
            this.mWarpTransformType3D = FastNoiseLite.TransformType3D.ImproveXYPlanes;
            return;
        }
        if (rotationType3D == FastNoiseLite.RotationType3D.ImproveXZPlanes)
        {
            this.mWarpTransformType3D = FastNoiseLite.TransformType3D.ImproveXZPlanes;
            return;
        }
        FastNoiseLite.DomainWarpType domainWarpType = this.mDomainWarpType;
        if (domainWarpType <= FastNoiseLite.DomainWarpType.OpenSimplex2Reduced)
        {
            this.mWarpTransformType3D = FastNoiseLite.TransformType3D.DefaultOpenSimplex2;
            return;
        }
        this.mWarpTransformType3D = FastNoiseLite.TransformType3D.None;
    }

    private float GenFractalFBm(float x, float y)
    {
        int num = this.mSeed;
        float num2 = 0f;
        float num3 = this.mFractalBounding;
        for (int i = 0; i < this.mOctaves; i++)
        {
            float num4 = this.GenNoiseSingle(num++, x, y);
            num2 += num4 * num3;
            num3 *= FastNoiseLite.Lerp(1f, FastNoiseLite.FastMin(num4 + 1f, 2f) * 0.5f, this.mWeightedStrength);
            x *= this.mLacunarity;
            y *= this.mLacunarity;
            num3 *= this.mGain;
        }
        return num2;
    }

    private float GenFractalFBm(float x, float y, float z)
    {
        int num = this.mSeed;
        float num2 = 0f;
        float num3 = this.mFractalBounding;
        for (int i = 0; i < this.mOctaves; i++)
        {
            float num4 = this.GenNoiseSingle(num++, x, y, z);
            num2 += num4 * num3;
            num3 *= FastNoiseLite.Lerp(1f, (num4 + 1f) * 0.5f, this.mWeightedStrength);
            x *= this.mLacunarity;
            y *= this.mLacunarity;
            z *= this.mLacunarity;
            num3 *= this.mGain;
        }
        return num2;
    }

    private float GenFractalRidged(float x, float y)
    {
        int num = this.mSeed;
        float num2 = 0f;
        float num3 = this.mFractalBounding;
        for (int i = 0; i < this.mOctaves; i++)
        {
            float num4 = FastNoiseLite.FastAbs(this.GenNoiseSingle(num++, x, y));
            num2 += (num4 * -2f + 1f) * num3;
            num3 *= FastNoiseLite.Lerp(1f, 1f - num4, this.mWeightedStrength);
            x *= this.mLacunarity;
            y *= this.mLacunarity;
            num3 *= this.mGain;
        }
        return num2;
    }

    private float GenFractalRidged(float x, float y, float z)
    {
        int num = this.mSeed;
        float num2 = 0f;
        float num3 = this.mFractalBounding;
        for (int i = 0; i < this.mOctaves; i++)
        {
            float num4 = FastNoiseLite.FastAbs(this.GenNoiseSingle(num++, x, y, z));
            num2 += (num4 * -2f + 1f) * num3;
            num3 *= FastNoiseLite.Lerp(1f, 1f - num4, this.mWeightedStrength);
            x *= this.mLacunarity;
            y *= this.mLacunarity;
            z *= this.mLacunarity;
            num3 *= this.mGain;
        }
        return num2;
    }

    private float GenFractalPingPong(float x, float y)
    {
        int num = this.mSeed;
        float num2 = 0f;
        float num3 = this.mFractalBounding;
        for (int i = 0; i < this.mOctaves; i++)
        {
            float num4 = FastNoiseLite.PingPong((this.GenNoiseSingle(num++, x, y) + 1f) * this.mPingPongStrength);
            num2 += (num4 - 0.5f) * 2f * num3;
            num3 *= FastNoiseLite.Lerp(1f, num4, this.mWeightedStrength);
            x *= this.mLacunarity;
            y *= this.mLacunarity;
            num3 *= this.mGain;
        }
        return num2;
    }

    private float GenFractalPingPong(float x, float y, float z)
    {
        int num = this.mSeed;
        float num2 = 0f;
        float num3 = this.mFractalBounding;
        for (int i = 0; i < this.mOctaves; i++)
        {
            float num4 = FastNoiseLite.PingPong((this.GenNoiseSingle(num++, x, y, z) + 1f) * this.mPingPongStrength);
            num2 += (num4 - 0.5f) * 2f * num3;
            num3 *= FastNoiseLite.Lerp(1f, num4, this.mWeightedStrength);
            x *= this.mLacunarity;
            y *= this.mLacunarity;
            z *= this.mLacunarity;
            num3 *= this.mGain;
        }
        return num2;
    }

    private float SingleSimplex(int seed, float x, float y)
    {
        int num = FastNoiseLite.FastFloor(x);
        int num2 = FastNoiseLite.FastFloor(y);
        float num3 = x - (float)num;
        float num4 = y - (float)num2;
        float num5 = (num3 + num4) * 0.21132487f;
        float num6 = num3 - num5;
        float num7 = num4 - num5;
        num *= 501125321;
        num2 *= 1136930381;
        float num8 = 0.5f - num6 * num6 - num7 * num7;
        float num9;
        if (num8 <= 0f)
        {
            num9 = 0f;
        }
        else
        {
            num9 = num8 * num8 * (num8 * num8) * FastNoiseLite.GradCoord(seed, num, num2, num6, num7);
        }
        float num10 = 3.1547005f * num5 + (-0.6666666f + num8);
        float num11;
        if (num10 <= 0f)
        {
            num11 = 0f;
        }
        else
        {
            float num12 = num6 + -0.57735026f;
            float num13 = num7 + -0.57735026f;
            num11 = num10 * num10 * (num10 * num10) * FastNoiseLite.GradCoord(seed, num + 501125321, num2 + 1136930381, num12, num13);
        }
        float num17;
        if (num7 > num6)
        {
            float num14 = num6 + 0.21132487f;
            float num15 = num7 + -0.7886751f;
            float num16 = 0.5f - num14 * num14 - num15 * num15;
            if (num16 <= 0f)
            {
                num17 = 0f;
            }
            else
            {
                num17 = num16 * num16 * (num16 * num16) * FastNoiseLite.GradCoord(seed, num, num2 + 1136930381, num14, num15);
            }
        }
        else
        {
            float num18 = num6 + -0.7886751f;
            float num19 = num7 + 0.21132487f;
            float num20 = 0.5f - num18 * num18 - num19 * num19;
            if (num20 <= 0f)
            {
                num17 = 0f;
            }
            else
            {
                num17 = num20 * num20 * (num20 * num20) * FastNoiseLite.GradCoord(seed, num + 501125321, num2, num18, num19);
            }
        }
        return (num9 + num17 + num11) * 99.83685f;
    }

    private float SingleOpenSimplex2(int seed, float x, float y, float z)
    {
        int num = FastNoiseLite.FastRound(x);
        int num2 = FastNoiseLite.FastRound(y);
        int num3 = FastNoiseLite.FastRound(z);
        float num4 = x - (float)num;
        float num5 = y - (float)num2;
        float num6 = z - (float)num3;
        int num7 = (int)(-1f - num4) | 1;
        int num8 = (int)(-1f - num5) | 1;
        int num9 = (int)(-1f - num6) | 1;
        float num10 = (float)num7 * -num4;
        float num11 = (float)num8 * -num5;
        float num12 = (float)num9 * -num6;
        num *= 501125321;
        num2 *= 1136930381;
        num3 *= 1720413743;
        float num13 = 0f;
        float num14 = 0.6f - num4 * num4 - (num5 * num5 + num6 * num6);
        int num15 = 0;
        for (; ; )
        {
            if (num14 > 0f)
            {
                num13 += num14 * num14 * (num14 * num14) * FastNoiseLite.GradCoord(seed, num, num2, num3, num4, num5, num6);
            }
            if (num10 >= num11 && num10 >= num12)
            {
                float num16 = num14 + num10 + num10;
                if (num16 > 1f)
                {
                    num16 -= 1f;
                    num13 += num16 * num16 * (num16 * num16) * FastNoiseLite.GradCoord(seed, num - num7 * 501125321, num2, num3, num4 + (float)num7, num5, num6);
                }
            }
            else if (num11 > num10 && num11 >= num12)
            {
                float num17 = num14 + num11 + num11;
                if (num17 > 1f)
                {
                    num17 -= 1f;
                    num13 += num17 * num17 * (num17 * num17) * FastNoiseLite.GradCoord(seed, num, num2 - num8 * 1136930381, num3, num4, num5 + (float)num8, num6);
                }
            }
            else
            {
                float num18 = num14 + num12 + num12;
                if (num18 > 1f)
                {
                    num18 -= 1f;
                    num13 += num18 * num18 * (num18 * num18) * FastNoiseLite.GradCoord(seed, num, num2, num3 - num9 * 1720413743, num4, num5, num6 + (float)num9);
                }
            }
            if (num15 == 1)
            {
                break;
            }
            num10 = 0.5f - num10;
            num11 = 0.5f - num11;
            num12 = 0.5f - num12;
            num4 = (float)num7 * num10;
            num5 = (float)num8 * num11;
            num6 = (float)num9 * num12;
            num14 += 0.75f - num10 - (num11 + num12);
            num += (num7 >> 1) & 501125321;
            num2 += (num8 >> 1) & 1136930381;
            num3 += (num9 >> 1) & 1720413743;
            num7 = -num7;
            num8 = -num8;
            num9 = -num9;
            seed = ~seed;
            num15++;
        }
        return num13 * 32.694283f;
    }

    private float SingleOpenSimplex2S(int seed, float x, float y)
    {
        int num = FastNoiseLite.FastFloor(x);
        int num2 = FastNoiseLite.FastFloor(y);
        float num3 = x - (float)num;
        float num4 = y - (float)num2;
        num *= 501125321;
        num2 *= 1136930381;
        int num5 = num + 501125321;
        int num6 = num2 + 1136930381;
        float num7 = (num3 + num4) * 0.21132487f;
        float num8 = num3 - num7;
        float num9 = num4 - num7;
        float num10 = 0.6666667f - num8 * num8 - num9 * num9;
        float num11 = num10 * num10 * (num10 * num10) * FastNoiseLite.GradCoord(seed, num, num2, num8, num9);
        float num12 = 3.1547005f * num7 + (-0.6666666f + num10);
        float num13 = num8 - 0.57735026f;
        float num14 = num9 - 0.57735026f;
        num11 += num12 * num12 * (num12 * num12) * FastNoiseLite.GradCoord(seed, num5, num6, num13, num14);
        float num15 = num3 - num4;
        if (num7 > 0.21132487f)
        {
            if (num3 + num15 > 1f)
            {
                float num16 = num8 + -1.3660254f;
                float num17 = num9 + -0.3660254f;
                float num18 = 0.6666667f - num16 * num16 - num17 * num17;
                if (num18 > 0f)
                {
                    num11 += num18 * num18 * (num18 * num18) * FastNoiseLite.GradCoord(seed, num + 1002250642, num2 + 1136930381, num16, num17);
                }
            }
            else
            {
                float num19 = num8 + 0.21132487f;
                float num20 = num9 + -0.7886751f;
                float num21 = 0.6666667f - num19 * num19 - num20 * num20;
                if (num21 > 0f)
                {
                    num11 += num21 * num21 * (num21 * num21) * FastNoiseLite.GradCoord(seed, num, num2 + 1136930381, num19, num20);
                }
            }
            if (num4 - num15 > 1f)
            {
                float num22 = num8 + -0.3660254f;
                float num23 = num9 + -1.3660254f;
                float num24 = 0.6666667f - num22 * num22 - num23 * num23;
                if (num24 > 0f)
                {
                    num11 += num24 * num24 * (num24 * num24) * FastNoiseLite.GradCoord(seed, num + 501125321, num2 + -2021106534, num22, num23);
                }
            }
            else
            {
                float num25 = num8 + -0.7886751f;
                float num26 = num9 + 0.21132487f;
                float num27 = 0.6666667f - num25 * num25 - num26 * num26;
                if (num27 > 0f)
                {
                    num11 += num27 * num27 * (num27 * num27) * FastNoiseLite.GradCoord(seed, num + 501125321, num2, num25, num26);
                }
            }
        }
        else
        {
            if (num3 + num15 < 0f)
            {
                float num28 = num8 + 0.7886751f;
                float num29 = num9 - 0.21132487f;
                float num30 = 0.6666667f - num28 * num28 - num29 * num29;
                if (num30 > 0f)
                {
                    num11 += num30 * num30 * (num30 * num30) * FastNoiseLite.GradCoord(seed, num - 501125321, num2, num28, num29);
                }
            }
            else
            {
                float num31 = num8 + -0.7886751f;
                float num32 = num9 + 0.21132487f;
                float num33 = 0.6666667f - num31 * num31 - num32 * num32;
                if (num33 > 0f)
                {
                    num11 += num33 * num33 * (num33 * num33) * FastNoiseLite.GradCoord(seed, num + 501125321, num2, num31, num32);
                }
            }
            if (num4 < num15)
            {
                float num34 = num8 - 0.21132487f;
                float num35 = num9 - -0.7886751f;
                float num36 = 0.6666667f - num34 * num34 - num35 * num35;
                if (num36 > 0f)
                {
                    num11 += num36 * num36 * (num36 * num36) * FastNoiseLite.GradCoord(seed, num, num2 - 1136930381, num34, num35);
                }
            }
            else
            {
                float num37 = num8 + 0.21132487f;
                float num38 = num9 + -0.7886751f;
                float num39 = 0.6666667f - num37 * num37 - num38 * num38;
                if (num39 > 0f)
                {
                    num11 += num39 * num39 * (num39 * num39) * FastNoiseLite.GradCoord(seed, num, num2 + 1136930381, num37, num38);
                }
            }
        }
        return num11 * 18.241962f;
    }

    private float SingleOpenSimplex2S(int seed, float x, float y, float z)
    {
        int num = FastNoiseLite.FastFloor(x);
        int num2 = FastNoiseLite.FastFloor(y);
        int num3 = FastNoiseLite.FastFloor(z);
        float num4 = x - (float)num;
        float num5 = y - (float)num2;
        float num6 = z - (float)num3;
        num *= 501125321;
        num2 *= 1136930381;
        num3 *= 1720413743;
        int num7 = seed + 1293373;
        int num8 = (int)(-0.5f - num4);
        int num9 = (int)(-0.5f - num5);
        int num10 = (int)(-0.5f - num6);
        float num11 = num4 + (float)num8;
        float num12 = num5 + (float)num9;
        float num13 = num6 + (float)num10;
        float num14 = 0.75f - num11 * num11 - num12 * num12 - num13 * num13;
        float num15 = num14 * num14 * (num14 * num14) * FastNoiseLite.GradCoord(seed, num + (num8 & 501125321), num2 + (num9 & 1136930381), num3 + (num10 & 1720413743), num11, num12, num13);
        float num16 = num4 - 0.5f;
        float num17 = num5 - 0.5f;
        float num18 = num6 - 0.5f;
        float num19 = 0.75f - num16 * num16 - num17 * num17 - num18 * num18;
        num15 += num19 * num19 * (num19 * num19) * FastNoiseLite.GradCoord(num7, num + 501125321, num2 + 1136930381, num3 + 1720413743, num16, num17, num18);
        float num20 = (float)((num8 | 1) << 1) * num16;
        float num21 = (float)((num9 | 1) << 1) * num17;
        float num22 = (float)((num10 | 1) << 1) * num18;
        float num23 = (float)(-2 - (num8 << 2)) * num16 - 1f;
        float num24 = (float)(-2 - (num9 << 2)) * num17 - 1f;
        float num25 = (float)(-2 - (num10 << 2)) * num18 - 1f;
        bool flag = false;
        float num26 = num20 + num14;
        if (num26 > 0f)
        {
            float num27 = num11 - (float)(num8 | 1);
            float num28 = num12;
            float num29 = num13;
            num15 += num26 * num26 * (num26 * num26) * FastNoiseLite.GradCoord(seed, num + (~num8 & 501125321), num2 + (num9 & 1136930381), num3 + (num10 & 1720413743), num27, num28, num29);
        }
        else
        {
            float num30 = num21 + num22 + num14;
            if (num30 > 0f)
            {
                float num31 = num11;
                float num32 = num12 - (float)(num9 | 1);
                float num33 = num13 - (float)(num10 | 1);
                num15 += num30 * num30 * (num30 * num30) * FastNoiseLite.GradCoord(seed, num + (num8 & 501125321), num2 + (~num9 & 1136930381), num3 + (~num10 & 1720413743), num31, num32, num33);
            }
            float num34 = num23 + num19;
            if (num34 > 0f)
            {
                float num35 = (float)(num8 | 1) + num16;
                float num36 = num17;
                float num37 = num18;
                num15 += num34 * num34 * (num34 * num34) * FastNoiseLite.GradCoord(num7, num + (num8 & 1002250642), num2 + 1136930381, num3 + 1720413743, num35, num36, num37);
                flag = true;
            }
        }
        bool flag2 = false;
        float num38 = num21 + num14;
        if (num38 > 0f)
        {
            float num39 = num11;
            float num40 = num12 - (float)(num9 | 1);
            float num41 = num13;
            num15 += num38 * num38 * (num38 * num38) * FastNoiseLite.GradCoord(seed, num + (num8 & 501125321), num2 + (~num9 & 1136930381), num3 + (num10 & 1720413743), num39, num40, num41);
        }
        else
        {
            float num42 = num20 + num22 + num14;
            if (num42 > 0f)
            {
                float num43 = num11 - (float)(num8 | 1);
                float num44 = num12;
                float num45 = num13 - (float)(num10 | 1);
                num15 += num42 * num42 * (num42 * num42) * FastNoiseLite.GradCoord(seed, num + (~num8 & 501125321), num2 + (num9 & 1136930381), num3 + (~num10 & 1720413743), num43, num44, num45);
            }
            float num46 = num24 + num19;
            if (num46 > 0f)
            {
                float num47 = num16;
                float num48 = (float)(num9 | 1) + num17;
                float num49 = num18;
                num15 += num46 * num46 * (num46 * num46) * FastNoiseLite.GradCoord(num7, num + 501125321, num2 + (num9 & -2021106534), num3 + 1720413743, num47, num48, num49);
                flag2 = true;
            }
        }
        bool flag3 = false;
        float num50 = num22 + num14;
        if (num50 > 0f)
        {
            float num51 = num11;
            float num52 = num12;
            float num53 = num13 - (float)(num10 | 1);
            num15 += num50 * num50 * (num50 * num50) * FastNoiseLite.GradCoord(seed, num + (num8 & 501125321), num2 + (num9 & 1136930381), num3 + (~num10 & 1720413743), num51, num52, num53);
        }
        else
        {
            float num54 = num20 + num21 + num14;
            if (num54 > 0f)
            {
                float num55 = num11 - (float)(num8 | 1);
                float num56 = num12 - (float)(num9 | 1);
                float num57 = num13;
                num15 += num54 * num54 * (num54 * num54) * FastNoiseLite.GradCoord(seed, num + (~num8 & 501125321), num2 + (~num9 & 1136930381), num3 + (num10 & 1720413743), num55, num56, num57);
            }
            float num58 = num25 + num19;
            if (num58 > 0f)
            {
                float num59 = num16;
                float num60 = num17;
                float num61 = (float)(num10 | 1) + num18;
                num15 += num58 * num58 * (num58 * num58) * FastNoiseLite.GradCoord(num7, num + 501125321, num2 + 1136930381, num3 + (num10 & -854139810), num59, num60, num61);
                flag3 = true;
            }
        }
        if (!flag)
        {
            float num62 = num24 + num25 + num19;
            if (num62 > 0f)
            {
                float num63 = num16;
                float num64 = (float)(num9 | 1) + num17;
                float num65 = (float)(num10 | 1) + num18;
                num15 += num62 * num62 * (num62 * num62) * FastNoiseLite.GradCoord(num7, num + 501125321, num2 + (num9 & -2021106534), num3 + (num10 & -854139810), num63, num64, num65);
            }
        }
        if (!flag2)
        {
            float num66 = num23 + num25 + num19;
            if (num66 > 0f)
            {
                float num67 = (float)(num8 | 1) + num16;
                float num68 = num17;
                float num69 = (float)(num10 | 1) + num18;
                num15 += num66 * num66 * (num66 * num66) * FastNoiseLite.GradCoord(num7, num + (num8 & 1002250642), num2 + 1136930381, num3 + (num10 & -854139810), num67, num68, num69);
            }
        }
        if (!flag3)
        {
            float num70 = num23 + num24 + num19;
            if (num70 > 0f)
            {
                float num71 = (float)(num8 | 1) + num16;
                float num72 = (float)(num9 | 1) + num17;
                float num73 = num18;
                num15 += num70 * num70 * (num70 * num70) * FastNoiseLite.GradCoord(num7, num + (num8 & 1002250642), num2 + (num9 & -2021106534), num3 + 1720413743, num71, num72, num73);
            }
        }
        return num15 * 9.046026f;
    }

    private float SingleCellular(int seed, float x, float y)
    {
        int num = FastNoiseLite.FastRound(x);
        int num2 = FastNoiseLite.FastRound(y);
        float num3 = float.MaxValue;
        float num4 = float.MaxValue;
        int num5 = 0;
        float num6 = 0.43701595f * this.mCellularJitterModifier;
        int num7 = (num - 1) * 501125321;
        int num8 = (num2 - 1) * 1136930381;
        switch (this.mCellularDistanceFunction)
        {
            default:
                {
                    for (int i = num - 1; i <= num + 1; i++)
                    {
                        int num9 = num8;
                        for (int j = num2 - 1; j <= num2 + 1; j++)
                        {
                            int num10 = FastNoiseLite.Hash(seed, num7, num9);
                            int num11 = num10 & 510;
                            float num12 = (float)i - x + FastNoiseLite.RandVecs2D[num11] * num6;
                            float num13 = (float)j - y + FastNoiseLite.RandVecs2D[num11 | 1] * num6;
                            float num14 = num12 * num12 + num13 * num13;
                            num4 = FastNoiseLite.FastMax(FastNoiseLite.FastMin(num4, num14), num3);
                            if (num14 < num3)
                            {
                                num3 = num14;
                                num5 = num10;
                            }
                            num9 += 1136930381;
                        }
                        num7 += 501125321;
                    }
                    break;
                }
            case FastNoiseLite.CellularDistanceFunction.Manhattan:
                {
                    for (int k = num - 1; k <= num + 1; k++)
                    {
                        int num15 = num8;
                        for (int l = num2 - 1; l <= num2 + 1; l++)
                        {
                            int num16 = FastNoiseLite.Hash(seed, num7, num15);
                            int num17 = num16 & 510;
                            float num18 = (float)k - x + FastNoiseLite.RandVecs2D[num17] * num6;
                            float num19 = (float)l - y + FastNoiseLite.RandVecs2D[num17 | 1] * num6;
                            float num20 = FastNoiseLite.FastAbs(num18) + FastNoiseLite.FastAbs(num19);
                            num4 = FastNoiseLite.FastMax(FastNoiseLite.FastMin(num4, num20), num3);
                            if (num20 < num3)
                            {
                                num3 = num20;
                                num5 = num16;
                            }
                            num15 += 1136930381;
                        }
                        num7 += 501125321;
                    }
                    break;
                }
            case FastNoiseLite.CellularDistanceFunction.Hybrid:
                {
                    for (int m = num - 1; m <= num + 1; m++)
                    {
                        int num21 = num8;
                        for (int n = num2 - 1; n <= num2 + 1; n++)
                        {
                            int num22 = FastNoiseLite.Hash(seed, num7, num21);
                            int num23 = num22 & 510;
                            float num24 = (float)m - x + FastNoiseLite.RandVecs2D[num23] * num6;
                            float num25 = (float)n - y + FastNoiseLite.RandVecs2D[num23 | 1] * num6;
                            float num26 = FastNoiseLite.FastAbs(num24) + FastNoiseLite.FastAbs(num25) + (num24 * num24 + num25 * num25);
                            num4 = FastNoiseLite.FastMax(FastNoiseLite.FastMin(num4, num26), num3);
                            if (num26 < num3)
                            {
                                num3 = num26;
                                num5 = num22;
                            }
                            num21 += 1136930381;
                        }
                        num7 += 501125321;
                    }
                    break;
                }
        }
        if (this.mCellularDistanceFunction == FastNoiseLite.CellularDistanceFunction.Euclidean && this.mCellularReturnType >= FastNoiseLite.CellularReturnType.Distance)
        {
            num3 = FastNoiseLite.FastSqrt(num3);
            if (this.mCellularReturnType >= FastNoiseLite.CellularReturnType.Distance2)
            {
                num4 = FastNoiseLite.FastSqrt(num4);
            }
        }
        switch (this.mCellularReturnType)
        {
            case FastNoiseLite.CellularReturnType.CellValue:
                return (float)num5 * 4.656613E-10f;
            case FastNoiseLite.CellularReturnType.Distance:
                return num3 - 1f;
            case FastNoiseLite.CellularReturnType.Distance2:
                return num4 - 1f;
            case FastNoiseLite.CellularReturnType.Distance2Add:
                return (num4 + num3) * 0.5f - 1f;
            case FastNoiseLite.CellularReturnType.Distance2Sub:
                return num4 - num3 - 1f;
            case FastNoiseLite.CellularReturnType.Distance2Mul:
                return num4 * num3 * 0.5f - 1f;
            case FastNoiseLite.CellularReturnType.Distance2Div:
                return num3 / num4 - 1f;
            default:
                return 0f;
        }
    }

    private float SingleCellular(int seed, float x, float y, float z)
    {
        int num = FastNoiseLite.FastRound(x);
        int num2 = FastNoiseLite.FastRound(y);
        int num3 = FastNoiseLite.FastRound(z);
        float num4 = float.MaxValue;
        float num5 = float.MaxValue;
        int num6 = 0;
        float num7 = 0.39614353f * this.mCellularJitterModifier;
        int num8 = (num - 1) * 501125321;
        int num9 = (num2 - 1) * 1136930381;
        int num10 = (num3 - 1) * 1720413743;
        switch (this.mCellularDistanceFunction)
        {
            case FastNoiseLite.CellularDistanceFunction.Euclidean:
            case FastNoiseLite.CellularDistanceFunction.EuclideanSq:
                {
                    for (int i = num - 1; i <= num + 1; i++)
                    {
                        int num11 = num9;
                        for (int j = num2 - 1; j <= num2 + 1; j++)
                        {
                            int num12 = num10;
                            for (int k = num3 - 1; k <= num3 + 1; k++)
                            {
                                int num13 = FastNoiseLite.Hash(seed, num8, num11, num12);
                                int num14 = num13 & 1020;
                                float num15 = (float)i - x + FastNoiseLite.RandVecs3D[num14] * num7;
                                float num16 = (float)j - y + FastNoiseLite.RandVecs3D[num14 | 1] * num7;
                                float num17 = (float)k - z + FastNoiseLite.RandVecs3D[num14 | 2] * num7;
                                float num18 = num15 * num15 + num16 * num16 + num17 * num17;
                                num5 = FastNoiseLite.FastMax(FastNoiseLite.FastMin(num5, num18), num4);
                                if (num18 < num4)
                                {
                                    num4 = num18;
                                    num6 = num13;
                                }
                                num12 += 1720413743;
                            }
                            num11 += 1136930381;
                        }
                        num8 += 501125321;
                    }
                    break;
                }
            case FastNoiseLite.CellularDistanceFunction.Manhattan:
                {
                    for (int l = num - 1; l <= num + 1; l++)
                    {
                        int num19 = num9;
                        for (int m = num2 - 1; m <= num2 + 1; m++)
                        {
                            int num20 = num10;
                            for (int n = num3 - 1; n <= num3 + 1; n++)
                            {
                                int num21 = FastNoiseLite.Hash(seed, num8, num19, num20);
                                int num22 = num21 & 1020;
                                float num23 = (float)l - x + FastNoiseLite.RandVecs3D[num22] * num7;
                                float num24 = (float)m - y + FastNoiseLite.RandVecs3D[num22 | 1] * num7;
                                float num25 = (float)n - z + FastNoiseLite.RandVecs3D[num22 | 2] * num7;
                                float num26 = FastNoiseLite.FastAbs(num23) + FastNoiseLite.FastAbs(num24) + FastNoiseLite.FastAbs(num25);
                                num5 = FastNoiseLite.FastMax(FastNoiseLite.FastMin(num5, num26), num4);
                                if (num26 < num4)
                                {
                                    num4 = num26;
                                    num6 = num21;
                                }
                                num20 += 1720413743;
                            }
                            num19 += 1136930381;
                        }
                        num8 += 501125321;
                    }
                    break;
                }
            case FastNoiseLite.CellularDistanceFunction.Hybrid:
                {
                    for (int num27 = num - 1; num27 <= num + 1; num27++)
                    {
                        int num28 = num9;
                        for (int num29 = num2 - 1; num29 <= num2 + 1; num29++)
                        {
                            int num30 = num10;
                            for (int num31 = num3 - 1; num31 <= num3 + 1; num31++)
                            {
                                int num32 = FastNoiseLite.Hash(seed, num8, num28, num30);
                                int num33 = num32 & 1020;
                                float num34 = (float)num27 - x + FastNoiseLite.RandVecs3D[num33] * num7;
                                float num35 = (float)num29 - y + FastNoiseLite.RandVecs3D[num33 | 1] * num7;
                                float num36 = (float)num31 - z + FastNoiseLite.RandVecs3D[num33 | 2] * num7;
                                float num37 = FastNoiseLite.FastAbs(num34) + FastNoiseLite.FastAbs(num35) + FastNoiseLite.FastAbs(num36) + (num34 * num34 + num35 * num35 + num36 * num36);
                                num5 = FastNoiseLite.FastMax(FastNoiseLite.FastMin(num5, num37), num4);
                                if (num37 < num4)
                                {
                                    num4 = num37;
                                    num6 = num32;
                                }
                                num30 += 1720413743;
                            }
                            num28 += 1136930381;
                        }
                        num8 += 501125321;
                    }
                    break;
                }
        }
        if (this.mCellularDistanceFunction == FastNoiseLite.CellularDistanceFunction.Euclidean && this.mCellularReturnType >= FastNoiseLite.CellularReturnType.Distance)
        {
            num4 = FastNoiseLite.FastSqrt(num4);
            if (this.mCellularReturnType >= FastNoiseLite.CellularReturnType.Distance2)
            {
                num5 = FastNoiseLite.FastSqrt(num5);
            }
        }
        switch (this.mCellularReturnType)
        {
            case FastNoiseLite.CellularReturnType.CellValue:
                return (float)num6 * 4.656613E-10f;
            case FastNoiseLite.CellularReturnType.Distance:
                return num4 - 1f;
            case FastNoiseLite.CellularReturnType.Distance2:
                return num5 - 1f;
            case FastNoiseLite.CellularReturnType.Distance2Add:
                return (num5 + num4) * 0.5f - 1f;
            case FastNoiseLite.CellularReturnType.Distance2Sub:
                return num5 - num4 - 1f;
            case FastNoiseLite.CellularReturnType.Distance2Mul:
                return num5 * num4 * 0.5f - 1f;
            case FastNoiseLite.CellularReturnType.Distance2Div:
                return num4 / num5 - 1f;
            default:
                return 0f;
        }
    }

    private float SinglePerlin(int seed, float x, float y)
    {
        int num = FastNoiseLite.FastFloor(x);
        int num2 = FastNoiseLite.FastFloor(y);
        float num3 = x - (float)num;
        float num4 = y - (float)num2;
        float num5 = num3 - 1f;
        float num6 = num4 - 1f;
        float num7 = FastNoiseLite.InterpQuintic(num3);
        float num8 = FastNoiseLite.InterpQuintic(num4);
        num *= 501125321;
        num2 *= 1136930381;
        int num9 = num + 501125321;
        int num10 = num2 + 1136930381;
        float num11 = FastNoiseLite.Lerp(FastNoiseLite.GradCoord(seed, num, num2, num3, num4), FastNoiseLite.GradCoord(seed, num9, num2, num5, num4), num7);
        float num12 = FastNoiseLite.Lerp(FastNoiseLite.GradCoord(seed, num, num10, num3, num6), FastNoiseLite.GradCoord(seed, num9, num10, num5, num6), num7);
        return FastNoiseLite.Lerp(num11, num12, num8) * 1.4247692f;
    }

    private float SinglePerlin(int seed, float x, float y, float z)
    {
        int num = FastNoiseLite.FastFloor(x);
        int num2 = FastNoiseLite.FastFloor(y);
        int num3 = FastNoiseLite.FastFloor(z);
        float num4 = x - (float)num;
        float num5 = y - (float)num2;
        float num6 = z - (float)num3;
        float num7 = num4 - 1f;
        float num8 = num5 - 1f;
        float num9 = num6 - 1f;
        float num10 = FastNoiseLite.InterpQuintic(num4);
        float num11 = FastNoiseLite.InterpQuintic(num5);
        float num12 = FastNoiseLite.InterpQuintic(num6);
        num *= 501125321;
        num2 *= 1136930381;
        num3 *= 1720413743;
        int num13 = num + 501125321;
        int num14 = num2 + 1136930381;
        int num15 = num3 + 1720413743;
        float num16 = FastNoiseLite.Lerp(FastNoiseLite.GradCoord(seed, num, num2, num3, num4, num5, num6), FastNoiseLite.GradCoord(seed, num13, num2, num3, num7, num5, num6), num10);
        float num17 = FastNoiseLite.Lerp(FastNoiseLite.GradCoord(seed, num, num14, num3, num4, num8, num6), FastNoiseLite.GradCoord(seed, num13, num14, num3, num7, num8, num6), num10);
        float num18 = FastNoiseLite.Lerp(FastNoiseLite.GradCoord(seed, num, num2, num15, num4, num5, num9), FastNoiseLite.GradCoord(seed, num13, num2, num15, num7, num5, num9), num10);
        float num19 = FastNoiseLite.Lerp(FastNoiseLite.GradCoord(seed, num, num14, num15, num4, num8, num9), FastNoiseLite.GradCoord(seed, num13, num14, num15, num7, num8, num9), num10);
        float num20 = FastNoiseLite.Lerp(num16, num17, num11);
        float num21 = FastNoiseLite.Lerp(num18, num19, num11);
        return FastNoiseLite.Lerp(num20, num21, num12) * 0.9649214f;
    }

    private float SingleValueCubic(int seed, float x, float y)
    {
        int num = FastNoiseLite.FastFloor(x);
        int num2 = FastNoiseLite.FastFloor(y);
        float num3 = x - (float)num;
        float num4 = y - (float)num2;
        num *= 501125321;
        num2 *= 1136930381;
        int num5 = num - 501125321;
        int num6 = num2 - 1136930381;
        int num7 = num + 501125321;
        int num8 = num2 + 1136930381;
        int num9 = num + 1002250642;
        int num10 = num2 + -2021106534;
        return FastNoiseLite.CubicLerp(FastNoiseLite.CubicLerp(FastNoiseLite.ValCoord(seed, num5, num6), FastNoiseLite.ValCoord(seed, num, num6), FastNoiseLite.ValCoord(seed, num7, num6), FastNoiseLite.ValCoord(seed, num9, num6), num3), FastNoiseLite.CubicLerp(FastNoiseLite.ValCoord(seed, num5, num2), FastNoiseLite.ValCoord(seed, num, num2), FastNoiseLite.ValCoord(seed, num7, num2), FastNoiseLite.ValCoord(seed, num9, num2), num3), FastNoiseLite.CubicLerp(FastNoiseLite.ValCoord(seed, num5, num8), FastNoiseLite.ValCoord(seed, num, num8), FastNoiseLite.ValCoord(seed, num7, num8), FastNoiseLite.ValCoord(seed, num9, num8), num3), FastNoiseLite.CubicLerp(FastNoiseLite.ValCoord(seed, num5, num10), FastNoiseLite.ValCoord(seed, num, num10), FastNoiseLite.ValCoord(seed, num7, num10), FastNoiseLite.ValCoord(seed, num9, num10), num3), num4) * 0.44444445f;
    }

    private float SingleValueCubic(int seed, float x, float y, float z)
    {
        int num = FastNoiseLite.FastFloor(x);
        int num2 = FastNoiseLite.FastFloor(y);
        int num3 = FastNoiseLite.FastFloor(z);
        float num4 = x - (float)num;
        float num5 = y - (float)num2;
        float num6 = z - (float)num3;
        num *= 501125321;
        num2 *= 1136930381;
        num3 *= 1720413743;
        int num7 = num - 501125321;
        int num8 = num2 - 1136930381;
        int num9 = num3 - 1720413743;
        int num10 = num + 501125321;
        int num11 = num2 + 1136930381;
        int num12 = num3 + 1720413743;
        int num13 = num + 1002250642;
        int num14 = num2 + -2021106534;
        int num15 = num3 + -854139810;
        return FastNoiseLite.CubicLerp(FastNoiseLite.CubicLerp(FastNoiseLite.CubicLerp(FastNoiseLite.ValCoord(seed, num7, num8, num9), FastNoiseLite.ValCoord(seed, num, num8, num9), FastNoiseLite.ValCoord(seed, num10, num8, num9), FastNoiseLite.ValCoord(seed, num13, num8, num9), num4), FastNoiseLite.CubicLerp(FastNoiseLite.ValCoord(seed, num7, num2, num9), FastNoiseLite.ValCoord(seed, num, num2, num9), FastNoiseLite.ValCoord(seed, num10, num2, num9), FastNoiseLite.ValCoord(seed, num13, num2, num9), num4), FastNoiseLite.CubicLerp(FastNoiseLite.ValCoord(seed, num7, num11, num9), FastNoiseLite.ValCoord(seed, num, num11, num9), FastNoiseLite.ValCoord(seed, num10, num11, num9), FastNoiseLite.ValCoord(seed, num13, num11, num9), num4), FastNoiseLite.CubicLerp(FastNoiseLite.ValCoord(seed, num7, num14, num9), FastNoiseLite.ValCoord(seed, num, num14, num9), FastNoiseLite.ValCoord(seed, num10, num14, num9), FastNoiseLite.ValCoord(seed, num13, num14, num9), num4), num5), FastNoiseLite.CubicLerp(FastNoiseLite.CubicLerp(FastNoiseLite.ValCoord(seed, num7, num8, num3), FastNoiseLite.ValCoord(seed, num, num8, num3), FastNoiseLite.ValCoord(seed, num10, num8, num3), FastNoiseLite.ValCoord(seed, num13, num8, num3), num4), FastNoiseLite.CubicLerp(FastNoiseLite.ValCoord(seed, num7, num2, num3), FastNoiseLite.ValCoord(seed, num, num2, num3), FastNoiseLite.ValCoord(seed, num10, num2, num3), FastNoiseLite.ValCoord(seed, num13, num2, num3), num4), FastNoiseLite.CubicLerp(FastNoiseLite.ValCoord(seed, num7, num11, num3), FastNoiseLite.ValCoord(seed, num, num11, num3), FastNoiseLite.ValCoord(seed, num10, num11, num3), FastNoiseLite.ValCoord(seed, num13, num11, num3), num4), FastNoiseLite.CubicLerp(FastNoiseLite.ValCoord(seed, num7, num14, num3), FastNoiseLite.ValCoord(seed, num, num14, num3), FastNoiseLite.ValCoord(seed, num10, num14, num3), FastNoiseLite.ValCoord(seed, num13, num14, num3), num4), num5), FastNoiseLite.CubicLerp(FastNoiseLite.CubicLerp(FastNoiseLite.ValCoord(seed, num7, num8, num12), FastNoiseLite.ValCoord(seed, num, num8, num12), FastNoiseLite.ValCoord(seed, num10, num8, num12), FastNoiseLite.ValCoord(seed, num13, num8, num12), num4), FastNoiseLite.CubicLerp(FastNoiseLite.ValCoord(seed, num7, num2, num12), FastNoiseLite.ValCoord(seed, num, num2, num12), FastNoiseLite.ValCoord(seed, num10, num2, num12), FastNoiseLite.ValCoord(seed, num13, num2, num12), num4), FastNoiseLite.CubicLerp(FastNoiseLite.ValCoord(seed, num7, num11, num12), FastNoiseLite.ValCoord(seed, num, num11, num12), FastNoiseLite.ValCoord(seed, num10, num11, num12), FastNoiseLite.ValCoord(seed, num13, num11, num12), num4), FastNoiseLite.CubicLerp(FastNoiseLite.ValCoord(seed, num7, num14, num12), FastNoiseLite.ValCoord(seed, num, num14, num12), FastNoiseLite.ValCoord(seed, num10, num14, num12), FastNoiseLite.ValCoord(seed, num13, num14, num12), num4), num5), FastNoiseLite.CubicLerp(FastNoiseLite.CubicLerp(FastNoiseLite.ValCoord(seed, num7, num8, num15), FastNoiseLite.ValCoord(seed, num, num8, num15), FastNoiseLite.ValCoord(seed, num10, num8, num15), FastNoiseLite.ValCoord(seed, num13, num8, num15), num4), FastNoiseLite.CubicLerp(FastNoiseLite.ValCoord(seed, num7, num2, num15), FastNoiseLite.ValCoord(seed, num, num2, num15), FastNoiseLite.ValCoord(seed, num10, num2, num15), FastNoiseLite.ValCoord(seed, num13, num2, num15), num4), FastNoiseLite.CubicLerp(FastNoiseLite.ValCoord(seed, num7, num11, num15), FastNoiseLite.ValCoord(seed, num, num11, num15), FastNoiseLite.ValCoord(seed, num10, num11, num15), FastNoiseLite.ValCoord(seed, num13, num11, num15), num4), FastNoiseLite.CubicLerp(FastNoiseLite.ValCoord(seed, num7, num14, num15), FastNoiseLite.ValCoord(seed, num, num14, num15), FastNoiseLite.ValCoord(seed, num10, num14, num15), FastNoiseLite.ValCoord(seed, num13, num14, num15), num4), num5), num6) * 0.2962963f;
    }

    private float SingleValue(int seed, float x, float y)
    {
        int num = FastNoiseLite.FastFloor(x);
        int num2 = FastNoiseLite.FastFloor(y);
        float num3 = FastNoiseLite.InterpHermite(x - (float)num);
        float num4 = FastNoiseLite.InterpHermite(y - (float)num2);
        num *= 501125321;
        num2 *= 1136930381;
        int num5 = num + 501125321;
        int num6 = num2 + 1136930381;
        float num7 = FastNoiseLite.Lerp(FastNoiseLite.ValCoord(seed, num, num2), FastNoiseLite.ValCoord(seed, num5, num2), num3);
        float num8 = FastNoiseLite.Lerp(FastNoiseLite.ValCoord(seed, num, num6), FastNoiseLite.ValCoord(seed, num5, num6), num3);
        return FastNoiseLite.Lerp(num7, num8, num4);
    }

    private float SingleValue(int seed, float x, float y, float z)
    {
        int num = FastNoiseLite.FastFloor(x);
        int num2 = FastNoiseLite.FastFloor(y);
        int num3 = FastNoiseLite.FastFloor(z);
        float num4 = FastNoiseLite.InterpHermite(x - (float)num);
        float num5 = FastNoiseLite.InterpHermite(y - (float)num2);
        float num6 = FastNoiseLite.InterpHermite(z - (float)num3);
        num *= 501125321;
        num2 *= 1136930381;
        num3 *= 1720413743;
        int num7 = num + 501125321;
        int num8 = num2 + 1136930381;
        int num9 = num3 + 1720413743;
        float num10 = FastNoiseLite.Lerp(FastNoiseLite.ValCoord(seed, num, num2, num3), FastNoiseLite.ValCoord(seed, num7, num2, num3), num4);
        float num11 = FastNoiseLite.Lerp(FastNoiseLite.ValCoord(seed, num, num8, num3), FastNoiseLite.ValCoord(seed, num7, num8, num3), num4);
        float num12 = FastNoiseLite.Lerp(FastNoiseLite.ValCoord(seed, num, num2, num9), FastNoiseLite.ValCoord(seed, num7, num2, num9), num4);
        float num13 = FastNoiseLite.Lerp(FastNoiseLite.ValCoord(seed, num, num8, num9), FastNoiseLite.ValCoord(seed, num7, num8, num9), num4);
        float num14 = FastNoiseLite.Lerp(num10, num11, num5);
        float num15 = FastNoiseLite.Lerp(num12, num13, num5);
        return FastNoiseLite.Lerp(num14, num15, num6);
    }

    private void DoSingleDomainWarp(int seed, float amp, float freq, float x, float y, ref float xr, ref float yr)
    {
        switch (this.mDomainWarpType)
        {
            case FastNoiseLite.DomainWarpType.OpenSimplex2:
                this.SingleDomainWarpSimplexGradient(seed, amp * 38.283688f, freq, x, y, ref xr, ref yr, false);
                return;
            case FastNoiseLite.DomainWarpType.OpenSimplex2Reduced:
                this.SingleDomainWarpSimplexGradient(seed, amp * 16f, freq, x, y, ref xr, ref yr, true);
                return;
            case FastNoiseLite.DomainWarpType.BasicGrid:
                this.SingleDomainWarpBasicGrid(seed, amp, freq, x, y, ref xr, ref yr);
                return;
            default:
                return;
        }
    }

    private void DoSingleDomainWarp(int seed, float amp, float freq, float x, float y, float z, ref float xr, ref float yr, ref float zr)
    {
        switch (this.mDomainWarpType)
        {
            case FastNoiseLite.DomainWarpType.OpenSimplex2:
                this.SingleDomainWarpOpenSimplex2Gradient(seed, amp * 32.694283f, freq, x, y, z, ref xr, ref yr, ref zr, false);
                return;
            case FastNoiseLite.DomainWarpType.OpenSimplex2Reduced:
                this.SingleDomainWarpOpenSimplex2Gradient(seed, amp * 7.716049f, freq, x, y, z, ref xr, ref yr, ref zr, true);
                return;
            case FastNoiseLite.DomainWarpType.BasicGrid:
                this.SingleDomainWarpBasicGrid(seed, amp, freq, x, y, z, ref xr, ref yr, ref zr);
                return;
            default:
                return;
        }
    }

    private void DomainWarpSingle(ref float x, ref float y)
    {
        int num = this.mSeed;
        float num2 = this.mDomainWarpAmp * this.mFractalBounding;
        float num3 = this.mFrequency;
        float num4 = x;
        float num5 = y;
        this.TransformDomainWarpCoordinate(ref num4, ref num5);
        this.DoSingleDomainWarp(num, num2, num3, num4, num5, ref x, ref y);
    }

    private void DomainWarpSingle(ref float x, ref float y, ref float z)
    {
        int num = this.mSeed;
        float num2 = this.mDomainWarpAmp * this.mFractalBounding;
        float num3 = this.mFrequency;
        float num4 = x;
        float num5 = y;
        float num6 = z;
        this.TransformDomainWarpCoordinate(ref num4, ref num5, ref num6);
        this.DoSingleDomainWarp(num, num2, num3, num4, num5, num6, ref x, ref y, ref z);
    }

    private void DomainWarpFractalProgressive(ref float x, ref float y)
    {
        int num = this.mSeed;
        float num2 = this.mDomainWarpAmp * this.mFractalBounding;
        float num3 = this.mFrequency;
        for (int i = 0; i < this.mOctaves; i++)
        {
            float num4 = x;
            float num5 = y;
            this.TransformDomainWarpCoordinate(ref num4, ref num5);
            this.DoSingleDomainWarp(num, num2, num3, num4, num5, ref x, ref y);
            num++;
            num2 *= this.mGain;
            num3 *= this.mLacunarity;
        }
    }

    private void DomainWarpFractalProgressive(ref float x, ref float y, ref float z)
    {
        int num = this.mSeed;
        float num2 = this.mDomainWarpAmp * this.mFractalBounding;
        float num3 = this.mFrequency;
        for (int i = 0; i < this.mOctaves; i++)
        {
            float num4 = x;
            float num5 = y;
            float num6 = z;
            this.TransformDomainWarpCoordinate(ref num4, ref num5, ref num6);
            this.DoSingleDomainWarp(num, num2, num3, num4, num5, num6, ref x, ref y, ref z);
            num++;
            num2 *= this.mGain;
            num3 *= this.mLacunarity;
        }
    }

    private void DomainWarpFractalIndependent(ref float x, ref float y)
    {
        float num = x;
        float num2 = y;
        this.TransformDomainWarpCoordinate(ref num, ref num2);
        int num3 = this.mSeed;
        float num4 = this.mDomainWarpAmp * this.mFractalBounding;
        float num5 = this.mFrequency;
        for (int i = 0; i < this.mOctaves; i++)
        {
            this.DoSingleDomainWarp(num3, num4, num5, num, num2, ref x, ref y);
            num3++;
            num4 *= this.mGain;
            num5 *= this.mLacunarity;
        }
    }

    private void DomainWarpFractalIndependent(ref float x, ref float y, ref float z)
    {
        float num = x;
        float num2 = y;
        float num3 = z;
        this.TransformDomainWarpCoordinate(ref num, ref num2, ref num3);
        int num4 = this.mSeed;
        float num5 = this.mDomainWarpAmp * this.mFractalBounding;
        float num6 = this.mFrequency;
        for (int i = 0; i < this.mOctaves; i++)
        {
            this.DoSingleDomainWarp(num4, num5, num6, num, num2, num3, ref x, ref y, ref z);
            num4++;
            num5 *= this.mGain;
            num6 *= this.mLacunarity;
        }
    }

    private void SingleDomainWarpBasicGrid(int seed, float warpAmp, float frequency, float x, float y, ref float xr, ref float yr)
    {
        float num = x * frequency;
        float num2 = y * frequency;
        int num3 = FastNoiseLite.FastFloor(num);
        int num4 = FastNoiseLite.FastFloor(num2);
        float num5 = FastNoiseLite.InterpHermite(num - (float)num3);
        float num6 = FastNoiseLite.InterpHermite(num2 - (float)num4);
        num3 *= 501125321;
        num4 *= 1136930381;
        int num7 = num3 + 501125321;
        int num8 = num4 + 1136930381;
        int num9 = FastNoiseLite.Hash(seed, num3, num4) & 510;
        int num10 = FastNoiseLite.Hash(seed, num7, num4) & 510;
        float num11 = FastNoiseLite.Lerp(FastNoiseLite.RandVecs2D[num9], FastNoiseLite.RandVecs2D[num10], num5);
        float num12 = FastNoiseLite.Lerp(FastNoiseLite.RandVecs2D[num9 | 1], FastNoiseLite.RandVecs2D[num10 | 1], num5);
        num9 = FastNoiseLite.Hash(seed, num3, num8) & 510;
        num10 = FastNoiseLite.Hash(seed, num7, num8) & 510;
        float num13 = FastNoiseLite.Lerp(FastNoiseLite.RandVecs2D[num9], FastNoiseLite.RandVecs2D[num10], num5);
        float num14 = FastNoiseLite.Lerp(FastNoiseLite.RandVecs2D[num9 | 1], FastNoiseLite.RandVecs2D[num10 | 1], num5);
        xr += FastNoiseLite.Lerp(num11, num13, num6) * warpAmp;
        yr += FastNoiseLite.Lerp(num12, num14, num6) * warpAmp;
    }

    private void SingleDomainWarpBasicGrid(int seed, float warpAmp, float frequency, float x, float y, float z, ref float xr, ref float yr, ref float zr)
    {
        float num = x * frequency;
        float num2 = y * frequency;
        float num3 = z * frequency;
        int num4 = FastNoiseLite.FastFloor(num);
        int num5 = FastNoiseLite.FastFloor(num2);
        int num6 = FastNoiseLite.FastFloor(num3);
        float num7 = FastNoiseLite.InterpHermite(num - (float)num4);
        float num8 = FastNoiseLite.InterpHermite(num2 - (float)num5);
        float num9 = FastNoiseLite.InterpHermite(num3 - (float)num6);
        num4 *= 501125321;
        num5 *= 1136930381;
        num6 *= 1720413743;
        int num10 = num4 + 501125321;
        int num11 = num5 + 1136930381;
        int num12 = num6 + 1720413743;
        int num13 = FastNoiseLite.Hash(seed, num4, num5, num6) & 1020;
        int num14 = FastNoiseLite.Hash(seed, num10, num5, num6) & 1020;
        float num15 = FastNoiseLite.Lerp(FastNoiseLite.RandVecs3D[num13], FastNoiseLite.RandVecs3D[num14], num7);
        float num16 = FastNoiseLite.Lerp(FastNoiseLite.RandVecs3D[num13 | 1], FastNoiseLite.RandVecs3D[num14 | 1], num7);
        float num17 = FastNoiseLite.Lerp(FastNoiseLite.RandVecs3D[num13 | 2], FastNoiseLite.RandVecs3D[num14 | 2], num7);
        num13 = FastNoiseLite.Hash(seed, num4, num11, num6) & 1020;
        num14 = FastNoiseLite.Hash(seed, num10, num11, num6) & 1020;
        float num18 = FastNoiseLite.Lerp(FastNoiseLite.RandVecs3D[num13], FastNoiseLite.RandVecs3D[num14], num7);
        float num19 = FastNoiseLite.Lerp(FastNoiseLite.RandVecs3D[num13 | 1], FastNoiseLite.RandVecs3D[num14 | 1], num7);
        float num20 = FastNoiseLite.Lerp(FastNoiseLite.RandVecs3D[num13 | 2], FastNoiseLite.RandVecs3D[num14 | 2], num7);
        float num21 = FastNoiseLite.Lerp(num15, num18, num8);
        float num22 = FastNoiseLite.Lerp(num16, num19, num8);
        float num23 = FastNoiseLite.Lerp(num17, num20, num8);
        num13 = FastNoiseLite.Hash(seed, num4, num5, num12) & 1020;
        num14 = FastNoiseLite.Hash(seed, num10, num5, num12) & 1020;
        num15 = FastNoiseLite.Lerp(FastNoiseLite.RandVecs3D[num13], FastNoiseLite.RandVecs3D[num14], num7);
        num16 = FastNoiseLite.Lerp(FastNoiseLite.RandVecs3D[num13 | 1], FastNoiseLite.RandVecs3D[num14 | 1], num7);
        num17 = FastNoiseLite.Lerp(FastNoiseLite.RandVecs3D[num13 | 2], FastNoiseLite.RandVecs3D[num14 | 2], num7);
        num13 = FastNoiseLite.Hash(seed, num4, num11, num12) & 1020;
        num14 = FastNoiseLite.Hash(seed, num10, num11, num12) & 1020;
        num18 = FastNoiseLite.Lerp(FastNoiseLite.RandVecs3D[num13], FastNoiseLite.RandVecs3D[num14], num7);
        num19 = FastNoiseLite.Lerp(FastNoiseLite.RandVecs3D[num13 | 1], FastNoiseLite.RandVecs3D[num14 | 1], num7);
        num20 = FastNoiseLite.Lerp(FastNoiseLite.RandVecs3D[num13 | 2], FastNoiseLite.RandVecs3D[num14 | 2], num7);
        xr += FastNoiseLite.Lerp(num21, FastNoiseLite.Lerp(num15, num18, num8), num9) * warpAmp;
        yr += FastNoiseLite.Lerp(num22, FastNoiseLite.Lerp(num16, num19, num8), num9) * warpAmp;
        zr += FastNoiseLite.Lerp(num23, FastNoiseLite.Lerp(num17, num20, num8), num9) * warpAmp;
    }

    private void SingleDomainWarpSimplexGradient(int seed, float warpAmp, float frequency, float x, float y, ref float xr, ref float yr, bool outGradOnly)
    {
        x *= frequency;
        y *= frequency;
        int num = FastNoiseLite.FastFloor(x);
        int num2 = FastNoiseLite.FastFloor(y);
        float num3 = x - (float)num;
        float num4 = y - (float)num2;
        float num5 = (num3 + num4) * 0.21132487f;
        float num6 = num3 - num5;
        float num7 = num4 - num5;
        num *= 501125321;
        num2 *= 1136930381;
        float num9;
        float num8 = (num9 = 0f);
        float num10 = 0.5f - num6 * num6 - num7 * num7;
        if (num10 > 0f)
        {
            float num11 = num10 * num10 * (num10 * num10);
            float num12;
            float num13;
            if (outGradOnly)
            {
                FastNoiseLite.GradCoordOut(seed, num, num2, out num12, out num13);
            }
            else
            {
                FastNoiseLite.GradCoordDual(seed, num, num2, num6, num7, out num12, out num13);
            }
            num9 += num11 * num12;
            num8 += num11 * num13;
        }
        float num14 = 3.1547005f * num5 + (-0.6666666f + num10);
        if (num14 > 0f)
        {
            float num15 = num6 + -0.57735026f;
            float num16 = num7 + -0.57735026f;
            float num17 = num14 * num14 * (num14 * num14);
            float num18;
            float num19;
            if (outGradOnly)
            {
                FastNoiseLite.GradCoordOut(seed, num + 501125321, num2 + 1136930381, out num18, out num19);
            }
            else
            {
                FastNoiseLite.GradCoordDual(seed, num + 501125321, num2 + 1136930381, num15, num16, out num18, out num19);
            }
            num9 += num17 * num18;
            num8 += num17 * num19;
        }
        if (num7 > num6)
        {
            float num20 = num6 + 0.21132487f;
            float num21 = num7 + -0.7886751f;
            float num22 = 0.5f - num20 * num20 - num21 * num21;
            if (num22 > 0f)
            {
                float num23 = num22 * num22 * (num22 * num22);
                float num24;
                float num25;
                if (outGradOnly)
                {
                    FastNoiseLite.GradCoordOut(seed, num, num2 + 1136930381, out num24, out num25);
                }
                else
                {
                    FastNoiseLite.GradCoordDual(seed, num, num2 + 1136930381, num20, num21, out num24, out num25);
                }
                num9 += num23 * num24;
                num8 += num23 * num25;
            }
        }
        else
        {
            float num26 = num6 + -0.7886751f;
            float num27 = num7 + 0.21132487f;
            float num28 = 0.5f - num26 * num26 - num27 * num27;
            if (num28 > 0f)
            {
                float num29 = num28 * num28 * (num28 * num28);
                float num30;
                float num31;
                if (outGradOnly)
                {
                    FastNoiseLite.GradCoordOut(seed, num + 501125321, num2, out num30, out num31);
                }
                else
                {
                    FastNoiseLite.GradCoordDual(seed, num + 501125321, num2, num26, num27, out num30, out num31);
                }
                num9 += num29 * num30;
                num8 += num29 * num31;
            }
        }
        xr += num9 * warpAmp;
        yr += num8 * warpAmp;
    }

    private void SingleDomainWarpOpenSimplex2Gradient(int seed, float warpAmp, float frequency, float x, float y, float z, ref float xr, ref float yr, ref float zr, bool outGradOnly)
    {
        x *= frequency;
        y *= frequency;
        z *= frequency;
        int num = FastNoiseLite.FastRound(x);
        int num2 = FastNoiseLite.FastRound(y);
        int num3 = FastNoiseLite.FastRound(z);
        float num4 = x - (float)num;
        float num5 = y - (float)num2;
        float num6 = z - (float)num3;
        int num7 = (int)(-num4 - 1f) | 1;
        int num8 = (int)(-num5 - 1f) | 1;
        int num9 = (int)(-num6 - 1f) | 1;
        float num10 = (float)num7 * -num4;
        float num11 = (float)num8 * -num5;
        float num12 = (float)num9 * -num6;
        num *= 501125321;
        num2 *= 1136930381;
        num3 *= 1720413743;
        float num15;
        float num14;
        float num13 = (num14 = (num15 = 0f));
        float num16 = 0.6f - num4 * num4 - (num5 * num5 + num6 * num6);
        int num17 = 0;
        for (; ; )
        {
            if (num16 > 0f)
            {
                float num18 = num16 * num16 * (num16 * num16);
                float num19;
                float num20;
                float num21;
                if (outGradOnly)
                {
                    FastNoiseLite.GradCoordOut(seed, num, num2, num3, out num19, out num20, out num21);
                }
                else
                {
                    FastNoiseLite.GradCoordDual(seed, num, num2, num3, num4, num5, num6, out num19, out num20, out num21);
                }
                num14 += num18 * num19;
                num13 += num18 * num20;
                num15 += num18 * num21;
            }
            float num22 = num16;
            int num23 = num;
            int num24 = num2;
            int num25 = num3;
            float num26 = num4;
            float num27 = num5;
            float num28 = num6;
            if (num10 >= num11 && num10 >= num12)
            {
                num26 += (float)num7;
                num22 = num22 + num10 + num10;
                num23 -= num7 * 501125321;
            }
            else if (num11 > num10 && num11 >= num12)
            {
                num27 += (float)num8;
                num22 = num22 + num11 + num11;
                num24 -= num8 * 1136930381;
            }
            else
            {
                num28 += (float)num9;
                num22 = num22 + num12 + num12;
                num25 -= num9 * 1720413743;
            }
            if (num22 > 1f)
            {
                num22 -= 1f;
                float num29 = num22 * num22 * (num22 * num22);
                float num30;
                float num31;
                float num32;
                if (outGradOnly)
                {
                    FastNoiseLite.GradCoordOut(seed, num23, num24, num25, out num30, out num31, out num32);
                }
                else
                {
                    FastNoiseLite.GradCoordDual(seed, num23, num24, num25, num26, num27, num28, out num30, out num31, out num32);
                }
                num14 += num29 * num30;
                num13 += num29 * num31;
                num15 += num29 * num32;
            }
            if (num17 == 1)
            {
                break;
            }
            num10 = 0.5f - num10;
            num11 = 0.5f - num11;
            num12 = 0.5f - num12;
            num4 = (float)num7 * num10;
            num5 = (float)num8 * num11;
            num6 = (float)num9 * num12;
            num16 += 0.75f - num10 - (num11 + num12);
            num += (num7 >> 1) & 501125321;
            num2 += (num8 >> 1) & 1136930381;
            num3 += (num9 >> 1) & 1720413743;
            num7 = -num7;
            num8 = -num8;
            num9 = -num9;
            seed += 1293373;
            num17++;
        }
        xr += num14 * warpAmp;
        yr += num13 * warpAmp;
        zr += num15 * warpAmp;
    }

    private const short INLINE = 256;

    private const short OPTIMISE = 512;

    private int mSeed = 1337;

    private float mFrequency = 0.01f;

    private FastNoiseLite.NoiseType mNoiseType;

    private FastNoiseLite.RotationType3D mRotationType3D;

    private FastNoiseLite.TransformType3D mTransformType3D = FastNoiseLite.TransformType3D.DefaultOpenSimplex2;

    private FastNoiseLite.FractalType mFractalType;

    private int mOctaves = 3;

    private float mLacunarity = 2f;

    private float mGain = 0.5f;

    private float mWeightedStrength;

    private float mPingPongStrength = 2f;

    private float mFractalBounding = 0.5714286f;

    private FastNoiseLite.CellularDistanceFunction mCellularDistanceFunction = FastNoiseLite.CellularDistanceFunction.EuclideanSq;

    private FastNoiseLite.CellularReturnType mCellularReturnType = FastNoiseLite.CellularReturnType.Distance;

    private float mCellularJitterModifier = 1f;

    private FastNoiseLite.DomainWarpType mDomainWarpType;

    private FastNoiseLite.TransformType3D mWarpTransformType3D = FastNoiseLite.TransformType3D.DefaultOpenSimplex2;

    private float mDomainWarpAmp = 1f;

    private static readonly float[] Gradients2D = new float[]
    {
        0.13052619f, 0.9914449f, 0.38268343f, 0.9238795f, 0.6087614f, 0.7933533f, 0.7933533f, 0.6087614f, 0.9238795f, 0.38268343f,
        0.9914449f, 0.13052619f, 0.9914449f, -0.13052619f, 0.9238795f, -0.38268343f, 0.7933533f, -0.6087614f, 0.6087614f, -0.7933533f,
        0.38268343f, -0.9238795f, 0.13052619f, -0.9914449f, -0.13052619f, -0.9914449f, -0.38268343f, -0.9238795f, -0.6087614f, -0.7933533f,
        -0.7933533f, -0.6087614f, -0.9238795f, -0.38268343f, -0.9914449f, -0.13052619f, -0.9914449f, 0.13052619f, -0.9238795f, 0.38268343f,
        -0.7933533f, 0.6087614f, -0.6087614f, 0.7933533f, -0.38268343f, 0.9238795f, -0.13052619f, 0.9914449f, 0.13052619f, 0.9914449f,
        0.38268343f, 0.9238795f, 0.6087614f, 0.7933533f, 0.7933533f, 0.6087614f, 0.9238795f, 0.38268343f, 0.9914449f, 0.13052619f,
        0.9914449f, -0.13052619f, 0.9238795f, -0.38268343f, 0.7933533f, -0.6087614f, 0.6087614f, -0.7933533f, 0.38268343f, -0.9238795f,
        0.13052619f, -0.9914449f, -0.13052619f, -0.9914449f, -0.38268343f, -0.9238795f, -0.6087614f, -0.7933533f, -0.7933533f, -0.6087614f,
        -0.9238795f, -0.38268343f, -0.9914449f, -0.13052619f, -0.9914449f, 0.13052619f, -0.9238795f, 0.38268343f, -0.7933533f, 0.6087614f,
        -0.6087614f, 0.7933533f, -0.38268343f, 0.9238795f, -0.13052619f, 0.9914449f, 0.13052619f, 0.9914449f, 0.38268343f, 0.9238795f,
        0.6087614f, 0.7933533f, 0.7933533f, 0.6087614f, 0.9238795f, 0.38268343f, 0.9914449f, 0.13052619f, 0.9914449f, -0.13052619f,
        0.9238795f, -0.38268343f, 0.7933533f, -0.6087614f, 0.6087614f, -0.7933533f, 0.38268343f, -0.9238795f, 0.13052619f, -0.9914449f,
        -0.13052619f, -0.9914449f, -0.38268343f, -0.9238795f, -0.6087614f, -0.7933533f, -0.7933533f, -0.6087614f, -0.9238795f, -0.38268343f,
        -0.9914449f, -0.13052619f, -0.9914449f, 0.13052619f, -0.9238795f, 0.38268343f, -0.7933533f, 0.6087614f, -0.6087614f, 0.7933533f,
        -0.38268343f, 0.9238795f, -0.13052619f, 0.9914449f, 0.13052619f, 0.9914449f, 0.38268343f, 0.9238795f, 0.6087614f, 0.7933533f,
        0.7933533f, 0.6087614f, 0.9238795f, 0.38268343f, 0.9914449f, 0.13052619f, 0.9914449f, -0.13052619f, 0.9238795f, -0.38268343f,
        0.7933533f, -0.6087614f, 0.6087614f, -0.7933533f, 0.38268343f, -0.9238795f, 0.13052619f, -0.9914449f, -0.13052619f, -0.9914449f,
        -0.38268343f, -0.9238795f, -0.6087614f, -0.7933533f, -0.7933533f, -0.6087614f, -0.9238795f, -0.38268343f, -0.9914449f, -0.13052619f,
        -0.9914449f, 0.13052619f, -0.9238795f, 0.38268343f, -0.7933533f, 0.6087614f, -0.6087614f, 0.7933533f, -0.38268343f, 0.9238795f,
        -0.13052619f, 0.9914449f, 0.13052619f, 0.9914449f, 0.38268343f, 0.9238795f, 0.6087614f, 0.7933533f, 0.7933533f, 0.6087614f,
        0.9238795f, 0.38268343f, 0.9914449f, 0.13052619f, 0.9914449f, -0.13052619f, 0.9238795f, -0.38268343f, 0.7933533f, -0.6087614f,
        0.6087614f, -0.7933533f, 0.38268343f, -0.9238795f, 0.13052619f, -0.9914449f, -0.13052619f, -0.9914449f, -0.38268343f, -0.9238795f,
        -0.6087614f, -0.7933533f, -0.7933533f, -0.6087614f, -0.9238795f, -0.38268343f, -0.9914449f, -0.13052619f, -0.9914449f, 0.13052619f,
        -0.9238795f, 0.38268343f, -0.7933533f, 0.6087614f, -0.6087614f, 0.7933533f, -0.38268343f, 0.9238795f, -0.13052619f, 0.9914449f,
        0.38268343f, 0.9238795f, 0.9238795f, 0.38268343f, 0.9238795f, -0.38268343f, 0.38268343f, -0.9238795f, -0.38268343f, -0.9238795f,
        -0.9238795f, -0.38268343f, -0.9238795f, 0.38268343f, -0.38268343f, 0.9238795f
    };

    private static readonly float[] RandVecs2D = new float[]
    {
        -0.2700222f, -0.9628541f, 0.38630927f, -0.9223693f, 0.04444859f, -0.9990117f, -0.59925234f, -0.80056024f, -0.781928f, 0.62336874f,
        0.9464672f, 0.32279992f, -0.6514147f, -0.7587219f, 0.93784726f, 0.34704837f, -0.8497876f, -0.52712524f, -0.87904257f, 0.47674325f,
        -0.8923003f, -0.45144236f, -0.37984443f, -0.9250504f, -0.9951651f, 0.09821638f, 0.7724398f, -0.635088f, 0.75732833f, -0.6530343f,
        -0.9928005f, -0.119780056f, -0.05326657f, 0.99858034f, 0.97542536f, -0.22033007f, -0.76650184f, 0.64224213f, 0.9916367f, 0.12906061f,
        -0.99469686f, 0.10285038f, -0.53792053f, -0.8429955f, 0.50228155f, -0.86470413f, 0.45598215f, -0.8899889f, -0.8659131f, -0.50019443f,
        0.08794584f, -0.9961253f, -0.5051685f, 0.8630207f, 0.7753185f, -0.6315704f, -0.69219446f, 0.72171104f, -0.51916593f, -0.85467345f,
        0.8978623f, -0.4402764f, -0.17067741f, 0.98532695f, -0.935343f, -0.35374206f, -0.99924046f, 0.038967468f, -0.2882064f, -0.9575683f,
        -0.96638113f, 0.2571138f, -0.87597144f, -0.48236302f, -0.8303123f, -0.55729836f, 0.051101338f, -0.99869347f, -0.85583735f, -0.51724505f,
        0.098870255f, 0.9951003f, 0.9189016f, 0.39448678f, -0.24393758f, -0.96979094f, -0.81214094f, -0.5834613f, -0.99104315f, 0.13354214f,
        0.8492424f, -0.52800316f, -0.9717839f, -0.23587295f, 0.9949457f, 0.10041421f, 0.6241065f, -0.7813392f, 0.6629103f, 0.74869883f,
        -0.7197418f, 0.6942418f, -0.8143371f, -0.58039224f, 0.10452105f, -0.9945227f, -0.10659261f, -0.99430275f, 0.44579968f, -0.8951328f,
        0.105547406f, 0.99441427f, -0.9927903f, 0.11986445f, -0.83343667f, 0.55261505f, 0.9115562f, -0.4111756f, 0.8285545f, -0.55990845f,
        0.7217098f, -0.6921958f, 0.49404928f, -0.8694339f, -0.36523214f, -0.9309165f, -0.9696607f, 0.24445485f, 0.089255095f, -0.9960088f,
        0.5354071f, -0.8445941f, -0.10535762f, 0.9944344f, -0.98902845f, 0.1477251f, 0.004856105f, 0.9999882f, 0.98855984f, 0.15082914f,
        0.92861295f, -0.37104982f, -0.5832394f, -0.8123003f, 0.30152076f, 0.9534596f, -0.95751107f, 0.28839657f, 0.9715802f, -0.23671055f,
        0.2299818f, 0.97319496f, 0.9557638f, -0.2941352f, 0.7409561f, 0.67155343f, -0.9971514f, -0.07542631f, 0.69057107f, -0.7232645f,
        -0.2907137f, -0.9568101f, 0.5912778f, -0.80646795f, -0.94545925f, -0.3257405f, 0.66644555f, 0.7455537f, 0.6236135f, 0.78173286f,
        0.9126994f, -0.40863165f, -0.8191762f, 0.57354194f, -0.8812746f, -0.4726046f, 0.99533135f, 0.09651673f, 0.98556507f, -0.16929697f,
        -0.8495981f, 0.52743065f, 0.6174854f, -0.78658235f, 0.85081565f, 0.5254643f, 0.99850327f, -0.0546925f, 0.19713716f, -0.98037595f,
        0.66078556f, -0.7505747f, -0.030974941f, 0.9995202f, -0.6731661f, 0.73949134f, -0.71950185f, -0.69449055f, 0.97275114f, 0.2318516f,
        0.9997059f, -0.02425069f, 0.44217876f, -0.89692694f, 0.9981351f, -0.061043672f, -0.9173661f, -0.39804456f, -0.81500566f, -0.579453f,
        -0.87893313f, 0.476945f, 0.015860584f, 0.99987423f, -0.8095465f, 0.5870558f, -0.9165899f, -0.39982867f, -0.8023543f, 0.5968481f,
        -0.5176738f, 0.85557806f, -0.8154407f, -0.57884055f, 0.40220103f, -0.91555136f, -0.9052557f, -0.4248672f, 0.7317446f, 0.681579f,
        -0.56476325f, -0.825253f, -0.8403276f, -0.54207885f, -0.93142813f, 0.36392525f, 0.52381986f, 0.85182905f, 0.7432804f, -0.66898f,
        -0.9853716f, -0.17041974f, 0.46014687f, 0.88784283f, 0.8258554f, 0.56388193f, 0.6182366f, 0.785992f, 0.83315027f, -0.55304664f,
        0.15003075f, 0.9886813f, -0.6623304f, -0.7492119f, -0.66859865f, 0.74362344f, 0.7025606f, 0.7116239f, -0.54193896f, -0.84041786f,
        -0.33886164f, 0.9408362f, 0.833153f, 0.55304253f, -0.29897207f, -0.95426184f, 0.2638523f, 0.9645631f, 0.12410874f, -0.9922686f,
        -0.7282649f, -0.6852957f, 0.69625f, 0.71779937f, -0.91835356f, 0.395761f, -0.6326102f, -0.7744703f, -0.9331892f, -0.35938552f,
        -0.11537793f, -0.99332166f, 0.9514975f, -0.30765656f, -0.08987977f, -0.9959526f, 0.6678497f, 0.7442962f, 0.79524004f, -0.6062947f,
        -0.6462007f, -0.7631675f, -0.27335986f, 0.96191186f, 0.966959f, -0.25493184f, -0.9792895f, 0.20246519f, -0.5369503f, -0.84361386f,
        -0.27003646f, -0.9628501f, -0.6400277f, 0.76835185f, -0.78545374f, -0.6189204f, 0.060059056f, -0.9981948f, -0.024557704f, 0.9996984f,
        -0.65983623f, 0.7514095f, -0.62538946f, -0.7803128f, -0.6210409f, -0.7837782f, 0.8348889f, 0.55041856f, -0.15922752f, 0.9872419f,
        0.83676225f, 0.54756635f, -0.8675754f, -0.4973057f, -0.20226626f, -0.97933054f, 0.939919f, 0.34139755f, 0.98774046f, -0.1561049f,
        -0.90344554f, 0.42870283f, 0.12698042f, -0.9919052f, -0.3819601f, 0.92417884f, 0.9754626f, 0.22016525f, -0.32040158f, -0.94728184f,
        -0.9874761f, 0.15776874f, 0.025353484f, -0.99967855f, 0.4835131f, -0.8753371f, -0.28508f, -0.9585037f, -0.06805516f, -0.99768156f,
        -0.7885244f, -0.61500347f, 0.3185392f, -0.9479097f, 0.8880043f, 0.45983514f, 0.64769214f, -0.76190215f, 0.98202413f, 0.18875542f,
        0.93572754f, -0.35272372f, -0.88948953f, 0.45695552f, 0.7922791f, 0.6101588f, 0.74838185f, 0.66326815f, -0.728893f, -0.68462765f,
        0.8729033f, -0.48789328f, 0.8288346f, 0.5594937f, 0.08074567f, 0.99673474f, 0.97991484f, -0.1994165f, -0.5807307f, -0.81409574f,
        -0.47000498f, -0.8826638f, 0.2409493f, 0.9705377f, 0.9437817f, -0.33056942f, -0.89279985f, -0.45045355f, -0.80696225f, 0.59060305f,
        0.062589735f, 0.99803936f, -0.93125975f, 0.36435598f, 0.57774496f, 0.81621736f, -0.3360096f, -0.9418586f, 0.69793206f, -0.71616393f,
        -0.0020081573f, -0.999998f, -0.18272944f, -0.98316324f, -0.6523912f, 0.7578824f, -0.43026268f, -0.9027037f, -0.9985126f, -0.054520912f,
        -0.010281022f, -0.99994713f, -0.49460712f, 0.86911666f, -0.299935f, 0.95395964f, 0.8165472f, 0.5772787f, 0.26974604f, 0.9629315f,
        -0.7306287f, -0.68277496f, -0.7590952f, -0.65097964f, -0.9070538f, 0.4210146f, -0.5104861f, -0.859886f, 0.86133504f, 0.5080373f,
        0.50078815f, -0.8655699f, -0.6541582f, 0.7563578f, -0.83827555f, -0.54524684f, 0.6940071f, 0.7199682f, 0.06950936f, 0.9975813f,
        0.17029423f, -0.9853933f, 0.26959732f, 0.9629731f, 0.55196124f, -0.83386976f, 0.2256575f, -0.9742067f, 0.42152628f, -0.9068162f,
        0.48818734f, -0.87273884f, -0.3683855f, -0.92967314f, -0.98253906f, 0.18605645f, 0.81256473f, 0.582871f, 0.3196461f, -0.947537f,
        0.9570914f, 0.28978625f, -0.6876655f, -0.7260276f, -0.9988771f, -0.04737673f, -0.1250179f, 0.9921545f, -0.82801336f, 0.56070834f,
        0.93248636f, -0.36120513f, 0.63946533f, 0.7688199f, -0.016238471f, -0.99986815f, -0.99550146f, -0.094746135f, -0.8145332f, 0.580117f,
        0.4037328f, -0.91487694f, 0.9944263f, 0.10543368f, -0.16247116f, 0.9867133f, -0.9949488f, -0.10038388f, -0.69953024f, 0.714603f,
        0.5263415f, -0.85027325f, -0.5395222f, 0.8419714f, 0.65793705f, 0.7530729f, 0.014267588f, -0.9998982f, -0.6734384f, 0.7392433f,
        0.6394121f, -0.7688642f, 0.9211571f, 0.38919085f, -0.14663722f, -0.98919034f, -0.7823181f, 0.6228791f, -0.5039611f, -0.8637264f,
        -0.774312f, -0.632804f
    };

    private static readonly float[] Gradients3D = new float[]
    {
        0f, 1f, 1f, 0f, 0f, -1f, 1f, 0f, 0f, 1f,
        -1f, 0f, 0f, -1f, -1f, 0f, 1f, 0f, 1f, 0f,
        -1f, 0f, 1f, 0f, 1f, 0f, -1f, 0f, -1f, 0f,
        -1f, 0f, 1f, 1f, 0f, 0f, -1f, 1f, 0f, 0f,
        1f, -1f, 0f, 0f, -1f, -1f, 0f, 0f, 0f, 1f,
        1f, 0f, 0f, -1f, 1f, 0f, 0f, 1f, -1f, 0f,
        0f, -1f, -1f, 0f, 1f, 0f, 1f, 0f, -1f, 0f,
        1f, 0f, 1f, 0f, -1f, 0f, -1f, 0f, -1f, 0f,
        1f, 1f, 0f, 0f, -1f, 1f, 0f, 0f, 1f, -1f,
        0f, 0f, -1f, -1f, 0f, 0f, 0f, 1f, 1f, 0f,
        0f, -1f, 1f, 0f, 0f, 1f, -1f, 0f, 0f, -1f,
        -1f, 0f, 1f, 0f, 1f, 0f, -1f, 0f, 1f, 0f,
        1f, 0f, -1f, 0f, -1f, 0f, -1f, 0f, 1f, 1f,
        0f, 0f, -1f, 1f, 0f, 0f, 1f, -1f, 0f, 0f,
        -1f, -1f, 0f, 0f, 0f, 1f, 1f, 0f, 0f, -1f,
        1f, 0f, 0f, 1f, -1f, 0f, 0f, -1f, -1f, 0f,
        1f, 0f, 1f, 0f, -1f, 0f, 1f, 0f, 1f, 0f,
        -1f, 0f, -1f, 0f, -1f, 0f, 1f, 1f, 0f, 0f,
        -1f, 1f, 0f, 0f, 1f, -1f, 0f, 0f, -1f, -1f,
        0f, 0f, 0f, 1f, 1f, 0f, 0f, -1f, 1f, 0f,
        0f, 1f, -1f, 0f, 0f, -1f, -1f, 0f, 1f, 0f,
        1f, 0f, -1f, 0f, 1f, 0f, 1f, 0f, -1f, 0f,
        -1f, 0f, -1f, 0f, 1f, 1f, 0f, 0f, -1f, 1f,
        0f, 0f, 1f, -1f, 0f, 0f, -1f, -1f, 0f, 0f,
        1f, 1f, 0f, 0f, 0f, -1f, 1f, 0f, -1f, 1f,
        0f, 0f, 0f, -1f, -1f, 0f
    };

    private static readonly float[] RandVecs3D = new float[]
    {
        -0.7292737f, -0.66184396f, 0.17355819f, 0f, 0.7902921f, -0.5480887f, -0.2739291f, 0f, 0.7217579f, 0.62262124f,
        -0.3023381f, 0f, 0.5656831f, -0.8208298f, -0.079000026f, 0f, 0.76004905f, -0.55559796f, -0.33709997f, 0f,
        0.37139457f, 0.50112647f, 0.78162545f, 0f, -0.12770624f, -0.4254439f, -0.8959289f, 0f, -0.2881561f, -0.5815839f,
        0.7607406f, 0f, 0.5849561f, -0.6628202f, -0.4674352f, 0f, 0.33071712f, 0.039165374f, 0.94291687f, 0f,
        0.8712122f, -0.41133744f, -0.26793817f, 0f, 0.580981f, 0.7021916f, 0.41156778f, 0f, 0.5037569f, 0.6330057f,
        -0.5878204f, 0f, 0.44937122f, 0.6013902f, 0.6606023f, 0f, -0.6878404f, 0.090188906f, -0.7202372f, 0f,
        -0.59589565f, -0.64693505f, 0.47579765f, 0f, -0.5127052f, 0.1946922f, -0.83619875f, 0f, -0.99115074f, -0.054102764f,
        -0.12121531f, 0f, -0.21497211f, 0.9720882f, -0.09397608f, 0f, -0.7518651f, -0.54280573f, 0.37424695f, 0f,
        0.5237069f, 0.8516377f, -0.021078179f, 0f, 0.6333505f, 0.19261672f, -0.74951047f, 0f, -0.06788242f, 0.39983058f,
        0.9140719f, 0f, -0.55386287f, -0.47298968f, -0.6852129f, 0f, -0.72614557f, -0.5911991f, 0.35099334f, 0f,
        -0.9229275f, -0.17828088f, 0.34120494f, 0f, -0.6968815f, 0.65112746f, 0.30064803f, 0f, 0.96080446f, -0.20983632f,
        -0.18117249f, 0f, 0.068171464f, -0.9743405f, 0.21450691f, 0f, -0.3577285f, -0.6697087f, -0.65078455f, 0f,
        -0.18686211f, 0.7648617f, -0.61649746f, 0f, -0.65416974f, 0.3967915f, 0.64390874f, 0f, 0.699334f, -0.6164538f,
        0.36182392f, 0f, -0.15466657f, 0.6291284f, 0.7617583f, 0f, -0.6841613f, -0.2580482f, -0.68215424f, 0f,
        0.5383981f, 0.4258655f, 0.727163f, 0f, -0.5026988f, -0.7939833f, -0.3418837f, 0f, 0.32029718f, 0.28344154f,
        0.9039196f, 0f, 0.86832273f, -0.00037626564f, -0.49599952f, 0f, 0.79112005f, -0.085110456f, 0.60571057f, 0f,
        -0.04011016f, -0.43972486f, 0.8972364f, 0f, 0.914512f, 0.35793462f, -0.18854876f, 0f, -0.96120393f, -0.27564842f,
        0.010246669f, 0f, 0.65103614f, -0.28777993f, -0.70237786f, 0f, -0.20417863f, 0.73652375f, 0.6448596f, 0f,
        -0.7718264f, 0.37906268f, 0.5104856f, 0f, -0.30600828f, -0.7692988f, 0.56083715f, 0f, 0.45400733f, -0.5024843f,
        0.73578995f, 0f, 0.48167956f, 0.6021208f, -0.636738f, 0f, 0.69619805f, -0.32221973f, 0.6414692f, 0f,
        -0.65321606f, -0.6781149f, 0.33685157f, 0f, 0.50893015f, -0.61546624f, -0.60182345f, 0f, -0.16359198f, -0.9133605f,
        -0.37284088f, 0f, 0.5240802f, -0.8437664f, 0.11575059f, 0f, 0.5902587f, 0.4983818f, -0.63498837f, 0f,
        0.5863228f, 0.49476475f, 0.6414308f, 0f, 0.6779335f, 0.23413453f, 0.6968409f, 0f, 0.7177054f, -0.68589795f,
        0.12017863f, 0f, -0.532882f, -0.5205125f, 0.6671608f, 0f, -0.8654874f, -0.07007271f, -0.4960054f, 0f,
        -0.286181f, 0.79520893f, 0.53454953f, 0f, -0.048495296f, 0.98108363f, -0.18741156f, 0f, -0.63585216f, 0.60583484f,
        0.47818002f, 0f, 0.62547946f, -0.28616196f, 0.72586966f, 0f, -0.258526f, 0.50619495f, -0.8227582f, 0f,
        0.021363068f, 0.50640166f, -0.862033f, 0f, 0.20011178f, 0.85992634f, 0.46955505f, 0f, 0.47435614f, 0.6014985f,
        -0.6427953f, 0f, 0.6622994f, -0.52024746f, -0.539168f, 0f, 0.08084973f, -0.65327203f, 0.7527941f, 0f,
        -0.6893687f, 0.059286036f, 0.7219805f, 0f, -0.11218871f, -0.96731853f, 0.22739525f, 0f, 0.7344116f, 0.59796685f,
        -0.3210533f, 0f, 0.5789393f, -0.24888498f, 0.776457f, 0f, 0.69881827f, 0.35571697f, -0.6205791f, 0f,
        -0.86368454f, -0.27487713f, -0.4224826f, 0f, -0.4247028f, -0.46408808f, 0.77733505f, 0f, 0.5257723f, -0.84270173f,
        0.11583299f, 0f, 0.93438303f, 0.31630248f, -0.16395439f, 0f, -0.10168364f, -0.8057303f, -0.58348876f, 0f,
        -0.6529239f, 0.50602126f, -0.5635893f, 0f, -0.24652861f, -0.9668206f, -0.06694497f, 0f, -0.9776897f, -0.20992506f,
        -0.0073688254f, 0f, 0.7736893f, 0.57342446f, 0.2694238f, 0f, -0.6095088f, 0.4995679f, 0.6155737f, 0f,
        0.5794535f, 0.7434547f, 0.33392924f, 0f, -0.8226211f, 0.081425816f, 0.56272936f, 0f, -0.51038545f, 0.47036678f,
        0.719904f, 0f, -0.5764972f, -0.072316565f, -0.81389266f, 0f, 0.7250629f, 0.39499715f, -0.56414634f, 0f,
        -0.1525424f, 0.48608407f, -0.8604958f, 0f, -0.55509764f, -0.49578208f, 0.6678823f, 0f, -0.18836144f, 0.91458696f,
        0.35784173f, 0f, 0.76255566f, -0.54144084f, -0.35404897f, 0f, -0.5870232f, -0.3226498f, -0.7424964f, 0f,
        0.30511242f, 0.2262544f, -0.9250488f, 0f, 0.63795763f, 0.57724243f, -0.50970703f, 0f, -0.5966776f, 0.14548524f,
        -0.7891831f, 0f, -0.65833056f, 0.65554875f, -0.36994147f, 0f, 0.74348927f, 0.23510846f, 0.6260573f, 0f,
        0.5562114f, 0.82643604f, -0.08736329f, 0f, -0.302894f, -0.8251527f, 0.47684193f, 0f, 0.11293438f, -0.9858884f,
        -0.123571075f, 0f, 0.5937653f, -0.5896814f, 0.5474657f, 0f, 0.6757964f, -0.58357584f, -0.45026484f, 0f,
        0.7242303f, -0.11527198f, 0.67985505f, 0f, -0.9511914f, 0.0753624f, -0.29925808f, 0f, 0.2539471f, -0.18863393f,
        0.9486454f, 0f, 0.5714336f, -0.16794509f, -0.8032796f, 0f, -0.06778235f, 0.39782694f, 0.9149532f, 0f,
        0.6074973f, 0.73306f, -0.30589226f, 0f, -0.54354787f, 0.16758224f, 0.8224791f, 0f, -0.5876678f, -0.3380045f,
        -0.7351187f, 0f, -0.79675627f, 0.040978227f, -0.60290986f, 0f, -0.19963509f, 0.8706295f, 0.4496111f, 0f,
        -0.027876602f, -0.91062325f, -0.4122962f, 0f, -0.7797626f, -0.6257635f, 0.019757755f, 0f, -0.5211233f, 0.74016446f,
        -0.42495546f, 0f, 0.8575425f, 0.4053273f, -0.31675017f, 0f, 0.10452233f, 0.8390196f, -0.53396744f, 0f,
        0.3501823f, 0.9242524f, -0.15208502f, 0f, 0.19878499f, 0.076476134f, 0.9770547f, 0f, 0.78459966f, 0.6066257f,
        -0.12809642f, 0f, 0.09006737f, -0.97509897f, -0.20265691f, 0f, -0.82743436f, -0.54229957f, 0.14582036f, 0f,
        -0.34857976f, -0.41580227f, 0.8400004f, 0f, -0.2471779f, -0.730482f, -0.6366311f, 0f, -0.3700155f, 0.8577948f,
        0.35675845f, 0f, 0.59133947f, -0.54831195f, -0.59133035f, 0f, 0.120487355f, -0.7626472f, -0.6354935f, 0f,
        0.6169593f, 0.03079648f, 0.7863923f, 0f, 0.12581569f, -0.664083f, -0.73699677f, 0f, -0.6477565f, -0.17401473f,
        -0.74170774f, 0f, 0.6217889f, -0.7804431f, -0.06547655f, 0f, 0.6589943f, -0.6096988f, 0.44044736f, 0f,
        -0.26898375f, -0.6732403f, -0.68876356f, 0f, -0.38497752f, 0.56765425f, 0.7277094f, 0f, 0.57544446f, 0.81104714f,
        -0.10519635f, 0f, 0.91415936f, 0.3832948f, 0.13190056f, 0f, -0.10792532f, 0.9245494f, 0.36545935f, 0f,
        0.3779771f, 0.30431488f, 0.87437165f, 0f, -0.21428852f, -0.8259286f, 0.5214617f, 0f, 0.58025444f, 0.41480985f,
        -0.7008834f, 0f, -0.19826609f, 0.85671616f, -0.47615966f, 0f, -0.033815537f, 0.37731808f, -0.9254661f, 0f,
        -0.68679225f, -0.6656598f, 0.29191336f, 0f, 0.7731743f, -0.28757936f, -0.565243f, 0f, -0.09655942f, 0.91937083f,
        -0.3813575f, 0f, 0.27157024f, -0.957791f, -0.09426606f, 0f, 0.24510157f, -0.6917999f, -0.6792188f, 0f,
        0.97770077f, -0.17538553f, 0.115503654f, 0f, -0.522474f, 0.8521607f, 0.029036159f, 0f, -0.77348804f, -0.52612925f,
        0.35341796f, 0f, -0.71344924f, -0.26954725f, 0.6467878f, 0f, 0.16440372f, 0.5105846f, -0.84396374f, 0f,
        0.6494636f, 0.055856112f, 0.7583384f, 0f, -0.4711971f, 0.50172806f, -0.7254256f, 0f, -0.63357645f, -0.23816863f,
        -0.7361091f, 0f, -0.9021533f, -0.2709478f, -0.33571818f, 0f, -0.3793711f, 0.8722581f, 0.3086152f, 0f,
        -0.68555987f, -0.32501432f, 0.6514394f, 0f, 0.29009423f, -0.7799058f, -0.5546101f, 0f, -0.20983194f, 0.8503707f,
        0.48253515f, 0f, -0.45926037f, 0.6598504f, -0.5947077f, 0f, 0.87159455f, 0.09616365f, -0.48070312f, 0f,
        -0.6776666f, 0.71185046f, -0.1844907f, 0f, 0.7044378f, 0.3124276f, 0.637304f, 0f, -0.7052319f, -0.24010932f,
        -0.6670798f, 0f, 0.081921004f, -0.72073364f, -0.68835455f, 0f, -0.6993681f, -0.5875763f, -0.4069869f, 0f,
        -0.12814544f, 0.6419896f, 0.75592864f, 0f, -0.6337388f, -0.67854714f, -0.3714147f, 0f, 0.5565052f, -0.21688876f,
        -0.8020357f, 0f, -0.57915545f, 0.7244372f, -0.3738579f, 0f, 0.11757791f, -0.7096451f, 0.69467926f, 0f,
        -0.613462f, 0.13236311f, 0.7785528f, 0f, 0.69846356f, -0.029805163f, -0.7150247f, 0f, 0.83180827f, -0.3930172f,
        0.39195976f, 0f, 0.14695764f, 0.055416517f, -0.98758924f, 0f, 0.70886856f, -0.2690504f, 0.65201014f, 0f,
        0.27260533f, 0.67369765f, -0.68688995f, 0f, -0.65912956f, 0.30354586f, -0.68804663f, 0f, 0.48151314f, -0.752827f,
        0.4487723f, 0f, 0.943001f, 0.16756473f, -0.28752613f, 0f, 0.43480295f, 0.7695305f, -0.46772778f, 0f,
        0.39319962f, 0.5944736f, 0.70142365f, 0f, 0.72543365f, -0.60392565f, 0.33018148f, 0f, 0.75902355f, -0.6506083f,
        0.024333132f, 0f, -0.8552769f, -0.3430043f, 0.38839358f, 0f, -0.6139747f, 0.6981725f, 0.36822575f, 0f,
        -0.74659055f, -0.575201f, 0.33428493f, 0f, 0.5730066f, 0.8105555f, -0.12109168f, 0f, -0.92258775f, -0.3475211f,
        -0.16751404f, 0f, -0.71058166f, -0.47196922f, -0.5218417f, 0f, -0.0856461f, 0.35830015f, 0.9296697f, 0f,
        -0.8279698f, -0.2043157f, 0.5222271f, 0f, 0.42794403f, 0.278166f, 0.8599346f, 0f, 0.539908f, -0.78571206f,
        -0.3019204f, 0f, 0.5678404f, -0.5495414f, -0.61283076f, 0f, -0.9896071f, 0.13656391f, -0.045034185f, 0f,
        -0.6154343f, -0.64408755f, 0.45430374f, 0f, 0.10742044f, -0.79463404f, 0.59750944f, 0f, -0.359545f, -0.888553f,
        0.28495783f, 0f, -0.21804053f, 0.1529889f, 0.9638738f, 0f, -0.7277432f, -0.61640507f, -0.30072346f, 0f,
        0.7249729f, -0.0066971947f, 0.68874484f, 0f, -0.5553659f, -0.5336586f, 0.6377908f, 0f, 0.5137558f, 0.79762083f,
        -0.316f, 0f, -0.3794025f, 0.92456084f, -0.035227515f, 0f, 0.82292485f, 0.27453658f, -0.49741766f, 0f,
        -0.5404114f, 0.60911417f, 0.5804614f, 0f, 0.8036582f, -0.27030295f, 0.5301602f, 0f, 0.60443187f, 0.68329686f,
        0.40959433f, 0f, 0.06389989f, 0.96582085f, -0.2512108f, 0f, 0.10871133f, 0.74024713f, -0.6634878f, 0f,
        -0.7134277f, -0.6926784f, 0.10591285f, 0f, 0.64588976f, -0.57245487f, -0.50509584f, 0f, -0.6553931f, 0.73814714f,
        0.15999562f, 0f, 0.39109614f, 0.91888714f, -0.05186756f, 0f, -0.48790225f, -0.5904377f, 0.64291114f, 0f,
        0.601479f, 0.77074414f, -0.21018201f, 0f, -0.5677173f, 0.7511361f, 0.33688518f, 0f, 0.7858574f, 0.22667466f,
        0.5753667f, 0f, -0.45203456f, -0.6042227f, -0.65618575f, 0f, 0.0022721163f, 0.4132844f, -0.9105992f, 0f,
        -0.58157516f, -0.5162926f, 0.6286591f, 0f, -0.03703705f, 0.8273786f, 0.5604221f, 0f, -0.51196927f, 0.79535437f,
        -0.324498f, 0f, -0.26824173f, -0.957229f, -0.10843876f, 0f, -0.23224828f, -0.9679131f, -0.09594243f, 0f,
        0.3554329f, -0.8881506f, 0.29130062f, 0f, 0.73465204f, -0.4371373f, 0.5188423f, 0f, 0.998512f, 0.046590112f,
        -0.028339446f, 0f, -0.37276876f, -0.9082481f, 0.19007573f, 0f, 0.9173738f, -0.3483642f, 0.19252984f, 0f,
        0.2714911f, 0.41475296f, -0.86848867f, 0f, 0.5131763f, -0.71163344f, 0.4798207f, 0f, -0.87373537f, 0.18886992f,
        -0.44823506f, 0f, 0.84600437f, -0.3725218f, 0.38145f, 0f, 0.89787275f, -0.17802091f, -0.40265754f, 0f,
        0.21780656f, -0.9698323f, -0.10947895f, 0f, -0.15180314f, -0.7788918f, -0.6085091f, 0f, -0.2600385f, -0.4755398f,
        -0.840382f, 0f, 0.5723135f, -0.7474341f, -0.33734185f, 0f, -0.7174141f, 0.16990171f, -0.67561114f, 0f,
        -0.6841808f, 0.021457076f, -0.72899675f, 0f, -0.2007448f, 0.06555606f, -0.9774477f, 0f, -0.11488037f, -0.8044887f,
        0.5827524f, 0f, -0.787035f, 0.03447489f, 0.6159443f, 0f, -0.20155965f, 0.68598723f, 0.69913894f, 0f,
        -0.085810825f, -0.10920836f, -0.99030805f, 0f, 0.5532693f, 0.73252505f, -0.39661077f, 0f, -0.18424894f, -0.9777375f,
        -0.100407675f, 0f, 0.07754738f, -0.9111506f, 0.40471104f, 0f, 0.13998385f, 0.7601631f, -0.63447344f, 0f,
        0.44844192f, -0.84528923f, 0.29049253f, 0f
    };

    private const int PrimeX = 501125321;

    private const int PrimeY = 1136930381;

    private const int PrimeZ = 1720413743;

    public enum NoiseType
    {
        OpenSimplex2,

        OpenSimplex2S,

        Cellular,

        Perlin,

        ValueCubic,

        Value
    }

    public enum RotationType3D
    {
        None,

        ImproveXYPlanes,

        ImproveXZPlanes
    }

    public enum FractalType
    {
        None,

        FBm,

        Ridged,

        PingPong,

        DomainWarpProgressive,

        DomainWarpIndependent
    }

    public enum CellularDistanceFunction
    {
        Euclidean,

        EuclideanSq,

        Manhattan,

        Hybrid
    }

    public enum CellularReturnType
    {
        CellValue,

        Distance,

        Distance2,

        Distance2Add,

        Distance2Sub,

        Distance2Mul,

        Distance2Div
    }

    public enum DomainWarpType
    {
        OpenSimplex2,

        OpenSimplex2Reduced,

        BasicGrid
    }

    private enum TransformType3D
    {
        None,

        ImproveXYPlanes,

        ImproveXZPlanes,

        DefaultOpenSimplex2
    }
}