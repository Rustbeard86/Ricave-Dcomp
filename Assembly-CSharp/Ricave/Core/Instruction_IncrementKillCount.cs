using System;

namespace Ricave.Core
{
    public class Instruction_IncrementKillCount : Instruction
    {
        public EntitySpec ActorSpec
        {
            get
            {
                return this.actorSpec;
            }
        }

        protected Instruction_IncrementKillCount()
        {
        }

        public Instruction_IncrementKillCount(EntitySpec actorSpec)
        {
            this.actorSpec = actorSpec;
        }

        protected override void DoImpl()
        {
            Get.KillCounter.ChangeKillCount(this.actorSpec, 1);
        }

        protected override void UndoImpl()
        {
            Get.KillCounter.ChangeKillCount(this.actorSpec, -1);
        }

        [Saved]
        private EntitySpec actorSpec;
    }
}