using System;

namespace Ricave.Core
{
    public static class XmlUtility
    {
        public static string Encode(string str)
        {
            return str.Replace("\n", "\\n");
        }

        public static string Decode(string str)
        {
            return str.Replace("\\n", "\n");
        }

        public static string SanitizeComment(string str)
        {
            return str.Replace("--", "").Replace("-->", "");
        }
    }
}