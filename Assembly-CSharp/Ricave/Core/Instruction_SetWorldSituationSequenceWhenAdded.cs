using System;

namespace Ricave.Core
{
    public class Instruction_SetWorldSituationSequenceWhenAdded : Instruction
    {
        public WorldSituation WorldSituation
        {
            get
            {
                return this.worldSituation;
            }
        }

        public int Sequence
        {
            get
            {
                return this.sequence;
            }
        }

        protected Instruction_SetWorldSituationSequenceWhenAdded()
        {
        }

        public Instruction_SetWorldSituationSequenceWhenAdded(WorldSituation worldSituation, int sequence)
        {
            this.worldSituation = worldSituation;
            this.sequence = sequence;
        }

        protected override void DoImpl()
        {
            this.prevSequence = this.worldSituation.SequenceWhenAdded;
            this.worldSituation.SequenceWhenAdded = new int?(this.sequence);
        }

        protected override void UndoImpl()
        {
            this.worldSituation.SequenceWhenAdded = this.prevSequence;
        }

        [Saved]
        private WorldSituation worldSituation;

        [Saved]
        private int sequence;

        [Saved]
        private int? prevSequence;
    }
}