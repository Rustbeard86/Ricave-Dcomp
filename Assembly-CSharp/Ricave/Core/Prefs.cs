using System;
using System.Collections.Generic;
using System.IO;

namespace Ricave.Core
{
    public static class Prefs
    {
        static Prefs()
        {
            Prefs.Load();
        }

        public static string GetString(string name, string defaultValue = null)
        {
            return Prefs.data.strings.GetOrDefault(name, defaultValue);
        }

        public static void SetString(string name, string value)
        {
            Prefs.data.strings[name] = value;
        }

        public static int GetInt(string name, int defaultValue = 0)
        {
            return Prefs.data.ints.GetOrDefault(name, defaultValue);
        }

        public static void SetInt(string name, int value)
        {
            Prefs.data.ints[name] = value;
        }

        public static float GetFloat(string name, float defaultValue = 0f)
        {
            return Prefs.data.floats.GetOrDefault(name, defaultValue);
        }

        public static void SetFloat(string name, float value)
        {
            Prefs.data.floats[name] = value;
        }

        public static void DeleteKey(string name)
        {
            Prefs.data.strings.Remove(name);
            Prefs.data.ints.Remove(name);
            Prefs.data.floats.Remove(name);
        }

        public static void Clear()
        {
            Prefs.data.strings.Clear();
            Prefs.data.ints.Clear();
            Prefs.data.floats.Clear();
        }

        private static void Load()
        {
            if (File.Exists(FilePaths.Prefs))
            {
                SaveLoadManager.Load(Prefs.data, FilePaths.Prefs, "Prefs");
            }
        }

        public static void Save()
        {
            SaveLoadManager.Save(Prefs.data, FilePaths.Prefs, "Prefs");
        }

        private static Prefs.Data data = new Prefs.Data();

        private const string RootXmlNodeName = "Prefs";

        private class Data
        {
            [Saved(Default.New, false)]
            public Dictionary<string, string> strings = new Dictionary<string, string>();

            [Saved(Default.New, false)]
            public Dictionary<string, int> ints = new Dictionary<string, int>();

            [Saved(Default.New, false)]
            public Dictionary<string, float> floats = new Dictionary<string, float>();
        }
    }
}