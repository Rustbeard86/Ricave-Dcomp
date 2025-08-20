using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ricave.Core;
using UnityEngine;

namespace Ricave.Rendering
{
    public static class ShatterMeshes
    {
        public static void Init()
        {
            ShatterMeshes.GenerateMeshes();
        }

        public static Mesh[] GetShatterMeshes()
        {
            return ShatterMeshes.cachedMeshes;
        }

        public static bool[] GetVisibilityMaskFor(Texture2D texture, Rect? occupiedTextureRect = null)
        {
            bool[] array;
            if (ShatterMeshes.cachedVisibilityMasks.TryGetValue(texture, out array))
            {
                return array;
            }
            ShatterMeshes.CacheVisibilityMaskFor(texture, occupiedTextureRect);
            if (ShatterMeshes.cachedVisibilityMasks.TryGetValue(texture, out array))
            {
                return array;
            }
            return null;
        }

        public static bool[] GetVisibilityMaskFor(Texture2D bodyMap, Color bodyPartColor, Rect? occupiedTextureRect = null)
        {
            bool[] array;
            if (ShatterMeshes.cachedBodyPartVisibilityMasks.TryGetValue(new ValueTuple<Texture2D, Color>(bodyMap, bodyPartColor), out array))
            {
                return array;
            }
            ShatterMeshes.CacheVisibilityMaskFor(bodyMap, bodyPartColor, occupiedTextureRect);
            if (ShatterMeshes.cachedBodyPartVisibilityMasks.TryGetValue(new ValueTuple<Texture2D, Color>(bodyMap, bodyPartColor), out array))
            {
                return array;
            }
            return null;
        }

        public static void CacheVisibilityMaskFor(Texture2D texture, Rect? occupiedTextureRect = null)
        {
            if (texture == null || !texture.isReadable)
            {
                return;
            }
            if (ShatterMeshes.cachedVisibilityMasks.ContainsKey(texture))
            {
                return;
            }
            bool[] mask = new bool[64];
            int width = texture.width;
            int height = texture.height;
            if (occupiedTextureRect != null)
            {
                Rect valueOrDefault = occupiedTextureRect.GetValueOrDefault();
                int j = 0;
                int num = 64;
                while (j < num)
                {
                    int num2;
                    int num3;
                    int num4;
                    int num5;
                    ShatterMeshes.GetMinMaxTexPos(j, width, height, out num2, out num3, out num4, out num5);
                    if (valueOrDefault.Overlaps(new Rect((float)num2 / (float)width, (float)num3 / (float)height, (float)(num4 - num2) / (float)width, (float)(num5 - num3) / (float)height)))
                    {
                        mask[j] = true;
                    }
                    j++;
                }
            }
            else
            {
                Color32[] pixels = texture.GetPixels32Cached();
                Parallel.For(0, 64, delegate (int i)
                {
                    int num6;
                    int num7;
                    int num8;
                    int num9;
                    ShatterMeshes.GetMinMaxTexPos(i, width, height, out num6, out num7, out num8, out num9);
                    for (int k = num6; k <= num8; k++)
                    {
                        for (int l = num7; l <= num9; l++)
                        {
                            int num10 = l * width + k;
                            if (pixels[num10].a > 0)
                            {
                                mask[i] = true;
                                return;
                            }
                        }
                    }
                });
            }
            ShatterMeshes.cachedVisibilityMasks.Add(texture, mask);
        }

        public static void CacheVisibilityMaskFor(Texture2D bodyMap, Color bodyPartColor, Rect? occupiedTextureRect = null)
        {
            if (ShatterMeshes.cachedBodyPartVisibilityMasks.ContainsKey(new ValueTuple<Texture2D, Color>(bodyMap, bodyPartColor)))
            {
                return;
            }
            bool[] mask = new bool[64];
            int width = bodyMap.width;
            int height = bodyMap.height;
            if (occupiedTextureRect != null)
            {
                Rect valueOrDefault = occupiedTextureRect.GetValueOrDefault();
                int j = 0;
                int num = 64;
                while (j < num)
                {
                    int num2;
                    int num3;
                    int num4;
                    int num5;
                    ShatterMeshes.GetMinMaxTexPos(j, width, height, out num2, out num3, out num4, out num5);
                    if (valueOrDefault.Overlaps(new Rect((float)num2 / (float)width, (float)num3 / (float)height, (float)(num4 - num2) / (float)width, (float)(num5 - num3) / (float)height)))
                    {
                        mask[j] = true;
                    }
                    j++;
                }
            }
            else
            {
                Color32[] pixels = bodyMap.GetPixels32Cached();
                Parallel.For(0, 64, delegate (int i)
                {
                    int num6;
                    int num7;
                    int num8;
                    int num9;
                    ShatterMeshes.GetMinMaxTexPos(i, width, height, out num6, out num7, out num8, out num9);
                    for (int k = num6; k <= num8; k++)
                    {
                        for (int l = num7; l <= num9; l++)
                        {
                            int num10 = l * width + k;
                            if (pixels[num10] == bodyPartColor)
                            {
                                mask[i] = true;
                                return;
                            }
                        }
                    }
                });
            }
            ShatterMeshes.cachedBodyPartVisibilityMasks.Add(new ValueTuple<Texture2D, Color>(bodyMap, bodyPartColor), mask);
        }

