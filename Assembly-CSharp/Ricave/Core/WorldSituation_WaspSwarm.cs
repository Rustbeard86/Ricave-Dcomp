using System;

namespace Ricave.Core
{
    public class WorldSituation_WaspSwarm : WorldSituation_Spawner
    {
        protected override int MaxCount
        {
            get
            {
                return 2;
            }
        }

        protected override int IntervalTurns
        {
            get
            {
                return 30;
            }
        }

        protected override EntitySpec EntitySpecToSpawn
        {
            get
            {
                return Get.Entity_Wasp;
            }
        }

        protected override bool FromFloorBars
        {
            get
            {
                return true;
            }
        }

        protected WorldSituation_WaspSwarm()
        {
        }

        public WorldSituation_WaspSwarm(WorldSituationSpec spec)
            : base(spec)
        {
        }
    }
}