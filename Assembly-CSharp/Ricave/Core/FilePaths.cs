using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ricave.Rendering;

namespace Ricave.Core
{
    public static class FilePaths
    {
        private static string RootPath
        {
            get
            {
                if (FilePaths.isEditor)
                {
                    return FilePaths.applicationDataPath;
                }
                return new DirectoryInfo(FilePaths.applicationDataPath).Parent.FullName;
            }
        }

        public static string Data
        {
            get
            {
                return Path.Combine(FilePaths.RootPath, "Data");
            }
        }

        public static string Mods
        {
            get
            {
                return Path.Combine(FilePaths.RootPath, "Mods");
            }
        }

        public static string Builds
        {
            get
            {
                return Path.Combine(new DirectoryInfo(FilePaths.applicationDataPath).Parent.FullName, "Builds");
            }
        }

        public static string Saves
        {
            get
            {
                if (FilePaths.isEditor)
                {
                    return Path.Combine(new DirectoryInfo(FilePaths.applicationDataPath).Parent.FullName, "Saves");
                }
                return FilePaths.applicationPersistentDataPath;
            }
        }

        public static string CoreSpecs
        {
            get
            {
                return Path.Combine(FilePaths.Data, "Specs");
            }
        }

        public static string CoreLanguages
        {
            get
            {
                return Path.Combine(FilePaths.Data, "Languages");
            }
        }

        public static string Progress
        {
            get
            {
                return FilePaths.Saves + "/Progress.prg";
            }
        }

        public static string ModsPlayerWantsActive
        {
            get
            {
                return FilePaths.Saves + "/ActiveMods.xml";
            }
        }

        public static string Prefs
        {
            get
            {
                return FilePaths.Saves + "/Prefs.xml";
            }
        }

        public static string Savefile(string name)
        {
            return FilePaths.Saves + "/" + name + ".sav";
        }

        public static IEnumerable<ValueTuple<string, Mod>> CoreAndActiveModsDirectories
        {
            get
            {
                yield return new ValueTuple<string, Mod>(FilePaths.Data, null);
                foreach (Mod mod in Get.ModManager.ActiveMods)
                {
                    yield return new ValueTuple<string, Mod>(mod.Info.Directory, mod);
                }
                List<Mod>.Enumerator enumerator = default(List<Mod>.Enumerator);
                yield break;
                yield break;
            }
        }

        public static IEnumerable<ValueTuple<string, Mod, string>> AllSpecFiles
        {
            get
            {
                foreach (ValueTuple<string, Mod> valueTuple in FilePaths.CoreAndActiveModsDirectories)
                {
                    string item = valueTuple.Item1;
                    Mod mod = valueTuple.Item2;
                    string specsDir = Path.Combine(item, "Specs");
                    if (Directory.Exists(specsDir))
                    {
                        foreach (string text in from x in Directory.GetFiles(specsDir, "*.xml", SearchOption.AllDirectories)
                                                orderby x
                                                select x)
                        {
                            string relativePath = FilePathUtility.GetRelativePath(text, specsDir);
                            yield return new ValueTuple<string, Mod, string>(text, mod, relativePath);
                        }
                        IEnumerator<string> enumerator2 = null;
                        specsDir = null;
                        mod = null;
                    }
                }
                IEnumerator<ValueTuple<string, Mod>> enumerator = null;
                yield break;
                yield break;
            }
        }

        public static string[] AllModsDirectories
        {
            get
            {
                if (!Directory.Exists(FilePaths.Mods))
                {
                    return EmptyArrays<string>.Get();
                }
                return Directory.GetDirectories(FilePaths.Mods);
            }
        }

        public static IEnumerable<string> AllLanguages
        {
            get
            {
                foreach (string text in FilePaths.AllLanguagesParentFolders)
                {
                    foreach (string text2 in Directory.GetDirectories(text))
                    {
                        yield return text2;
                    }
                    string[] array = null;
                }
                IEnumerator<string> enumerator = null;
                yield break;
                yield break;
            }
        }

