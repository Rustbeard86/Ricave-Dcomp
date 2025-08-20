using System;
using System.Collections.Generic;
using System.IO;
using Steamworks;

namespace Ricave.Core
{
    public class ModManager
    {
        public List<Mod> ActiveAndInactiveMods
        {
            get
            {
                return this.mods;
            }
        }

        public List<Mod> ActiveMods
        {
            get
            {
                return this.activeModsCached;
            }
        }

        public ModsEventsManager ModsEventsManager
        {
            get
            {
                return this.modsEventsManager;
            }
        }

        public ModsPlayerWantsActive ModsPlayerWantsActive
        {
            get
            {
                return this.modsPlayerWantsActive;
            }
        }

        public Mod GetMod(string modId)
        {
            if (modId.NullOrEmpty())
            {
                return null;
            }
            for (int i = 0; i < this.mods.Count; i++)
            {
                if (this.mods[i].ModId == modId)
                {
                    return this.mods[i];
                }
            }
            return null;
        }

        public void AddMod(Mod mod)
        {
            if (mod == null)
            {
                Log.Error("Tried to add null mod to ModManager.", false);
                return;
            }
            if (mod.ModId.NullOrEmpty())
            {
                Log.Error("Tried to add mod with null ID to ModManager.", false);
                return;
            }
            if (this.mods.Contains(mod))
            {
                Log.Error("Tried to add the same mod twice to ModManager.", false);
                return;
            }
            if (this.GetMod(mod.ModId) != null)
            {
                Log.Error("Tried to add 2 different mods with the same ID to ModManager.", false);
                return;
            }
            if (mod.Active)
            {
                Log.Error("Tried to add an already active mod to ModManager.", false);
                return;
            }
            this.WarnAboutModIdFormat(mod.ModId);
            this.mods.Add(mod);
        }

        public void Enable(Mod mod)
        {
            if (mod == null)
            {
                Log.Error("Tried to enable null mod.", false);
                return;
            }
            if (!this.mods.Contains(mod))
            {
                Log.Error("Tried to enable mod but it's not added to ModManager.", false);
                return;
            }
            if (mod.Active)
            {
                Log.Error("Tried to enable mod but it's already active.", false);
                return;
            }
            if (mod.GetOrCreateEventsHandler() == null)
            {
                Log.Error("Tried to enable mod but there was an error while instantiating the events handler.", false);
                return;
            }
            this.activeModsCached.Add(mod);
            TypeUtility.ClearCache();
            SaveableUtility.ClearCache();
            TranslatableUtility.ClearCache();
            mod.OnActivated();
            this.modsEventsManager.ClearEventSubscribers();
        }

        public void Disable(Mod mod)
        {
            if (mod == null)
            {
                Log.Error("Tried to disable null mod.", false);
                return;
            }
            if (!this.mods.Contains(mod))
            {
                Log.Error("Tried to disable mod but it's not added to ModManager.", false);
                return;
            }
            if (!mod.Active)
            {
                Log.Error("Tried to disable mod but it's already disabled.", false);
                return;
            }
            this.activeModsCached.Remove(mod);
            TypeUtility.ClearCache();
            SaveableUtility.ClearCache();
            TranslatableUtility.ClearCache();
            mod.OnDeactivated();
            this.modsEventsManager.ClearEventSubscribers();
        }

        public void LoadAll()
        {
            if (this.mods.Count != 0)
            {
                Log.Error("Called ModManager.LoadAll() but some mods are already loaded. They should be cleared first.", false);
                return;
            }
            this.modsPlayerWantsActive = new ModsPlayerWantsActive();
            if (File.Exists(FilePaths.ModsPlayerWantsActive))
            {
                SaveLoadManager.Load(this.modsPlayerWantsActive, FilePaths.ModsPlayerWantsActive, "ActiveMods");
            }
            foreach (string text in FilePaths.AllModsDirectories)
            {
                ModInfo modInfo = new ModInfo(text);
                if (SaveLoadManager.Load(modInfo, Path.Combine(text, "Info.xml"), "ModInfo"))
                {
                    this.AddMod(new Mod(modInfo));
                }
            }
            foreach (ValueTuple<PublishedFileId_t, string> valueTuple in SteamWorkshop.SubscribedItems)
            {
                ModInfo modInfo2 = new ModInfo(valueTuple.Item2, valueTuple.Item1);
                if (SaveLoadManager.Load(modInfo2, Path.Combine(valueTuple.Item2, "Info.xml"), "ModInfo"))
                {
                    this.AddMod(new Mod(modInfo2));
                }
            }
        }

        public void ClearAllContent()
        {
            Get.Specs.DestroyAndClear();
            Get.Languages.Clear();
            this.modsEventsManager.ClearEventSubscribers();
        }

        public void ReloadAllContent()
        {
            this.ClearAllContent();
            Get.Specs.LoadAll();
            Get.Languages.LoadAll();
            Root.LoadProgress();
            this.modsEventsManager.AskAllActiveModsToSubscribeToEvents();
            this.modsEventsManager.CallEvent(ModEventType.ContentReloaded, null);
        }

        private void WarnAboutModIdFormat(string modId)
        {
            if (modId.NullOrEmpty())
            {
                return;
            }
            if (char.IsLetter(modId[0]) && char.IsLower(modId[0]))
            {
                Log.Warning("Mod ID \"" + modId + "\" starts with a lowercase letter. The proper format is \"ThisIsAnExample\".", false);
            }
            if (char.IsDigit(modId[0]))
            {
                Log.Warning("Mod ID \"" + modId + "\" starts with a digit. The proper format is \"ThisIsAnExample\".", false);
            }
            for (int i = 0; i < modId.Length; i++)
            {
                if (!char.IsLetter(modId[i]) && !char.IsDigit(modId[i]) && modId[i] != '-' && modId[i] != '_')
                {
                    Log.Warning("Mod ID \"" + modId + "\" uses disallowed characters. Only letters, digits, '-', and '_' are allowed.", false);
                    return;
                }
            }
        }

        private List<Mod> mods = new List<Mod>();

        private ModsPlayerWantsActive modsPlayerWantsActive;

        private ModsEventsManager modsEventsManager = new ModsEventsManager();

        private List<Mod> activeModsCached = new List<Mod>();
    }
}