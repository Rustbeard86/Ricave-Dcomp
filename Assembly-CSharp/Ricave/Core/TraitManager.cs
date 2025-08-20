using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class TraitManager
    {
        public List<TraitSpec> Unlocked
        {
            get
            {
                return this.unlocked;
            }
        }

        public HashSet<TraitSpec> Chosen
        {
            get
            {
                if (Get.InMainMenu || Get.InLobby || !Get.RunConfig.IsDailyChallenge)
                {
                    return this.chosen;
                }
                return DailyChallengeUtility.TraitsForThisRun;
            }
        }

        public float GoldPerFloorFactor
        {
            get
            {
                float num = 1f;
                foreach (TraitSpec traitSpec in this.Chosen)
                {
                    num *= traitSpec.GoldPerFloorFactor;
                }
                return num;
            }
        }

        public int MaxHPOffset
        {
            get
            {
                int num = 0;
                foreach (TraitSpec traitSpec in this.Chosen)
                {
                    num += traitSpec.MaxHPOffset;
                }
                return num;
            }
        }

        public int MaxManaOffset
        {
            get
            {
                int num = 0;
                foreach (TraitSpec traitSpec in this.Chosen)
                {
                    num += traitSpec.MaxManaOffset;
                }
                return num;
            }
        }

        public int MaxStaminaOffset
        {
            get
            {
                int num = 0;
                foreach (TraitSpec traitSpec in this.Chosen)
                {
                    num += traitSpec.MaxStaminaOffset;
                }
                return num;
            }
        }

        public float PoisonIncomingDamageFactor
        {
            get
            {
                float num = 1f;
                foreach (TraitSpec traitSpec in this.Chosen)
                {
                    num *= traitSpec.PoisonIncomingDamageFactor;
                }
                return num;
            }
        }

        public float FallIncomingDamageFactor
        {
            get
            {
                float num = 1f;
                foreach (TraitSpec traitSpec in this.Chosen)
                {
                    num *= traitSpec.FallIncomingDamageFactor;
                }
                return num;
            }
        }

        public float FireIncomingDamageFactor
        {
            get
            {
                float num = 1f;
                foreach (TraitSpec traitSpec in this.Chosen)
                {
                    num *= traitSpec.FireIncomingDamageFactor;
                }
                return num;
            }
        }

        public float MagicIncomingDamageFactor
        {
            get
            {
                float num = 1f;
                foreach (TraitSpec traitSpec in this.Chosen)
                {
                    num *= traitSpec.MagicIncomingDamageFactor;
                }
                return num;
            }
        }

        public int NativeStaminaRegenIntervalFactor
        {
            get
            {
                int num = 1;
                foreach (TraitSpec traitSpec in this.Chosen)
                {
                    num *= traitSpec.NativeStaminaRegenIntervalFactor;
                }
                return num;
            }
        }

        public int MaxHPOffsetPerLevel
        {
            get
            {
                int num = 0;
                foreach (TraitSpec traitSpec in this.Chosen)
                {
                    num += traitSpec.MaxHPOffsetPerLevel;
                }
                return num;
            }
        }

        public int WalkingNoiseOffset
        {
            get
            {
                int num = 0;
                foreach (TraitSpec traitSpec in this.Chosen)
                {
                    num += traitSpec.WalkingNoiseOffset;
                }
                return num;
            }
        }

        public int HPPerWorthyKill
        {
            get
            {
                int num = 0;
                foreach (TraitSpec traitSpec in this.Chosen)
                {
                    num += traitSpec.HPPerWorthyKill;
                }
                return num;
            }
        }

        public int ManaPerWorthyKill
        {
            get
            {
                int num = 0;
                foreach (TraitSpec traitSpec in this.Chosen)
                {
                    num += traitSpec.ManaPerWorthyKill;
                }
                return num;
            }
        }

        public int BackpackSlotsOffset
        {
            get
            {
                int num = 0;
                foreach (TraitSpec traitSpec in this.Chosen)
                {
                    num += traitSpec.BackpackSlotsOffset;
                }
                return num;
            }
        }

        public float MaxHPFactor
        {
            get
            {
                float num = 1f;
                foreach (TraitSpec traitSpec in this.Chosen)
                {
                    num *= traitSpec.MaxHPFactor;
                }
                return num;
            }
        }

        public float DamageFactorAgainstBosses
        {
            get
            {
                float num = 1f;
                foreach (TraitSpec traitSpec in this.Chosen)
                {
                    num *= traitSpec.DamageFactorAgainstBosses;
                }
                return num;
            }
        }

        public float NativeHealingRate
        {
            get
            {
                float num = 1f;
                foreach (TraitSpec traitSpec in this.Chosen)
                {
                    num *= traitSpec.NativeHealingRate;
                }
                return num;
            }
        }

        public bool ImmuneToPushing
        {
            get
            {
                using (HashSet<TraitSpec>.Enumerator enumerator = this.Chosen.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        if (enumerator.Current.ImmuneToPushing)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }

        public float HungerRateFactor
        {
            get
            {
                float num = 1f;
                foreach (TraitSpec traitSpec in this.Chosen)
                {
                    num *= traitSpec.HungerRateFactor;
                }
                return num;
            }
        }

        public float WanderersAndShopkeepersPricesFactor
        {
            get
            {
                float num = 1f;
                foreach (TraitSpec traitSpec in this.Chosen)
                {
                    num *= traitSpec.WanderersAndShopkeepersPricesFactor;
                }
                return num;
            }
        }

        public bool CanFly
        {
            get
            {
                using (HashSet<TraitSpec>.Enumerator enumerator = this.Chosen.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        if (enumerator.Current.CanFly)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }

        public float GainedExpFactor
        {
            get
            {
                float num = 1f;
                foreach (TraitSpec traitSpec in this.Chosen)
                {
                    num *= traitSpec.GainedExpFactor;
                }
                return num;
            }
        }

        public bool IsUnlocked(TraitSpec traitSpec)
        {
            return traitSpec != null && this.unlocked.Contains(traitSpec);
        }

        public bool IsChosen(TraitSpec traitSpec)
        {
            return traitSpec != null && this.Chosen.Contains(traitSpec);
        }

        public void Unlock(TraitSpec traitSpec, int insertAt = -1)
        {
            if (traitSpec == null)
            {
                Log.Error("Tried to unlock null trait.", false);
                return;
            }
            if (this.IsUnlocked(traitSpec))
            {
                Log.Error("Tried to unlock the same trait twice.", false);
                return;
            }
            if (insertAt >= 0)
            {
                if (insertAt > this.unlocked.Count)
                {
                    Log.Error(string.Concat(new string[]
                    {
                        "Tried to insert unlocked trait at index ",
                        insertAt.ToString(),
                        " but unlocked count is only ",
                        this.unlocked.Count.ToString(),
                        "."
                    }), false);
                    return;
                }
                this.unlocked.Insert(insertAt, traitSpec);
            }
            else
            {
                this.unlocked.Add(traitSpec);
            }
            this.timeUnlocked[traitSpec] = Clock.UnscaledTime;
            if (this.unlocked.Count >= Get.Specs.GetAll<TraitSpec>().Count && !Get.Achievement_UnlockAllTraits.IsCompleted)
            {
                Get.Achievement_UnlockAllTraits.TryComplete();
            }
        }

        public int RemoveFromUnlocked(TraitSpec traitSpec)
        {
            if (traitSpec == null)
            {
                Log.Error("Tried to remove null trait.", false);
                return -1;
            }
            if (!this.IsUnlocked(traitSpec))
            {
                Log.Error("Tried to remove trait which isn't unlocked.", false);
                return -1;
            }
            int num = this.unlocked.IndexOf(traitSpec);
            this.unlocked.RemoveAt(num);
            this.chosen.Remove(traitSpec);
            this.timeUnlocked.Remove(traitSpec);
            return num;
        }

        public float GetTimeUnlocked(TraitSpec traitSpec)
        {
            if (traitSpec == null)
            {
                return -99999f;
            }
            return this.timeUnlocked.GetOrDefault(traitSpec, -99999f);
        }

        public void Choose(TraitSpec traitSpec)
        {
            if (traitSpec == null)
            {
                Log.Error("Tried to choose null trait.", false);
                return;
            }
            if (!this.IsUnlocked(traitSpec))
            {
                Log.Error("Tried to choose trait which isn't unlocked.", false);
                return;
            }
            if (this.IsChosen(traitSpec))
            {
                Log.Error("Tried to choose the same trait twice.", false);
                return;
            }
            this.chosen.Add(traitSpec);
        }

        public void Unchoose(TraitSpec traitSpec)
        {
            if (traitSpec == null)
            {
                Log.Error("Tried to unchoose null trait.", false);
                return;
            }
            if (!this.IsChosen(traitSpec))
            {
                Log.Error("Tried to unchoose trait which isn't chosen.", false);
                return;
            }
            this.chosen.Remove(traitSpec);
        }

        public void ApplyRunStats(KillCounter killCounter)
        {
            int killCount = killCounter.GetKillCount(Get.Entity_Demon);
            if (killCount >= 1)
            {
                foreach (TraitSpec traitSpec in this.Chosen)
                {
                    this.timesKilledDemonWithTrait.SetOrIncrement(traitSpec, killCount);
                }
            }
            foreach (TraitSpec traitSpec2 in this.Chosen)
            {
                int num;
                if (!this.maxFloorReachedWithTrait.TryGetValue(traitSpec2, out num) || Get.Floor > num)
                {
                    this.maxFloorReachedWithTrait[traitSpec2] = Get.Floor;
                }
            }
        }

        public int GetDemonsKilledWithTrait(TraitSpec traitSpec)
        {
            return this.timesKilledDemonWithTrait.GetOrDefault(traitSpec, 0);
        }

        public int GetMaxFloorReachedWithTrait(TraitSpec traitSpec)
        {
            return this.maxFloorReachedWithTrait.GetOrDefault(traitSpec, 0);
        }

        [Saved(Default.New, true)]
        private List<TraitSpec> unlocked = new List<TraitSpec>();

        [Saved(Default.New, true)]
        private HashSet<TraitSpec> chosen = new HashSet<TraitSpec>();

        [Saved(Default.New, false)]
        private Dictionary<TraitSpec, int> timesKilledDemonWithTrait = new Dictionary<TraitSpec, int>();

        [Saved(Default.New, false)]
        private Dictionary<TraitSpec, int> maxFloorReachedWithTrait = new Dictionary<TraitSpec, int>();

        private Dictionary<TraitSpec, float> timeUnlocked = new Dictionary<TraitSpec, float>();
    }
}