using System;

namespace Ricave.Core
{
    public class Instruction_ChangeCurrentKillCombo : Instruction
    {
        public int Offset
        {
            get
            {
                return this.offset;
            }
        }

        protected Instruction_ChangeCurrentKillCombo()
        {
        }

        public Instruction_ChangeCurrentKillCombo(int offset)
        {
            this.offset = offset;
        }

        protected override void DoImpl()
        {
            Get.Player.CurrentKillCombo += this.offset;
        }

        protected override void UndoImpl()
        {
            Get.Player.CurrentKillCombo -= this.offset;
        }

        [Saved]
        private int offset;
    }
}