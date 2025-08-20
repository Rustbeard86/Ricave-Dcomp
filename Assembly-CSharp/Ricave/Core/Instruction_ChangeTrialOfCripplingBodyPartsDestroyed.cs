using System;

namespace Ricave.Core
{
    public class Instruction_ChangeTrialOfCripplingBodyPartsDestroyed : Instruction
    {
        public Condition_TrialOfCrippling Condition
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

        protected Instruction_ChangeTrialOfCripplingBodyPartsDestroyed()
        {
        }

        public Instruction_ChangeTrialOfCripplingBodyPartsDestroyed(Condition_TrialOfCrippling condition, int offset)
        {
            this.condition = condition;
            this.offset = offset;
        }

        protected override void DoImpl()
        {
            this.condition.BodyPartsDestroyed += this.offset;
        }

        protected override void UndoImpl()
        {
            this.condition.BodyPartsDestroyed -= this.offset;
        }

        [Saved]
        private Condition_TrialOfCrippling condition;

        [Saved]
        private int offset;
    }
}