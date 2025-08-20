using System;
using Ricave.UI;

namespace Ricave.Core
{
    public static class TranslateUtility
    {
        public static string Translate(this string str)
        {
            return Get.ActiveLanguage.Translate(str);
        }

        public static string Translate(this string str, object arg1)
        {
            return Get.ActiveLanguage.Translate(str).Formatted(arg1);
        }

        public static string Translate(this string str, object arg1, object arg2)
        {
            return Get.ActiveLanguage.Translate(str).Formatted(arg1, arg2);
        }

        public static string Translate(this string str, object arg1, object arg2, object arg3)
        {
            return Get.ActiveLanguage.Translate(str).Formatted(arg1, arg2, arg3);
        }

        public static string Translate(this string str, object arg1, object arg2, object arg3, object arg4)
        {
            return Get.ActiveLanguage.Translate(str).Formatted(arg1, arg2, arg3, arg4);
        }

        public static string Translate(this string str, object arg1, object arg2, object arg3, object arg4, object arg5)
        {
            return Get.ActiveLanguage.Translate(str).Formatted(arg1, arg2, arg3, arg4, arg5);
        }

        public static string Translate(this string str, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6)
        {
            return Get.ActiveLanguage.Translate(str).Formatted(arg1, arg2, arg3, arg4, arg5, arg6);
        }

        public static string Translate(this string str, params object[] args)
        {
            return Get.ActiveLanguage.Translate(str).Formatted(args);
        }

        public static string Translate(this string str, int arg1)
        {
            return Get.ActiveLanguage.Translate(str).Formatted(arg1);
        }

        public static string Translate(this string str, int arg1, int arg2)
        {
            return Get.ActiveLanguage.Translate(str).Formatted(arg1, arg2);
        }

        public static string Translate(this string str, int arg1, int arg2, int arg3)
        {
            return Get.ActiveLanguage.Translate(str).Formatted(arg1, arg2, arg3);
        }

        public static string Translate(this string str, int arg1, int arg2, int arg3, int arg4)
        {
            return Get.ActiveLanguage.Translate(str).Formatted(arg1, arg2, arg3, arg4);
        }

        public static string[] TranslateSplit(this string str, string splitBy)
        {
            return Get.ActiveLanguage.TranslateSplit(str, splitBy);
        }

        public static string Formatted(this string str, object arg1)
        {
            return str.FormatCached(TranslateUtility.GetStringRepresentation(arg1));
        }

        public static string Formatted(this string str, object arg1, object arg2)
        {
            return str.FormatCached(TranslateUtility.GetStringRepresentation(arg1), TranslateUtility.GetStringRepresentation(arg2));
        }

        public static string Formatted(this string str, object arg1, object arg2, object arg3)
        {
            return str.FormatCached(TranslateUtility.GetStringRepresentation(arg1), TranslateUtility.GetStringRepresentation(arg2), TranslateUtility.GetStringRepresentation(arg3));
        }

        public static string Formatted(this string str, object arg1, object arg2, object arg3, object arg4)
        {
            return str.FormatCached(TranslateUtility.GetStringRepresentation(arg1), TranslateUtility.GetStringRepresentation(arg2), TranslateUtility.GetStringRepresentation(arg3), TranslateUtility.GetStringRepresentation(arg4));
        }

        public static string Formatted(this string str, object arg1, object arg2, object arg3, object arg4, object arg5)
        {
            return str.FormatCached(TranslateUtility.GetStringRepresentation(arg1), TranslateUtility.GetStringRepresentation(arg2), TranslateUtility.GetStringRepresentation(arg3), TranslateUtility.GetStringRepresentation(arg4), TranslateUtility.GetStringRepresentation(arg5));
        }

        public static string Formatted(this string str, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6)
        {
            return str.FormatCached(TranslateUtility.GetStringRepresentation(arg1), TranslateUtility.GetStringRepresentation(arg2), TranslateUtility.GetStringRepresentation(arg3), TranslateUtility.GetStringRepresentation(arg4), TranslateUtility.GetStringRepresentation(arg5), TranslateUtility.GetStringRepresentation(arg6));
        }

        public static string Formatted(this string str, int arg1)
        {
            return str.FormatCached(arg1.ToStringCached());
        }

        public static string Formatted(this string str, int arg1, int arg2)
        {
            return str.FormatCached(arg1.ToStringCached(), arg2.ToStringCached());
        }

        public static string Formatted(this string str, int arg1, int arg2, int arg3)
        {
            return str.FormatCached(arg1.ToStringCached(), arg2.ToStringCached(), arg3.ToStringCached());
        }

        public static string Formatted(this string str, int arg1, int arg2, int arg3, int arg4)
        {
            return str.FormatCached(arg1.ToStringCached(), arg2.ToStringCached(), arg3.ToStringCached(), arg4.ToStringCached());
        }

        public static string Formatted(this string str, params object[] args)
        {
            if (args == null || args.Length == 0)
            {
                return str;
            }
            string[] array;
            if (args.Length < TranslateUtility.CachedStringArrays.Length)
            {
                if (TranslateUtility.CachedStringArrays[args.Length] == null)
                {
                    TranslateUtility.CachedStringArrays[args.Length] = new string[args.Length];
                }
                array = TranslateUtility.CachedStringArrays[args.Length];
            }
            else
            {
                array = new string[args.Length];
            }
            for (int i = 0; i < args.Length; i++)
            {
                array[i] = TranslateUtility.GetStringRepresentation(args[i]);
            }
            object[] array2 = array;
            string text = string.Format(str, array2);
            for (int j = 0; j < args.Length; j++)
            {
                array[j] = null;
            }
            return text;
        }

