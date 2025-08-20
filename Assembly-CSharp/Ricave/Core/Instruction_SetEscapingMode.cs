using System;

namespace Ricave.Core
{
    public class Instruction_SetEscapingMode : Instruction
    {
        public bool EscapingMode
        {
            get
            {
                return this.escapingMode;
            }
        }

        protected Instruction_SetEscapingMode()
        {
        }

        public Instruction_SetEscapingMode(bool escapingMode)
        {
            this.escapingMode = escapingMode;
        }

        protected override void DoImpl()
        {
            this.prevEscapingMode = Get.WorldInfo.EscapingMode;
            Get.WorldInfo.EscapingMode = this.escapingMode;
        }

        protected override void UndoImpl()
        {
            Get.WorldInfo.EscapingMode = this.prevEscapingMode;
        }

        [Saved]
        private bool escapingMode;

        [Saved]
        private bool prevEscapingMode;
    }
}