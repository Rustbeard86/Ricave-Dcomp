using System;

namespace Ricave.Core
{
    public class Instruction_SetCursed : Instruction
    {
        public Item Item
        {
            get
            {
                return this.item;
            }
        }

        public bool Cursed
        {
            get
            {
                return this.cursed;
            }
        }

        protected Instruction_SetCursed()
        {
        }

        public Instruction_SetCursed(Item item, bool cursed)
        {
            this.item = item;
            this.cursed = cursed;
        }

        protected override void DoImpl()
        {
            this.prevCursed = this.item.Cursed;
            this.item.Cursed = this.cursed;
        }

        protected override void UndoImpl()
        {
            this.item.Cursed = this.prevCursed;
        }

        [Saved]
        private Item item;

        [Saved]
        private bool cursed;

        [Saved]
        private bool prevCursed;
    }
}