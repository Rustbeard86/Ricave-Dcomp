using System;

namespace Ricave.Core
{
    public class Instruction_OptionalDelay : Instruction
    {
        public float Delay
        {
            get
            {
                return this.delay;
            }
        }

        protected Instruction_OptionalDelay()
        {
        }

        public Instruction_OptionalDelay(float delay)
        {
            this.delay = delay;
        }

        protected override void DoImpl()
        {
            Get.TurnManager.SetOptionalDelay(this.delay);
        }

        protected override void UndoImpl()
        {
        }

        [Saved]
        private float delay;
    }
}