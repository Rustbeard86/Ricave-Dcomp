using System;

namespace Ricave.Core
{
    public class Instruction_Retractable_ChangeTurnsPassed : Instruction
    {
        public RetractableComp Retractable
        {
            get
            {
                return this.retractable;
            }
        }

        public int Offset
        {
            get
            {
                return this.offset;
            }
        }

        protected Instruction_Retractable_ChangeTurnsPassed()
        {
        }

        public Instruction_Retractable_ChangeTurnsPassed(RetractableComp retractable, int offset)
        {
            this.retractable = retractable;
            this.offset = offset;
        }

        protected override void DoImpl()
        {
            this.retractable.TurnsPassed += this.offset;
        }

        protected override void UndoImpl()
        {
            this.retractable.TurnsPassed -= this.offset;
        }

        [Saved]
        private RetractableComp retractable;

        [Saved]
        private int offset;
    }
}