        public static string GetStringRepresentation(object obj)
        {
            string text = obj as string;
            if (text != null)
            {
                return text;
            }
            if (obj == null)
            {
                return "";
            }
            Entity entity = obj as Entity;
            if (entity != null)
            {
                return RichText.Label(entity);
            }
            EntitySpec entitySpec = obj as EntitySpec;
            if (entitySpec != null)
            {
                return RichText.Label(entitySpec, false);
            }
            IUsable usable = obj as IUsable;
            if (usable != null)
            {
                return RichText.Label(usable);
            }
            Faction faction = obj as Faction;
            if (faction != null)
            {
                return RichText.Label(faction);
            }
            if (obj is Target)
            {
                Target target = (Target)obj;
                return RichText.Label(target);
            }
            ITipSubject tipSubject = obj as ITipSubject;
            if (tipSubject != null)
            {
                return RichText.Label(tipSubject);
            }
            Condition condition = obj as Condition;
            if (condition != null)
            {
                return condition.LabelCap;
            }
            UseEffect useEffect = obj as UseEffect;
            if (useEffect != null)
            {
                return useEffect.LabelCap;
            }
            Conditions conditions = obj as Conditions;
            if (conditions != null)
            {
                if (conditions.Parent == null)
                {
                    return "Conditions";
                }
                return RichText.Label(conditions.Parent);
            }
            else
            {
                UseEffects useEffects = obj as UseEffects;
                if (useEffects != null)
                {
                    if (useEffects.Parent == null)
                    {
                        return "UseEffects";
                    }
                    return RichText.Label(useEffects.Parent);
                }
                else
                {
                    BodyPart bodyPart = obj as BodyPart;
                    if (bodyPart != null)
                    {
                        return RichText.Label(bodyPart);
                    }
                    Spec spec = obj as Spec;
                    if (spec != null)
                    {
                        return spec.LabelCap;
                    }
                    PriceTag priceTag = obj as PriceTag;
                    if (priceTag != null)
                    {
                        return priceTag.PriceRichText;
                    }
                    if (obj is int)
                    {
                        int num = (int)obj;
                        return num.ToStringCached();
                    }
                    return obj.ToString();
                }
            }
        }

        private static string FormatCached(this string str, string arg1)
        {
            return TranslateUtility.format1Cache.Get(new ValueTuple<string, string>(str, arg1));
        }

        private static string FormatCached(this string str, string arg1, string arg2)
        {
            return TranslateUtility.format2Cache.Get(new ValueTuple<string, string, string>(str, arg1, arg2));
        }

        private static string FormatCached(this string str, string arg1, string arg2, string arg3)
        {
            return TranslateUtility.format3Cache.Get(new ValueTuple<string, string, string, string>(str, arg1, arg2, arg3));
        }

        private static string FormatCached(this string str, string arg1, string arg2, string arg3, string arg4)
        {
            return TranslateUtility.format4Cache.Get(new ValueTuple<string, string, string, string, string>(str, arg1, arg2, arg3, arg4));
        }

        private static string FormatCached(this string str, string arg1, string arg2, string arg3, string arg4, string arg5)
        {
            return TranslateUtility.format5Cache.Get(new ValueTuple<string, string, string, string, string, string>(str, arg1, arg2, arg3, arg4, arg5));
        }

        private static string FormatCached(this string str, string arg1, string arg2, string arg3, string arg4, string arg5, string arg6)
        {
            return TranslateUtility.format6Cache.Get(new ValueTuple<string, string, string, string, string, string, string>(str, arg1, arg2, arg3, arg4, arg5, arg6));
        }

        public static void ValidateTranslationKey(string key)
        {
            if (key.NullOrEmpty())
            {
                return;
            }
            string text;
            if (!Get.ActiveLanguage.TryTranslateNoFallback(key, out text))
            {
                Log.Warning("Translation key \"" + key + "\" doesn't exist.", false);
            }
        }

        private static readonly string[][] CachedStringArrays = new string[20][];

        private static readonly CalculationCache<ValueTuple<string, string>, string> format1Cache = new CalculationCache<ValueTuple<string, string>, string>((ValueTuple<string, string> x) => string.Format(x.Item1, x.Item2), 300);

        private static readonly CalculationCache<ValueTuple<string, string, string>, string> format2Cache = new CalculationCache<ValueTuple<string, string, string>, string>((ValueTuple<string, string, string> x) => string.Format(x.Item1, x.Item2, x.Item3), 300);

        private static readonly CalculationCache<ValueTuple<string, string, string, string>, string> format3Cache = new CalculationCache<ValueTuple<string, string, string, string>, string>((ValueTuple<string, string, string, string> x) => string.Format(x.Item1, x.Item2, x.Item3, x.Item4), 300);

        private static readonly CalculationCache<ValueTuple<string, string, string, string, string>, string> format4Cache = new CalculationCache<ValueTuple<string, string, string, string, string>, string>((ValueTuple<string, string, string, string, string> x) => string.Format(x.Item1, new object[] { x.Item2, x.Item3, x.Item4, x.Item5 }), 300);

        private static readonly CalculationCache<ValueTuple<string, string, string, string, string, string>, string> format5Cache = new CalculationCache<ValueTuple<string, string, string, string, string, string>, string>((ValueTuple<string, string, string, string, string, string> x) => string.Format(x.Item1, new object[] { x.Item2, x.Item3, x.Item4, x.Item5, x.Item6 }), 300);

        private static readonly CalculationCache<ValueTuple<string, string, string, string, string, string, string>, string> format6Cache = new CalculationCache<ValueTuple<string, string, string, string, string, string, string>, string>((ValueTuple<string, string, string, string, string, string, string> x) => string.Format(x.Item1, new object[] { x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7 }), 300);
    }
}