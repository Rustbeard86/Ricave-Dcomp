using System;

namespace Ricave.Core
{
    public class WorldInfo : ISaveableEventsReceiver
    {
        public WorldConfig Config
        {
            get
            {
                return this.config;
            }
        }

        public bool EscapingMode
        {
            get
            {
                return this.escapingMode;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.escapingMode = value;
            }
        }

        public int? LastPenaltyRatSpawnSequence
        {
            get
            {
                return this.lastPenaltyRatSpawnSequence;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.lastPenaltyRatSpawnSequence = value;
            }
        }

        public WorldInfo()
        {
        }

        public WorldInfo(WorldConfig config)
        {
            this.config = config;
        }

        public void OnSaved()
        {
        }

        public void OnLoaded()
        {
            if (this.config == null)
            {
                Log.Error("World config was null after loading.", false);
                this.config = new WorldConfig(Get.World_Standard, 0, null, null, null, false);
            }
        }

        [Saved]
        private WorldConfig config;

        [Saved]
        private bool escapingMode;

        [Saved]
        private int? lastPenaltyRatSpawnSequence;
    }
}