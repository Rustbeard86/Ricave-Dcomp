using System;

namespace Ricave.Core
{
    public class Instruction_SetLastWorthyKillSequence : Instruction
    {
        public int LastWorthyKillSequence
        {
            get
            {
                return this.lastWorthyKillSequence;
            }
        }

        protected Instruction_SetLastWorthyKillSequence()
        {
        }

        public Instruction_SetLastWorthyKillSequence(int lastWorthyKillSequence)
        {
            this.lastWorthyKillSequence = lastWorthyKillSequence;
        }

        protected override void DoImpl()
        {
            this.prevLastWorthyKillSequence = Get.KillCounter.LastWorthyKillSequence;
            Get.KillCounter.LastWorthyKillSequence = new int?(this.lastWorthyKillSequence);
        }

        protected override void UndoImpl()
        {
            Get.KillCounter.LastWorthyKillSequence = this.prevLastWorthyKillSequence;
        }

        [Saved]
        private int lastWorthyKillSequence;

        [Saved]
        private int? prevLastWorthyKillSequence;
    }
}