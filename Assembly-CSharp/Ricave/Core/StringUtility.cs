using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Ricave.UI;

namespace Ricave.Core
{
    public static class StringUtility
    {
        public static bool NullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static bool NullOrWhitespace(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        public static string CapitalizeFirst(this string str)
        {
            if (str.NullOrEmpty())
            {
                return str;
            }
            if (!char.IsLetter(str[0]) || char.IsUpper(str[0]))
            {
                return str;
            }
            return StringUtility.capitalizeCache.Get(str);
        }

        private static string CapitalizeFirstImpl(this string str)
        {
            if (str.NullOrEmpty())
            {
                return str;
            }
            if (str[0] == '<' && str.Contains(">"))
            {
                int num = str.IndexOf('>');
                if (num == str.Length - 1)
                {
                    return str;
                }
                if (!char.IsLetter(str[num + 1]) || char.IsUpper(str[num + 1]))
                {
                    return str;
                }
                if (num + 1 == str.Length - 1)
                {
                    return str.Substring(0, num + 1) + char.ToUpper(str[num + 1]).ToString();
                }
                return str.Substring(0, num + 1) + char.ToUpper(str[num + 1]).ToString() + str.Substring(num + 2);
            }
            else
            {
                if (!char.IsLetter(str[0]) || char.IsUpper(str[0]))
                {
                    return str;
                }
                if (str.Length == 1)
                {
                    return char.ToUpper(str[0]).ToString();
                }
                return char.ToUpper(str[0]).ToString() + str.Substring(1);
            }
        }

        public static string ToUppercase(this string str)
        {
            if (str.NullOrEmpty())
            {
                return str;
            }
            return StringUtility.uppercaseCache.Get(str);
        }

        public static StringBuilder AppendInNewLine(this StringBuilder sb, string text)
        {
            if (text.NullOrEmpty())
            {
                return sb;
            }
            if (sb.Length != 0)
            {
                sb.AppendLine();
            }
            sb.Append(text);
            return sb;
        }

        public static string AppendedInNewLine(this string str, string text)
        {
            if (text.NullOrEmpty())
            {
                return str;
            }
            if (str.Length != 0)
            {
                return "{0}\n{1}".Formatted(str, text);
            }
            return text;
        }

        public static string AppendedInDoubleNewLine(this string str, string text)
        {
            if (text.NullOrEmpty())
            {
                return str;
            }
            if (str.Length != 0)
            {
                return "{0}\n\n{1}".Formatted(str, text);
            }
            return text;
        }

        public static string AppendedWithComma(this string str, string text)
        {
            if (text.NullOrEmpty())
            {
                return str;
            }
            if (str.Length != 0)
            {
                return "{0}, {1}".Formatted(str, text);
            }
            return text;
        }

        public static string AppendedWithSpace(this string str, string text)
        {
            if (text.NullOrEmpty())
            {
                return str;
            }
            if (str.Length != 0)
            {
                return "{0} {1}".Formatted(str, text);
            }
            return text;
        }

        public static string PadRightWithSpaces(this string str, int toLength, bool ensureAtLeastOneSpace = true)
        {
            int num = str.LengthWithoutTags();
            if (!ensureAtLeastOneSpace && num >= toLength)
            {
                return str;
            }
            int num2 = Math.Max(toLength - num, 1);
            if (num2 == 1)
            {
                return str.Concatenated(" ");
            }
            if (num2 == 2)
            {
                return str.Concatenated("  ");
            }
            if (num2 == 3)
            {
                return str.Concatenated("   ");
            }
            if (num2 == 4)
            {
                return str.Concatenated("    ");
            }
            if (num2 == 5)
            {
                return str.Concatenated("     ");
            }
            if (num2 == 6)
            {
                return str.Concatenated("      ");
            }
            if (num2 == 7)
            {
                return str.Concatenated("       ");
            }
            if (num2 == 8)
            {
                return str.Concatenated("        ");
            }
            return str.Concatenated("         ").PadRightWithSpaces(toLength, false);
        }

        public static string PadLeftWithSpaces(this string str, int toLength, bool ensureAtLeastOneSpace = false)
        {
            int num = str.LengthWithoutTags();
            if (!ensureAtLeastOneSpace && num >= toLength)
            {
                return str;
            }
            int num2 = Math.Max(toLength - num, 1);
            if (num2 == 1)
            {
                return " ".Concatenated(str);
            }
            if (num2 == 2)
            {
                return "  ".Concatenated(str);
            }
            if (num2 == 3)
            {
                return "   ".Concatenated(str);
            }
            if (num2 == 4)
            {
                return "    ".Concatenated(str);
            }
            if (num2 == 5)
            {
                return "     ".Concatenated(str);
            }
            if (num2 == 6)
            {
                return "      ".Concatenated(str);
            }
            if (num2 == 7)
            {
                return "       ".Concatenated(str);
            }
            if (num2 == 8)
            {
                return "        ".Concatenated(str);
            }
            return "         ".Concatenated(str).PadLeftWithSpaces(toLength, false);
        }

        public static string Concatenated(this string str, string text)
        {
            return StringUtility.Concat(str, text);
        }

        public static string ToStringOffset(this int value, bool zeroHasSign = true)
        {
            if (value == 0 && !zeroHasSign)
            {
                return "0";
            }
            if (value < 0)
            {
                return value.ToStringCached();
            }
            if (value < StringUtility.CachedNumbersWithPlus.Length)
            {
                if (StringUtility.CachedNumbersWithPlus[value] == null)
                {
                    StringUtility.CachedNumbersWithPlus[value] = "+" + value.ToStringCached();
                }
                return StringUtility.CachedNumbersWithPlus[value];
            }
            return StringUtility.numberWithPlusCache.Get(value);
        }

        public static string ToStringPercent(this float value, bool roundToInt = false)
        {
            float num = value * 100f;
            if (roundToInt)
            {
                num = Calc.Round(num);
            }
            int num2 = Calc.RoundToInt(num);
            if (num == (float)num2 && num2 >= 0 && num2 < StringUtility.CachedPercents.Length)
            {
                if (StringUtility.CachedPercents[num2] == null)
                {
                    StringUtility.CachedPercents[num2] = num2.ToStringCached() + "%";
                }
                return StringUtility.CachedPercents[num2];
            }
            return StringUtility.percentCache.Get(num);
        }

        public static string ToStringPercentOrWhole(this float value)
        {
            int num = Calc.RoundToInt(value);
            if (value == (float)num)
            {
                return num.ToStringCached();
            }
            return value.ToStringPercent(false);
        }

        public static string ToStringPercentOffset(this float value)
        {
            string text = Math.Abs(value).ToStringPercent(false);
            if (value < 0f && text != "0%")
            {
                return "-{0}".Formatted(text);
            }
            return "+{0}".Formatted(text);
        }

        public static string ToStringFactor(this float value)
        {
            return "x{0}".Formatted(value.ToString("0.##"));
        }

        public static string RangeToString(int from, int to)
        {
            if (from == to)
            {
                return from.ToStringCached();
            }
            if (from < 0 || to < 0)
            {
                return "FromTo".Translate(from, to);
            }
            return "{0}-{1}".Formatted(from, to);
        }

        public static string RangeToStringOffset(int from, int to)
        {
            if (from != to)
            {
                return "FromTo".Translate(from.ToStringOffset(false), to.ToStringOffset(false));
            }
            return from.ToStringOffset(true);
        }

        public static string TurnsString(int turns)
        {
            if (turns != 1)
            {
                return "TurnsCount".Translate(turns);
            }
            return "OneTurn".Translate();
        }

        public static string UsesString(int uses)
        {
            if (uses != 1)
            {
                return "UsesCount".Translate(uses);
            }
            return "OneUse".Translate();
        }

        public static string ChargesString(int uses)
        {
            if (uses != 1)
            {
                return "ChargesCount".Translate(uses);
            }
            return "OneCharge".Translate();
        }

        public static string DiamondsString(int diamonds)
        {
            if (diamonds != 1)
            {
                return "DiamondsCount".Translate(diamonds);
            }
            return "OneDiamond".Translate();
        }

        public static string StardustString(int stardust)
        {
            if (stardust != 1)
            {
                return "StardustCount".Translate(stardust);
            }
            return "OneStardust".Translate();
        }

        public static string CellsString(int cells)
        {
            if (cells != 1)
            {
                return "CellsCount".Translate(cells);
            }
            return "OneCell".Translate();
        }

        public static string GoldString(int gold)
        {
            if (gold != 1)
            {
                return "GoldCount".Translate(gold);
            }
            return "OneGold".Translate();
        }

        public static string DamagePerTurnsString(int damage, int intervalTurns)
        {
            if (intervalTurns != 1)
            {
                return "DamagePerTurns".Translate(damage, intervalTurns);
            }
            return "DamagePerTurn".Translate(damage);
        }

        public static string NumberPerTurnsString(int number, int intervalTurns)
        {
            if (intervalTurns != 1)
            {
                return "NumberPerTurns".Translate(number, intervalTurns);
            }
            return "NumberPerTurn".Translate(number);
        }

        public static string SecondsToTimeRoughStr(this double time)
        {
            int num = (int)(time / 60.0);
            int num2 = num / 60;
            int num3 = num2 / 24;
            if (num3 >= 10)
            {
                if (num3 != 1)
                {
                    return "Days".Translate(num3);
                }
                return "OneDay".Translate();
            }
            else if (num2 >= 1)
            {
                if (num2 != 1)
                {
                    return "Hours".Translate(num2);
                }
                return "OneHour".Translate();
            }
            else
            {
                if (num != 1)
                {
                    return "Minutes".Translate(num);
                }
                return "OneMinute".Translate();
            }
        }

        public static string SecondsToShortTimeRoughStr(this double time)
        {
            int num = (int)(time / 60.0);
            int num2 = num / 1440;
            int num3 = num - num2 * 60 * 24;
            int num4 = num3 / 60;
            int num5 = num3 - num4 * 60;
            string text = "";
            if (num2 >= 1)
            {
                text = "DaysShort".Translate(num2);
            }
            if (num4 >= 1)
            {
                text = text.AppendedWithSpace("HoursShort".Translate(num4));
                if (num2 >= 1)
                {
                    return text;
                }
            }
            if (text.Length == 0 || num5 >= 1)
            {
                text = text.AppendedWithSpace("MinutesShort".Translate(num5));
            }
            return text;
        }

        public static string SecondsToTimeStr(this double time)
        {
            int num = (int)time;
            int num2 = num / 3600;
            num -= num2 * 60 * 60;
            int num3 = num / 60;
            num -= num3 * 60;
            string text = ((num2 < 10) ? "0{0}".Formatted(num2) : num2.ToStringCached());
            string text2 = ((num3 < 10) ? "0{0}".Formatted(num3) : num3.ToStringCached());
            string text3 = ((num < 10) ? "0{0}".Formatted(num) : num.ToStringCached());
            return "{0}:{1}:{2}".Formatted(text, text2, text3);
        }

        public static string ToStringCached(this int number)
        {
            if (number >= 0 && number < StringUtility.CachedNumbers.Length)
            {
                if (StringUtility.CachedNumbers[number] == null)
                {
                    StringUtility.CachedNumbers[number] = number.ToString();
                }
                return StringUtility.CachedNumbers[number];
            }
            if (number < 0 && -number < StringUtility.CachedNegativeNumbers.Length && number != -2147483648)
            {
                if (StringUtility.CachedNegativeNumbers[-number] == null)
                {
                    StringUtility.CachedNegativeNumbers[-number] = number.ToString();
                }
                return StringUtility.CachedNegativeNumbers[-number];
            }
            return StringUtility.intToStringCache.Get(number);
        }

        public static string ToStringCached(this float number)
        {
            int num = Calc.RoundToInt(number);
            if (number == (float)num)
            {
                return num.ToStringCached();
            }
            return StringUtility.floatToStringCache.Get(number);
        }

        public static string ToStringCachedNoRounding(this float number)
        {
            int num = Calc.RoundToInt(number);
            if (number == (float)num)
            {
                return num.ToStringCached();
            }
            return StringUtility.floatToStringNoRoundingCache.Get(number);
        }

        public static string ToStringCached(this char c)
        {
            string text;
            if (StringUtility.CachedChars.TryGetValue(c, out text))
            {
                return text;
            }
            text = c.ToString();
            StringUtility.CachedChars.Add(c, text);
            return text;
        }

        public static string Concat(string arg1, string arg2)
        {
            if (arg1.NullOrEmpty())
            {
                if (arg2.NullOrEmpty())
                {
                    return "";
                }
                return arg2;
            }
            else
            {
                if (arg2.NullOrEmpty())
                {
                    return arg1;
                }
                return "{0}{1}".Formatted(arg1, arg2);
            }
        }

        public static string Concat(string arg1, string arg2, string arg3)
        {
            if (arg3.NullOrEmpty())
            {
                return StringUtility.Concat(arg1, arg2);
            }
            return "{0}{1}{2}".Formatted(arg1, arg2, arg3);
        }

        public static string Concat(string arg1, string arg2, string arg3, string arg4)
        {
            if (arg4.NullOrEmpty())
            {
                return StringUtility.Concat(arg1, arg2, arg3);
            }
            return "{0}{1}{2}{3}".Formatted(arg1, arg2, arg3, arg4);
        }

        public static string Concat(string arg1, string arg2, string arg3, string arg4, string arg5)
        {
            if (arg5.NullOrEmpty())
            {
                return StringUtility.Concat(arg1, arg2, arg3, arg4);
            }
            return "{0}{1}{2}{3}{4}".Formatted(arg1, arg2, arg3, arg4, arg5);
        }

        public static string Concat(string arg1, string arg2, string arg3, string arg4, string arg5, string arg6)
        {
            if (arg6.NullOrEmpty())
            {
                return StringUtility.Concat(arg1, arg2, arg3, arg4, arg5);
            }
            return "{0}{1}{2}{3}{4}{5}".Formatted(arg1, arg2, arg3, arg4, arg5, arg6);
        }

        public static string Truncate(this string str, float toWidth)
        {
            if (str.NullOrEmpty())
            {
                return str;
            }
            if (Widgets.CalcSize(str).x <= toWidth)
            {
                return str;
            }
            return StringUtility.truncateCache.Get(new ValueTuple<string, float, int>(str, toWidth, Widgets.FontSize));
        }

        private static string TruncateImpl(ValueTuple<string, float, int> data)
        {
            string item = data.Item1;
            float item2 = data.Item2;
            if (Widgets.CalcSize(item).x <= item2)
            {
                return item;
            }
            if (item.Contains('<') && item.Contains('>'))
            {
                StringUtility.truncateSb.Clear();
                StringUtility.truncateSb.Append(item);
                for (int i = StringUtility.truncateSb.Length - 1; i >= 0; i--)
                {
                    if (StringUtility.truncateSb[i] == '>')
                    {
                        do
                        {
                            i--;
                            if (i < 0)
                            {
                                break;
                            }
                        }
                        while (StringUtility.truncateSb[i] != '<');
                    }
                    else
                    {
                        StringUtility.truncateSb.Remove(i, 1);
                        StringUtility.truncateSb.Append("...");
                        string text = StringUtility.truncateSb.ToString();
                        if (Widgets.CalcSize(text).x <= item2)
                        {
                            return text;
                        }
                        StringUtility.truncateSb.Length -= 3;
                    }
                }
                return "...";
            }
            StringUtility.truncateSb.Clear();
            string text2 = "...";
            for (int j = 0; j < item.Length; j++)
            {
                StringUtility.truncateSb.Append(item[j]);
                StringUtility.truncateSb.Append("...");
                string text3 = StringUtility.truncateSb.ToString();
                if (Widgets.CalcSize(text3).x > item2)
                {
                    return text2;
                }
                text2 = text3;
                StringUtility.truncateSb.Length -= 3;
            }
            return item;
        }

        public static string TruncateToLength(this string str, int toLength)
        {
            if (str.NullOrEmpty() || str.Length <= toLength)
            {
                return str;
            }
            return str.Substring(0, toLength) + "...";
        }

        public static string FillWidthWithSpaces(float width)
        {
            if (width <= 0f)
            {
                return "";
            }
            string text = "";
            for (int i = 0; i < 1000; i++)
            {
                text += " ";
                if (Widgets.CalcSize(text).x >= width)
                {
                    return text;
                }
            }
            Log.Error("Too many spaces in FillWidthWithSpaces().", false);
            return text;
        }

        public static string IndentLines(this string str, string indentation, bool skipFirst = false)
        {
            if (str.NullOrEmpty())
            {
                return str;
            }
            StringUtility.indentLinesSb.Clear();
            if (!skipFirst)
            {
                StringUtility.indentLinesSb.Append(indentation);
            }
            for (int i = 0; i < str.Length; i++)
            {
                StringUtility.indentLinesSb.Append(str[i]);
                if (str[i] == '\n')
                {
                    StringUtility.indentLinesSb.Append(indentation);
                }
            }
            return StringUtility.indentLinesSb.ToString();
        }

        public static string ToCommaListOr(object obj1, object obj2)
        {
            return "{0} {1} {2}".Formatted(obj1, "Or".Translate(), obj2);
        }

        public static string ToCommaListOr(object obj1, object obj2, object obj3)
        {
            return "{0}, {1}, {2} {3}".Formatted(obj1, obj2, "Or".Translate(), obj3);
        }

        public static string ToCommaListOr(object obj1, object obj2, object obj3, object obj4)
        {
            return "{0}, {1}, {2}, {3} {4}".Formatted(obj1, obj2, obj3, "Or".Translate(), obj4);
        }

        public static string ToCommaListOr(IEnumerable<object> objects)
        {
            StringUtility.commaListSb.Clear();
            IList list = objects as IList;
            if (list != null)
            {
                if (list.Count == 0)
                {
                    return "";
                }
                if (list.Count == 1)
                {
                    return TranslateUtility.GetStringRepresentation(list[0]);
                }
                if (list.Count == 2)
                {
                    return StringUtility.ToCommaListOr(list[0], list[1]);
                }
                if (list.Count == 3)
                {
                    return StringUtility.ToCommaListOr(list[0], list[1], list[2]);
                }
                if (list.Count == 4)
                {
                    return StringUtility.ToCommaListOr(list[0], list[1], list[2], list[3]);
                }
                for (int i = 0; i < list.Count; i++)
                {
                    StringUtility.< ToCommaListOr > g__Process | 65_0(list[i], i, list.Count);
                }
            }
            else
            {
                int num = objects.Count<object>();
                if (num == 0)
                {
                    return "";
                }
                if (num == 1)
                {
                    return TranslateUtility.GetStringRepresentation(objects.FirstOrDefault<object>());
                }
                if (num == 2)
                {
                    return StringUtility.ToCommaListOr(objects.FirstOrDefault<object>(), objects.LastOrDefault<object>());
                }
                int num2 = 0;
                foreach (object obj in objects)
                {
                    StringUtility.< ToCommaListOr > g__Process | 65_0(obj, num2, num);
                    num2++;
                }
            }
            return StringUtility.commaListSb.ToString();
        }

        public static string ToCommaListAnd(object obj1, object obj2)
        {
            return "{0} {1} {2}".Formatted(obj1, "And".Translate(), obj2);
        }

        public static string ToCommaListAnd(object obj1, object obj2, object obj3)
        {
            return "{0}, {1}, {2} {3}".Formatted(obj1, obj2, "And".Translate(), obj3);
        }

        public static string ToCommaListAnd(object obj1, object obj2, object obj3, object obj4)
        {
            return "{0}, {1}, {2}, {3} {4}".Formatted(obj1, obj2, obj3, "And".Translate(), obj4);
        }

        public static string ToCommaListAnd(IEnumerable<object> objects)
        {
            StringUtility.commaListSb.Clear();
            IList list = objects as IList;
            if (list != null)
            {
                if (list.Count == 0)
                {
                    return "";
                }
                if (list.Count == 1)
                {
                    return TranslateUtility.GetStringRepresentation(list[0]);
                }
                if (list.Count == 2)
                {
                    return StringUtility.ToCommaListAnd(list[0], list[1]);
                }
                if (list.Count == 3)
                {
                    return StringUtility.ToCommaListAnd(list[0], list[1], list[2]);
                }
                if (list.Count == 4)
                {
                    return StringUtility.ToCommaListAnd(list[0], list[1], list[2], list[3]);
                }
                for (int i = 0; i < list.Count; i++)
                {
                    StringUtility.< ToCommaListAnd > g__Process | 69_0(list[i], i, list.Count);
                }
            }
            else
            {
                int num = objects.Count<object>();
                if (num == 0)
                {
                    return "";
                }
                if (num == 1)
                {
                    return TranslateUtility.GetStringRepresentation(objects.FirstOrDefault<object>());
                }
                if (num == 2)
                {
                    return StringUtility.ToCommaListAnd(objects.FirstOrDefault<object>(), objects.LastOrDefault<object>());
                }
                int num2 = 0;
                foreach (object obj in objects)
                {
                    StringUtility.< ToCommaListAnd > g__Process | 69_0(obj, num2, num);
                    num2++;
                }
            }
            return StringUtility.commaListSb.ToString();
        }

        public static bool IsSubstringAt(this string str, string substring, int at)
        {
            if (str == null || at < 0 || at >= str.Length)
            {
                return false;
            }
            if (substring.NullOrEmpty())
            {
                return true;
            }
            if (at > str.Length - substring.Length)
            {
                return false;
            }
            for (int i = 0; i < substring.Length; i++)
            {
                if (str[at + i] != substring[i])
                {
                    return false;
                }
            }
            return true;
        }

        public static int StableHashCode(this string str)
        {
            if (str == null)
            {
                return 0;
            }
            int num = 5381;
            int num2 = num;
            for (int i = 0; i < str.Length; i += 2)
            {
                num = ((num << 5) + num) ^ (int)str[i];
                if (i == str.Length - 1)
                {
                    break;
                }
                num2 = ((num2 << 5) + num2) ^ (int)str[i + 1];
            }
            return num + num2 * 1566083941;
        }

        public static int LengthWithoutTags(this string str)
        {
            if (str.Length == 0)
            {
                return 0;
            }
            if (!str.Contains('<') || !str.Contains('>'))
            {
                return str.Length;
            }
            int num = 0;
            bool flag = false;
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == '<')
                {
                    flag = true;
                }
                else if (str[i] == '>')
                {
                    flag = false;
                }
                else if (!flag)
                {
                    num++;
                }
            }
            return num;
        }

