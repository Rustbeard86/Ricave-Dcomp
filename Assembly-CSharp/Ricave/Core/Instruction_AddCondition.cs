using System;

namespace Ricave.Core
{
    public class Instruction_AddCondition : Instruction
    {
        public Condition Condition
        {
            get
            {
                return this.condition;
            }
        }

        public Conditions To
        {
            get
            {
                return this.to;
            }
        }

        protected Instruction_AddCondition()
        {
        }

        public Instruction_AddCondition(Condition condition, Conditions to)
        {
            this.condition = condition;
            this.to = to;
        }

        protected override void DoImpl()
        {
            this.to.AddCondition(this.condition, -1);
        }

        protected override void UndoImpl()
        {
            this.to.RemoveCondition(this.condition);
        }

        [Saved]
        private Condition condition;

        [Saved]
        private Conditions to;
    }
}