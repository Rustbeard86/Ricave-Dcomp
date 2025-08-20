using System;
using System.Collections.Generic;
using System.Linq;

namespace Ricave.Core
{
    public class TotalKillCounter
    {
        public List<TotalKillCounter.KilledBoss> KilledBosses
        {
            get
            {
                return this.killedBosses;
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

        public int BossKillCount
        {
            get
            {
                return this.killedBosses.Count;
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

        public int GetBossKillCount(EntitySpec actorSpec)
        {
            if (actorSpec == null)
            {
                return 0;
            }
            int num = 0;
            for (int i = 0; i < this.killedBosses.Count; i++)
            {
                if (this.killedBosses[i].ActorSpec == actorSpec)
                {
                    num++;
                }
            }
            return num;
        }

        public void Apply(KillCounter killCounter)
        {
            foreach (KeyValuePair<EntitySpec, int> keyValuePair in killCounter.KillCounts)
            {
                this.ChangeKillCount(keyValuePair.Key, keyValuePair.Value);
            }
            if (killCounter.KilledBosses.Any())
            {
                (from x in killCounter.KilledBosses
                 orderby x.KilledOnFloor descending, x.KillTime descending
                 select x).First<TotalKillCounter.KilledBoss>().CanBeTrophy = true;
                this.killedBosses.AddRange(killCounter.KilledBosses);
            }
            if (Get.Floor >= 2)
            {
                foreach (TotalKillCounter.KilledBoss killedBoss in this.killedBosses)
                {
                    killedBoss.TrophyHasStardust = true;
                }
            }
        }

        private void ChangeKillCount(EntitySpec actorSpec, int offset)
        {
            if (actorSpec == null)
            {
                Log.Error("Tried to change total kill count for null actor spec.", false);
                return;
            }
            if (!actorSpec.IsActor)
            {
                Log.Error("Tried to change total kill count for entity spec which is not an actor spec.", false);
                return;
            }
            this.killCounts.SetOrIncrement(actorSpec, offset);
        }

        [Saved(Default.New, false)]
        private Dictionary<EntitySpec, int> killCounts = new Dictionary<EntitySpec, int>();

        [Saved(Default.New, true)]
        private List<TotalKillCounter.KilledBoss> killedBosses = new List<TotalKillCounter.KilledBoss>();

        public class KilledBoss
        {
            public EntitySpec ActorSpec
            {
                get
                {
                    return this.actorSpec;
                }
            }

            public string Name
            {
                get
                {
                    return this.name;
                }
            }

            public int KilledOnFloor
            {
                get
                {
                    return this.killedOnFloor;
                }
            }

            public int RampUp
            {
                get
                {
                    return this.rampUp;
                }
            }

            public DateTime KillTime
            {
                get
                {
                    return this.killTime;
                }
            }

            public bool CanBeTrophy
            {
                get
                {
                    return this.canBeTrophy;
                }
                set
                {
                    this.canBeTrophy = value;
                }
            }

            public bool TrophyHasStardust
            {
                get
                {
                    return this.trophyHasStardust;
                }
                set
                {
                    this.trophyHasStardust = value;
                }
            }

            public string KillTimeString
            {
                get
                {
                    if (this.killTimeStringCached == null)
                    {
                        this.killTimeStringCached = this.killTime.ToString("m", Get.ActiveLanguage.CultureInfo);
                    }
                    return this.killTimeStringCached;
                }
            }

            protected KilledBoss()
            {
            }

            public KilledBoss(EntitySpec actorSpec, string name, int killedOnFloor, int rampUp, DateTime killTime)
            {
                this.actorSpec = actorSpec;
                this.name = name;
                this.killedOnFloor = killedOnFloor;
                this.rampUp = rampUp;
                this.killTime = killTime;
            }

            [Saved]
            private EntitySpec actorSpec;

            [Saved]
            private string name;

            [Saved]
            private int killedOnFloor;

            [Saved]
            private int rampUp;

            [Saved]
            private DateTime killTime;

            [Saved]
            private bool canBeTrophy;

            [Saved(true, false)]
            private bool trophyHasStardust = true;

            private string killTimeStringCached;
        }
    }
}