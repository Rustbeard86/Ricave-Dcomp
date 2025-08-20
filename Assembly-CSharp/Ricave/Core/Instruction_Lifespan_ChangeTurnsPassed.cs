using System;

namespace Ricave.Core
{
    public class Instruction_Lifespan_ChangeTurnsPassed : Instruction
    {
        public LifespanComp Lifespan
        {
            get
            {
                return this.lifespan;
            }
        }

        public int Offset
        {
            get
            {
                return this.offset;
            }
        }

        protected Instruction_Lifespan_ChangeTurnsPassed()
        {
        }

        public Instruction_Lifespan_ChangeTurnsPassed(LifespanComp lifespan, int offset)
        {
            this.lifespan = lifespan;
            this.offset = offset;
        }

        protected override void DoImpl()
        {
            this.lifespan.TurnsPassed += this.offset;
        }

        protected override void UndoImpl()
        {
            this.lifespan.TurnsPassed -= this.offset;
        }

        [Saved]
        private LifespanComp lifespan;

        [Saved]
        private int offset;
    }
}