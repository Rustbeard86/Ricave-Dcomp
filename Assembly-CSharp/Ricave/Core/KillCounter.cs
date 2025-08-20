using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class KillCounter
    {
        public Dictionary<EntitySpec, int> KillCounts
        {
            get
            {
                return this.killCounts;
            }
        }

        public List<TotalKillCounter.KilledBoss> KilledBosses
        {
            get
            {
                return this.killedBosses;
            }
        }

        public int? LastKillSequence
        {
            get
            {
                return this.lastKillSequence;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.lastKillSequence = value;
            }
        }

        public int? LastWorthyKillSequence
        {
            get
            {
                return this.lastWorthyKillSequence;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.lastWorthyKillSequence = value;
            }
        }

        public int KillCount
        {
            get
            {
                int num = 0;
                foreach (KeyValuePair<EntitySpec, int> keyValuePair in this.killCounts)
                {
                    num += keyValuePair.Value;
                }
                return num;
            }
        }

        public List<ValueTuple<EntitySpec, int>> KillCountsOrderedByExp
        {
            get
            {
                if (this.killCountsOrderedByExpCached == null)
                {
                    this.killCountsOrderedByExpCached = new List<ValueTuple<EntitySpec, int>>();
                    foreach (KeyValuePair<EntitySpec, int> keyValuePair in this.killCounts)
                    {
                        this.killCountsOrderedByExpCached.Add(new ValueTuple<EntitySpec, int>(keyValuePair.Key, keyValuePair.Value));
                    }
                    this.killCountsOrderedByExpCached.Sort((ValueTuple<EntitySpec, int> a, ValueTuple<EntitySpec, int> b) => b.Item1.Actor.KilledExperience.CompareTo(a.Item1.Actor.KilledExperience));
                }
                return this.killCountsOrderedByExpCached;
            }
        }

        public List<TotalKillCounter.KilledBoss> KilledBossesOrderedByKillTime
        {
            get
            {
                if (this.killedBossesOrderedByKillTimeCached == null)
                {
                    this.killedBossesOrderedByKillTimeCached = new List<TotalKillCounter.KilledBoss>();
                    this.killedBossesOrderedByKillTimeCached.AddRange(this.killedBosses);
                    this.killedBossesOrderedByKillTimeCached.Sort((TotalKillCounter.KilledBoss a, TotalKillCounter.KilledBoss b) => b.KillTime.CompareTo(a.KillTime));
                }
                return this.killedBossesOrderedByKillTimeCached;
            }
        }

        public int GetKillCount(EntitySpec actorSpec)
        {
            if (actorSpec == null)
            {
                return 0;
            }
            return this.killCounts.GetOrDefault(actorSpec, 0);
        }

        public void ChangeKillCount(EntitySpec actorSpec, int offset)
        {
            Instruction.ThrowIfNotExecuting();
            if (actorSpec == null)
            {
                Log.Error("Tried to change kill count for null actor spec.", false);
                return;
            }
            if (!actorSpec.IsActor)
            {
                Log.Error("Tried to change kill count for entity spec which is not an actor spec.", false);
                return;
            }
            this.killCounts.SetOrIncrement(actorSpec, offset);
            this.killCountsOrderedByExpCached = null;
        }

        public int RegisterKilledBoss(EntitySpec actorSpec, string name, int killedOnFloor, int rampUp, DateTime killTime)
        {
            Instruction.ThrowIfNotExecuting();
            if (actorSpec == null)
            {
                Log.Error("Tried to register killed boss using null actor spec.", false);
                return -1;
            }
            if (!actorSpec.IsActor)
            {
                Log.Error("Tried to register killed boss using entity spec which is not an actor spec.", false);
                return -1;
            }
            this.killedBosses.Add(new TotalKillCounter.KilledBoss(actorSpec, name, killedOnFloor, rampUp, killTime));
            this.killedBossesOrderedByKillTimeCached = null;
            return this.killedBosses.Count - 1;
        }

        public void DeregisterKilledBoss(int index)
        {
            Instruction.ThrowIfNotExecuting();
            if (index < 0 || index >= this.killedBosses.Count)
            {
                Log.Error("Tried to deregister killed boss using index which is out of bounds.", false);
                return;
            }
            this.killedBosses.RemoveAt(index);
            this.killedBossesOrderedByKillTimeCached = null;
        }

        [Saved(Default.New, false)]
        private Dictionary<EntitySpec, int> killCounts = new Dictionary<EntitySpec, int>();

        [Saved(Default.New, true)]
        private List<TotalKillCounter.KilledBoss> killedBosses = new List<TotalKillCounter.KilledBoss>();

        [Saved]
        private int? lastKillSequence;

        [Saved]
        private int? lastWorthyKillSequence;

        private List<ValueTuple<EntitySpec, int>> killCountsOrderedByExpCached;

        private List<TotalKillCounter.KilledBoss> killedBossesOrderedByKillTimeCached;
    }
}