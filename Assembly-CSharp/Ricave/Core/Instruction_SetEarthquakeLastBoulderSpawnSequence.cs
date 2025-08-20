using System;

namespace Ricave.Core
{
    public class Instruction_SetEarthquakeLastBoulderSpawnSequence : Instruction
    {
        public WorldSituation_Earthquake Earthquake
        {
            get
            {
                return this.earthquake;
            }
        }

        public int Sequence
        {
            get
            {
                return this.sequence;
            }
        }

        protected Instruction_SetEarthquakeLastBoulderSpawnSequence()
        {
        }

        public Instruction_SetEarthquakeLastBoulderSpawnSequence(WorldSituation_Earthquake earthquake, int sequence)
        {
            this.earthquake = earthquake;
            this.sequence = sequence;
        }

        protected override void DoImpl()
        {
            this.prevSequence = this.earthquake.LastBoulderSpawnSequence;
            this.earthquake.LastBoulderSpawnSequence = new int?(this.sequence);
        }

        protected override void UndoImpl()
        {
            this.earthquake.LastBoulderSpawnSequence = this.prevSequence;
        }

        [Saved]
        private WorldSituation_Earthquake earthquake;

        [Saved]
        private int sequence;

        [Saved]
        private int? prevSequence;
    }
}