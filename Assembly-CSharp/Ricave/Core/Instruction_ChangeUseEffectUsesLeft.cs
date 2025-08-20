using System;

namespace Ricave.Core
{
    public class Instruction_ChangeUseEffectUsesLeft : Instruction
    {
        public UseEffect UseEffect
        {
            get
            {
                return this.useEffect;
            }
        }

        public int Offset
        {
            get
            {
                return this.offset;
            }
        }

        protected Instruction_ChangeUseEffectUsesLeft()
        {
        }

        public Instruction_ChangeUseEffectUsesLeft(UseEffect useEffect, int offset)
        {
            this.useEffect = useEffect;
            this.offset = offset;
        }

        protected override void DoImpl()
        {
            this.useEffect.UsesLeft += this.offset;
        }

        protected override void UndoImpl()
        {
            this.useEffect.UsesLeft -= this.offset;
        }

        [Saved]
        private UseEffect useEffect;

        [Saved]
        private int offset;
    }
}