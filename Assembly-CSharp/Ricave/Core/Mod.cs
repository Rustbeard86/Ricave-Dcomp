using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Ricave.Rendering;
using UnityEngine;

namespace Ricave.Core
{
    public class Mod
    {
        public ModInfo Info
        {
            get
            {
                return this.info;
            }
        }

        public string ModId
        {
            get
            {
                return this.info.ModId;
            }
        }

        public bool Active
        {
            get
            {
                return this.active;
            }
        }

        public Dictionary<string, Object> Assets
        {
            get
            {
                return this.assets;
            }
        }

        public List<Assembly> Assemblies
        {
            get
            {
                return this.assemblies;
            }
        }

        public Mod(ModInfo info)
        {
            this.info = info;
        }

        public ModEventsHandler GetOrCreateEventsHandler()
        {
            if (this.eventsHandler != null)
            {
                return this.eventsHandler;
            }
            if (this.failedToCreateEventsHandler)
            {
                return null;
            }
            Type type = TypeUtility.GetType(this.info.EventsHandler);
            if (type == null)
            {
                Log.Error(string.Concat(new string[]
                {
                    "Mod events handler type ",
                    this.info.EventsHandler,
                    " for mod ",
                    (this != null) ? this.ToString() : null,
                    " doesn't exist."
                }), false);
                this.failedToCreateEventsHandler = true;
                return null;
            }
            ModEventsHandler modEventsHandler;
            try
            {
                this.eventsHandler = (ModEventsHandler)Activator.CreateInstance(type, new object[] { this });
                modEventsHandler = this.eventsHandler;
            }
            catch (Exception ex)
            {
                Log.Error("Error while creating mod events handler for mod " + ((this != null) ? this.ToString() : null) + ".", ex);
                this.failedToCreateEventsHandler = true;
                modEventsHandler = null;
            }
            return modEventsHandler;
        }

        public void OnActivated()
        {
            this.active = true;
            if (!this.eventsHandlerInitialized)
            {
                this.eventsHandlerInitialized = true;
                try
                {
                    this.LoadAssets();
                }
                catch (Exception ex)
                {
                    Log.Error("Error while loading mod assets.", ex);
                }
                try
                {
                    this.LoadAssemblies();
                }
                catch (Exception ex2)
                {
                    Log.Error("Error while loading assemblies.", ex2);
                }
                try
                {
                    this.GetOrCreateEventsHandler().Initialize();
                }
                catch (Exception ex3)
                {
                    Log.Error("Error in ModEventsHandler.Initialize().", ex3);
                }
            }
            try
            {
                this.GetOrCreateEventsHandler().OnEnabled();
            }
            catch (Exception ex4)
            {
                Log.Error("Error in ModEventsHandler.OnEnabled().", ex4);
            }
        }

        public void OnDeactivated()
        {
            this.active = false;
            try
            {
                this.GetOrCreateEventsHandler().OnDisabled();
            }
            catch (Exception ex)
            {
                Log.Error("Error in ModEventsHandler.OnDisabled().", ex);
            }
        }

        private void LoadAssets()
        {
            this.ClearAndDestroyAssets();
            string text = FilePaths.ModAssets(this);
            if (!Directory.Exists(text))
            {
                return;
            }
            foreach (string text2 in from x in Directory.GetFiles(text, "*", SearchOption.AllDirectories)
                                     orderby x
                                     select x)
            {
                string extension = Path.GetExtension(text2);
                Object @object;
                if (!(extension == ".meta") && !(extension == ".DS_Store") && RuntimeAssetLoader.TryLoad(text2, out @object))
                {
                    string text3 = FilePathUtility.GetRelativePath(FilePathUtility.GetFilePathWithoutExtension(text2), text);
                    text3 = FilePathUtility.NormalizeToForwardSlashes(text3);
                    @object.name = text3;
                    this.assets.Add(text3, @object);
                }
            }
        }

        private void LoadAssemblies()
        {
            if (this.assemblies.Any())
            {
                Log.Error("Called LoadAssemblies() but the assemblies are already loaded.", false);
                return;
            }
            if (!Directory.Exists(FilePaths.ModAssemblies(this)))
            {
                return;
            }
            using (IEnumerator<string> enumerator = (from x in Directory.GetFiles(FilePaths.ModAssemblies(this), "*.dll", SearchOption.AllDirectories)
                                                     orderby x
                                                     select x).GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    Assembly assembly;
                    if (AssemblyLoadUtility.TryLoadAssembly(enumerator.Current, out assembly))
                    {
                        this.assemblies.Add(assembly);
                    }
                }
            }
            for (int i = this.assemblies.Count - 1; i >= 0; i--)
            {
                if (!AssemblyLoadUtility.VerifyAssembly(this.assemblies[i]))
                {
                    this.assemblies.RemoveAt(i);
                }
            }
        }

        private void ClearAndDestroyAssets()
        {
            foreach (KeyValuePair<string, Object> keyValuePair in this.assets)
            {
                Object.Destroy(keyValuePair.Value);
            }
            this.assets.Clear();
        }

        public bool TryGetAsset<T>(string path, out T asset) where T : Object
        {
            Object @object;
            if (this.assets.TryGetValue(path, out @object))
            {
                T t = @object as T;
                if (t != null)
                {
                    asset = t;
                    return true;
                }
            }
            asset = default(T);
            return false;
        }

        public override string ToString()
        {
            return this.info.ToString();
        }

        private ModInfo info;

        private ModEventsHandler eventsHandler;

        private bool active;

        private bool failedToCreateEventsHandler;

        private bool eventsHandlerInitialized;

        private Dictionary<string, Object> assets = new Dictionary<string, Object>();

        private List<Assembly> assemblies = new List<Assembly>();
    }
}