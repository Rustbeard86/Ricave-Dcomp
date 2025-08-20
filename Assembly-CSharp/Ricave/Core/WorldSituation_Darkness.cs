using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ricave.Core
{
    public class WorldSituation_Darkness : WorldSituation
    {
        public override ValueTuple<Color, float>? FogOverride
        {
            get
            {
                return new ValueTuple<Color, float>?(new ValueTuple<Color, float>(Color.black, 3f));
            }
        }

        public override float AmbientLightFactor
        {
            get
            {
                return 0.6f;
            }
        }

        protected WorldSituation_Darkness()
        {
        }

        public WorldSituation_Darkness(WorldSituationSpec spec)
            : base(spec)
        {
        }

        public override IEnumerable<Instruction> MakePostAddInstructions()
        {
            foreach (Instruction instruction in base.MakePostAddInstructions())
            {
                yield return instruction;
            }
            IEnumerator<Instruction> enumerator = null;
            foreach (Entity entity in Get.World.GetEntitiesOfSpec(Get.Entity_Torch).Concat<Entity>(Get.World.GetEntitiesOfSpec(Get.Entity_VioletTorch)).ToTemporaryList<Entity>())
            {
                foreach (Instruction instruction2 in InstructionSets_Entity.Destroy(entity, null, null))
                {
                    yield return instruction2;
                }
                enumerator = null;
            }
            List<Entity>.Enumerator enumerator2 = default(List<Entity>.Enumerator);
            int num;
            for (int i = 0; i < 2; i = num + 1)
            {
                int index = i;
                Vector3Int vector3Int;
                if (SpawnPositionFinder.FromFloorBars(out vector3Int))
                {
                    Actor actor = Maker.Make<Actor>(Get.Entity_Ghost, delegate (Actor x)
                    {
                        x.RampUp = RampUpUtility.GenerateRandomRampUpFor(x, true);
                        DifficultyUtility.AddConditionsForDifficulty(x);
                        x.CalculateInitialHPManaAndStamina();
                        if (!Get.Trait_Scary.IsChosen())
                        {
                            if (index == 0)
                            {
                                x.Inventory.Add(ItemGenerator.Amulet(false), default(ValueTuple<Vector2Int?, int?, int?>));
                            }
                            else
                            {
                                x.Inventory.Add(ItemGenerator.Gold(10), default(ValueTuple<Vector2Int?, int?, int?>));
                            }
                        }
                        string text;
                        if (NameGenerator.TryGenerateGhostName(out text))
                        {
                            x.Name = text;
                        }
                    }, false, false, true);
                    foreach (Instruction instruction3 in InstructionSets_Entity.Spawn(actor, vector3Int, null))
                    {
                        yield return instruction3;
                    }
                    enumerator = null;
                }
                num = i;
            }
            yield break;
            yield break;
        }
    }
}