using System;

namespace Ricave.Core
{
    public class Instruction_SetWorldSituationSpawnerLastSpawnSequence : Instruction
    {
        public WorldSituation_Spawner Spawner
        {
            get
            {
                return this.spawner;
            }
        }

        public int Sequence
        {
            get
            {
                return this.sequence;
            }
        }

        protected Instruction_SetWorldSituationSpawnerLastSpawnSequence()
        {
        }

        public Instruction_SetWorldSituationSpawnerLastSpawnSequence(WorldSituation_Spawner spawner, int sequence)
        {
            this.spawner = spawner;
            this.sequence = sequence;
        }

        protected override void DoImpl()
        {
            this.prevSequence = this.spawner.LastSpawnSequence;
            this.spawner.LastSpawnSequence = new int?(this.sequence);
        }

        protected override void UndoImpl()
        {
            this.spawner.LastSpawnSequence = this.prevSequence;
        }

        [Saved]
        private WorldSituation_Spawner spawner;

        [Saved]
        private int sequence;

        [Saved]
        private int? prevSequence;
    }
}