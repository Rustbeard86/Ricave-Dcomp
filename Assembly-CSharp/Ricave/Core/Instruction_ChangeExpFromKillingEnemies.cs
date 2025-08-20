using System;

namespace Ricave.Core
{
    public class Instruction_ChangeExpFromKillingEnemies : Instruction
    {
        public int Offset
        {
            get
            {
                return this.offset;
            }
        }

        protected Instruction_ChangeExpFromKillingEnemies()
        {
        }

        public Instruction_ChangeExpFromKillingEnemies(int offset)
        {
            this.offset = offset;
        }

        protected override void DoImpl()
        {
            Get.Player.ExpFromKillingEnemies += this.offset;
        }

        protected override void UndoImpl()
        {
            Get.Player.ExpFromKillingEnemies -= this.offset;
        }

        [Saved]
        private int offset;
    }
}