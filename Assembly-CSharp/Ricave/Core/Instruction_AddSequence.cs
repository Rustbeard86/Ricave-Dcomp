using System;

namespace Ricave.Core
{
    public class Instruction_AddSequence : Instruction
    {
        public ISequenceable Sequenceable
        {
            get
            {
                return this.sequenceable;
            }
        }

        public int SequenceOffset
        {
            get
            {
                return this.sequenceOffset;
            }
        }

        public override bool CausesRewindPoint
        {
            get
            {
                Actor actor = this.sequenceable as Actor;
                return actor != null && actor.IsNowControlledActor;
            }
        }

        protected Instruction_AddSequence()
        {
        }

        public Instruction_AddSequence(ISequenceable sequenceable, int sequenceOffset)
        {
            this.sequenceable = sequenceable;
            this.sequenceOffset = sequenceOffset;
        }

        protected override void DoImpl()
        {
            this.sequenceable.Sequence += this.sequenceOffset;
            Get.TurnManager.OnSequenceChanged();
            Get.TimeDrawer.OnSequenceAdded(this.sequenceable, this.sequenceOffset);
        }

        protected override void UndoImpl()
        {
            this.sequenceable.Sequence -= this.sequenceOffset;
            Get.TurnManager.OnSequenceChanged();
            Get.TimeDrawer.OnSequenceAdded(this.sequenceable, -this.sequenceOffset);
        }

        [Saved]
        private ISequenceable sequenceable;

        [Saved]
        private int sequenceOffset;
    }
}