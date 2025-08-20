using System;

namespace Ricave.Core
{
    public class Instruction_Immediate : Instruction
    {
        protected Instruction_Immediate()
        {
        }

        public Instruction_Immediate(Action action)
        {
            this.action = action;
        }

        protected override void DoImpl()
        {
            Action action = this.action;
            if (action != null)
            {
                action();
            }
            this.action = null;
        }

        protected override void UndoImpl()
        {
        }

        private Action action;
    }
}