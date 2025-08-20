using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public static class FloodFiller
    {
        public static void FloodFill(Vector3Int start, Predicate<Vector3Int> shouldEnter, Action<Vector3Int> processor)
        {
            if (FloodFiller.working)
            {
                Log.Error("Nested FloodFiller calls are not allowed.", false);
                return;
            }
            if (!shouldEnter(start))
            {
                return;
            }
            FloodFiller.working = true;
            FloodFiller.queue.Clear();
            FloodFiller.queue.Enqueue(start);
            FloodFiller.visited.Clear();
            FloodFiller.visited.Add(start);
            try
            {
                while (FloodFiller.queue.Count != 0)
                {
                    Vector3Int vector3Int = FloodFiller.queue.Dequeue();
                    processor(vector3Int);
                    for (int i = 0; i < Vector3IntUtility.DirectionsCardinal.Length; i++)
                    {
                        Vector3Int vector3Int2 = vector3Int + Vector3IntUtility.DirectionsCardinal[i];
                        if (!FloodFiller.visited.Contains(vector3Int2) && vector3Int2.InBounds() && shouldEnter(vector3Int2))
                        {
                            FloodFiller.queue.Enqueue(vector3Int2);
                            FloodFiller.visited.Add(vector3Int2);
                        }
                    }
                }
            }
            finally
            {
                FloodFiller.working = false;
                FloodFiller.queue.Clear();
            }
        }

        public static IEnumerable<Vector3Int> FloodFillEnumerable(Vector3Int start, Predicate<Vector3Int> shouldEnter)
        {
            if (FloodFiller.working)
            {
                Log.Error("Nested FloodFiller calls are not allowed.", false);
                yield break;
            }
            if (!shouldEnter(start))
            {
                yield break;
            }
            FloodFiller.working = true;
            FloodFiller.queue.Clear();
            FloodFiller.queue.Enqueue(start);
            FloodFiller.visited.Clear();
            FloodFiller.visited.Add(start);
            try
            {
                while (FloodFiller.queue.Count != 0)
                {
                    Vector3Int cur = FloodFiller.queue.Dequeue();
                    yield return cur;
                    for (int i = 0; i < Vector3IntUtility.DirectionsCardinal.Length; i++)
                    {
                        Vector3Int vector3Int = cur + Vector3IntUtility.DirectionsCardinal[i];
                        if (!FloodFiller.visited.Contains(vector3Int) && vector3Int.InBounds() && shouldEnter(vector3Int))
                        {
                            FloodFiller.queue.Enqueue(vector3Int);
                            FloodFiller.visited.Add(vector3Int);
                        }
                    }
                    cur = default(Vector3Int);
                }
            }
            finally
            {
                FloodFiller.working = false;
                FloodFiller.queue.Clear();
            }
            yield break;
            yield break;
        }

        private static Queue<Vector3Int> queue = new Queue<Vector3Int>();

        private static HashSet<Vector3Int> visited = new HashSet<Vector3Int>();

        private static bool working;
    }
}