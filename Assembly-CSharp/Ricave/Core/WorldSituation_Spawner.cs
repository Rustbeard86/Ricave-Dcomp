using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public abstract class WorldSituation_Spawner : WorldSituation
    {
        public int? LastSpawnSequence
        {
            get
            {
                return this.lastSpawnSequence;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.lastSpawnSequence = value;
            }
        }

        public List<Entity> SpawnedEntities
        {
            get
            {
                return this.spawnedEntities;
            }
        }

        protected abstract int MaxCount { get; }

        protected abstract int IntervalTurns { get; }

        protected abstract EntitySpec EntitySpecToSpawn { get; }

        protected abstract bool FromFloorBars { get; }

        private bool ShouldSpawnMore
        {
            get
            {
                int num = 0;
                for (int i = this.spawnedEntities.Count - 1; i >= 0; i--)
                {
                    if (this.spawnedEntities[i].Spawned)
                    {
                        num++;
                        if (num >= this.MaxCount)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
        }

        protected WorldSituation_Spawner()
        {
        }

        public WorldSituation_Spawner(WorldSituationSpec spec)
            : base(spec)
        {
        }

        public override IEnumerable<Instruction> MakeResolveWorldInstructions()
        {
            foreach (Instruction instruction in base.MakeResolveWorldInstructions())
            {
                yield return instruction;
            }
            IEnumerator<Instruction> enumerator = null;
            if (!this.ShouldSpawnMore)
            {
                yield break;
            }
            Vector3Int pos;
            if ((this.lastSpawnSequence == null || Get.TurnManager.CurrentSequence - this.lastSpawnSequence.Value >= this.IntervalTurns * 12) && this.TryFindSpawnPos(out pos))
            {
                yield return new Instruction_SetWorldSituationSpawnerLastSpawnSequence(this, Get.TurnManager.CurrentSequence);
                Entity toSpawn = Maker.Make(this.EntitySpecToSpawn, delegate (Entity x)
                {
                    Actor actor = x as Actor;
                    if (actor != null)
                    {
                        actor.RampUp = RampUpUtility.GenerateRandomRampUpFor(actor, true);
                        DifficultyUtility.AddConditionsForDifficulty(actor);
                        actor.CalculateInitialHPManaAndStamina();
                        return;
                    }
                    Item item = x as Item;
                    if (item != null)
                    {
                        item.RampUp = RampUpUtility.GenerateRandomRampUpFor(item, true);
                    }
                }, false, false, true);
                foreach (Instruction instruction2 in InstructionSets_Entity.Spawn(toSpawn, pos, null))
                {
                    yield return instruction2;
                }
                enumerator = null;
                yield return new Instruction_AddToWorldSituationSpawnerSpawnedEntities(toSpawn, this);
                toSpawn = null;
            }
            yield break;
            yield break;
        }

        private bool TryFindSpawnPos(out Vector3Int pos)
        {
            if (this.FromFloorBars)
            {
                return SpawnPositionFinder.FromFloorBars(out pos);
            }
            return SpawnPositionFinder.OutOfPlayerSight(this.EntitySpecToSpawn, out pos, true, false, false, false);
        }

        [Saved]
        private int? lastSpawnSequence;

        [Saved(Default.New, true)]
        private List<Entity> spawnedEntities = new List<Entity>();
    }
}