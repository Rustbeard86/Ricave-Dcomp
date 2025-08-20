using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ricave.Core
{
    public class FactionManager : ISaveableEventsReceiver
    {
        public List<Faction> Factions
        {
            get
            {
                return this.factions;
            }
        }

        public bool Contains(Faction faction)
        {
            return faction != null && this.factions.Contains(faction);
        }

        public bool Contains_ProbablyAt(Faction faction, int probablyAt)
        {
            return faction != null && this.factions.Contains_ProbablyAt(faction, probablyAt);
        }

        public void AddFaction(Faction faction, int insertAt = -1)
        {
            Instruction.ThrowIfNotExecuting();
            if (faction == null)
            {
                Log.Error("Tried to add null faction.", false);
                return;
            }
            if (this.Contains(faction))
            {
                Log.Error("Tried to add the same faction twice.", false);
                return;
            }
            if (insertAt >= 0)
            {
                if (insertAt > this.factions.Count)
                {
                    Log.Error(string.Concat(new string[]
                    {
                        "Tried to insert faction at index ",
                        insertAt.ToString(),
                        " but factions count is only ",
                        this.factions.Count.ToString(),
                        "."
                    }), false);
                    return;
                }
                this.factions.Insert(insertAt, faction);
            }
            else
            {
                this.factions.Add(faction);
            }
            this.RecacheFactions();
        }

        public int RemoveFaction(Faction faction)
        {
            Instruction.ThrowIfNotExecuting();
            if (faction == null)
            {
                Log.Error("Tried to remove null faction.", false);
                return -1;
            }
            if (!this.Contains(faction))
            {
                Log.Error("Tried to remove faction but it's not here.", false);
                return -1;
            }
            int num = this.factions.IndexOf(faction);
            this.factions.RemoveAt(num);
            this.RecacheFactions();
            return num;
        }

        private void RecacheFactions()
        {
        }

        public int GetFactionCountOfSpec(FactionSpec spec)
        {
            int num = 0;
            for (int i = 0; i < this.factions.Count; i++)
            {
                if (this.factions[i].Spec == spec)
                {
                    num++;
                }
            }
            return num;
        }

        public Faction GetFirstOfSpec(FactionSpec spec)
        {
            for (int i = 0; i < this.factions.Count; i++)
            {
                if (this.factions[i].Spec == spec)
                {
                    return this.factions[i];
                }
            }
            return null;
        }

        public bool AnyOfSpec(FactionSpec spec)
        {
            return this.GetFirstOfSpec(spec) != null;
        }

        public bool HostilityExists(Faction faction1, Faction faction2)
        {
            if (faction1 == null || faction2 == null)
            {
                return false;
            }
            if (faction1 == faction2)
            {
                return false;
            }
            int i = 0;
            int count = this.hostilities.Count;
            while (i < count)
            {
                FactionHostility factionHostility = this.hostilities[i];
                if (factionHostility.Faction1 == faction1 && factionHostility.Faction2 == faction2)
                {
                    return true;
                }
                if (factionHostility.Faction1 == faction2 && factionHostility.Faction2 == faction1)
                {
                    return true;
                }
                i++;
            }
            return false;
        }

        public void AddHostility(Faction faction1, Faction faction2, int insertAt = -1)
        {
            Instruction.ThrowIfNotExecuting();
            if (faction1 == null || faction2 == null)
            {
                Log.Error("Tried to add hostility between null faction.", false);
                return;
            }
            if (faction1 == faction2)
            {
                Log.Error("Tried to add hostility with the same faction.", false);
                return;
            }
            if (this.HostilityExists(faction1, faction2))
            {
                Log.Error("Tried to add the same hostility twice.", false);
                return;
            }
            if (insertAt >= 0)
            {
                if (insertAt > this.hostilities.Count)
                {
                    Log.Error(string.Concat(new string[]
                    {
                        "Tried to insert faction hostility at index ",
                        insertAt.ToString(),
                        " but list count is only ",
                        this.hostilities.Count.ToString(),
                        "."
                    }), false);
                    return;
                }
                this.hostilities.Insert(insertAt, new FactionHostility(faction1, faction2));
            }
            else
            {
                this.hostilities.Add(new FactionHostility(faction1, faction2));
            }
            Get.Minimap.OnFactionHostilityChanged();
        }

        public int RemoveHostility(Faction faction1, Faction faction2)
        {
            Instruction.ThrowIfNotExecuting();
            if (faction1 == null || faction2 == null)
            {
                Log.Error("Tried to remove hostility between null faction.", false);
                return -1;
            }
            if (faction1 == faction2)
            {
                Log.Error("Tried to remove hostility with the same faction. It can't exist.", false);
                return -1;
            }
            if (!this.HostilityExists(faction1, faction2))
            {
                Log.Error("Tried to remove hostility but it's not here.", false);
                return -1;
            }
            for (int i = 0; i < this.hostilities.Count; i++)
            {
                if ((this.hostilities[i].Faction1 == faction1 && this.hostilities[i].Faction2 == faction2) || (this.hostilities[i].Faction1 == faction2 && this.hostilities[i].Faction2 == faction1))
                {
                    this.hostilities.RemoveAt(i);
                    Get.Minimap.OnFactionHostilityChanged();
                    return i;
                }
            }
            return -1;
        }

        public void GenerateInitialFactions()
        {
            foreach (FactionSpec factionSpec in from x in Get.Specs.GetAll<FactionSpec>()
                                                where x.GenerateOneAtStart
                                                orderby x.GenerateOrder
                                                select x)
            {
                foreach (Instruction instruction in InstructionSets_Misc.AddFaction(new Faction(factionSpec, FactionUtility.GenerateFactionName(factionSpec))))
                {
                    instruction.Do();
                }
            }
        }

        public void ReserveFactionColors()
        {
            foreach (Faction faction in this.factions)
            {
                Color color = faction.Color;
            }
        }

        public void OnSaved()
        {
        }

        public void OnLoaded()
        {
            if (this.factions.RemoveAll((Faction x) => x.Spec == null) != 0)
            {
                Log.Error("Removed some factions with null spec from FactionManager.", false);
            }
            if (this.hostilities.RemoveAll((FactionHostility x) => x.Faction1 == null || x.Faction2 == null) != 0)
            {
                Log.Error("Removed some faction hostilities with null factions from FactionManager.", false);
            }
            this.RecacheFactions();
        }

        [Saved(Default.New, true)]
        private List<Faction> factions = new List<Faction>();

        [Saved(Default.New, true)]
        private List<FactionHostility> hostilities = new List<FactionHostility>();
    }
}