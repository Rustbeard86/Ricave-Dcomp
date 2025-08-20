using System;

namespace Ricave.Core
{
    public class Instruction_TradeSequenceablePlaces : Instruction
    {
        public ISequenceable SequenceableA
        {
            get
            {
                return this.sequenceableA;
            }
        }

        public ISequenceable SequenceableB
        {
            get
            {
                return this.sequenceableB;
            }
        }

        protected Instruction_TradeSequenceablePlaces()
        {
        }

        public Instruction_TradeSequenceablePlaces(ISequenceable sequenceableA, ISequenceable sequenceableB)
        {
            this.sequenceableA = sequenceableA;
            this.sequenceableB = sequenceableB;
        }

        protected override void DoImpl()
        {
            Get.TurnManager.TradePlaces(this.sequenceableA, this.sequenceableB);
        }

        protected override void UndoImpl()
        {
            Get.TurnManager.TradePlaces(this.sequenceableA, this.sequenceableB);
        }

        [Saved]
        private ISequenceable sequenceableA;

        [Saved]
        private ISequenceable sequenceableB;
    }
}