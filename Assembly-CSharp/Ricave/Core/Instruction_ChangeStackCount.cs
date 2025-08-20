using System;

namespace Ricave.Core
{
    public class Instruction_ChangeStackCount : Instruction
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

        protected Instruction_ChangeStackCount()
        {
        }

        public Instruction_ChangeStackCount(Item item, int offset)
        {
            this.item = item;
            this.offset = offset;
        }

        protected override void DoImpl()
        {
            this.item.StackCount += this.offset;
        }

        protected override void UndoImpl()
        {
            this.item.StackCount -= this.offset;
        }

        [Saved]
        private Item item;

        [Saved]
        private int offset;
    }
}