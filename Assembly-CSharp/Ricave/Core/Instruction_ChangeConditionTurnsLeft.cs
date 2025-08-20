using System;

namespace Ricave.Core
{
    public class Instruction_ChangeConditionTurnsLeft : Instruction
    {
        public Condition Condition
        {
            get
            {
                return this.condition;
            }
        }

        public int Offset
        {
            get
            {
                return this.offset;
            }
        }

        protected Instruction_ChangeConditionTurnsLeft()
        {
        }

        public Instruction_ChangeConditionTurnsLeft(Condition condition, int offset)
        {
            this.condition = condition;
            this.offset = offset;
        }

        protected override void DoImpl()
        {
            this.condition.TurnsLeft += this.offset;
        }

        protected override void UndoImpl()
        {
            this.condition.TurnsLeft -= this.offset;
        }

        [Saved]
        private Condition condition;

        [Saved]
        private int offset;
    }
}