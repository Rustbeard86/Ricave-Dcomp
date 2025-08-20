using System;

namespace Ricave.Core
{
    public class Instruction_AddWorldSituation : Instruction
    {
        public WorldSituation WorldSituation
        {
            get
            {
                return this.situation;
            }
        }

        protected Instruction_AddWorldSituation()
        {
        }

        public Instruction_AddWorldSituation(WorldSituation situation)
        {
            this.situation = situation;
        }

        protected override void DoImpl()
        {
            Get.WorldSituationsManager.AddWorldSituation(this.situation, -1);
        }

        protected override void UndoImpl()
        {
            Get.WorldSituationsManager.RemoveWorldSituation(this.situation);
        }

        [Saved]
        private WorldSituation situation;
    }
}