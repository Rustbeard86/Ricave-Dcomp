using System;

namespace Ricave.Core
{
    public class Instruction_AddSequenceable : Instruction
    {
        public ISequenceable Sequenceable
        {
            get
            {
                return this.sequenceable;
            }
        }

        public int InitialSequenceOffset
        {
            get
            {
                return this.initialSequenceOffset;
            }
        }

        public int InsertAt
        {
            get
            {
                return this.insertAt;
            }
        }

        protected Instruction_AddSequenceable()
        {
        }

        public Instruction_AddSequenceable(ISequenceable sequenceable, int initialSequenceOffset, int insertAt)
        {
            this.sequenceable = sequenceable;
            this.initialSequenceOffset = initialSequenceOffset;
            this.insertAt = insertAt;
        }

        protected override void DoImpl()
        {
            Get.TurnManager.AddSequenceable(this.sequenceable, this.initialSequenceOffset, this.insertAt);
        }

        protected override void UndoImpl()
        {
            Get.TurnManager.RemoveSequenceable(this.sequenceable);
        }

        [Saved]
        private ISequenceable sequenceable;

        [Saved]
        private int initialSequenceOffset;

        [Saved(-1, false)]
        private int insertAt = -1;
    }
}