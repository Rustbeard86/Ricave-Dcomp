using System;

namespace Ricave.Core
{
    public class Instruction_SetLastPenaltyRatSpawnSequence : Instruction
    {
        public int SpawnSequence
        {
            get
            {
                return this.spawnSequence;
            }
        }

        protected Instruction_SetLastPenaltyRatSpawnSequence()
        {
        }

        public Instruction_SetLastPenaltyRatSpawnSequence(int spawnSequence)
        {
            this.spawnSequence = spawnSequence;
        }

        protected override void DoImpl()
        {
            this.prevSpawnSequence = Get.WorldInfo.LastPenaltyRatSpawnSequence;
            Get.WorldInfo.LastPenaltyRatSpawnSequence = new int?(this.spawnSequence);
        }

        protected override void UndoImpl()
        {
            Get.WorldInfo.LastPenaltyRatSpawnSequence = this.prevSpawnSequence;
        }

        [Saved]
        private int spawnSequence;

        [Saved]
        private int? prevSpawnSequence;
    }
}