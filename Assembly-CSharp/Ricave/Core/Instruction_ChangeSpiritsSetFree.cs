using System;

namespace Ricave.Core
{
    public class Instruction_ChangeSpiritsSetFree : Instruction
    {
        public int Offset
        {
            get
            {
                return this.offset;
            }
        }

        protected Instruction_ChangeSpiritsSetFree()
        {
        }

        public Instruction_ChangeSpiritsSetFree(int offset)
        {
            this.offset = offset;
        }

        protected override void DoImpl()
        {
            Get.Player.SpiritsSetFree += this.offset;
        }

        protected override void UndoImpl()
        {
            Get.Player.SpiritsSetFree -= this.offset;
        }

        [Saved]
        private int offset;
    }
}