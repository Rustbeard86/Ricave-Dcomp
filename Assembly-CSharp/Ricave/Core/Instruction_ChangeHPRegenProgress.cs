using System;

namespace Ricave.Core
{
    public class Instruction_ChangeHPRegenProgress : Instruction
    {
        public Condition_HPRegen Regen
        {
            get
            {
                return this.regen;
            }
        }

        public int Offset
        {
            get
            {
                return this.offset;
            }
        }

        protected Instruction_ChangeHPRegenProgress()
        {
        }

        public Instruction_ChangeHPRegenProgress(Condition_HPRegen regen, int offset)
        {
            this.regen = regen;
            this.offset = offset;
        }

        protected override void DoImpl()
        {
            this.regen.Progress += this.offset;
        }

        protected override void UndoImpl()
        {
            this.regen.Progress -= this.offset;
        }

        [Saved]
        private Condition_HPRegen regen;

        [Saved]
        private int offset;
    }
}