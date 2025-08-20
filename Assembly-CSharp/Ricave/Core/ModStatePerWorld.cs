using System;

namespace Ricave.Core
{
    public class ModStatePerWorld
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

        public World World
        {
            get
            {
                return this.world;
            }
            set
            {
                this.world = value;
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

        [Saved]
        private World world;

        private Mod modCached;
    }
}