using System;

namespace Ricave.Core
{
    public class Instruction_UseCycle_ChangeTurnsPassed : Instruction
    {
        public UseCycleComp UseCycle
        {
            get
            {
                return this.useCycle;
            }
        }

        public int Offset
        {
            get
            {
                return this.offset;
            }
        }

        protected Instruction_UseCycle_ChangeTurnsPassed()
        {
        }

        public Instruction_UseCycle_ChangeTurnsPassed(UseCycleComp useCycle, int offset)
        {
            this.useCycle = useCycle;
            this.offset = offset;
        }

        protected override void DoImpl()
        {
            this.useCycle.TurnsPassed += this.offset;
        }

        protected override void UndoImpl()
        {
            this.useCycle.TurnsPassed -= this.offset;
        }

        [Saved]
        private UseCycleComp useCycle;

        [Saved]
        private int offset;
    }
}