        private static void GetMinMaxTexPos(int i, int width, int height, out int minX, out int minY, out int maxX, out int maxY)
        {
            int num = i % 8;
            int num2 = i / 8;
            Vector2 vector = ShatterMeshes.GetSegmentPos(num2, num) + new Vector2(0.5f, 0.5f);
            Vector2 vector2 = ShatterMeshes.GetSegmentPos(num2, num + 1) + new Vector2(0.5f, 0.5f);
            Vector2 vector3 = ShatterMeshes.GetSegmentPos(num2 + 1, num + 1) + new Vector2(0.5f, 0.5f);
            Vector2 vector4 = ShatterMeshes.GetSegmentPos(num2 + 1, num) + new Vector2(0.5f, 0.5f);
            minX = Math.Max((int)(Math.Min(Math.Min(Math.Min(vector.x, vector2.x), vector3.x), vector4.x) * (float)width), 0);
            minY = Math.Max((int)(Math.Min(Math.Min(Math.Min(vector.y, vector2.y), vector3.y), vector4.y) * (float)height), 0);
            maxX = Math.Min((int)(Math.Max(Math.Max(Math.Max(vector.x, vector2.x), vector3.x), vector4.x) * (float)width), width - 1);
            maxY = Math.Min((int)(Math.Max(Math.Max(Math.Max(vector.y, vector2.y), vector3.y), vector4.y) * (float)height), height - 1);
        }

        private static void GenerateMeshes()
        {
            ShatterMeshes.cachedMeshes = new Mesh[64];
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Mesh mesh = new Mesh();
                    Vector2 segmentPos = ShatterMeshes.GetSegmentPos(i, j);
                    Vector2 segmentPos2 = ShatterMeshes.GetSegmentPos(i, j + 1);
                    Vector2 segmentPos3 = ShatterMeshes.GetSegmentPos(i + 1, j + 1);
                    Vector2 segmentPos4 = ShatterMeshes.GetSegmentPos(i + 1, j);
                    mesh.SetVertices(new Vector3[] { segmentPos, segmentPos2, segmentPos3, segmentPos4 });
                    mesh.SetUVs(0, new Vector2[]
                    {
                        segmentPos + new Vector2(0.5f, 0.5f),
                        segmentPos2 + new Vector2(0.5f, 0.5f),
                        segmentPos3 + new Vector2(0.5f, 0.5f),
                        segmentPos4 + new Vector2(0.5f, 0.5f)
                    });
                    mesh.SetNormals(new Vector3[]
                    {
                        Vector3.up,
                        Vector3.up,
                        Vector3.up,
                        Vector3.up
                    });
                    mesh.SetTriangles(new int[]
                    {
                        0, 1, 2, 0, 2, 3, 1, 0, 3, 1,
                        3, 2
                    }, 0);
                    ShatterMeshes.cachedMeshes[i * 8 + j] = mesh;
                }
            }
        }

        private static Vector2 GetSegmentPos(int x, int y)
        {
            float num = 0.046875f;
            float num2;
            if (x != 0 && x != 8)
            {
                num2 = (float)Calc.AbsSafe(Calc.CombineHashes<int, int, int>(x, y, 841906456)) / 2.1474836E+09f * num * 2f - num;
            }
            else
            {
                num2 = 0f;
            }
            float num3;
            if (y != 0 && y != 8)
            {
                num3 = (float)Calc.AbsSafe(Calc.CombineHashes<int, int, int>(x, y, 164092412)) / 2.1474836E+09f * num * 2f - num;
            }
            else
            {
                num3 = 0f;
            }
            return new Vector2((float)x / 8f - 0.5f + num2, (float)y / 8f - 0.5f + num3);
        }

        private static Mesh[] cachedMeshes;

        private static Dictionary<Texture2D, bool[]> cachedVisibilityMasks = new Dictionary<Texture2D, bool[]>();

        private static Dictionary<ValueTuple<Texture2D, Color>, bool[]> cachedBodyPartVisibilityMasks = new Dictionary<ValueTuple<Texture2D, Color>, bool[]>();

        private const int Divisions = 8;
    }
}