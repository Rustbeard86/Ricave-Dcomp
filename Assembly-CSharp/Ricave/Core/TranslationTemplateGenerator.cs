using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Ricave.Core
{
    public static class TranslationTemplateGenerator
    {
        public static void GenerateTranslationTemplate()
        {
            if (Directory.Exists(FilePaths.TranslationTemplate))
            {
                Directory.Delete(FilePaths.TranslationTemplate, true);
            }
            if (!Directory.Exists(FilePaths.TranslationTemplate))
            {
                Directory.CreateDirectory(FilePaths.TranslationTemplate);
            }
            if (!Directory.Exists(FilePaths.TranslationTemplateStrings))
            {
                Directory.CreateDirectory(FilePaths.TranslationTemplateStrings);
            }
            if (!Directory.Exists(FilePaths.TranslationTemplateSpecTranslations))
            {
                Directory.CreateDirectory(FilePaths.TranslationTemplateSpecTranslations);
            }
            foreach (string text in from x in Directory.GetFiles(Path.Combine(FilePaths.CoreLanguages, "English", "Strings"), "*.xml", SearchOption.AllDirectories)
                                    orderby x
                                    select x)
            {
                File.WriteAllText(Path.Combine(FilePaths.TranslationTemplateStrings, Path.GetFileName(text)), TranslationTemplateGenerator.GetStringsTemplate(text));
            }
            foreach (Mod mod in Get.ModManager.ActiveMods)
            {
                string text2 = Path.Combine(FilePaths.ModLanguages(mod), "English", "Strings");
                if (Directory.Exists(text2))
                {
                    foreach (string text3 in from x in Directory.GetFiles(text2, "*.xml", SearchOption.AllDirectories)
                                             orderby x
                                             select x)
                    {
                        if (!Directory.Exists(FilePaths.TranslationTemplateModStrings(mod.ModId)))
                        {
                            Directory.CreateDirectory(FilePaths.TranslationTemplateModStrings(mod.ModId));
                        }
                        File.WriteAllText(Path.Combine(FilePaths.TranslationTemplateModStrings(mod.ModId), Path.GetFileName(text3)), TranslationTemplateGenerator.GetStringsTemplate(text3));
                    }
                }
            }
            Dictionary<ValueTuple<Mod, string>, List<ValueTuple<string, string>>> dictionary = new Dictionary<ValueTuple<Mod, string>, List<ValueTuple<string, string>>>();
            foreach (ValueTuple<string, object, Action<object>, Spec> valueTuple in SpecTranslationsInjecter.GetTranslatableFieldsValuesRecursivelyInAllSpecs())
            {
                string item = valueTuple.Item1;
                object item2 = valueTuple.Item2;
                Spec item3 = valueTuple.Item4;
                List<ValueTuple<string, string>> list;
                if (!dictionary.TryGetValue(new ValueTuple<Mod, string>(item3.ModSource, item3.FileSourceRelPath), out list))
                {
                    list = new List<ValueTuple<string, string>>();
                    dictionary.Add(new ValueTuple<Mod, string>(item3.ModSource, item3.FileSourceRelPath), list);
                }
                string text4;
                if (!Get.ActiveLanguage.TryGetOriginalSpecValue(item, out text4))
                {
                    List<string> list2 = item2 as List<string>;
                    text4 = ((list2 != null) ? SpecTranslationsInjecter.ListToString(list2) : XmlUtility.Encode((string)item2));
                }
                list.Add(new ValueTuple<string, string>(item, text4));
            }
            foreach (KeyValuePair<ValueTuple<Mod, string>, List<ValueTuple<string, string>>> keyValuePair in dictionary)
            {
                ValueTuple<Mod, string> key = keyValuePair.Key;
                Mod item4 = key.Item1;
                string item5 = key.Item2;
                string text5 = Path.Combine((item4 == null) ? FilePaths.TranslationTemplateSpecTranslations : FilePaths.TranslationTemplateModSpecTranslations(item4.ModId), item5);
                string fullName = new FileInfo(text5).Directory.FullName;
                if (!Directory.Exists(fullName))
                {
                    Directory.CreateDirectory(fullName);
                }
                File.WriteAllText(text5, TranslationTemplateGenerator.GetSpecTranslationsTemplate(keyValuePair.Value));
            }
        }

        private static string GetStringsTemplate(string path)
        {
            string text = File.ReadAllText(path);
            return TranslationTemplateGenerator.StringRegex.Replace(text, new MatchEvaluator(TranslationTemplateGenerator.StringRegexMatchEvaluator)).Trim();
        }

        private static string GetSpecTranslationsTemplate(List<ValueTuple<string, string>> fields)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendInNewLine("<").Append("SpecTranslations").Append(">");
            string text = null;
            foreach (ValueTuple<string, string> valueTuple in fields)
            {
                string item = valueTuple.Item1;
                string item2 = valueTuple.Item2;
                int num = item.LastIndexOf('.');
                string text2;
                if (num >= 0)
                {
                    text2 = item.Substring(0, num);
                }
                else
                {
                    text2 = null;
                }
                if (text != text2)
                {
                    stringBuilder.AppendLine();
                    text = text2;
                }
                stringBuilder.AppendInNewLine("  <").Append(item).Append(">");
                string text3;
                if (!Get.ActiveLanguage.IsDefaultLanguage && Get.ActiveLanguage.TryGetSpecTranslation(item, out text3))
                {
                    stringBuilder.Append(TranslationTemplateGenerator.PrettifyListElements(XmlUtility.Encode(text3)));
                }
                stringBuilder.Append("</").Append(item).Append("> <!-- <En>")
                    .Append(XmlUtility.SanitizeComment(item2).IndentLines("  ", true))
                    .Append("</En> -->");
            }
            stringBuilder.AppendLine();
            stringBuilder.AppendInNewLine("</").Append("SpecTranslations").Append(">");
            return stringBuilder.ToString();
        }

        private static string StringRegexMatchEvaluator(Match match)
        {
            string value = match.Groups[1].Value;
            string value2 = match.Groups[2].Value;
            string value3 = match.Groups[3].Value;
            string value4 = match.Groups[4].Value;
            string text;
            if (Get.ActiveLanguage.IsDefaultLanguage || !Get.ActiveLanguage.TryTranslateNoFallback(value2, out text))
            {
                text = "";
            }
            return string.Format("{0}<{1}>{2}</{4}> <!-- <En>{3}</En> -->", new object[]
            {
                value,
                value2,
                XmlUtility.Encode(text),
                XmlUtility.SanitizeComment(value3).IndentLines("  ", true),
                value4
            });
        }

        private static string PrettifyListElements(string str)
        {
            if (!str.Contains("<li>"))
            {
                return str;
            }
            TranslationTemplateGenerator.indentListElementsSb.Clear();
            for (int i = 0; i < str.Length; i++)
            {
                if (str.IsSubstringAt("<li>", i))
                {
                    TranslationTemplateGenerator.indentListElementsSb.Append("\n    ");
                }
                TranslationTemplateGenerator.indentListElementsSb.Append(str[i]);
            }
            TranslationTemplateGenerator.indentListElementsSb.Append("\n  ");
            return TranslationTemplateGenerator.indentListElementsSb.ToString();
        }

        private static readonly Regex StringRegex = new Regex("([ \\t]*)<(.*?)>(.*)</(.*?)>");

        private static StringBuilder indentListElementsSb = new StringBuilder();
    }
}