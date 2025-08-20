using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class WorldSituationsManager : ISaveableEventsReceiver
    {
        public List<WorldSituation> Situations
        {
            get
            {
                return this.situations;
            }
        }

        public ValueTuple<Color, float>? FogOverride
        {
            get
            {
                for (int i = 0; i < this.situations.Count; i++)
                {
                    ValueTuple<Color, float>? fogOverride = this.situations[i].FogOverride;
                    if (fogOverride != null)
                    {
                        return fogOverride;
                    }
                }
                return null;
            }
        }

        public float AmbientLightFactor
        {
            get
            {
                float num = 1f;
                for (int i = 0; i < this.situations.Count; i++)
                {
                    num *= this.situations[i].AmbientLightFactor;
                }
                return num;
            }
        }

        public ValueTuple<AudioClip, float>? Ambience
        {
            get
            {
                for (int i = 0; i < this.situations.Count; i++)
                {
                    if (this.situations[i].Spec.Ambience != null)
                    {
                        return new ValueTuple<AudioClip, float>?(new ValueTuple<AudioClip, float>(this.situations[i].Spec.Ambience, this.situations[i].Spec.AmbienceVolume));
                    }
                }
                return null;
            }
        }

        protected WorldSituationsManager()
        {
        }

        public WorldSituationsManager(World world)
        {
            this.world = world;
        }

        public bool Contains(WorldSituation situation)
        {
            return situation != null && this.situations.Contains(situation);
        }

        public bool Contains_ProbablyAt(WorldSituation situation, int probablyAt)
        {
            return situation != null && this.situations.Contains_ProbablyAt(situation, probablyAt);
        }

        public void AddWorldSituation(WorldSituation situation, int insertAt = -1)
        {
            Instruction.ThrowIfNotExecuting();
            if (situation == null)
            {
                Log.Error("Tried to add null world situation.", false);
                return;
            }
            if (this.Contains(situation))
            {
                Log.Error("Tried to add the same world situation twice.", false);
                return;
            }
            if (insertAt >= 0)
            {
                if (insertAt > this.situations.Count)
                {
                    Log.Error(string.Concat(new string[]
                    {
                        "Tried to insert world situation at index ",
                        insertAt.ToString(),
                        " but situations count is only ",
                        this.situations.Count.ToString(),
                        "."
                    }), false);
                    return;
                }
                this.situations.Insert(insertAt, situation);
            }
            else
            {
                this.situations.Add(situation);
            }
            situation.TimeAdded = Clock.UnscaledTime;
        }

        public int RemoveWorldSituation(WorldSituation situation)
        {
            Instruction.ThrowIfNotExecuting();
            if (situation == null)
            {
                Log.Error("Tried to remove null world situation.", false);
                return -1;
            }
            if (!this.Contains(situation))
            {
                Log.Error("Tried to remove world situation but it's not here.", false);
                return -1;
            }
            int num = this.situations.IndexOf(situation);
            this.situations.RemoveAt(num);
            return num;
        }

        public WorldSituation GetFirstOfSpec(WorldSituationSpec spec)
        {
            for (int i = 0; i < this.situations.Count; i++)
            {
                if (this.situations[i].Spec == spec)
                {
                    return this.situations[i];
                }
            }
            return null;
        }

        public bool AnyOfSpec(WorldSituationSpec spec)
        {
            return this.GetFirstOfSpec(spec) != null;
        }

        public void OnSaved()
        {
        }

        public void OnLoaded()
        {
            if (this.situations.RemoveAll((WorldSituation x) => x.Spec == null) != 0)
            {
                Log.Error("Removed some world situations with null spec from WorldSituationsManager.", false);
            }
        }

        [Saved]
        private World world;

        [Saved(Default.New, true)]
        private List<WorldSituation> situations = new List<WorldSituation>();
    }
}