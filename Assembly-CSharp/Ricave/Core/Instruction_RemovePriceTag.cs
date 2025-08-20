using System;

namespace Ricave.Core
{
    public class Instruction_RemovePriceTag : Instruction
    {
        public Item Item
        {
            get
            {
                return this.item;
            }
        }

        protected Instruction_RemovePriceTag()
        {
        }

        public Instruction_RemovePriceTag(Item item)
        {
            this.item = item;
        }

        protected override void DoImpl()
        {
            this.prevPriceTag = this.item.PriceTag;
            this.item.PriceTag = null;
        }

        protected override void UndoImpl()
        {
            this.item.PriceTag = this.prevPriceTag;
        }

        [Saved]
        private Item item;

        [Saved]
        private PriceTag prevPriceTag;
    }
}