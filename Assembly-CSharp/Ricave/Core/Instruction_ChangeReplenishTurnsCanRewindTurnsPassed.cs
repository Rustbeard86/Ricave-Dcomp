using System;

namespace Ricave.Core
{
    public class Instruction_ChangeReplenishTurnsCanRewindTurnsPassed : Instruction
    {
        public int Offset
        {
            get
            {
                return this.offset;
            }
        }

        protected Instruction_ChangeReplenishTurnsCanRewindTurnsPassed()
        {
        }

        public Instruction_ChangeReplenishTurnsCanRewindTurnsPassed(int offset)
        {
            this.offset = offset;
        }

        protected override void DoImpl()
        {
            Get.Player.ReplenishTurnsCanRewindTurnsPassed += this.offset;
        }

        protected override void UndoImpl()
        {
        }

        [Saved]
        private int offset;
    }
}