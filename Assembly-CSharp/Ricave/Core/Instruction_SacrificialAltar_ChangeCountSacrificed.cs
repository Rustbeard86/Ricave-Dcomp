using System;

namespace Ricave.Core
{
    public class Instruction_SacrificialAltar_ChangeCountSacrificed : Instruction
    {
        public SacrificialAltarComp Comp
        {
            get
            {
                return this.comp;
            }
        }

        public int Offset
        {
            get
            {
                return this.offset;
            }
        }

        protected Instruction_SacrificialAltar_ChangeCountSacrificed()
        {
        }

        public Instruction_SacrificialAltar_ChangeCountSacrificed(SacrificialAltarComp comp, int offset)
        {
            this.comp = comp;
            this.offset = offset;
        }

        protected override void DoImpl()
        {
            this.comp.CountSacrificed += this.offset;
        }

        protected override void UndoImpl()
        {
            this.comp.CountSacrificed -= this.offset;
        }

        [Saved]
        private SacrificialAltarComp comp;

        [Saved]
        private int offset;
    }
}