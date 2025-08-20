using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Ricave.Core
{
    public class Languages
    {
        public ActiveLanguage ActiveLanguage
        {
            get
            {
                return this.activeLanguage;
            }
        }

        public List<LanguageInfo> AllLanguages
        {
            get
            {
                return this.allLanguages;
            }
        }

        public void Clear()
        {
            this.allLanguages.Clear();
            this.activeLanguage = null;
        }

        public void LoadAll()
        {
            if (this.activeLanguage != null || this.allLanguages.Count != 0)
            {
                Log.Error("Called Languages.LoadAll() but some languages are already loaded. They should be cleared first.", false);
                return;
            }
            foreach (string text in FilePaths.AllLanguages)
            {
                string text2 = Path.Combine(text, "Info.xml");
                if (File.Exists(text2))
                {
                    LanguageInfo languageInfo = new LanguageInfo();
                    if (SaveLoadManager.Load(languageInfo, text2, "LanguageInfo"))
                    {
                        this.AddLanguage(languageInfo);
                    }
                }
            }
            string activeLanguageFromPrefs = this.GetActiveLanguageFromPrefs();
            this.ApplyActiveLanguage(activeLanguageFromPrefs);
        }

        public void AddLanguage(LanguageInfo language)
        {
            if (language == null)
            {
                Log.Error("Tried to add null language to Languages.", false);
                return;
            }
            if (language.EnglishName.NullOrEmpty())
            {
                Log.Error("Tried to add language with null English name to Languages.", false);
                return;
            }
            if (this.allLanguages.Contains(language))
            {
                Log.Error("Tried to add the same language twice to Languages.", false);
                return;
            }
            if (this.allLanguages.Any<LanguageInfo>((LanguageInfo x) => x.EnglishName == language.EnglishName))
            {
                Log.Error("Tried to add 2 different languages with the same English name to Languages.", false);
                return;
            }
            this.allLanguages.Add(language);
        }

        private void ApplyActiveLanguage(string englishName)
        {
            if (englishName.NullOrEmpty())
            {
                Log.Error("Tried to set null active language.", false);
                return;
            }
            LanguageInfo languageInfo = this.allLanguages.Find((LanguageInfo x) => x.EnglishName == englishName);
            if (languageInfo == null)
            {
                Log.Error("Tried to set " + englishName + " as active language, but it doesn't exist.", false);
                if (englishName == "English")
                {
                    return;
                }
                englishName = "English";
                languageInfo = this.allLanguages.Find((LanguageInfo x) => x.EnglishName == englishName);
                if (languageInfo == null)
                {
                    return;
                }
            }
            this.activeLanguage = new ActiveLanguage(languageInfo);
            this.activeLanguage.Load();
            this.ApplySpecTranslations();
            foreach (Spec spec in Get.Specs.AllSpecs)
            {
                try
                {
                    spec.OnActiveLanguageChanged();
                }
                catch (Exception ex)
                {
                    string text = "Error in Spec.OnActiveLanguageChanged(): ";
                    Exception ex2 = ex;
                    Log.Error(text + ((ex2 != null) ? ex2.ToString() : null), false);
                }
            }
        }

        private void ApplySpecTranslations()
        {
            if (this.activeLanguage == null)
            {
                Log.Error("Tried to apply spec translations but there's no active language.", false);
                return;
            }
            this.activeLanguage.ApplySpecTranslations();
        }

        private string GetActiveLanguageFromPrefs()
        {
            string text = Prefs.GetString("Language", null);
            if (text.NullOrEmpty())
            {
                text = "English";
            }
            return text;
        }

        public void SetActiveLanguageInPrefs(string englishName)
        {
            if (englishName.NullOrEmpty())
            {
                Log.Error("Tried to set null active language in prefs.", false);
                return;
            }
            if (!this.allLanguages.Any<LanguageInfo>((LanguageInfo x) => x.EnglishName == englishName))
            {
                Log.Error("Tried to set " + englishName + " as active language in prefs, but it doesn't exist.", false);
                return;
            }
            Prefs.SetString("Language", englishName);
            Prefs.Save();
        }

        private List<LanguageInfo> allLanguages = new List<LanguageInfo>();

        private ActiveLanguage activeLanguage;

        private const string LanguagePrefKey = "Language";

        public const string DefaultLanguage = "English";
    }
}