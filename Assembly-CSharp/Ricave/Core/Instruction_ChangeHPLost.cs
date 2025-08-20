using System;

namespace Ricave.Core
{
    public class Instruction_ChangeHPLost : Instruction
    {
        public int Offset
        {
            get
            {
                return this.offset;
            }
        }

        protected Instruction_ChangeHPLost()
        {
        }

        public Instruction_ChangeHPLost(int offset)
        {
            this.offset = offset;
        }

        protected override void DoImpl()
        {
            Get.Player.HPLost += this.offset;
        }

        protected override void UndoImpl()
        {
            Get.Player.HPLost -= this.offset;
        }

        [Saved]
        private int offset;
    }
}