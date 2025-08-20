using System;

namespace Ricave.Core
{
    public class Instruction_SacrificialAltar_SetBossSacrificed : Instruction
    {
        public SacrificialAltarComp Comp
        {
            get
            {
                return this.comp;
            }
        }

        public bool Value
        {
            get
            {
                return this.value;
            }
        }

        protected Instruction_SacrificialAltar_SetBossSacrificed()
        {
        }

        public Instruction_SacrificialAltar_SetBossSacrificed(SacrificialAltarComp comp, bool value)
        {
            this.comp = comp;
            this.value = value;
        }

        protected override void DoImpl()
        {
            this.prevValue = this.comp.BossSacrificed;
            this.comp.BossSacrificed = this.value;
        }

        protected override void UndoImpl()
        {
            this.comp.BossSacrificed = this.prevValue;
        }

        [Saved]
        private SacrificialAltarComp comp;

        [Saved]
        private bool value;

        [Saved]
        private bool prevValue;
    }
}