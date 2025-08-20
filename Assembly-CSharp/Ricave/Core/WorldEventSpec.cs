using System;
using UnityEngine;

namespace Ricave.Core
{
    public class WorldEventSpec : Spec, ISaveableEventsReceiver
    {
        public WorldEventSpecBehavior Behavior
        {
            get
            {
                WorldEventSpecBehavior worldEventSpecBehavior;
                if ((worldEventSpecBehavior = this.behavior) == null)
                {
                    worldEventSpecBehavior = (this.behavior = new WorldEventSpecBehavior(this));
                }
                return worldEventSpecBehavior;
            }
        }

        public WorldSituationSpec AddSituation
        {
            get
            {
                return this.addSituation;
            }
        }

        public bool CanBeChosenRandomly
        {
            get
            {
                return this.canBeChosenRandomly;
            }
        }

        public Texture2D Icon
        {
            get
            {
                Texture2D texture2D;
                if ((texture2D = this.icon) == null)
                {
                    WorldSituationSpec worldSituationSpec = this.addSituation;
                    if (worldSituationSpec == null)
                    {
                        return null;
                    }
                    texture2D = worldSituationSpec.Icon;
                }
                return texture2D;
            }
        }

        public void OnSaved()
        {
        }

        public void OnLoaded()
        {
            if (this.behavior != null)
            {
                this.behavior.WorldEventSpec = this;
            }
            if (!this.iconPath.NullOrEmpty())
            {
                this.icon = Assets.Get<Texture2D>(this.iconPath);
            }
        }

        [Saved]
        private WorldEventSpecBehavior behavior;

        [Saved]
        private WorldSituationSpec addSituation;

        [Saved]
        private bool canBeChosenRandomly;

        [Saved]
        private string iconPath;

        private Texture2D icon;
    }
}