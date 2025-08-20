using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class UseEffect_Fishing : UseEffect
    {
        protected UseEffect_Fishing()
        {
        }

        public UseEffect_Fishing(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            Entity targetEntity = target.Entity;
            if (targetEntity == null)
            {
                yield break;
            }
            FishableComp comp = targetEntity.GetComp<FishableComp>();
            if (comp != null && comp.Emptied)
            {
                yield return new Instruction_PlayLog("NoMoreFishHere".Translate());
                yield break;
            }
            yield return new Instruction_Sound(Get.Sound_Fishing, new Vector3?(UseEffect_Sound.GetSourcePos(user, usable, target)), 1f, 1f);
            Item fish;
            if (targetEntity.Spec == Get.Entity_Water)
            {
                fish = Maker.Make<Item>(Get.Entity_Fish, null, false, false, true);
            }
            else if (targetEntity.Spec == Get.Entity_ToxicWater)
            {
                fish = Maker.Make<Item>(Get.Entity_ToxicFish, null, false, false, true);
            }
            else if (targetEntity.Spec == Get.Entity_WallWithHole)
            {
                Rand.PushState(Calc.CombineHashes<int, int, int>(Get.WorldSeed, targetEntity.MyStableHash, 429606265));
                fish = ItemGenerator.SmallReward(true);
                Rand.PopState();
            }
            else
            {
                Actor actor = targetEntity as Actor;
                if (actor != null)
                {
                    fish = null;
                    if (user != null && !actor.ImmuneToPushing)
                    {
                        int gridDistance = actor.Position.GetGridDistance(user.Position);
                        foreach (Instruction instruction in InstructionSets_Entity.Push(actor, user.Position - actor.Position, gridDistance, true, true, true))
                        {
                            yield return instruction;
                        }
                        IEnumerator<Instruction> enumerator = null;
                    }
                }
                else
                {
                    fish = Maker.Make<Item>(Get.Entity_Fish, null, false, false, true);
                }
            }
            if (fish != null)
            {
                foreach (Instruction instruction2 in InstructionSets_Actor.AddToInventoryOrSpawnNear(user, fish))
                {
                    yield return instruction2;
                }
                IEnumerator<Instruction> enumerator = null;
                yield return new Instruction_PlayLog("CaughtFishing".Translate(fish));
            }
            Vector3Int position = target.Position;
            Predicate<Vector3Int> <> 9__0;
            Predicate<Vector3Int> predicate;
            if ((predicate = <> 9__0) == null)
            {
                predicate = (<> 9__0 = (Vector3Int x) => Get.World.AnyEntityOfSpecAt(x, targetEntity.Spec));
            }
            foreach (Vector3Int vector3Int in FloodFiller.FloodFillEnumerable(position, predicate))
            {
                foreach (Entity entity in Get.World.GetEntitiesAt(vector3Int))
                {
                    if (entity.Spec == targetEntity.Spec)
                    {
                        FishableComp comp2 = entity.GetComp<FishableComp>();
                        if (comp2 != null && !comp2.Emptied)
                        {
                            yield return new Instruction_Fishable_SetEmptied(comp2, true);
                        }
                    }
                }
                List<Entity>.Enumerator enumerator3 = default(List<Entity>.Enumerator);
            }
            IEnumerator<Vector3Int> enumerator2 = null;
            yield break;
            yield break;
        }
    }
}