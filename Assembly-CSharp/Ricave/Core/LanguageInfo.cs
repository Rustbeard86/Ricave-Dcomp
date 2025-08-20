using System;

namespace Ricave.Core
{
    public class LanguageInfo
    {
        public string EnglishName
        {
            get
            {
                return this.englishName;
            }
        }

        public string NativeName
        {
            get
            {
                return this.nativeName;
            }
        }

        public string CultureName
        {
            get
            {
                return this.cultureName;
            }
        }

        public bool IsDefaultLanguage
        {
            get
            {
                return this.englishName == "English";
            }
        }

        public string EnglishAndNativeName
        {
            get
            {
                if (this.IsDefaultLanguage)
                {
                    return this.englishName;
                }
                return "{0} ({1})".Formatted(this.englishName, this.nativeName);
            }
        }

        public override string ToString()
        {
            return this.englishName;
        }

        [Saved]
        private string englishName;

        [Saved]
        private string nativeName;

        [Saved]
        private string cultureName;
    }
}