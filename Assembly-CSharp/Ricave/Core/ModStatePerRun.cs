using System;

namespace Ricave.Core
{
    public class ModStatePerRun
    {
        public string ModId
        {
            get
            {
                return this.modId;
            }
            set
            {
                this.modId = value;
            }
        }

        public Mod Mod
        {
            get
            {
                if (this.modCached == null)
                {
                    this.modCached = Get.ModManager.GetMod(this.modId);
                }
                return this.modCached;
            }
        }

        [Saved]
        private string modId;

        private Mod modCached;
    }
}