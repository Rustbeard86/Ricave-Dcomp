using System;

namespace Ricave.Core
{
    public class Instruction_RemoveUseEffect : Instruction
    {
        public UseEffect UseEffect
        {
            get
            {
                return this.useEffect;
            }
        }

        protected Instruction_RemoveUseEffect()
        {
        }

        public Instruction_RemoveUseEffect(UseEffect useEffect)
        {
            this.useEffect = useEffect;
        }

        protected override void DoImpl()
        {
            this.removedFromParent = this.useEffect.Parent;
            this.removedFromIndex = this.useEffect.Parent.RemoveUseEffect(this.useEffect);
        }

        protected override void UndoImpl()
        {
            this.removedFromParent.AddUseEffect(this.useEffect, this.removedFromIndex);
        }

        [Saved]
        private UseEffect useEffect;

        [Saved]
        private UseEffects removedFromParent;

        [Saved]
        private int removedFromIndex;
    }
}