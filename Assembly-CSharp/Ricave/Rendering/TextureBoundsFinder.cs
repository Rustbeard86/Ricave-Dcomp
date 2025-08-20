using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ricave.Core;
using UnityEngine;

namespace Ricave.Rendering
{
    public static class TextureBoundsFinder
    {
        public static Rect FindBounds(Texture2D texture)
        {
            if (texture == null)
            {
                return new Rect(0f, 0f, 0f, 0f);
            }
            Rect rect;
            if (TextureBoundsFinder.cachedResults.TryGetValue(texture, out rect))
            {
                return rect;
            }
            if (!texture.isReadable)
            {
                Log.Error("Can't find texture bounds because the texture is not readable.", false);
                return new Rect(0f, 0f, 1f, 1f);
            }
            TextureBoundsFinder.pixels = texture.GetPixels32Cached();
            TextureBoundsFinder.width = texture.width;
            TextureBoundsFinder.height = texture.height;
            TextureBoundsFinder.minX = TextureBoundsFinder.width - 1;
            TextureBoundsFinder.maxX = -1;
            TextureBoundsFinder.minY = TextureBoundsFinder.height - 1;
            TextureBoundsFinder.maxY = -1;
            Parallel.For(0, TextureBoundsFinder.height, TextureBoundsFinder.ParallelBodyDelegate);
            Rect rect2;
            if (TextureBoundsFinder.maxX == -1 || TextureBoundsFinder.maxY == -1)
            {
                rect2 = new Rect(0f, 0f, 0f, 0f);
            }
            else
            {
                rect2 = new Rect((float)TextureBoundsFinder.minX / (float)TextureBoundsFinder.width, (float)TextureBoundsFinder.minY / (float)TextureBoundsFinder.height, (float)(TextureBoundsFinder.maxX - TextureBoundsFinder.minX + 1) / (float)TextureBoundsFinder.width, (float)(TextureBoundsFinder.maxY - TextureBoundsFinder.minY + 1) / (float)TextureBoundsFinder.height);
            }
            TextureBoundsFinder.cachedResults.Add(texture, rect2);
            return rect2;
        }

        private static void ParallelBody(int y)
        {
            int num = y * TextureBoundsFinder.width;
            for (int i = 0; i < TextureBoundsFinder.width; i++)
            {
                int num2 = num + i;
                if (TextureBoundsFinder.pixels[num2].a >= 10)
                {
                    object obj = TextureBoundsFinder.resultLock;
                    lock (obj)
                    {
                        if (i < TextureBoundsFinder.minX)
                        {
                            TextureBoundsFinder.minX = i;
                        }
                        if (i > TextureBoundsFinder.maxX)
                        {
                            TextureBoundsFinder.maxX = i;
                        }
                        if (y < TextureBoundsFinder.minY)
                        {
                            TextureBoundsFinder.minY = y;
                        }
                        if (y > TextureBoundsFinder.maxY)
                        {
                            TextureBoundsFinder.maxY = y;
                        }
                    }
                    for (int j = TextureBoundsFinder.width - 1; j > i; j--)
                    {
                        int num3 = num + j;
                        if (TextureBoundsFinder.pixels[num3].a >= 10)
                        {
                            obj = TextureBoundsFinder.resultLock;
                            lock (obj)
                            {
                                if (j > TextureBoundsFinder.maxX)
                                {
                                    TextureBoundsFinder.maxX = j;
                                }
                                return;
                            }
                        }
                    }
                    return;
                }
            }
        }

        private static Color32[] pixels;

        private static int width;

        private static int height;

        private static int minX;

        private static int maxX;

        private static int minY;

        private static int maxY;

        private static Dictionary<Texture, Rect> cachedResults = new Dictionary<Texture, Rect>();

        private const int AlphaThreshold = 10;

        private static Action<int> ParallelBodyDelegate = new Action<int>(TextureBoundsFinder.ParallelBody);

        private static object resultLock = new object();
    }
}