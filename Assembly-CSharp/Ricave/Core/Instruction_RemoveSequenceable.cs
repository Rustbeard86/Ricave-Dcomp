using System;

namespace Ricave.Core
{
    public class Instruction_RemoveSequenceable : Instruction
    {
        public ISequenceable Sequenceable
        {
            get
            {
                return this.sequenceable;
            }
        }

        protected Instruction_RemoveSequenceable()
        {
        }

        public Instruction_RemoveSequenceable(ISequenceable sequenceable)
        {
            this.sequenceable = sequenceable;
        }

        protected override void DoImpl()
        {
            this.prevSequence = this.sequenceable.Sequence;
            this.removedFromIndex = Get.TurnManager.RemoveSequenceable(this.sequenceable);
        }

        protected override void UndoImpl()
        {
            Get.TurnManager.AddSequenceable(this.sequenceable, 0, this.removedFromIndex);
            this.sequenceable.Sequence = this.prevSequence;
        }

        [Saved]
        private ISequenceable sequenceable;

        [Saved]
        private int prevSequence;

        [Saved]
        private int removedFromIndex;
    }
}