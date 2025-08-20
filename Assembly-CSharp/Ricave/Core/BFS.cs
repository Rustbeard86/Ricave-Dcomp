using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public static class BFS<T>
    {
        public static void TraverseAll(T startNode, Func<T, IEnumerable<T>> neighborsGetter, Dictionary<T, int> outDistances, Action<T> visitor = null)
        {
            if (BFS<T>.working)
            {
                Log.Error("Nested BFS calls are not allowed.", false);
                return;
            }
            BFS<T>.working = true;
            BFS<T>.queue.Clear();
            BFS<T>.queue.Enqueue(startNode);
            outDistances.Clear();
            outDistances.Add(startNode, 0);
            try
            {
                while (BFS<T>.queue.Count != 0)
                {
                    T t = BFS<T>.queue.Dequeue();
                    if (visitor != null)
                    {
                        visitor(t);
                    }
                    int num = outDistances[t];
                    foreach (T t2 in neighborsGetter(t))
                    {
                        if (!outDistances.ContainsKey(t2))
                        {
                            outDistances.Add(t2, num + 1);
                            BFS<T>.queue.Enqueue(t2);
                        }
                    }
                }
            }
            finally
            {
                BFS<T>.working = false;
                BFS<T>.queue.Clear();
            }
        }

        public static void TraverseAll(IEnumerable<T> startNodes, Func<T, IEnumerable<T>> neighborsGetter, Dictionary<T, int> outDistances, Action<T> visitor = null)
        {
            if (BFS<T>.working)
            {
                Log.Error("Nested BFS calls are not allowed.", false);
                return;
            }
            BFS<T>.working = true;
            BFS<T>.queue.Clear();
            outDistances.Clear();
            foreach (T t in startNodes)
            {
                BFS<T>.queue.Enqueue(t);
                outDistances.Add(t, 0);
            }
            try
            {
                while (BFS<T>.queue.Count != 0)
                {
                    T t2 = BFS<T>.queue.Dequeue();
                    if (visitor != null)
                    {
                        visitor(t2);
                    }
                    int num = outDistances[t2];
                    foreach (T t3 in neighborsGetter(t2))
                    {
                        if (!outDistances.ContainsKey(t3))
                        {
                            outDistances.Add(t3, num + 1);
                            BFS<T>.queue.Enqueue(t3);
                        }
                    }
                }
            }
            finally
            {
                BFS<T>.working = false;
                BFS<T>.queue.Clear();
            }
        }

        private static Queue<T> queue = new Queue<T>();

        private static bool working;
    }
}