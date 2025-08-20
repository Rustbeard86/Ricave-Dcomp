using System;

namespace Ricave.Core
{
    public class Instruction_ChangeItemUsesLeft : Instruction
    {
        public Item Item
        {
            get
            {
                return this.item;
            }
        }

        public int Offset
        {
            get
            {
                return this.offset;
            }
        }

        protected Instruction_ChangeItemUsesLeft()
        {
        }

        public Instruction_ChangeItemUsesLeft(Item item, int offset)
        {
            this.item = item;
            this.offset = offset;
        }

        protected override void DoImpl()
        {
            this.item.UsesLeft += this.offset;
        }

        protected override void UndoImpl()
        {
            this.item.UsesLeft -= this.offset;
        }

        [Saved]
        private Item item;

        [Saved]
        private int offset;
    }
}