using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Ricave.Core
{
    public class ActiveLanguage
    {
        public LanguageInfo Info
        {
            get
            {
                return this.info;
            }
        }

        public CultureInfo CultureInfo
        {
            get
            {
                return this.cultureInfo;
            }
        }

        public bool IsDefaultLanguage
        {
            get
            {
                return this.info.IsDefaultLanguage;
            }
        }

        public ActiveLanguage(LanguageInfo languageInfo)
        {
            this.info = languageInfo;
            this.cultureInfo = new CultureInfo(languageInfo.CultureName);
        }

        public void Load()
        {
            this.strings.Clear();
            this.specTranslations.Clear();
            this.defaultLanguageStrings.Clear();
            this.nonTranslatableCached.Clear();
            this.splitCached.Clear();
            foreach (string text in FilePaths.AllLanguagesParentFolders)
            {
                string text2 = Path.Combine(text, this.info.EnglishName);
                if (Directory.Exists(text2))
                {
                    ActiveLanguage.TryLoadStringsFrom(Path.Combine(text2, "Strings"), this.strings);
                    ActiveLanguage.TryLoadSpecTranslationsFrom(Path.Combine(text2, "SpecTranslations"), this.specTranslations);
                }
            }
            if (!this.IsDefaultLanguage)
            {
                foreach (string text3 in FilePaths.AllLanguagesParentFolders)
                {
                    string text4 = Path.Combine(text3, "English");
                    if (Directory.Exists(text4))
                    {
                        ActiveLanguage.TryLoadStringsFrom(Path.Combine(text4, "Strings"), this.defaultLanguageStrings);
                    }
                }
            }
        }

        public void ApplySpecTranslations()
        {
            this.originalSpecValues.Clear();
            if (!this.specTranslations.Any<KeyValuePair<string, string>>())
            {
                return;
            }
            SpecTranslationsInjecter.Inject(this.specTranslations, this.originalSpecValues);
        }

        public string Translate(string key)
        {
            string text;
            if (this.strings.TryGetValue(key, out text))
            {
                return text;
            }
            if (this.defaultLanguageStrings.TryGetValue(key, out text))
            {
                return text;
            }
            string text2;
            if (this.nonTranslatableCached.TryGetValue(key, out text2))
            {
                return text2;
            }
            text2 = "?(" + key + ")";
            this.nonTranslatableCached.Add(key, text2);
            return text2;
        }

        public string[] TranslateSplit(string key, string splitBy)
        {
            string[] array;
            if (this.splitCached.TryGetValue(new ValueTuple<string, string>(key, splitBy), out array))
            {
                return array;
            }
            array = key.Translate().Split(splitBy, StringSplitOptions.None);
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = array[i].Trim();
            }
            this.splitCached.Add(new ValueTuple<string, string>(key, splitBy), array);
            return array;
        }

        public bool TryTranslateNoFallback(string key, out string translated)
        {
            return this.strings.TryGetValue(key, out translated);
        }

        public bool TryGetOriginalSpecValue(string fieldKey, out string value)
        {
            return this.originalSpecValues.TryGetValue(fieldKey, out value);
        }

        public bool TryGetSpecTranslation(string key, out string translated)
        {
            return this.specTranslations.TryGetValue(key, out translated);
        }

        private static void TryLoadStringsFrom(string dir, Dictionary<string, string> addTo)
        {
            if (!Directory.Exists(dir))
            {
                return;
            }
            foreach (string text in from x in Directory.GetFiles(dir, "*.xml", SearchOption.AllDirectories)
                                    orderby x
                                    select x)
            {
                Dictionary<string, string> dictionary = new Dictionary<string, string>();
                if (SaveLoadManager.Load(dictionary, text, "Strings"))
                {
                    foreach (KeyValuePair<string, string> keyValuePair in dictionary)
                    {
                        if (!keyValuePair.Value.NullOrEmpty())
                        {
                            addTo[keyValuePair.Key] = keyValuePair.Value;
                        }
                    }
                }
            }
        }

        private static void TryLoadSpecTranslationsFrom(string dir, Dictionary<string, string> addTo)
        {
            if (!Directory.Exists(dir))
            {
                return;
            }
            foreach (string text in from x in Directory.GetFiles(dir, "*.xml", SearchOption.AllDirectories)
                                    orderby x
                                    select x)
            {
                Dictionary<string, string> dictionary = new Dictionary<string, string>();
                if (SaveLoadManager.Load(dictionary, text, "SpecTranslations"))
                {
                    foreach (KeyValuePair<string, string> keyValuePair in dictionary)
                    {
                        if (!keyValuePair.Value.NullOrEmpty())
                        {
                            addTo[keyValuePair.Key] = keyValuePair.Value;
                        }
                    }
                }
            }
        }

        private LanguageInfo info;

        private Dictionary<string, string> strings = new Dictionary<string, string>();

        private Dictionary<string, string> specTranslations = new Dictionary<string, string>();

        private Dictionary<string, string> defaultLanguageStrings = new Dictionary<string, string>();

        private Dictionary<string, string> originalSpecValues = new Dictionary<string, string>();

        private CultureInfo cultureInfo;

        private Dictionary<string, string> nonTranslatableCached = new Dictionary<string, string>();

        private Dictionary<ValueTuple<string, string>, string[]> splitCached = new Dictionary<ValueTuple<string, string>, string[]>();

        public const string StringsRootXmlNodeName = "Strings";

        public const string SpecTranslationsRootXmlNodeName = "SpecTranslations";
    }
}