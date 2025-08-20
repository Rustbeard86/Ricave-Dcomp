using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public static class BiggestCycleFinderHeur<T>
    {
        public static int FindBiggestCycle(T startNode, Func<T, IEnumerable<T>> neighborsGetter)
        {
            int num = BiggestCycleFinderHeur<T>.FindBiggestCycleInternal(startNode, neighborsGetter);
            T t;
            if (neighborsGetter(startNode).TryGetRandomElement<T>(out t))
            {
                num = Math.Max(num, BiggestCycleFinderHeur<T>.FindBiggestCycleInternal(t, neighborsGetter));
            }
            return num;
        }

        private static int FindBiggestCycleInternal(T startNode, Func<T, IEnumerable<T>> neighborsGetter)
        {
            if (BiggestCycleFinderHeur<T>.working)
            {
                Log.Error("Nested FindBiggestCycle calls are not allowed.", false);
                return 0;
            }
            BiggestCycleFinderHeur<T>.working = true;
            BiggestCycleFinderHeur<T>.distances.Clear();
            int num = 1;
            try
            {
                BiggestCycleFinderHeur<T>.distances.Add(startNode, 0);
                BiggestCycleFinderHeur<T>.CheckNode(startNode, neighborsGetter, ref num);
            }
            finally
            {
                BiggestCycleFinderHeur<T>.working = false;
                BiggestCycleFinderHeur<T>.distances.Clear();
            }
            return num;
        }

        private static void CheckNode(T cur, Func<T, IEnumerable<T>> neighborsGetter, ref int biggestCycle)
        {
            int num = BiggestCycleFinderHeur<T>.distances[cur];
            foreach (T t in neighborsGetter(cur).InRandomOrder<T>())
            {
                int num2;
                if (BiggestCycleFinderHeur<T>.distances.TryGetValue(t, out num2))
                {
                    biggestCycle = Math.Max(biggestCycle, num - num2 + 1);
                }
                else
                {
                    BiggestCycleFinderHeur<T>.distances.Add(t, num + 1);
                    BiggestCycleFinderHeur<T>.CheckNode(t, neighborsGetter, ref biggestCycle);
                }
            }
        }

        private static bool working;

        private static Dictionary<T, int> distances = new Dictionary<T, int>();
    }
}