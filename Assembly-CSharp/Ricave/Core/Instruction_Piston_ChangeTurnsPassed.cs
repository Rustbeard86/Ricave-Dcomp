using System;

namespace Ricave.Core
{
    public class Instruction_Piston_ChangeTurnsPassed : Instruction
    {
        public PistonComp Piston
        {
            get
            {
                return this.piston;
            }
        }

        public int Offset
        {
            get
            {
                return this.offset;
            }
        }

        protected Instruction_Piston_ChangeTurnsPassed()
        {
        }

        public Instruction_Piston_ChangeTurnsPassed(PistonComp piston, int offset)
        {
            this.piston = piston;
            this.offset = offset;
        }

        protected override void DoImpl()
        {
            this.piston.TurnsPassed += this.offset;
        }

        protected override void UndoImpl()
        {
            this.piston.TurnsPassed -= this.offset;
        }

        [Saved]
        private PistonComp piston;

        [Saved]
        private int offset;
    }
}