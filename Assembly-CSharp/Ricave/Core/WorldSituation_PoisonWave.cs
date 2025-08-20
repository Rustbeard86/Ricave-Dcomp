using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class WorldSituation_PoisonWave : WorldSituation
    {
        public override ValueTuple<Color, float>? FogOverride
        {
            get
            {
                return new ValueTuple<Color, float>?(new ValueTuple<Color, float>(new Color32(132, 154, 97, byte.MaxValue), 3f));
            }
        }

        public int? LastPoisonCloudSpawnSequence
        {
            get
            {
                return this.lastPoisonCloudSpawnSequence;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.lastPoisonCloudSpawnSequence = value;
            }
        }

        protected WorldSituation_PoisonWave()
        {
        }

        public WorldSituation_PoisonWave(WorldSituationSpec spec)
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
            if (this.lastPoisonCloudSpawnSequence == null || Get.TurnManager.CurrentSequence - this.lastPoisonCloudSpawnSequence.Value >= 24)
            {
                yield return new Instruction_SetPoisonWaveLastPoisonCloudSpawnSequence(this, Get.TurnManager.CurrentSequence);
                Vector3Int vector3Int;
                if (SpawnPositionFinder.OutOfPlayerSight(Get.Entity_PoisonCloud, out vector3Int, true, true, false, false))
                {
                    Entity entity = Maker.Make(Get.Entity_PoisonCloud, null, false, false, true);
                    foreach (Instruction instruction2 in InstructionSets_Entity.Spawn(entity, vector3Int, null))
                    {
                        yield return instruction2;
                    }
                    enumerator = null;
                }
            }
            yield break;
            yield break;
        }

        [Saved]
        private int? lastPoisonCloudSpawnSequence;

        private const int SpawnEveryTurns = 2;
    }
}