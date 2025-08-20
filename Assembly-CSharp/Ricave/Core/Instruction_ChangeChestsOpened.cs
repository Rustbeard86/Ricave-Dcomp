using System;

namespace Ricave.Core
{
    public class Instruction_ChangeChestsOpened : Instruction
    {
        public int Offset
        {
            get
            {
                return this.offset;
            }
        }

        protected Instruction_ChangeChestsOpened()
        {
        }

        public Instruction_ChangeChestsOpened(int offset)
        {
            this.offset = offset;
        }

        protected override void DoImpl()
        {
            Get.Player.ChestsOpened += this.offset;
        }

        protected override void UndoImpl()
        {
            Get.Player.ChestsOpened -= this.offset;
        }

        [Saved]
        private int offset;
    }
}