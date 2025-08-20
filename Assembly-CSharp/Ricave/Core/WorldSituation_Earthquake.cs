using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class WorldSituation_Earthquake : WorldSituation
    {
        public int? LastBoulderSpawnSequence
        {
            get
            {
                return this.lastBoulderSpawnSequence;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.lastBoulderSpawnSequence = value;
            }
        }

        protected WorldSituation_Earthquake()
        {
        }

        public WorldSituation_Earthquake(WorldSituationSpec spec)
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
            if (Get.World.GetEntitiesOfSpec(Get.Entity_Boulder).Count >= 20)
            {
                yield break;
            }
            if (this.lastBoulderSpawnSequence == null || Get.TurnManager.CurrentSequence - this.lastBoulderSpawnSequence.Value >= 24)
            {
                yield return new Instruction_SetEarthquakeLastBoulderSpawnSequence(this, Get.TurnManager.CurrentSequence);
                Vector3Int pos;
                if (SpawnPositionFinder.OutOfPlayerSight(Get.Entity_Boulder, out pos, true, true, true, false))
                {
                    Entity entity = Maker.Make(Get.Entity_Boulder, null, false, false, true);
                    foreach (Instruction instruction2 in InstructionSets_Entity.Spawn(entity, pos, null))
                    {
                        yield return instruction2;
                    }
                    enumerator = null;
                    if (pos.GetGridDistance(Get.NowControlledActor.Position) <= 4)
                    {
                        yield return new Instruction_VisualEffect(Get.VisualEffect_BoulderFall, pos);
                    }
                }
                pos = default(Vector3Int);
            }
            yield break;
            yield break;
        }

        [Saved]
        private int? lastBoulderSpawnSequence;

        private const int SpawnEveryTurns = 2;

        private const int MaxBoulders = 20;
    }
}