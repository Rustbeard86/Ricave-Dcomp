using System;

namespace Ricave.Core
{
    public class Instruction_Spawner_ChangeTurnsPassed : Instruction
    {
        public SpawnerComp Spawner
        {
            get
            {
                return this.spawner;
            }
        }

        public int Offset
        {
            get
            {
                return this.offset;
            }
        }

        protected Instruction_Spawner_ChangeTurnsPassed()
        {
        }

        public Instruction_Spawner_ChangeTurnsPassed(SpawnerComp spawner, int offset)
        {
            this.spawner = spawner;
            this.offset = offset;
        }

        protected override void DoImpl()
        {
            this.spawner.TurnsPassed += this.offset;
        }

        protected override void UndoImpl()
        {
            this.spawner.TurnsPassed -= this.offset;
        }

        [Saved]
        private SpawnerComp spawner;

        [Saved]
        private int offset;
    }
}