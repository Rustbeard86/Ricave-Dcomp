using System;
using System.Collections.Generic;
using System.Linq;

namespace Ricave.Core
{
    public class ModsPlayerWantsActive
    {
        public void Save()
        {
            SaveLoadManager.Save(this, FilePaths.ModsPlayerWantsActive, "ActiveMods");
        }

        public int GetOrder(string modId)
        {
            if (modId.NullOrEmpty())
            {
                return -1;
            }
            return this.mods.IndexOf(modId);
        }

        public int GetOrderOrInfinite(string modId)
        {
            if (modId.NullOrEmpty())
            {
                return int.MaxValue;
            }
            int num = this.mods.IndexOf(modId);
            if (num < 0)
            {
                return int.MaxValue;
            }
            return num;
        }

        public bool WantsActive(string modId)
        {
            return !modId.NullOrEmpty() && this.mods.Contains(modId);
        }

        public void SetWantsActive(string modId, bool wantsActive)
        {
            if (modId.NullOrEmpty())
            {
                Log.Error("Tried to toggle active null mod ID.", false);
                return;
            }
            if (wantsActive)
            {
                if (!this.mods.Contains(modId))
                {
                    this.mods.Add(modId);
                    this.hasModsToActivateOrDeactivate = true;
                    return;
                }
            }
            else if (this.mods.Contains(modId))
            {
                this.mods.Remove(modId);
                this.hasModsToActivateOrDeactivate = true;
            }
        }

        public void Reorder(string modId, int to)
        {
            int num = this.mods.IndexOf(modId);
            if (num < 0)
            {
                Log.Error("Tried to reorder mod but it's not here.", false);
                return;
            }
            if (num == to)
            {
                return;
            }
            this.mods.RemoveAt(num);
            this.mods.Insert(to, modId);
            this.hasModsToActivateOrDeactivate = true;
        }

        public void ActivateAndDeactiveModsAsRequested()
        {
            if (!this.hasModsToActivateOrDeactivate)
            {
                return;
            }
            this.hasModsToActivateOrDeactivate = false;
            bool flag = false;
            foreach (Mod mod in Get.ModManager.ActiveMods.ToList<Mod>())
            {
                flag = true;
                Get.ModManager.Disable(mod);
            }
            foreach (string text in this.mods)
            {
                Mod mod2 = Get.ModManager.GetMod(text);
                if (mod2 != null)
                {
                    flag = true;
                    Get.ModManager.Enable(mod2);
                }
            }
            if (flag)
            {
                Log.Message("Reloading all content for mods.");
                Get.ModManager.ReloadAllContent();
            }
        }

        [Saved(Default.New, true)]
        private List<string> mods = new List<string>();

        public const string RootXmlNodeName = "ActiveMods";

        private bool hasModsToActivateOrDeactivate = true;
    }
}