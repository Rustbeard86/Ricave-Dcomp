using System;

namespace Ricave.Core
{
    public class Instruction_AddToIdentifiedList : Instruction
    {
        public EntitySpec ItemSpec
        {
            get
            {
                return this.itemSpec;
            }
        }

        protected Instruction_AddToIdentifiedList()
        {
        }

        public Instruction_AddToIdentifiedList(EntitySpec itemSpec)
        {
            this.itemSpec = itemSpec;
        }

        protected override void DoImpl()
        {
            Get.IdentificationGroups.AddToIdentified(this.itemSpec, -1);
        }

        protected override void UndoImpl()
        {
            Get.IdentificationGroups.RemoveFromIdentified(this.itemSpec);
        }

        [Saved]
        private EntitySpec itemSpec;
    }
}