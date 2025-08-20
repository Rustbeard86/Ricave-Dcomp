using System;

namespace Ricave.Core
{
    public class Instruction_Piston_SetRetracted : Instruction
    {
        public PistonComp Piston
        {
            get
            {
                return this.piston;
            }
        }

        public bool Retracted
        {
            get
            {
                return this.retracted;
            }
        }

        protected Instruction_Piston_SetRetracted()
        {
        }

        public Instruction_Piston_SetRetracted(PistonComp piston, bool retracted)
        {
            this.piston = piston;
            this.retracted = retracted;
        }

        protected override void DoImpl()
        {
            this.prevRetracted = this.piston.Retracted;
            this.piston.Retracted = this.retracted;
        }

        protected override void UndoImpl()
        {
            this.piston.Retracted = this.prevRetracted;
        }

        [Saved]
        private PistonComp piston;

        [Saved]
        private bool retracted;

        [Saved]
        private bool prevRetracted;
    }
}