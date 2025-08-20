using System;

namespace Ricave.Core
{
    public class Instruction_ChangeTurnsCanRewind : Instruction
    {
        public int Offset
        {
            get
            {
                return this.offset;
            }
        }

        protected Instruction_ChangeTurnsCanRewind()
        {
        }

        public Instruction_ChangeTurnsCanRewind(int offset)
        {
            this.offset = offset;
        }

        protected override void DoImpl()
        {
            Get.Player.TurnsCanRewind += this.offset;
        }

        protected override void UndoImpl()
        {
        }

        [Saved]
        private int offset;
    }
}