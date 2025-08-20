using System;

namespace Ricave.Core
{
    public class Instruction_ChangeHungerTurnsPassed : Instruction
    {
        public Condition_Hunger Hunger
        {
            get
            {
                return this.hunger;
            }
        }

        public int Offset
        {
            get
            {
                return this.offset;
            }
        }

        protected Instruction_ChangeHungerTurnsPassed()
        {
        }

        public Instruction_ChangeHungerTurnsPassed(Condition_Hunger hunger, int offset)
        {
            this.hunger = hunger;
            this.offset = offset;
        }

        protected override void DoImpl()
        {
            this.hunger.TurnsPassed += this.offset;
        }

        protected override void UndoImpl()
        {
            this.hunger.TurnsPassed -= this.offset;
        }

        [Saved]
        private Condition_Hunger hunger;

        [Saved]
        private int offset;
    }
}