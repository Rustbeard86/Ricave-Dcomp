using System;

namespace Ricave.Core
{
    public class Instruction_StartItemJumpAnimation : Instruction
    {
        public Item Item
        {
            get
            {
                return this.item;
            }
        }

        protected Instruction_StartItemJumpAnimation()
        {
        }

        public Instruction_StartItemJumpAnimation(Item item)
        {
            this.item = item;
        }

        protected override void DoImpl()
        {
            this.item.StartJumpAnimation();
        }

        protected override void UndoImpl()
        {
        }

        [Saved]
        private Item item;
    }
}