using System;

namespace Ricave.Core
{
    public class Instruction_SetTitle : Instruction
    {
        public Item Item
        {
            get
            {
                return this.item;
            }
        }

        public TitleSpec Title
        {
            get
            {
                return this.title;
            }
        }

        protected Instruction_SetTitle()
        {
        }

        public Instruction_SetTitle(Item item, TitleSpec title)
        {
            this.item = item;
            this.title = title;
        }

        protected override void DoImpl()
        {
            this.prevTitle = this.item.Title;
            this.item.Title = this.title;
        }

        protected override void UndoImpl()
        {
            this.item.Title = this.prevTitle;
        }

        [Saved]
        private Item item;

        [Saved]
        private TitleSpec title;

        [Saved]
        private TitleSpec prevTitle;
    }
}