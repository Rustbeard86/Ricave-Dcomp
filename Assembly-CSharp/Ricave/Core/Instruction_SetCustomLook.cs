using System;

namespace Ricave.Core
{
    public class Instruction_SetCustomLook : Instruction
    {
        public Item Item
        {
            get
            {
                return this.item;
            }
        }

        public ItemLookSpec Look
        {
            get
            {
                return this.look;
            }
        }

        protected Instruction_SetCustomLook()
        {
        }

        public Instruction_SetCustomLook(Item item, ItemLookSpec look)
        {
            this.item = item;
            this.look = look;
        }

        protected override void DoImpl()
        {
            this.prevLook = this.item.CustomLook;
            this.item.CustomLook = this.look;
        }

        protected override void UndoImpl()
        {
            this.item.CustomLook = this.prevLook;
        }

        [Saved]
        private Item item;

        [Saved]
        private ItemLookSpec look;

        [Saved]
        private ItemLookSpec prevLook;
    }
}