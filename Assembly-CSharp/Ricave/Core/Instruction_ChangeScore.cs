using System;

namespace Ricave.Core
{
    public class Instruction_ChangeScore : Instruction
    {
        public int Offset
        {
            get
            {
                return this.offset;
            }
        }

        protected Instruction_ChangeScore()
        {
        }

        public Instruction_ChangeScore(int offset)
        {
            this.offset = offset;
        }

        protected override void DoImpl()
        {
            Get.Player.Score += this.offset;
        }

        protected override void UndoImpl()
        {
            Get.Player.Score -= this.offset;
        }

        [Saved]
        private int offset;
    }
}