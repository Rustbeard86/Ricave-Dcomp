using System;

namespace Ricave.Core
{
    public class Instruction_RewindTime : Instruction
    {
        public int Turns
        {
            get
            {
                return this.turns;
            }
        }

        protected Instruction_RewindTime()
        {
        }

        public Instruction_RewindTime(int turns)
        {
            this.turns = turns;
        }

        protected override void DoImpl()
        {
            Get.TurnManager.RewindTime(this.turns);
        }

        protected override void UndoImpl()
        {
        }

        [Saved]
        private int turns;
    }
}