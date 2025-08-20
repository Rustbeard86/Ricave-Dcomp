using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Ricave.Core
{
    public static class Calc
    {
        public static float Clamp(float val, float min, float max)
        {
            if (val < min)
            {
                return min;
            }
            if (val <= max)
            {
                return val;
            }
            return max;
        }

        public static int Clamp(int val, int min, int max)
        {
            if (val < min)
            {
                return min;
            }
            if (val <= max)
            {
                return val;
            }
            return max;
        }

        public static float Clamp01(float val)
        {
            if (val < 0f)
            {
                return 0f;
            }
            if (val <= 1f)
            {
                return val;
            }
            return 1f;
        }

        public static float Lerp(float a, float b, float t)
        {
            return a + (b - a) * Calc.Clamp01(t);
        }

        public static float InverseLerp(float a, float b, float value)
        {
            if (a != b)
            {
                return Calc.Clamp01((value - a) / (b - a));
            }
            return 0f;
        }

        public static float Cos(float val)
        {
            return (float)Math.Cos((double)val);
        }

        public static float Sin(float val)
        {
            return (float)Math.Sin((double)val);
        }

        public static float Tan(float val)
        {
            return (float)Math.Tan((double)val);
        }

        public static float Asin(float val)
        {
            return (float)Math.Asin((double)val);
        }

        public static float Acos(float val)
        {
            return (float)Math.Acos((double)val);
        }

        public static float Atan(float val)
        {
            return (float)Math.Atan((double)val);
        }

        public static float Atan2(float y, float x)
        {
            return (float)Math.Atan2((double)y, (double)x);
        }

        public static float Pow(float val, float p)
        {
            return (float)Math.Pow((double)val, (double)p);
        }

        public static float Exp(float val)
        {
            return (float)Math.Exp((double)val);
        }

        public static float Log(float val)
        {
            return (float)Math.Log((double)val);
        }

        public static float Round(float val)
        {
            return (float)Math.Round((double)val);
        }

        public static float Sqrt(float val)
        {
            return (float)Math.Sqrt((double)val);
        }

        public static int RoundToInt(float val)
        {
            return (int)Math.Round((double)val);
        }

        public static int FloorToInt(float val)
        {
            return (int)Math.Floor((double)val);
        }

        public static int CeilToInt(float val)
        {
            return (int)Math.Ceiling((double)val);
        }

        public static float Floor(float val)
        {
            return (float)Math.Floor((double)val);
        }

        public static float Ceil(float val)
        {
            return (float)Math.Ceiling((double)val);
        }

        public static bool Approximately(float a, float b)
        {
            return Math.Abs(b - a) < Math.Max(1E-06f * Math.Max(Math.Abs(a), Math.Abs(b)), Calc.Epsilon * 8f);
        }

        public static float Repeat(float t, float length)
        {
            return Calc.Clamp(t - Calc.Floor(t / length) * length, 0f, length);
        }

        public static float PingPong(float t, float length)
        {
            return length - Math.Abs(Calc.Repeat(t, length * 2f) - length);
        }

        public static float Sign(float val)
        {
            if (val >= 0f)
            {
                return 1f;
            }
            return -1f;
        }

        public static float SmoothStep(float from, float to, float t)
        {
            t = Calc.Clamp01(t);
            t = -2f * t * t * t + 3f * t * t;
            return to * t + from * (1f - t);
        }

        public static float StepTowards(float from, float to, float step)
        {
            if (Math.Abs(from - to) <= step)
            {
                return to;
            }
            if (from < to)
            {
                return from + step;
            }
            return from - step;
        }

        public static float DeltaAngle(float angle, float target)
        {
            float num = Calc.Repeat(target - angle, 360f);
            if (num > 180f)
            {
                num -= 360f;
            }
            return num;
        }

        public static float LerpAngle(float from, float to, float t)
        {
            float num = Calc.Repeat(to - from, 360f);
            if (num > 180f)
            {
                num -= 360f;
            }
            return from + num * Calc.Clamp01(t);
        }

        public static float LerpDouble(float inFrom, float inTo, float outFrom, float outTo, float value)
        {
            return Calc.Lerp(outFrom, outTo, Calc.InverseLerp(inFrom, inTo, value));
        }

        public static int CombineHashes<T1, T2>(T1 val1, T2 val2)
        {
            return Calc.CombineHash<T2>((val1 == null) ? 0 : val1.GetHashCode(), val2);
        }

        public static int CombineHashes<T1, T2, T3>(T1 val1, T2 val2, T3 val3)
        {
            return Calc.CombineHash<T3>(Calc.CombineHash<T2>((val1 == null) ? 0 : val1.GetHashCode(), val2), val3);
        }

        public static int CombineHashes<T1, T2, T3, T4>(T1 val1, T2 val2, T3 val3, T4 val4)
        {
            return Calc.CombineHash<T4>(Calc.CombineHash<T3>(Calc.CombineHash<T2>((val1 == null) ? 0 : val1.GetHashCode(), val2), val3), val4);
        }

        public static int CombineHashes<T1, T2, T3, T4, T5>(T1 val1, T2 val2, T3 val3, T4 val4, T5 val5)
        {
            return Calc.CombineHash<T5>(Calc.CombineHash<T4>(Calc.CombineHash<T3>(Calc.CombineHash<T2>((val1 == null) ? 0 : val1.GetHashCode(), val2), val3), val4), val5);
        }

        public static int CombineHashes<T1, T2, T3, T4, T5, T6>(T1 val1, T2 val2, T3 val3, T4 val4, T5 val5, T6 val6)
        {
            return Calc.CombineHash<T6>(Calc.CombineHash<T5>(Calc.CombineHash<T4>(Calc.CombineHash<T3>(Calc.CombineHash<T2>((val1 == null) ? 0 : val1.GetHashCode(), val2), val3), val4), val5), val6);
        }

        public static int CombineHash(int seed, int val)
        {
            uint num = (uint)(val * -862048943);
            num = Calc.< CombineHash > g__rotl32 | 43_0(num, 15);
            num *= 461845907U;
            return (int)Calc.< CombineHash > g__fmix | 43_1((Calc.< CombineHash > g__rotl32 | 43_0((uint)(seed ^ (int)num), 13) * 5U + 3864292196U) ^ 4U);
        }

        public static int CombineHash<T>(int seed, T val)
        {
            int num = ((val == null) ? 0 : val.GetHashCode());
            return Calc.CombineHash(seed, num);
        }

        public static float NormalizeDir(float dir)
        {
            if (dir > 0f)
            {
                return dir % 360f;
            }
            dir = -(-dir % 360f);
            if (dir < 0f)
            {
                dir += 360f;
            }
            return dir;
        }

        public static float SphericalDistance(Vector3 aNormalized, Vector3 bNormalized)
        {
            if (aNormalized == bNormalized)
            {
                return 0f;
            }
            return Calc.Acos(Vector3.Dot(aNormalized, bNormalized));
        }

        public static float Smooth(float a)
        {
            if (a > 0.55f)
            {
                return Calc.LerpDouble(0.55f, 0.6f, Calc.< Smooth > g__Func | 47_0(0.55f), 1f, a);
            }
            return Calc.< Smooth > g__Func | 47_0(a);
        }

        public static float InverseSmooth(float a)
        {
            return 1f - Calc.Pow(1f - a, 0.25f);
        }

        public static float Mirror(float value, float mirror)
        {
            return mirror + (mirror - value);
        }

        public static float RoundTo(float value, float roundTo)
        {
            return Calc.Round(value / roundTo) * roundTo;
        }

        public static float ResolveFadeInStayOut(float timePassed, float fadeInTime, float stayTime, float fadeOutTime)
        {
            if (timePassed < fadeInTime)
            {
                return Math.Max(timePassed / fadeInTime, 0f);
            }
            if (timePassed < fadeInTime + stayTime)
            {
                return 1f;
            }
            return Math.Max(1f - (timePassed - fadeInTime - stayTime) / fadeOutTime, 0f);
        }

        public static float ResolveFadeIn(float timePassed, float fadeInTime)
        {
            if (timePassed < fadeInTime)
            {
                return Math.Max(timePassed / fadeInTime, 0f);
            }
            return 1f;
        }

        public static void Swap<T>(ref T obj1, ref T obj2)
        {
            T t = obj1;
            obj1 = obj2;
            obj2 = t;
        }

        public static void FrequencyToTimesPerInterval(float frequency, out int timesPerInterval, out int interval)
        {
            if (frequency <= 0f)
            {
                timesPerInterval = 0;
                interval = 1;
                return;
            }
            if (frequency >= 1f)
            {
                timesPerInterval = Calc.RoundToIntHalfUp(frequency);
                interval = 1;
                return;
            }
            timesPerInterval = 1;
            interval = Calc.RoundToIntHalfUp(1f / frequency);
        }

        public static void MultiplyTimesPerInterval(ref int timesPerInterval, ref int interval, float multiplyBy)
        {
            if (multiplyBy <= 0f || timesPerInterval <= 0 || interval <= 0)
            {
                timesPerInterval = 0;
                interval = 1;
                return;
            }
            if (multiplyBy != 1f)
            {
                Calc.FrequencyToTimesPerInterval((float)timesPerInterval / (float)interval * multiplyBy, out timesPerInterval, out interval);
            }
        }

        public static int RoundToIntHalfUp(float value)
        {
            return (int)Math.Round((double)value, MidpointRounding.AwayFromZero);
        }

        public static IEnumerable<int> DistributeRandomly(int count, int min, int max, int toDistribute)
        {
            if (count <= 0)
            {
                yield break;
            }
            List<int> distributed = FrameLocalPool<List<int>>.Get();
            int num;
            if (toDistribute >= count * min)
            {
                for (int j = 0; j < count; j++)
                {
                    distributed.Add(min);
                }
                if (max != 2147483647 && toDistribute >= count * max)
                {
                    for (int k = 0; k < count; k++)
                    {
                        distributed[k] = max;
                    }
                    for (int l = 0; l < toDistribute - count * max; l++)
                    {
                        List<int> list = distributed;
                        num = Rand.RangeInclusive(0, count - 1);
                        int num2 = list[num];
                        list[num] = num2 + 1;
                    }
                }
                else
                {
                    List<int> list2 = FrameLocalPool<List<int>>.Get();
                    for (int m = 0; m < count; m++)
                    {
                        list2.Add(m);
                    }
                    for (int n = 0; n < toDistribute - count * min; n++)
                    {
                        int num3 = Rand.RangeInclusive(0, list2.Count - 1);
                        List<int> list3 = distributed;
                        int num2 = list2[num3];
                        num = list3[num2];
                        list3[num2] = num + 1;
                        if (distributed[list2[num3]] >= max)
                        {
                            list2.RemoveAt(num3);
                        }
                    }
                    list2.Clear();
                }
            }
            else
            {
                int num4 = toDistribute;
                if (num4 >= count)
                {
                    for (int num5 = 0; num5 < count; num5++)
                    {
                        distributed.Add(1);
                        num4--;
                    }
                }
                else
                {
                    List<int> list4 = FrameLocalPool<List<int>>.Get();
                    for (int num6 = 0; num6 < count; num6++)
                    {
                        list4.Add(num6);
                    }
                    for (int num7 = 0; num7 < count; num7++)
                    {
                        distributed.Add(0);
                    }
                    for (int num8 = 0; num8 < toDistribute; num8++)
                    {
                        int num9 = Rand.RangeInclusive(0, list4.Count - 1);
                        List<int> list5 = distributed;
                        num = list4[num9];
                        int num2 = list5[num];
                        list5[num] = num2 + 1;
                        list4.RemoveAt(num9);
                        num4--;
                    }
                    list4.Clear();
                }
                List<int> list6 = FrameLocalPool<List<int>>.Get();
                for (int num10 = 0; num10 < count; num10++)
                {
                    if (distributed[num10] < min)
                    {
                        list6.Add(num10);
                    }
                }
                for (int num11 = 0; num11 < num4; num11++)
                {
                    int num12 = Rand.RangeInclusive(0, list6.Count - 1);
                    List<int> list7 = distributed;
                    int num2 = list6[num12];
                    num = list7[num2];
                    list7[num2] = num + 1;
                    if (distributed[list6[num12]] >= min)
                    {
                        list6.RemoveAt(num12);
                    }
                }
                list6.Clear();
            }
            for (int i = 0; i < count; i = num + 1)
            {
                yield return distributed[i];
                num = i;
            }
            yield break;
        }

        public static int NextPerfectSquare(int number)
        {
            if (number < 0)
            {
                return 0;
            }
            int num = Calc.FloorToInt(Calc.Sqrt((float)number));
            if (num * num == number)
            {
                return number;
            }
            return (int)Calc.Pow((float)(num + 1), 2f);
        }

        public static Vector3 RandomPerpendicularVector(Vector3 v)
        {
            float num = Rand.Range(0f, 6.2831855f);
            Vector3 vector = new Vector3(Calc.Cos(num), Calc.Sin(num), 0f);
            return Quaternion.LookRotation(v) * vector;
        }

        public static float HashToFloat01(int hash)
        {
            hash = Calc.CombineHash(hash, 1361795363);
            return (float)Calc.AbsSafe(hash) / 2.1474836E+09f;
        }

        public static float HashToRange(int hash, FloatRange range)
        {
            return Calc.HashToRange(hash, range.from, range.to);
        }

        public static float HashToRange(int hash, float min, float max)
        {
            return min + Calc.HashToFloat01(hash) * (max - min);
        }

        public static int HashToRange(int hash, int minInclusive, int maxExclusive)
        {
            if (maxExclusive <= minInclusive)
            {
                return minInclusive;
            }
            hash = Calc.CombineHash(hash, 831647121);
            return minInclusive + Calc.AbsSafe(hash % (maxExclusive - minInclusive));
        }

        public static int HashToRangeInclusive(int hash, int minInclusive, int maxInclusive)
        {
            return Calc.HashToRange(hash, minInclusive, maxInclusive + 1);
        }

        public static int AbsSafe(int value)
        {
            if (value != -2147483648)
            {
                return Math.Abs(value);
            }
            return int.MaxValue;
        }

        public static float Pulse(float curTime, float speed, float strength)
        {
            return Math.Abs(Calc.Sin(curTime * speed)) * strength;
        }

        public static float Pulse(float speed, float strength)
        {
            return Calc.Pulse(Clock.Time, speed, strength);
        }

        public static float PulseUnscaled(float speed, float strength)
        {
            return Calc.Pulse(Clock.UnscaledTime, speed, strength);
        }

        public static float LerpWithDeltaTime(float a, float b, float lerpSpeed)
        {
            return Calc.Lerp(a, b, 1f - Calc.Exp(-lerpSpeed * Clock.DeltaTime));
        }

        public static float LerpWithUnscaledDeltaTime(float a, float b, float lerpSpeed)
        {
            return Calc.Lerp(a, b, 1f - Calc.Exp(-lerpSpeed * Clock.UnscaledDeltaTime));
        }

        public static float ConvertConstantFixedUpdateLerpSpeedToNewLerpWithDeltaTime(float oldLerpSpeed)
        {
            return -Calc.Log(1f - oldLerpSpeed) / Clock.FixedDeltaTime;
        }

        [CompilerGenerated]
        internal static uint <CombineHash>g__rotl32|43_0(uint x, byte r)
		{
			return (x << (int) r) | (x >> (int) (32 - r));
		}

    [CompilerGenerated]
    internal static uint <CombineHash>g__fmix|43_1(uint h)
		{
			h ^= h >> 16;
			h *= 2246822507U;
			h ^= h >> 13;
			h *= 3266489909U;
			h ^= h >> 16;
			return h;
		}

[CompilerGenerated]
internal static float < Smooth > g__Func | 47_0(float x)

        {
    return 1f - Calc.Pow(1f - x, 4f);
}

public const float PI = 3.1415927f;

public static readonly float Sqrt2 = Calc.Sqrt(2f);

public static readonly float Epsilon = ((Calc.MinFloatDenormal == 0f) ? Calc.MinFloatNormal : Calc.MinFloatDenormal);

public const float Deg2Rad = 0.017453292f;

public const float Rad2Deg = 57.29578f;

private static volatile float MinFloatNormal = 1.1754944E-38f;

private static volatile float MinFloatDenormal = float.Epsilon;
	}
}