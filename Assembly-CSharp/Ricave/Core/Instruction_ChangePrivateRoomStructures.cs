using System;

namespace Ricave.Core
{
    public class Instruction_ChangePrivateRoomStructures : Instruction
    {
        public EntitySpec StructureSpec
        {
            get
            {
                return this.structureSpec;
            }
        }

        public int Offset
        {
            get
            {
                return this.offset;
            }
        }

        protected Instruction_ChangePrivateRoomStructures()
        {
        }

        public Instruction_ChangePrivateRoomStructures(EntitySpec structureSpec, int offset)
        {
            this.structureSpec = structureSpec;
            this.offset = offset;
        }

        protected override void DoImpl()
        {
            if (Get.InLobby)
            {
                Get.PrivateRoom.ChangeInventoryCount(this.structureSpec, this.offset);
                return;
            }
            Get.ThisRunPrivateRoomStructures.ChangeCount(this.structureSpec, this.offset);
        }

        protected override void UndoImpl()
        {
            if (Get.InLobby)
            {
                Get.PrivateRoom.ChangeInventoryCount(this.structureSpec, -this.offset);
                return;
            }
            Get.ThisRunPrivateRoomStructures.ChangeCount(this.structureSpec, -this.offset);
        }

        [Saved]
        private EntitySpec structureSpec;

        [Saved]
        private int offset;
    }
}