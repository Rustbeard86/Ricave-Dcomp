using System;

namespace Ricave.Core
{
    public class Instruction_ChangeTurnsLeftToIdentify : Instruction
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

        protected Instruction_ChangeTurnsLeftToIdentify()
        {
        }

        public Instruction_ChangeTurnsLeftToIdentify(Item item, int offset)
        {
            this.item = item;
            this.offset = offset;
        }

        protected override void DoImpl()
        {
            this.item.TurnsLeftToIdentify += this.offset;
        }

        protected override void UndoImpl()
        {
            this.item.TurnsLeftToIdentify -= this.offset;
        }

        [Saved]
        private Item item;

        [Saved]
        private int offset;
    }
}