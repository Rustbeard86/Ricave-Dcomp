using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Ricave.Core
{
    public static class SpecTranslationsInjecter
    {
        public static void Inject(Dictionary<string, string> specTranslations, Dictionary<string, string> originalValues)
        {
            HashSet<string> hashSet = new HashSet<string>();
            StringBuilder stringBuilder = new StringBuilder();
            foreach (ValueTuple<string, object, Action<object>, Spec> valueTuple in SpecTranslationsInjecter.GetTranslatableFieldsValuesRecursivelyInAllSpecs())
            {
                string item = valueTuple.Item1;
                object item2 = valueTuple.Item2;
                Action<object> item3 = valueTuple.Item3;
                string text;
                if (specTranslations.TryGetValue(item, out text))
                {
                    List<string> list = item2 as List<string>;
                    if (list != null)
                    {
                        originalValues.Add(item, SpecTranslationsInjecter.ListToString(list));
                        item3(SpecTranslationsInjecter.ParseAsList(text, stringBuilder, item));
                    }
                    else
                    {
                        originalValues.Add(item, XmlUtility.Encode((string)item2));
                        item3(text);
                    }
                    hashSet.Add(item);
                }
            }
            SpecTranslationsInjecter.AddWarningsAboutUnusedTranslations(specTranslations, hashSet, stringBuilder);
            if (stringBuilder.Length != 0)
            {
                string text2 = "The current language has some issues:\n";
                StringBuilder stringBuilder2 = stringBuilder;
                Log.Warning(text2 + ((stringBuilder2 != null) ? stringBuilder2.ToString() : null), false);
            }
        }

        public static IEnumerable<ValueTuple<string, object, Action<object>, Spec>> GetTranslatableFieldsValuesRecursivelyInAllSpecs()
        {
            HashSet<string> keysSoFar = new HashSet<string>();
            HashSet<object> visited = new HashSet<object>();
            new StringBuilder();
            foreach (Spec spec in Get.Specs.AllSpecs)
            {
                foreach (ValueTuple<object, FieldInfo, Action<object>> valueTuple in SpecTranslationsInjecter.GetSavedFieldsValuesRecursively(spec, null, null, visited))
                {
                    object item = valueTuple.Item1;
                    FieldInfo item2 = valueTuple.Item2;
                    Action<object> item3 = valueTuple.Item3;
                    if (item != spec && item2.GetTranslatableAttribute() != null)
                    {
                        string text = item as string;
                        List<string> list = item as List<string>;
                        if (text == null && list == null)
                        {
                            Log.Error("Internal error: Translatable field is not a string or List<string>. Field: " + item2.Name, false);
                        }
                        else if (list == null || list.Count != 0)
                        {
                            string key = SpecTranslationsKeyGenerator.GetKey(spec, text ?? item2.Name, (list != null) ? SpecTranslationsKeyGenerator.SpecialFieldType.List : SpecTranslationsInjecter.GetSpecialFieldType(item2), keysSoFar);
                            keysSoFar.Add(key);
                            yield return new ValueTuple<string, object, Action<object>, Spec>(key, item, item3, spec);
                        }
                    }
                }
                IEnumerator<ValueTuple<object, FieldInfo, Action<object>>> enumerator2 = null;
                spec = null;
            }
            List<Spec>.Enumerator enumerator = default(List<Spec>.Enumerator);
            yield break;
            yield break;
        }

        private static void AddWarningsAboutUnusedTranslations(Dictionary<string, string> specTranslations, HashSet<string> appliedSpecTranslations, StringBuilder warningsSb)
        {
            StringBuilder stringBuilder = null;
            foreach (KeyValuePair<string, string> keyValuePair in specTranslations)
            {
                if (!appliedSpecTranslations.Contains(keyValuePair.Key))
                {
                    if (stringBuilder == null)
                    {
                        stringBuilder = new StringBuilder();
                    }
                    if (stringBuilder.Length != 0)
                    {
                        stringBuilder.Append(", ");
                    }
                    stringBuilder.Append(keyValuePair.Key);
                }
            }
            if (stringBuilder != null)
            {
                if (warningsSb.Length != 0)
                {
                    warningsSb.AppendLine();
                }
                warningsSb.Append("  - Some spec translations reference non-existent keys: ").Append(stringBuilder);
            }
        }

        private static IEnumerable<ValueTuple<object, FieldInfo, Action<object>>> GetSavedFieldsValuesRecursively(object obj, FieldInfo fieldInfo, Action<object> objAssigner, HashSet<object> visited)
        {
            if (obj == null)
            {
                yield break;
            }
            yield return new ValueTuple<object, FieldInfo, Action<object>>(obj, fieldInfo, objAssigner);
            FieldInfo[] array = obj.GetType().GetAllSavedFields();
            for (int i = 0; i < array.Length; i++)
            {
                FieldInfo f = array[i];
                object value = f.GetValue(obj);
                if (value != null && !(value is Spec) && (!obj.GetType().IsValueTypeCached() || visited.Add(value)))
                {
                    object obj2 = value;
                    FieldInfo f2 = f;
                    Action<object> action;
                    Action<object> <> 9__0;
                    if ((action = <> 9__0) == null)
                    {
                        action = (<> 9__0 = delegate (object x)
                        {
                            f.SetValue(obj, x);
                        });
                    }
                    foreach (ValueTuple<object, FieldInfo, Action<object>> valueTuple in SpecTranslationsInjecter.GetSavedFieldsValuesRecursively(obj2, f2, action, visited))
                    {
                        yield return valueTuple;
                    }
                    IEnumerator<ValueTuple<object, FieldInfo, Action<object>>> enumerator = null;
                }
            }
            array = null;
            yield break;
            yield break;
        }

        private static SpecTranslationsKeyGenerator.SpecialFieldType GetSpecialFieldType(FieldInfo fieldInfo)
        {
            if (fieldInfo.DeclaringType.IsSpecCached())
            {
                if (fieldInfo.Name == "label")
                {
                    return SpecTranslationsKeyGenerator.SpecialFieldType.Label;
                }
                if (fieldInfo.Name == "description")
                {
                    return SpecTranslationsKeyGenerator.SpecialFieldType.Description;
                }
            }
            return SpecTranslationsKeyGenerator.SpecialFieldType.None;
        }

        private static List<string> ParseAsList(string str, StringBuilder warningsSb, string keyForReference)
        {
            List<string> list = new List<string>();
            SpecTranslationsInjecter.listElementSb.Clear();
            bool flag = false;
            bool flag2 = false;
            for (int i = 0; i < str.Length; i++)
            {
                if (!str.IsSubstringAt("<li>", i))
                {
                    if (!char.IsWhiteSpace(str[i]) && !flag)
                    {
                        flag = true;
                        warningsSb.AppendInNewLine("  - List translation ").Append(keyForReference).Append(" is malformed");
                    }
                }
                else
                {
                    SpecTranslationsInjecter.listElementSb.Clear();
                    i += "<li>".Length;
                    flag2 = true;
                    while (i < str.Length)
                    {
                        if (str.IsSubstringAt("</li>", i))
                        {
                            flag2 = false;
                            list.Add(SpecTranslationsInjecter.listElementSb.ToString());
                            i += "</li>".Length - 1;
                            break;
                        }
                        SpecTranslationsInjecter.listElementSb.Append(str[i]);
                        i++;
                    }
                }
            }
            if (flag2)
            {
                warningsSb.AppendInNewLine("  - List translation ").Append(keyForReference).Append(" has no closing tag");
            }
            return list;
        }

        public static string ListToString(List<string> list)
        {
            if (list.NullOrEmpty<string>())
            {
                return "";
            }
            SpecTranslationsInjecter.listToStringSb.Clear();
            foreach (string text in list)
            {
                SpecTranslationsInjecter.listToStringSb.AppendInNewLine("<li>");
                if (text != null)
                {
                    SpecTranslationsInjecter.listToStringSb.Append(XmlUtility.Encode(text));
                }
                SpecTranslationsInjecter.listToStringSb.Append("</li>");
            }
            return SpecTranslationsInjecter.listToStringSb.ToString();
        }

        private static StringBuilder listElementSb = new StringBuilder();

        private static StringBuilder listToStringSb = new StringBuilder();
    }
}