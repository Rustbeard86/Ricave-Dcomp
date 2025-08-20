using System;

namespace Ricave.Core
{
    public class Instruction_SetWorldEventsManagerDidRandomEvent : Instruction
    {
        public bool NewValue
        {
            get
            {
                return this.newValue;
            }
        }

        protected Instruction_SetWorldEventsManagerDidRandomEvent()
        {
        }

        public Instruction_SetWorldEventsManagerDidRandomEvent(bool newValue)
        {
            this.newValue = newValue;
        }

        protected override void DoImpl()
        {
            this.prevValue = Get.WorldEventsManager.DidRandomEvent;
            Get.WorldEventsManager.DidRandomEvent = this.newValue;
        }

        protected override void UndoImpl()
        {
            Get.WorldEventsManager.DidRandomEvent = this.prevValue;
        }

        [Saved]
        private bool newValue;

        [Saved]
        private bool prevValue;
    }
}