        public static IEnumerable<string> AllLanguagesParentFolders
        {
            get
            {
                foreach (ValueTuple<string, Mod> valueTuple in FilePaths.CoreAndActiveModsDirectories)
                {
                    string text = Path.Combine(valueTuple.Item1, "Languages");
                    if (Directory.Exists(text))
                    {
                        yield return text;
                    }
                }
                IEnumerator<ValueTuple<string, Mod>> enumerator = null;
                yield break;
                yield break;
            }
        }

        public static string TranslationTemplate
        {
            get
            {
                return Path.Combine(FilePaths.RootPath, "TranslationTemplate" + (Get.ActiveLanguage.IsDefaultLanguage ? "" : ("_" + Get.ActiveLanguage.Info.EnglishName)));
            }
        }

        public static string TranslationTemplateStrings
        {
            get
            {
                return Path.Combine(FilePaths.TranslationTemplate, "Strings");
            }
        }

        public static string TranslationTemplateSpecTranslations
        {
            get
            {
                return Path.Combine(FilePaths.TranslationTemplate, "SpecTranslations");
            }
        }

        public static string TranslationTemplateModStrings(string modId)
        {
            return Path.Combine(FilePaths.TranslationTemplate, "Mods", modId, "Strings");
        }

        public static string TranslationTemplateModSpecTranslations(string modId)
        {
            return Path.Combine(FilePaths.TranslationTemplate, "Mods", modId, "SpecTranslations");
        }

        public static string ModAssets(Mod mod)
        {
            return Path.Combine(mod.Info.Directory, "Assets");
        }

        public static string ModLanguages(Mod mod)
        {
            return Path.Combine(mod.Info.Directory, "Languages");
        }

        public static string ModAssemblies(Mod mod)
        {
            return Path.Combine(mod.Info.Directory, "Assemblies");
        }

        public static string ModPreviewImage(ModInfo modInfo)
        {
            return Path.Combine(modInfo.Directory, "Preview.png");
        }

        public static void Init()
        {
            FilePaths.isEditor = App.InEditor;
            FilePaths.applicationDataPath = App.DataPath;
            FilePaths.applicationPersistentDataPath = App.PersistentDataPath;
            if (App.InEditor)
            {
                string saves = FilePaths.Saves;
                if (!Directory.Exists(saves))
                {
                    Directory.CreateDirectory(saves);
                }
            }
        }

        private static bool isEditor;

        private static string applicationDataPath;

        private static string applicationPersistentDataPath;

        private const string SavefilesFolderInEditor = "Saves";

        public const string SavefileExtension = ".sav";

        public const string DataFolder = "Data";

        public const string ModsFolder = "Mods";

        private const string SpecsFolder = "Specs";

        private const string BuildsFolder = "Builds";

        public const string SpecsExtension = ".xml";

        private const string LanguagesFolder = "Languages";

        public const string LanguageStringsExtension = ".xml";

        public const string LanguageSpecTranslationsExtension = ".xml";

        public const string DefaultSavefileName = "Current";

        private const string ProgressFileName = "Progress";

        private const string ProgressFileExtension = ".prg";

        public const string ModInfoFileName = "Info";

        public const string ModInfoFileExtension = ".xml";

        public const string LanguageInfoFileName = "Info";

        public const string LanguageInfoFileExtension = ".xml";

        public const string ModsPlayerWantsActiveFileName = "ActiveMods";

        public const string ModsPlayerWantsActiveFileExtension = ".xml";

        public const string PrefsFileName = "Prefs";

        public const string PrefsFileExtension = ".xml";

        public const string LanguageStringsFolder = "Strings";

        public const string LanguageSpecTranslationsFolder = "SpecTranslations";

        public const string ModAssetsFolder = "Assets";

        public const string ModAssembliesFolder = "Assemblies";
    }
}