using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Ricave.Core;
using UnityEngine;

namespace Ricave.Rendering
{
    public static class Texture2DUtility
    {
        public static Color32[] GetPixels32Cached(this Texture2D texture)
        {
            if (texture == null)
            {
                return EmptyArrays<Color32>.Get();
            }
            Color32[] array;
            if (Texture2DUtility.cachedResults.TryGetValue(texture, out array))
            {
                return array;
            }
            Color32[] pixels = texture.GetPixels32();
            Texture2DUtility.cachedResults.Add(texture, pixels);
            return pixels;
        }

        public static void ToFile(this Texture2D texture, string file = "temp.png")
        {
            byte[] array = texture.EncodeToPNG();
            File.WriteAllBytes(file, array);
        }

        public static void GenerateTextureAndSaveToFile(int width, int height, Func<int, int, float> pixelGetter, string file = "temp.png", bool parallel = true)
        {
            Texture2DUtility.GenerateTextureAndSaveToFile(width, height, delegate (int x, int y)
            {
                float num = pixelGetter(x, y);
                return new Color(num, num, num);
            }, file, parallel);
        }

        public static void GenerateTextureAndSaveToFile(int width, int height, Func<int, int, Color> pixelGetter, string file = "temp.png", bool parallel = true)
        {
            Texture2D texture2D = null;
            try
            {
                texture2D = new Texture2D(width, height, TextureFormat.RGBA32, false);
                Color[] pixels = new Color[width * height];
                if (parallel)
                {
                    Parallel.For(0, width, delegate (int x)
                    {
                        for (int k = 0; k < height; k++)
                        {
                            pixels[x + k * width] = pixelGetter(x, k);
                        }
                    });
                }
                else
                {
                    for (int i = 0; i < width; i++)
                    {
                        for (int j = 0; j < height; j++)
                        {
                            pixels[i + j * width] = pixelGetter(i, j);
                        }
                    }
                }
                texture2D.SetPixels(pixels, 0);
                texture2D.ToFile(file);
            }
            finally
            {
                Object.Destroy(texture2D);
            }
        }

        public static Texture2D ToTexture2D(this RenderTexture rt)
        {
            Texture2D texture2D = new Texture2D(rt.width, rt.height);
            RenderTexture active = RenderTexture.active;
            RenderTexture.active = rt;
            Texture2D texture2D2;
            try
            {
                texture2D.ReadPixels(new Rect(0f, 0f, (float)rt.width, (float)rt.height), 0, 0);
                texture2D.Apply();
                texture2D2 = texture2D;
            }
            finally
            {
                RenderTexture.active = active;
            }
            return texture2D2;
        }

        private static Dictionary<Texture, Color32[]> cachedResults = new Dictionary<Texture, Color32[]>();
    }
}