using System;
using System.Globalization;
using UnityEngine;

namespace Dummiesman
{
    public static class OBJLoaderHelper
    {
        public static void EnableMaterialTransparency(Material mtl)
        {
            mtl.SetFloat("_Mode", 3f);
            mtl.SetInt("_SrcBlend", 5);
            mtl.SetInt("_DstBlend", 10);
            mtl.SetInt("_ZWrite", 0);
            mtl.DisableKeyword("_ALPHATEST_ON");
            mtl.EnableKeyword("_ALPHABLEND_ON");
            mtl.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            mtl.renderQueue = 3000;
        }

        public static float FastFloatParse(string input)
        {
            if (input.Contains("e") || input.Contains("E"))
            {
                return float.Parse(input, CultureInfo.InvariantCulture);
            }
            float num = 0f;
            int i = 0;
            int length = input.Length;
            if (length == 0)
            {
                return float.NaN;
            }
            char c = input[0];
            float num2 = 1f;
            if (c == '-')
            {
                num2 = -1f;
                i++;
                if (i >= length)
                {
                    return float.NaN;
                }
            }
            while (i < length)
            {
                c = input[i++];
                if (c >= '0' && c <= '9')
                {
                    num = num * 10f + (float)(c - '0');
                }
                else
                {
                    if (c != '.' && c != ',')
                    {
                        return float.NaN;
                    }
                    float num3 = 0.1f;
                    while (i < length)
                    {
                        c = input[i++];
                        if (c < '0' || c > '9')
                        {
                            return float.NaN;
                        }
                        num += (float)(c - '0') * num3;
                        num3 *= 0.1f;
                    }
                    return num2 * num;
                }
            }
            return num2 * num;
        }

        public static int FastIntParse(string input)
        {
            int num = 0;
            bool flag = input[0] == '-';
            for (int i = (flag ? 1 : 0); i < input.Length; i++)
            {
                num = num * 10 + (int)(input[i] - '0');
            }
            if (!flag)
            {
                return num;
            }
            return -num;
        }

        public static Material CreateNullMaterial()
        {
            return new Material(Shader.Find("Standard (Specular setup)"));
        }

        public static Vector3 VectorFromStrArray(string[] cmps)
        {
            float num = OBJLoaderHelper.FastFloatParse(cmps[1]);
            float num2 = OBJLoaderHelper.FastFloatParse(cmps[2]);
            if (cmps.Length == 4)
            {
                float num3 = OBJLoaderHelper.FastFloatParse(cmps[3]);
                return new Vector3(num, num2, num3);
            }
            return new Vector2(num, num2);
        }

        public static Color ColorFromStrArray(string[] cmps, float scalar = 1f)
        {
            float num = OBJLoaderHelper.FastFloatParse(cmps[1]) * scalar;
            float num2 = OBJLoaderHelper.FastFloatParse(cmps[2]) * scalar;
            float num3 = OBJLoaderHelper.FastFloatParse(cmps[3]) * scalar;
            return new Color(num, num2, num3);
        }
    }
}