        public static string Reversed(this string str)
        {
            if (str == null)
            {
                return null;
            }
            StringUtility.reverseSb.Clear();
            for (int i = str.Length - 1; i >= 0; i--)
            {
                StringUtility.reverseSb.Append(str[i]);
            }
            return StringUtility.reverseSb.ToString();
        }

        public static string ToStringYesNo(this bool value)
        {
            if (!value)
            {
                return "No".Translate();
            }
            return "Yes".Translate();
        }

        [CompilerGenerated]
        internal static void <ToCommaListOr>g__Process|65_0(object obj, int index, int count)
		{
			string stringRepresentation = TranslateUtility.GetStringRepresentation(obj);
			if (index != 0)
			{
				if (index == count - 1)
				{
					if (index != 1)
					{
						StringUtility.commaListSb.Append(",");
					}
    StringUtility.commaListSb.Append(" ").Append("Or".Translate()).Append(" ");
}

                else
{
    StringUtility.commaListSb.Append(", ");
}
			}
			StringUtility.commaListSb.Append(stringRepresentation);
		}

		[CompilerGenerated]
internal static void < ToCommaListAnd > g__Process | 69_0(object obj, int index, int count)

        {
    string stringRepresentation = TranslateUtility.GetStringRepresentation(obj);
    if (index != 0)
    {
        if (index == count - 1)
        {
            if (index != 1)
            {
                StringUtility.commaListSb.Append(",");
            }
            StringUtility.commaListSb.Append(" ").Append("And".Translate()).Append(" ");
        }
        else
        {
            StringUtility.commaListSb.Append(", ");
        }
    }
    StringUtility.commaListSb.Append(stringRepresentation);
}

private static readonly CalculationCache<string, string> capitalizeCache = new CalculationCache<string, string>(new Func<string, string>(StringUtility.CapitalizeFirstImpl), 300);

private static readonly CalculationCache<string, string> uppercaseCache = new CalculationCache<string, string>((string x) => x.ToUpper(), 300);

private static readonly CalculationCache<ValueTuple<string, float, int>, string> truncateCache = new CalculationCache<ValueTuple<string, float, int>, string>(new Func<ValueTuple<string, float, int>, string>(StringUtility.TruncateImpl), 300);

private static readonly CalculationCache<int, string> intToStringCache = new CalculationCache<int, string>((int x) => x.ToString(), 300);

private static readonly CalculationCache<float, string> floatToStringCache = new CalculationCache<float, string>((float x) => x.ToString("0.##"), 300);

private static readonly CalculationCache<float, string> floatToStringNoRoundingCache = new CalculationCache<float, string>((float x) => x.ToString(), 300);

private static readonly CalculationCache<float, string> percentCache = new CalculationCache<float, string>((float x) => x.ToString("0.#") + "%", 300);

private static readonly CalculationCache<int, string> numberWithPlusCache = new CalculationCache<int, string>((int x) => "+" + x.ToStringCached(), 300);

private static readonly string[] CachedNumbers = new string[1000];

private static readonly string[] CachedNumbersWithPlus = new string[1000];

private static readonly string[] CachedNegativeNumbers = new string[1000];

private static readonly string[] CachedPercents = new string[201];

private static readonly Dictionary<char, string> CachedChars = new Dictionary<char, string>();

private static StringBuilder truncateSb = new StringBuilder();

private static StringBuilder indentLinesSb = new StringBuilder();

private static StringBuilder commaListSb = new StringBuilder();

private static StringBuilder reverseSb = new StringBuilder();
	}
}