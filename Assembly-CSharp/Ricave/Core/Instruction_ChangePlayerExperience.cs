using System;

namespace Ricave.Core
{
    public class Instruction_ChangePlayerExperience : Instruction
    {
        public int Offset
        {
            get
            {
                return this.offset;
            }
        }

        protected Instruction_ChangePlayerExperience()
        {
        }

        public Instruction_ChangePlayerExperience(int offset)
        {
            this.offset = offset;
        }

        protected override void DoImpl()
        {
            Get.Player.Experience += this.offset;
        }

        protected override void UndoImpl()
        {
            Get.Player.Experience -= this.offset;
        }

        [Saved]
        private int offset;
    }
}