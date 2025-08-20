using System;

namespace Ricave.Core
{
    public class Instruction_ChangePetRatSatiation : Instruction
    {
        public int Offset
        {
            get
            {
                return this.offset;
            }
        }

        protected Instruction_ChangePetRatSatiation()
        {
        }

        public Instruction_ChangePetRatSatiation(int offset)
        {
            this.offset = offset;
        }

        protected override void DoImpl()
        {
            Get.Progress.PetRatSatiation += this.offset;
        }

        protected override void UndoImpl()
        {
            Get.Progress.PetRatSatiation -= this.offset;
        }

        [Saved]
        private int offset;
    }
}