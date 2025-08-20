using System;

namespace Ricave.Core
{
    public class Instruction_Spawner_AddToSpawnedEntities : Instruction
    {
        public SpawnerComp Spawner
        {
            get
            {
                return this.spawner;
            }
        }

        public Entity Entity
        {
            get
            {
                return this.entity;
            }
        }

        protected Instruction_Spawner_AddToSpawnedEntities()
        {
        }

        public Instruction_Spawner_AddToSpawnedEntities(Entity entity, SpawnerComp spawner)
        {
            this.entity = entity;
            this.spawner = spawner;
        }

        protected override void DoImpl()
        {
            this.spawner.SpawnedEntities.Add(this.entity);
        }

        protected override void UndoImpl()
        {
            this.spawner.SpawnedEntities.RemoveLast(this.entity);
        }

        [Saved]
        private SpawnerComp spawner;

        [Saved]
        private Entity entity;
    }
}