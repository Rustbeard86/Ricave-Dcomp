using System;

namespace Ricave.Core
{
    public class Instruction_Spawner_SetActive : Instruction
    {
        public SpawnerComp Spawner
        {
            get
            {
                return this.spawner;
            }
        }

        public bool Active
        {
            get
            {
                return this.active;
            }
        }

        protected Instruction_Spawner_SetActive()
        {
        }

        public Instruction_Spawner_SetActive(SpawnerComp spawner, bool active)
        {
            this.spawner = spawner;
            this.active = active;
        }

        protected override void DoImpl()
        {
            this.prevActive = this.spawner.Active;
            this.spawner.Active = this.active;
        }

        protected override void UndoImpl()
        {
            this.spawner.Active = this.prevActive;
        }

        [Saved]
        private SpawnerComp spawner;

        [Saved]
        private bool active;

        [Saved]
        private bool prevActive;
    }
}