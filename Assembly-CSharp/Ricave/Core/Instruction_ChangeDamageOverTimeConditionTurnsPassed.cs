using System;

namespace Ricave.Core
{
    public class Instruction_ChangeDamageOverTimeConditionTurnsPassed : Instruction
    {
        public Condition_DamageOverTimeBase Condition
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

        protected Instruction_ChangeDamageOverTimeConditionTurnsPassed()
        {
        }

        public Instruction_ChangeDamageOverTimeConditionTurnsPassed(Condition_DamageOverTimeBase condition, int offset)
        {
            this.condition = condition;
            this.offset = offset;
        }

        protected override void DoImpl()
        {
            this.condition.TurnsPassed += this.offset;
        }

        protected override void UndoImpl()
        {
            this.condition.TurnsPassed -= this.offset;
        }

        [Saved]
        private Condition_DamageOverTimeBase condition;

        [Saved]
        private int offset;
    }
}