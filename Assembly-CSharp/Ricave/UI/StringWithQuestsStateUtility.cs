using System;
using System.Text.RegularExpressions;
using Ricave.Core;

namespace Ricave.UI
{
    public static class StringWithQuestsStateUtility
    {
        public static string FormattedQuestsState(this string str)
        {
            if (str == null)
            {
                return str;
            }
            if (!str.Contains("["))
            {
                return str;
            }
            return StringWithQuestsStateUtility.formattedWithQuestsStateCache.Get(str);
        }

        private static string FormattedWithQuestsStateImpl(Match match)
        {
            string value = match.Groups[1].Value;
            return (Get.TotalQuestsState.Get(value) + Get.ThisRunQuestsState.Get(value)).ToStringCached();
        }

        public static void OnQuestsStateChanged()
        {
            StringWithQuestsStateUtility.formattedWithQuestsStateCache.Clear();
        }

        private static readonly CalculationCache<string, string> formattedWithQuestsStateCache = new CalculationCache<string, string>((string x) => StringWithQuestsStateUtility.FormattedWithQuestsStateRegex.Replace(x, StringWithQuestsStateUtility.FormattedWithQuestsStateMatchEvaluator), 30);

        private static readonly MatchEvaluator FormattedWithQuestsStateMatchEvaluator = new MatchEvaluator(StringWithQuestsStateUtility.FormattedWithQuestsStateImpl);

        private static readonly Regex FormattedWithQuestsStateRegex = new Regex("\\[([a-zA-Z0-9_-]*)\\]");
    }
}