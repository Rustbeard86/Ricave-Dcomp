using System;

namespace Ricave.Core
{
    public class Instruction_ChangeSatiation : Instruction
    {
        public int Offset
        {
            get
            {
                return this.offset;
            }
        }

        protected Instruction_ChangeSatiation()
        {
        }

        public Instruction_ChangeSatiation(int offset)
        {
            this.offset = offset;
        }

        protected override void DoImpl()
        {
            Get.Player.Satiation += this.offset;
        }

        protected override void UndoImpl()
        {
            Get.Player.Satiation -= this.offset;
        }

        [Saved]
        private int offset;
    }
}