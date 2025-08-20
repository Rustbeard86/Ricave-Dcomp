using System;

namespace Ricave.Core
{
    public class Instruction_RemoveCondition : Instruction
    {
        public Condition Condition
        {
            get
            {
                return this.condition;
            }
        }

        protected Instruction_RemoveCondition()
        {
        }

        public Instruction_RemoveCondition(Condition condition)
        {
            this.condition = condition;
        }

        protected override void DoImpl()
        {
            this.removedFromParent = this.condition.Parent;
            this.removedFromIndex = this.condition.Parent.RemoveCondition(this.condition);
        }

        protected override void UndoImpl()
        {
            this.removedFromParent.AddCondition(this.condition, this.removedFromIndex);
        }

        [Saved]
        private Condition condition;

        [Saved]
        private Conditions removedFromParent;

        [Saved]
        private int removedFromIndex;
    }
}