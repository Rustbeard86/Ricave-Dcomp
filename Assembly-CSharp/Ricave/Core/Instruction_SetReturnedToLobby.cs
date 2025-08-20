using System;

namespace Ricave.Core
{
    public class Instruction_SetReturnedToLobby : Instruction
    {
        public bool Returned
        {
            get
            {
                return this.returned;
            }
        }

        protected Instruction_SetReturnedToLobby()
        {
        }

        public Instruction_SetReturnedToLobby(bool returned)
        {
            this.returned = returned;
        }

        protected override void DoImpl()
        {
            this.prevReturned = Get.RunInfo.ReturnedToLobby;
            Get.RunInfo.ReturnedToLobby = this.returned;
        }

        protected override void UndoImpl()
        {
            Get.RunInfo.ReturnedToLobby = this.prevReturned;
        }

        [Saved]
        private bool returned;

        [Saved]
        private bool prevReturned;
    }
}