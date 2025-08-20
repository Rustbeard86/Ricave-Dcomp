using System;

namespace Ricave.Core
{
    public class Instruction_RemoveWorldSituation : Instruction
    {
        public WorldSituation WorldSituation
        {
            get
            {
                return this.situation;
            }
        }

        protected Instruction_RemoveWorldSituation()
        {
        }

        public Instruction_RemoveWorldSituation(WorldSituation situation)
        {
            this.situation = situation;
        }

        protected override void DoImpl()
        {
            this.removedFromIndex = Get.WorldSituationsManager.RemoveWorldSituation(this.situation);
        }

        protected override void UndoImpl()
        {
            Get.WorldSituationsManager.AddWorldSituation(this.situation, this.removedFromIndex);
        }

        [Saved]
        private WorldSituation situation;

        [Saved]
        private int removedFromIndex;
    }
}