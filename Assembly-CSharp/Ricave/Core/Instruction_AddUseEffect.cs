using System;

namespace Ricave.Core
{
    public class Instruction_AddUseEffect : Instruction
    {
        public UseEffect UseEffect
        {
            get
            {
                return this.useEffect;
            }
        }

        public UseEffects To
        {
            get
            {
                return this.to;
            }
        }

        protected Instruction_AddUseEffect()
        {
        }

        public Instruction_AddUseEffect(UseEffect useEffect, UseEffects to)
        {
            this.useEffect = useEffect;
            this.to = to;
        }

        protected override void DoImpl()
        {
            this.to.AddUseEffect(this.useEffect, -1);
        }

        protected override void UndoImpl()
        {
            this.to.RemoveUseEffect(this.useEffect);
        }

        [Saved]
        private UseEffect useEffect;

        [Saved]
        private UseEffects to;
    }
}