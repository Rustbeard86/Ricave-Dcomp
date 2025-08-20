using System;

namespace Ricave.Core
{
    public class Instruction_FireSpreader_ChangeTurnsPassed : Instruction
    {
        public FireSpreaderComp FireSpreader
        {
            get
            {
                return this.fireSpreader;
            }
        }

        public int Offset
        {
            get
            {
                return this.offset;
            }
        }

        protected Instruction_FireSpreader_ChangeTurnsPassed()
        {
        }

        public Instruction_FireSpreader_ChangeTurnsPassed(FireSpreaderComp fireSpreader, int offset)
        {
            this.fireSpreader = fireSpreader;
            this.offset = offset;
        }

        protected override void DoImpl()
        {
            this.fireSpreader.TurnsPassed += this.offset;
        }

        protected override void UndoImpl()
        {
            this.fireSpreader.TurnsPassed -= this.offset;
        }

        [Saved]
        private FireSpreaderComp fireSpreader;

        [Saved]
        private int offset;
    }
}