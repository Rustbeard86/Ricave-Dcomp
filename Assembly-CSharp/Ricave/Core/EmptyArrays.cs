using System;

namespace Ricave.Core
{
    public static class EmptyArrays<T>
    {
        public static T[] Get()
        {
            return EmptyArrays<T>.array;
        }

        private static readonly T[] array = new T[0];
    }
}