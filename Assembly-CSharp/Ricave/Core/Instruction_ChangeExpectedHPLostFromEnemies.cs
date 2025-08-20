using System;

namespace Ricave.Core
{
    public class Instruction_ChangeExpectedHPLostFromEnemies : Instruction
    {
        public float Offset
        {
            get
            {
                return this.offset;
            }
        }

        protected Instruction_ChangeExpectedHPLostFromEnemies()
        {
        }

        public Instruction_ChangeExpectedHPLostFromEnemies(float offset)
        {
            this.offset = offset;
        }

        protected override void DoImpl()
        {
            this.prevValue = Get.Player.ExpectedHPLostFromEnemies;
            Get.Player.ExpectedHPLostFromEnemies += this.offset;
        }

        protected override void UndoImpl()
        {
            Get.Player.ExpectedHPLostFromEnemies = this.prevValue;
        }

        [Saved]
        private float offset;

        [Saved]
        private float prevValue;
    }
}