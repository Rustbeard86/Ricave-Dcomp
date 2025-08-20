using System;

namespace Ricave.Core
{
    public class Instruction_ProximityTrigger_SetTriggered : Instruction
    {
        public ProximityTriggerComp Trigger
        {
            get
            {
                return this.trigger;
            }
        }

        public bool Triggered
        {
            get
            {
                return this.triggered;
            }
        }

        protected Instruction_ProximityTrigger_SetTriggered()
        {
        }

        public Instruction_ProximityTrigger_SetTriggered(ProximityTriggerComp trigger, bool triggered)
        {
            this.trigger = trigger;
            this.triggered = triggered;
        }

        protected override void DoImpl()
        {
            this.prevTriggered = this.trigger.Triggered;
            this.trigger.Triggered = this.triggered;
        }

        protected override void UndoImpl()
        {
            this.trigger.Triggered = this.prevTriggered;
        }

        [Saved]
        private ProximityTriggerComp trigger;

        [Saved]
        private bool triggered;

        [Saved]
        private bool prevTriggered;
    }
}