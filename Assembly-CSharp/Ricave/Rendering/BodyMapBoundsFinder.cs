using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Ricave.Core;
using Ricave.UI;
using UnityEngine;

namespace Ricave.Rendering
{
    public static class BodyMapBoundsFinder
    {
        public static Rect[] FindBounds(Texture2D bodyMap)
        {
            if (bodyMap == null)
            {
                return BodyMapBoundsFinder.EmptyRects;
            }
            Rect[] array;
            if (BodyMapBoundsFinder.cachedResults.TryGetValue(bodyMap, out array))
            {
                return array;
            }
            if (!bodyMap.isReadable)
            {
                Log.Error("Can't find body map bounds because the texture is not readable.", false);
                return BodyMapBoundsFinder.EmptyRects;
            }
            if (BodyMapBoundsFinder.resultLocks[0] == null)
            {
                for (int i = 0; i < BodyMapBoundsFinder.resultLocks.Length; i++)
                {
                    BodyMapBoundsFinder.resultLocks[i] = new object();
                }
            }
            BodyMapBoundsFinder.pixels = bodyMap.GetPixels32Cached();
            BodyMapBoundsFinder.width = bodyMap.width;
            BodyMapBoundsFinder.height = bodyMap.height;
            BodyMapBoundsFinder.result = new ValueTuple<int, int, int, int>[8];
            for (int j = 0; j < BodyMapBoundsFinder.result.Length; j++)
            {
                BodyMapBoundsFinder.result[j] = new ValueTuple<int, int, int, int>(BodyMapBoundsFinder.width - 1, BodyMapBoundsFinder.height - 1, -1, -1);
            }
            Parallel.For(0, BodyMapBoundsFinder.height * 8, BodyMapBoundsFinder.ParallelBodyDelegate);
            Rect[] array2 = new Rect[8];
            for (int k = 0; k < BodyMapBoundsFinder.result.Length; k++)
            {
                ValueTuple<int, int, int, int> valueTuple = BodyMapBoundsFinder.result[k];
                int item = valueTuple.Item1;
                int item2 = valueTuple.Item2;
                int item3 = valueTuple.Item3;
                int item4 = valueTuple.Item4;
                if (item3 == -1 || item4 == -1)
                {
                    array2[k] = new Rect(0f, 0f, 0f, 0f);
                }
                else
                {
                    array2[k] = new Rect((float)item / (float)BodyMapBoundsFinder.width, (float)item2 / (float)BodyMapBoundsFinder.height, (float)(item3 - item + 1) / (float)BodyMapBoundsFinder.width, (float)(item4 - item2 + 1) / (float)BodyMapBoundsFinder.height);
                }
            }
            BodyMapBoundsFinder.cachedResults.Add(bodyMap, array2);
            return array2;
        }

        private static void ParallelBody(int parallelIndex)
        {
            int num = parallelIndex / BodyMapBoundsFinder.height;
            int num2 = parallelIndex % BodyMapBoundsFinder.height;
            Color32 color = BodyPartUtility.ColorToIndex[num];
            int num3 = num2 * BodyMapBoundsFinder.width;
            for (int i = 0; i < BodyMapBoundsFinder.width; i++)
            {
                int num4 = num3 + i;
                if (BodyMapBoundsFinder.pixels[num4].EqualsOperator(color))
                {
                    object obj = BodyMapBoundsFinder.resultLocks[num];
                    lock (obj)
                    {
                        ValueTuple<int, int, int, int> valueTuple = BodyMapBoundsFinder.result[num];
                        int num5 = valueTuple.Item1;
                        int num6 = valueTuple.Item2;
                        int num7 = valueTuple.Item3;
                        int num8 = valueTuple.Item4;
                        if (i < num5)
                        {
                            num5 = i;
                        }
                        if (i > num7)
                        {
                            num7 = i;
                        }
                        if (num2 < num6)
                        {
                            num6 = num2;
                        }
                        if (num2 > num8)
                        {
                            num8 = num2;
                        }
                        BodyMapBoundsFinder.result[num] = new ValueTuple<int, int, int, int>(num5, num6, num7, num8);
                    }
                    for (int j = BodyMapBoundsFinder.width - 1; j > i; j--)
                    {
                        int num9 = num3 + j;
                        if (BodyMapBoundsFinder.pixels[num9].EqualsOperator(color))
                        {
                            obj = BodyMapBoundsFinder.resultLocks[num];
                            lock (obj)
                            {
                                ValueTuple<int, int, int, int> valueTuple2 = BodyMapBoundsFinder.result[num];
                                int item = valueTuple2.Item1;
                                int item2 = valueTuple2.Item2;
                                int num10 = valueTuple2.Item3;
                                int item3 = valueTuple2.Item4;
                                if (j > num10)
                                {
                                    num10 = j;
                                }
                                BodyMapBoundsFinder.result[num] = new ValueTuple<int, int, int, int>(item, item2, num10, item3);
                                return;
                            }
                        }
                    }
                    return;
                }
            }
        }

        private static Color32[] pixels;

        [TupleElementNames(new string[] { "minX", "minY", "maxX", "maxY" })]
        private static ValueTuple<int, int, int, int>[] result;

        private static int width;

        private static int height;

        private static Dictionary<Texture, Rect[]> cachedResults = new Dictionary<Texture, Rect[]>();

        private static Action<int> ParallelBodyDelegate = new Action<int>(BodyMapBoundsFinder.ParallelBody);

        private static readonly Rect[] EmptyRects = new Rect[8];

        private static object[] resultLocks = new object[8];
    }
}