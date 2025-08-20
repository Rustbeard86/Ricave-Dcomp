using System;

namespace Ricave.Core
{
    public class Instruction_ChangeAncientDevicesResearched : Instruction
    {
        public int Offset
        {
            get
            {
                return this.offset;
            }
        }

        protected Instruction_ChangeAncientDevicesResearched()
        {
        }

        public Instruction_ChangeAncientDevicesResearched(int offset)
        {
            this.offset = offset;
        }

        protected override void DoImpl()
        {
            Get.Player.AncientDevicesResearched += this.offset;
        }

        protected override void UndoImpl()
        {
            Get.Player.AncientDevicesResearched -= this.offset;
        }

        [Saved]
        private int offset;
    }
}