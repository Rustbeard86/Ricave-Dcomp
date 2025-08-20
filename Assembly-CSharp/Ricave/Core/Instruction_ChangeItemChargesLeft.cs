using System;

namespace Ricave.Core
{
    public class Instruction_ChangeItemChargesLeft : Instruction
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

        protected Instruction_ChangeItemChargesLeft()
        {
        }

        public Instruction_ChangeItemChargesLeft(Item item, int offset)
        {
            this.item = item;
            this.offset = offset;
        }

        protected override void DoImpl()
        {
            this.item.ChargesLeft += this.offset;
        }

        protected override void UndoImpl()
        {
            this.item.ChargesLeft -= this.offset;
        }

        [Saved]
        private Item item;

        [Saved]
        private int offset;
    }
}