using System;

namespace Ricave.Core
{
    public static class ObjectUtility
    {
        public static bool EqualsSafe(this object first, object second)
        {
            if (first == null)
            {
                return second == null;
            }
            return second != null && first.Equals(second);
        }

        public static string ToStringSafe(this object obj)
        {
            string text;
            try
            {
                text = ((obj != null) ? obj.ToString() : "null");
            }
            catch (Exception)
            {
                text = "[excepted]";
            }
            return text;
        }
    }
}