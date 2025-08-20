using System;

namespace Ricave.Core
{
    public class Instruction_SetLastKillSequence : Instruction
    {
        public int LastKillSequence
        {
            get
            {
                return this.lastKillSequence;
            }
        }

        protected Instruction_SetLastKillSequence()
        {
        }

        public Instruction_SetLastKillSequence(int lastKillSequence)
        {
            this.lastKillSequence = lastKillSequence;
        }

        protected override void DoImpl()
        {
            this.prevLastKillSequence = Get.KillCounter.LastKillSequence;
            Get.KillCounter.LastKillSequence = new int?(this.lastKillSequence);
        }

        protected override void UndoImpl()
        {
            Get.KillCounter.LastKillSequence = this.prevLastKillSequence;
        }

        [Saved]
        private int lastKillSequence;

        [Saved]
        private int? prevLastKillSequence;
    }
}