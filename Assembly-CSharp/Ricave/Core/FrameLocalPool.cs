using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Ricave.Core
{
    public static class FrameLocalPool<T> where T : new()
    {
        static FrameLocalPool()
        {
            FrameLocalPools.RegisterPool(new Action(FrameLocalPool<T>.OnNewFrame), new Action(FrameLocalPool<T>.Clear));
            Type typeFromHandle = typeof(T);
            if (typeFromHandle.IsGenericTypeCached() && typeFromHandle.GetGenericTypeDefinition() == typeof(HashSet<>))
            {
                MethodInfo clearMethod = typeFromHandle.GetMethod("Clear");
                FrameLocalPool<T>.customClearCollectionAction = delegate (T x)
                {
                    clearMethod.Invoke(x, null);
                };
            }
        }

        public static T Get()
        {
            T t;
            if (FrameLocalPool<T>.available.Count == 0)
            {
                t = new T();
            }
            else
            {
                int num = FrameLocalPool<T>.available.Count - 1;
                t = FrameLocalPool<T>.available[num];
                FrameLocalPool<T>.available.RemoveAt(num);
            }
            if (FrameLocalPool<T>.yieldedThisFrame.Count < 100)
            {
                FrameLocalPool<T>.yieldedThisFrame.Add(t);
            }
            return t;
        }

        public static void OnNewFrame()
        {
            if (FrameLocalPool<T>.yieldedThisFrame.Count == 0)
            {
                return;
            }
            if (FrameLocalPool<T>.yieldedThisFrame[0] is IList)
            {
                for (int i = 0; i < FrameLocalPool<T>.yieldedThisFrame.Count; i++)
                {
                    IList list = FrameLocalPool<T>.yieldedThisFrame[i] as IList;
                    if (list != null)
                    {
                        list.Clear();
                    }
                }
            }
            if (FrameLocalPool<T>.yieldedThisFrame[0] is IDictionary)
            {
                for (int j = 0; j < FrameLocalPool<T>.yieldedThisFrame.Count; j++)
                {
                    IDictionary dictionary = FrameLocalPool<T>.yieldedThisFrame[j] as IDictionary;
                    if (dictionary != null)
                    {
                        dictionary.Clear();
                    }
                }
            }
            if (FrameLocalPool<T>.customClearCollectionAction != null)
            {
                for (int k = 0; k < FrameLocalPool<T>.yieldedThisFrame.Count; k++)
                {
                    FrameLocalPool<T>.customClearCollectionAction(FrameLocalPool<T>.yieldedThisFrame[k]);
                }
            }
            FrameLocalPool<T>.available.AddRange(FrameLocalPool<T>.yieldedThisFrame);
            FrameLocalPool<T>.yieldedThisFrame.Clear();
        }

        public static void Clear()
        {
            FrameLocalPool<T>.available.Clear();
            FrameLocalPool<T>.yieldedThisFrame.Clear();
        }

        private static List<T> available = new List<T>();

        private static List<T> yieldedThisFrame = new List<T>();

        private static Action<T> customClearCollectionAction;

        private const int MaxCapacity = 100;
    }
}