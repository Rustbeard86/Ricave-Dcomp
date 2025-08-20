using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class UseEffect_SpawnerPressurePlate : UseEffect
    {
        protected UseEffect_SpawnerPressurePlate()
        {
        }

        public UseEffect_SpawnerPressurePlate(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            List<EntitySpec> toSpawn;
            if (Get.RunSpec.AllowMobSpawners)
            {
                toSpawn = new List<EntitySpec>
                {
                    Get.Entity_Wasp,
                    Get.Entity_Rat,
                    Get.Entity_Rat,
                    Get.Entity_Mummy
                };
            }
            else
            {
                toSpawn = new List<EntitySpec>
                {
                    Get.Entity_Rat,
                    Get.Entity_Rat
                };
            }
            toSpawn.Shuffle<EntitySpec>();
            int num;
            for (int i = 0; i < toSpawn.Count; i = num + 1)
            {
                int localI = i;
                Actor actor = Maker.Make<Actor>(toSpawn[i], delegate (Actor x)
                {
                    x.RampUp = RampUpUtility.GenerateRandomRampUpFor(x, false);
                    DifficultyUtility.AddConditionsForDifficulty(x);
                    x.CalculateInitialHPManaAndStamina();
                }, false, false, true);
                Vector3Int vector3Int = SpawnPositionFinder.Near(target.Position, actor, false, false, delegate (Vector3Int pos)
                {
                    if (localI == 0)
                    {
                        return pos.IsAdjacent(target.Position);
                    }
                    return pos.GetGridDistance(target.Position) == 2;
                });
                foreach (Instruction instruction in InstructionSets_Entity.Spawn(actor, vector3Int, null))
                {
                    yield return instruction;
                }
                IEnumerator<Instruction> enumerator = null;
                num = i;
            }
            yield break;
            yield break;
        }
    }
}