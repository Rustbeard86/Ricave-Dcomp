using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public static class EmptyLists<T>
    {
        public static List<T> Get()
        {
            return EmptyLists<T>.list;
        }

        private static readonly List<T> list = new List<T>();
    }
}