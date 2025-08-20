using System;

namespace Ricave.Core
{
    public class Instruction_ProximityTrigger_ChangeTurnsPassed : Instruction
    {
        public ProximityTriggerComp Trigger
        {
            get
            {
                return this.trigger;
            }
        }

        public int Offset
        {
            get
            {
                return this.offset;
            }
        }

        protected Instruction_ProximityTrigger_ChangeTurnsPassed()
        {
        }

        public Instruction_ProximityTrigger_ChangeTurnsPassed(ProximityTriggerComp trigger, int offset)
        {
            this.trigger = trigger;
            this.offset = offset;
        }

        protected override void DoImpl()
        {
            this.trigger.TurnsPassed += this.offset;
        }

        protected override void UndoImpl()
        {
            this.trigger.TurnsPassed -= this.offset;
        }

        [Saved]
        private ProximityTriggerComp trigger;

        [Saved]
        private int offset;
    }
}