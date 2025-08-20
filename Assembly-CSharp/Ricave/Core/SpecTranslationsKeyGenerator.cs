using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Ricave.Core
{
    public static class SpecTranslationsKeyGenerator
    {
        public static string GetKey(Spec spec, string text, SpecTranslationsKeyGenerator.SpecialFieldType specialFieldType, HashSet<string> otherKeysSoFar)
        {
            string text2 = ((spec.ModSource != null) ? (spec.ModSource.ModId + ".") : "") + spec.SpecID;
            if (specialFieldType == SpecTranslationsKeyGenerator.SpecialFieldType.Label)
            {
                return SpecTranslationsKeyGenerator.AppendUniqueNumber(text2 + ".label", otherKeysSoFar);
            }
            if (specialFieldType == SpecTranslationsKeyGenerator.SpecialFieldType.Description)
            {
                return SpecTranslationsKeyGenerator.AppendUniqueNumber(text2 + ".description", otherKeysSoFar);
            }
            if (specialFieldType == SpecTranslationsKeyGenerator.SpecialFieldType.List)
            {
                return SpecTranslationsKeyGenerator.AppendUniqueNumber(text2 + "." + text, otherKeysSoFar);
            }
            if (text.NullOrEmpty())
            {
                return SpecTranslationsKeyGenerator.AppendUniqueNumber(text2 + ".Text", otherKeysSoFar);
            }
            if (text.Contains("{"))
            {
                text = SpecTranslationsKeyGenerator.FormatRegex.Replace(text, "");
            }
            SpecTranslationsKeyGenerator.tmpStringBuilder.Clear();
            foreach (string text3 in text.Split(SpecTranslationsKeyGenerator.Separators))
            {
                bool flag = true;
                foreach (char c in text3)
                {
                    if (SpecTranslationsKeyGenerator.WhiteListedCharactersHashSet.Contains(c))
                    {
                        SpecTranslationsKeyGenerator.tmpStringBuilder.Append(flag ? char.ToUpper(c) : c);
                        flag = false;
                        if (SpecTranslationsKeyGenerator.tmpStringBuilder.Length >= 40)
                        {
                            break;
                        }
                    }
                }
                if (SpecTranslationsKeyGenerator.tmpStringBuilder.Length >= 20)
                {
                    break;
                }
            }
            if (SpecTranslationsKeyGenerator.tmpStringBuilder.Length == 0)
            {
                return SpecTranslationsKeyGenerator.AppendUniqueNumber(text2 + ".Text", otherKeysSoFar);
            }
            return SpecTranslationsKeyGenerator.AppendUniqueNumber(text2 + "." + SpecTranslationsKeyGenerator.tmpStringBuilder.ToString(), otherKeysSoFar);
        }

        private static string AppendUniqueNumber(string str, HashSet<string> otherKeysSoFar)
        {
            if (!otherKeysSoFar.Contains(str))
            {
                return str;
            }
            for (int i = 2; i <= 1000; i++)
            {
                string text = str + "_" + i.ToString();
                if (!otherKeysSoFar.Contains(text))
                {
                    return text;
                }
            }
            return str + Rand.Int.ToString();
        }

        private static StringBuilder tmpStringBuilder = new StringBuilder();

        private static readonly char[] Separators = new char[] { ' ', '\n' };

        private static readonly string WhiteListedCharacters = "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM1234567890";

        private static readonly HashSet<char> WhiteListedCharactersHashSet = new HashSet<char>(SpecTranslationsKeyGenerator.WhiteListedCharacters);

        private const int TryStopAfterLength = 20;

        private const int MaxLength = 40;

        private static readonly Regex FormatRegex = new Regex("{([^}]*)}");

        public enum SpecialFieldType
        {
            None,

            Label,

            Description,

            List
        }
    }
}