using System;

namespace Ricave.Core
{
    public class Instruction_ChangePlayerGold : Instruction
    {
        public int Offset
        {
            get
            {
                return this.offset;
            }
        }

        protected Instruction_ChangePlayerGold()
        {
        }

        public Instruction_ChangePlayerGold(int offset)
        {
            this.offset = offset;
        }

        protected override void DoImpl()
        {
            Get.Player.Gold += this.offset;
        }

        protected override void UndoImpl()
        {
            Get.Player.Gold -= this.offset;
        }

        [Saved]
        private int offset;
    }
}