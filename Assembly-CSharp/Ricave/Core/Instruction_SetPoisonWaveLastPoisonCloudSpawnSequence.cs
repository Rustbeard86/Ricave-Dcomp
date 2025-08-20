using System;

namespace Ricave.Core
{
    public class Instruction_SetPoisonWaveLastPoisonCloudSpawnSequence : Instruction
    {
        public WorldSituation_PoisonWave PoisonWave
        {
            get
            {
                return this.poisonWave;
            }
        }

        public int Sequence
        {
            get
            {
                return this.sequence;
            }
        }

        protected Instruction_SetPoisonWaveLastPoisonCloudSpawnSequence()
        {
        }

        public Instruction_SetPoisonWaveLastPoisonCloudSpawnSequence(WorldSituation_PoisonWave poisonWave, int sequence)
        {
            this.poisonWave = poisonWave;
            this.sequence = sequence;
        }

        protected override void DoImpl()
        {
            this.prevSequence = this.poisonWave.LastPoisonCloudSpawnSequence;
            this.poisonWave.LastPoisonCloudSpawnSequence = new int?(this.sequence);
        }

        protected override void UndoImpl()
        {
            this.poisonWave.LastPoisonCloudSpawnSequence = this.prevSequence;
        }

        [Saved]
        private WorldSituation_PoisonWave poisonWave;

        [Saved]
        private int sequence;

        [Saved]
        private int? prevSequence;
    }
}