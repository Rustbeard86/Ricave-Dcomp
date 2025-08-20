using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class CosmeticsManager
    {
        public List<CosmeticSpec> Unlocked
        {
            get
            {
                return this.unlocked;
            }
        }

        public List<CosmeticSpec> Chosen
        {
            get
            {
                return this.chosen;
            }
        }

        public bool IsUnlocked(CosmeticSpec cosmeticSpec)
        {
            return cosmeticSpec != null && this.unlocked.Contains(cosmeticSpec);
        }

        public bool IsChosen(CosmeticSpec cosmeticSpec)
        {
            return cosmeticSpec != null && this.chosen.Contains(cosmeticSpec);
        }

        public void Unlock(CosmeticSpec cosmeticSpec)
        {
            if (cosmeticSpec == null)
            {
                Log.Error("Tried to unlock null CosmeticSpec.", false);
                return;
            }
            if (this.IsUnlocked(cosmeticSpec))
            {
                Log.Error("Tried to unlock the same CosmeticSpec twice: " + cosmeticSpec.SpecID, false);
                return;
            }
            this.unlocked.Add(cosmeticSpec);
            this.timeUnlocked[cosmeticSpec] = Clock.UnscaledTime;
        }

        public void RemoveFromUnlocked(CosmeticSpec cosmeticSpec)
        {
            if (cosmeticSpec == null)
            {
                Log.Error("Tried to remove from unlocked null CosmeticSpec.", false);
                return;
            }
            if (!this.IsUnlocked(cosmeticSpec))
            {
                Log.Error("Tried to remove from unlocked a CosmeticSpec that is not unlocked: " + cosmeticSpec.SpecID, false);
                return;
            }
            this.unlocked.Remove(cosmeticSpec);
            this.chosen.Remove(cosmeticSpec);
            this.timeUnlocked.Remove(cosmeticSpec);
        }

        public void Choose(CosmeticSpec cosmeticSpec)
        {
            if (cosmeticSpec == null)
            {
                Log.Error("Tried to choose null CosmeticSpec.", false);
                return;
            }
            if (!this.IsUnlocked(cosmeticSpec))
            {
                Log.Error("Tried to choose a CosmeticSpec which isn't unlocked: " + cosmeticSpec.SpecID, false);
                return;
            }
            if (this.IsChosen(cosmeticSpec))
            {
                Log.Error("Tried to choose the same CosmeticSpec twice: " + cosmeticSpec.SpecID, false);
                return;
            }
            this.chosen.Add(cosmeticSpec);
        }

        public void Unchoose(CosmeticSpec cosmeticSpec)
        {
            if (cosmeticSpec == null)
            {
                Log.Error("Tried to unchoose null CosmeticSpec.", false);
                return;
            }
            if (!this.IsChosen(cosmeticSpec))
            {
                Log.Error("Tried to unchoose a CosmeticSpec which isn't chosen: " + cosmeticSpec.SpecID, false);
                return;
            }
            this.chosen.Remove(cosmeticSpec);
        }

        public float GetTimeUnlocked(CosmeticSpec cosmeticSpec)
        {
            if (cosmeticSpec == null)
            {
                return -99999f;
            }
            return this.timeUnlocked.GetOrDefault(cosmeticSpec, -99999f);
        }

        [Saved(Default.New, true)]
        private List<CosmeticSpec> unlocked = new List<CosmeticSpec>();

        [Saved(Default.New, true)]
        private List<CosmeticSpec> chosen = new List<CosmeticSpec>();

        private Dictionary<CosmeticSpec, float> timeUnlocked = new Dictionary<CosmeticSpec, float>();
    }
}