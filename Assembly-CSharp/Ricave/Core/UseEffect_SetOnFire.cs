using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class UseEffect_SetOnFire : UseEffect
    {
        public override GoodBadNeutral GoodBadNeutral
        {
            get
            {
                return GoodBadNeutral.Bad;
            }
        }

        public override string LabelBase
        {
            get
            {
                return base.Spec.LabelFormat.Formatted(8);
            }
        }

        protected UseEffect_SetOnFire()
        {
        }

        public UseEffect_SetOnFire(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public UseEffect_SetOnFire(UseEffectSpec spec, float chance = 1f, int usesLeft = 0)
            : base(spec, chance, usesLeft, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            if (Get.CellsInfo.AnyWaterAt(target.Position) && (!target.IsEntity || !UseEffect_SetOnFire.AllowSetOnFireEvenInWater(target.Entity)))
            {
                yield break;
            }
            Actor actor = target.Entity as Actor;
            if (actor != null && actor.ImmuneToFire)
            {
                yield break;
            }
            if (actor != null)
            {
                Condition firstOfSpec = actor.Conditions.GetFirstOfSpec(Get.Condition_Burning);
                if (firstOfSpec != null)
                {
                    if (firstOfSpec.TurnsLeft > 0 && firstOfSpec.TurnsLeft < 8)
                    {
                        int num = 8 - firstOfSpec.TurnsLeft;
                        yield return new Instruction_ChangeConditionTurnsLeft(firstOfSpec, num);
                    }
                }
                else
                {
                    Condition_Burning condition_Burning = new Condition_Burning(Get.Condition_Burning, 1, 8);
                    foreach (Instruction instruction in InstructionSets_Misc.AddCondition(condition_Burning, actor.Conditions, user != null && user.IsPlayerParty, true))
                    {
                        yield return instruction;
                    }
                    IEnumerator<Instruction> enumerator = null;
                }
            }
            else
            {
                Structure structure = target.Entity as Structure;
                if (structure != null && structure.Spec.Structure.Flammable)
                {
                    foreach (Instruction instruction2 in UseEffect_SetOnFire.TrySetStructureOnFireInstructions(structure, user))
                    {
                        yield return instruction2;
                    }
                    IEnumerator<Instruction> enumerator = null;
                }
                else
                {
                    Item item = target.Entity as Item;
                    if (item != null && item.Spec.Item.CookedItemSpec != null && item.Spawned)
                    {
                        foreach (Instruction instruction3 in UseEffect_SetOnFire.CookSpawnedItem(item))
                        {
                            yield return instruction3;
                        }
                        IEnumerator<Instruction> enumerator = null;
                    }
                }
            }
            yield break;
            yield break;
        }

        public static IEnumerable<Instruction> TrySetStructureOnFireInstructions(Structure structure, Actor responsible)
        {
            if (!structure.Spawned)
            {
                yield break;
            }
            if (!structure.Spec.Structure.Flammable)
            {
                yield break;
            }
            if (Get.CellsInfo.AnyWaterAt(structure.Position) && !UseEffect_SetOnFire.AllowSetOnFireEvenInWater(structure))
            {
                yield break;
            }
            foreach (Instruction instruction in InstructionSets_Entity.Destroy(structure, null, responsible))
            {
                yield return instruction;
            }
            IEnumerator<Instruction> enumerator = null;
            if (!Get.World.AnyEntityOfSpecAt(structure.Position, Get.Entity_Fire))
            {
                Entity fire = Maker.Make(Get.Entity_Fire, null, false, false, true);
                foreach (Instruction instruction2 in InstructionSets_Entity.Spawn(fire, structure.Position, null))
                {
                    yield return instruction2;
                }
                enumerator = null;
                foreach (Instruction instruction3 in InstructionSets_Noise.MakeNoise(fire.Position, 2, NoiseType.StructureSetOnFire, responsible, false, true))
                {
                    yield return instruction3;
                }
                enumerator = null;
                fire = null;
            }
            yield break;
            yield break;
        }

        public static IEnumerable<Instruction> CookSpawnedItem(Item cookable)
        {
            foreach (Instruction instruction in InstructionSets_Entity.DeSpawn(cookable, false))
            {
                yield return instruction;
            }
            IEnumerator<Instruction> enumerator = null;
            Item item = Maker.Make<Item>(cookable.Spec.Item.CookedItemSpec, delegate (Item x)
            {
                x.StackCount = cookable.StackCount;
            }, false, false, true);
            foreach (Instruction instruction2 in InstructionSets_Entity.Spawn(item, cookable.Position, null))
            {
                yield return instruction2;
            }
            enumerator = null;
            yield break;
            yield break;
        }

        private static bool AllowSetOnFireEvenInWater(Entity entity)
        {
            return entity.Spec == Get.Entity_Bridge || entity.Spec == Get.Entity_UnstableBridge;
        }

        private const int BurningTurnsDuration = 8;
    }
}