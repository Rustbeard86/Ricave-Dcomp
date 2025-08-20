using System;

namespace Ricave.Core
{
    public class Instruction_ChangeLobbyItems : Instruction
    {
        public EntitySpec ItemSpec
        {
            get
            {
                return this.itemSpec;
            }
        }

        public int Offset
        {
            get
            {
                return this.offset;
            }
        }

        protected Instruction_ChangeLobbyItems()
        {
        }

        public Instruction_ChangeLobbyItems(EntitySpec itemSpec, int offset)
        {
            this.itemSpec = itemSpec;
            this.offset = offset;
        }

        protected override void DoImpl()
        {
            if (Get.InLobby)
            {
                Get.TotalLobbyItems.ChangeCount(this.itemSpec, this.offset);
                return;
            }
            Get.ThisRunLobbyItems.ChangeCount(this.itemSpec, this.offset);
        }

        protected override void UndoImpl()
        {
            if (Get.InLobby)
            {
                Get.TotalLobbyItems.ChangeCount(this.itemSpec, -this.offset);
                return;
            }
            Get.ThisRunLobbyItems.ChangeCount(this.itemSpec, -this.offset);
        }

        [Saved]
        private EntitySpec itemSpec;

        [Saved]
        private int offset;
    }
}