using System;
using System.IO;
using System.Threading;
using System.Xml;

namespace Ricave.Core
{
    public static class SaveLoadManager
    {
        public static bool Save(object start, string filePath, string rootXmlNodeName)
        {
            for (int i = 0; i < 2; i++)
            {
                Profiler.Begin("Save");
                try
                {
                    string text = filePath + "_temp";
                    SaveLoadManager.save.Save(start, text, rootXmlNodeName);
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                    File.Move(text, filePath);
                    return true;
                }
                catch (Exception ex)
                {
                    Log.Error("Could not save data to \"" + filePath + "\".", ex);
                }
                finally
                {
                    Profiler.End();
                }
                if (i == 0)
                {
                    Thread.Sleep(1000);
                }
            }
            return false;
        }

        public static bool Load(object start, string filePath, string rootXmlNodeName)
        {
            string text;
            return SaveLoadManager.Load(start, filePath, rootXmlNodeName, out text);
        }

        public static bool Load(object start, string filePath, string rootXmlNodeName, out string version)
        {
            for (int i = 0; i < 2; i++)
            {
                Profiler.Begin("Load");
                try
                {
                    new SaveLoadManager_Load().Load(start, filePath, rootXmlNodeName, false, out version);
                    return true;
                }
                catch (Exception ex)
                {
                    Log.Error("Could not load data from \"" + filePath + "\".", ex);
                }
                finally
                {
                    Profiler.End();
                }
                if (i == 0)
                {
                    Thread.Sleep(1000);
                }
            }
            version = null;
            return false;
        }

        public static SpecsToResolve LoadSpecs(object start, string filePath, string rootXmlNodeName)
        {
            for (int i = 0; i < 2; i++)
            {
                try
                {
                    string text;
                    return new SaveLoadManager_Load().Load(start, filePath, rootXmlNodeName, true, out text);
                }
                catch (Exception ex)
                {
                    Log.Error("Could not load specs from \"" + filePath + "\".", ex);
                }
                if (i == 0)
                {
                    Thread.Sleep(1000);
                }
            }
            return new SpecsToResolve();
        }

        public static SpecsToResolve LoadSpecs(object start, XmlDocument xmlDoc, string rootXmlNodeName)
        {
            for (int i = 0; i < 2; i++)
            {
                try
                {
                    string text;
                    return new SaveLoadManager_Load().Load(start, xmlDoc, rootXmlNodeName, true, out text);
                }
                catch (Exception ex)
                {
                    Log.Error("Could not load specs from XmlDocument with root node \"" + rootXmlNodeName + "\".", ex);
                }
                if (i == 0)
                {
                    Thread.Sleep(1000);
                }
            }
            return new SpecsToResolve();
        }

        private static SaveLoadManager_Save save = new SaveLoadManager_Save();

        private const string TempSuffix = "_temp";
    }
}