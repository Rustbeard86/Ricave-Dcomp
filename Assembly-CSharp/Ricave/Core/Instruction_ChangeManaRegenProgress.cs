using System;

namespace Ricave.Core
{
    public class Instruction_ChangeManaRegenProgress : Instruction
    {
        public Condition_ManaRegen Regen
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

        protected Instruction_ChangeManaRegenProgress()
        {
        }

        public Instruction_ChangeManaRegenProgress(Condition_ManaRegen regen, int offset)
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
        private Condition_ManaRegen regen;

        [Saved]
        private int offset;
    }
}