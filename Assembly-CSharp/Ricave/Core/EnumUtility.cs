using System;

namespace Ricave.Core
{
    public static class EnumUtility
    {
        public static T[] GetValues<T>() where T : Enum
        {
            return (T[])Enum.GetValues(typeof(T));
        }

        public static int GetMaxValue<T>() where T : Enum
        {
            int num = 0;
            T[] values = EnumUtility.GetValues<T>();
            for (int i = 0; i < values.Length; i++)
            {
                int num2 = Convert.ToInt32(values[i]);
                if (num2 != 2147483647)
                {
                    num = Math.Max(num, num2);
                }
            }
            return num;
        }
    }
}