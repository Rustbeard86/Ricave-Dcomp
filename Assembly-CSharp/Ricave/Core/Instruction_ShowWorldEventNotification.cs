using System;

namespace Ricave.Core
{
    public class Instruction_ShowWorldEventNotification : Instruction
    {
        public WorldEventSpec WorldEventSpec
        {
            get
            {
                return this.worldEventSpec;
            }
        }

        protected Instruction_ShowWorldEventNotification()
        {
        }

        public Instruction_ShowWorldEventNotification(WorldEventSpec worldEventSpec)
        {
            this.worldEventSpec = worldEventSpec;
        }

        protected override void DoImpl()
        {
            Get.WorldEventNotification.ShowOrQueueFor(this.worldEventSpec);
        }

        protected override void UndoImpl()
        {
            Get.WorldEventNotification.StopShowingFor(this.worldEventSpec);
        }

        [Saved]
        private WorldEventSpec worldEventSpec;
    }
}