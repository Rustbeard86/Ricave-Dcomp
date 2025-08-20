using System;
using System.Runtime.CompilerServices;
using System.Text;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public static class ColorUtility
    {
        public static Color WithAlpha(this Color color, float alpha)
        {
            return new Color(color.r, color.g, color.b, alpha);
        }

        public static Color WithAlphaFactor(this Color color, float factor)
        {
            return new Color(color.r, color.g, color.b, color.a * factor);
        }

        public static Color WithRedFactor(this Color color, float factor)
        {
            return new Color(color.r * factor, color.g, color.b, color.a);
        }

        public static Color Lighter(this Color color, float offset)
        {
            return new Color(Calc.Clamp01(color.r + offset), Calc.Clamp01(color.g + offset), Calc.Clamp01(color.b + offset), color.a);
        }

        public static Color Darker(this Color color, float offset)
        {
            return new Color(Calc.Clamp01(color.r - offset), Calc.Clamp01(color.g - offset), Calc.Clamp01(color.b - offset), color.a);
        }

        public static Color MultipliedColor(this Color color, float factor)
        {
            return new Color(Calc.Clamp01(color.r * factor), Calc.Clamp01(color.g * factor), Calc.Clamp01(color.b * factor), color.a);
        }

        public static Color MoveTowards(Color from, Color to, float dist)
        {
            Vector3 vector = new Vector3(from.r, from.g, from.b);
            Vector3 vector2 = new Vector3(to.r, to.g, to.b);
            Vector3 vector3 = Vector3.MoveTowards(vector, vector2, dist);
            float num = Calc.StepTowards(from.a, to.a, dist);
            return new Color(Calc.Clamp01(vector3.x), Calc.Clamp01(vector3.y), Calc.Clamp01(vector3.z), Calc.Clamp01(num));
        }

        public static Color HSVMoveTowards(Color from, Color to, float dist)
        {
            if (dist == 0f || from == to)
            {
                return from;
            }
            float num;
            float num2;
            float num3;
            Color.RGBToHSV(from, out num, out num2, out num3);
            float num4;
            float num5;
            float num6;
            Color.RGBToHSV(to, out num4, out num5, out num6);
            ColorUtility.MatchHIfUndefined(ref num, ref num4, num2, num5);
            ColorUtility.ResolveClosestH(ref num, ref num4);
            Vector3 vector = new Vector3(num, num2, num3);
            Vector3 vector2 = new Vector3(num4, num5, num6);
            Vector3 vector3 = Vector3.MoveTowards(vector, vector2, dist);
            float num7 = Calc.StepTowards(from.a, to.a, dist);
            Color color = Color.HSVToRGB(Calc.Clamp01(vector3.x % 1f), Calc.Clamp01(vector3.y), Calc.Clamp01(vector3.z));
            color.a = Calc.Clamp01(num7);
            return color;
        }

        public static float HSVDist(Color from, Color to)
        {
            if (from.r == to.r && from.g == to.g && from.b == to.b)
            {
                return 0f;
            }
            float num;
            float num2;
            float num3;
            Color.RGBToHSV(from, out num, out num2, out num3);
            float num4;
            float num5;
            float num6;
            Color.RGBToHSV(to, out num4, out num5, out num6);
            ColorUtility.MatchHIfUndefined(ref num, ref num4, num2, num5);
            ColorUtility.ResolveClosestH(ref num, ref num4);
            return Vector3.Distance(new Vector3(num, num2, num3), new Vector3(num4, num5, num6));
        }

        public static Color HSVLerp(Color from, Color to, float t)
        {
            if (t <= 0f || from == to)
            {
                return from;
            }
            if (t >= 1f)
            {
                return to;
            }
            float num;
            float num2;
            float num3;
            Color.RGBToHSV(from, out num, out num2, out num3);
            float num4;
            float num5;
            float num6;
            Color.RGBToHSV(to, out num4, out num5, out num6);
            ColorUtility.MatchHIfUndefined(ref num, ref num4, num2, num5);
            ColorUtility.ResolveClosestH(ref num, ref num4);
            Vector3 vector = new Vector3(num, num2, num3);
            Vector3 vector2 = new Vector3(num4, num5, num6);
            Vector3 vector3 = Vector3.Lerp(vector, vector2, t);
            float num7 = Calc.Lerp(from.a, to.a, t);
            Color color = Color.HSVToRGB(Calc.Clamp01(vector3.x % 1f), Calc.Clamp01(vector3.y), Calc.Clamp01(vector3.z));
            color.a = Calc.Clamp01(num7);
            return color;
        }

        private static void MatchHIfUndefined(ref float h1, ref float h2, float s1, float s2)
        {
            if (s1 <= 1E-45f)
            {
                h1 = h2;
            }
            if (s2 <= 1E-45f)
            {
                h2 = h1;
            }
        }

        private static void ResolveClosestH(ref float h1, ref float h2)
        {
            float num = Math.Abs(h1 - h2);
            if (Math.Abs(h1 - (h2 + 1f)) < num)
            {
                h2 += 1f;
                return;
            }
            if (Math.Abs(h1 - (h2 - 1f)) < num)
            {
                h1 += 1f;
            }
        }

        public static bool EqualsOperator(this Color32 a, Color32 b)
        {
            return a.r == b.r && a.g == b.g && a.b == b.b && a.a == b.a;
        }

        public static Color RoundTo255(this Color color)
        {
            return color;
        }

        public static void AppendColorInHex(StringBuilder sb, Color color)
        {
            color = color.RoundTo255();
            ColorUtility.< AppendColorInHex > g__AppendByteInHex | 14_1(sb, Calc.Clamp((int)(color.r * 255f), 0, 255));
            ColorUtility.< AppendColorInHex > g__AppendByteInHex | 14_1(sb, Calc.Clamp((int)(color.g * 255f), 0, 255));
            ColorUtility.< AppendColorInHex > g__AppendByteInHex | 14_1(sb, Calc.Clamp((int)(color.b * 255f), 0, 255));
            ColorUtility.< AppendColorInHex > g__AppendByteInHex | 14_1(sb, Calc.Clamp((int)(color.a * 255f), 0, 255));
        }

        [CompilerGenerated]
        internal static char <AppendColorInHex>g__GetHexDigit|14_0(int value)
		{
			if (value >= 0 && value <= 9)
			{
				return (char) (48 + value);
			}
			if (value >= 10 && value <= 15)
			{
				return (char) (65 + (value - 10));
			}
return '\0';
		}

		[CompilerGenerated]
internal static void < AppendColorInHex > g__AppendByteInHex | 14_1(StringBuilder sb, int value)

        {
    char c = ColorUtility.< AppendColorInHex > g__GetHexDigit | 14_0((value >> 4) & 15);
    char c2 = ColorUtility.< AppendColorInHex > g__GetHexDigit | 14_0(value & 15);
    sb.Append(c);
    sb.Append(c2);
}
	}
}