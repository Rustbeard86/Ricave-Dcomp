using System;

namespace Ricave.Core
{
    public class Instruction_AddToWorldSituationSpawnerSpawnedEntities : Instruction
    {
        public Entity Entity
        {
            get
            {
                return this.entity;
            }
        }

        public WorldSituation_Spawner Spawner
        {
            get
            {
                return this.spawner;
            }
        }

        protected Instruction_AddToWorldSituationSpawnerSpawnedEntities()
        {
        }

        public Instruction_AddToWorldSituationSpawnerSpawnedEntities(Entity entity, WorldSituation_Spawner spawner)
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
        private Entity entity;

        [Saved]
        private WorldSituation_Spawner spawner;
    }
}