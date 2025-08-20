using System;

namespace Ricave.Core
{
    public class Instruction_ChangeHPLostFromEnemies : Instruction
    {
        public int Offset
        {
            get
            {
                return this.offset;
            }
        }

        protected Instruction_ChangeHPLostFromEnemies()
        {
        }

        public Instruction_ChangeHPLostFromEnemies(int offset)
        {
            this.offset = offset;
        }

        protected override void DoImpl()
        {
            Get.Player.HPLostFromEnemies += this.offset;
        }

        protected override void UndoImpl()
        {
            Get.Player.HPLostFromEnemies -= this.offset;
        }

        [Saved]
        private int offset;
    }
}