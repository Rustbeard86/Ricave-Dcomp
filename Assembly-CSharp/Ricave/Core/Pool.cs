using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public static class Pool<T> where T : new()
    {
        public static T Get()
        {
            if (Pool<T>.pool.Count == 0)
            {
                return new T();
            }
            int num = Pool<T>.pool.Count - 1;
            T t = Pool<T>.pool[num];
            Pool<T>.pool.RemoveAt(num);
            return t;
        }

        public static void Return(T obj)
        {
            if (Pool<T>.pool.Count >= 100)
            {
                return;
            }
            if (obj == null)
            {
                Log.Error("Tried to return null object to Pool.", false);
                return;
            }
            Pool<T>.pool.Add(obj);
        }

        private static List<T> pool = new List<T>();

        private const int MaxCapacity = 100;
    }
}