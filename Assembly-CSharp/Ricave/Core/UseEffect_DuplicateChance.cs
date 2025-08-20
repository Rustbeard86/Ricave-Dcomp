using System;
using System.Collections.Generic;
using System.Linq;

namespace Ricave.Core
{
    public class UseEffect_DuplicateChance : UseEffect
    {
        public float SuccessChance
        {
            get
            {
                return this.successChance;
            }
        }

        public override string LabelBase
        {
            get
            {
                return base.Spec.LabelFormat.Formatted(this.successChance.ToStringPercent(false));
            }
        }

        protected UseEffect_DuplicateChance()
        {
        }

        public UseEffect_DuplicateChance(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
            ((UseEffect_DuplicateChance)clone).successChance = this.successChance;
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            Actor actor = target.Entity as Actor;
            Item item = target.Entity as Item;
            Item toDuplicate;
            if (item != null)
            {
                toDuplicate = item;
            }
            else
            {
                if (actor != null)
                {
                    Item item2 = UsePrompt.Choice as Item;
                    if (item2 != null && actor.Inventory.AllItems.Contains(item2))
                    {
                        toDuplicate = item2;
                        goto IL_00ED;
                    }
                }
                if (actor != null)
                {
                    toDuplicate = actor.Inventory.AllItems.FirstOrDefault<Item>((Item x) => x != usable);
                }
                else
                {
                    toDuplicate = null;
                }
            }
        IL_00ED:
            if (toDuplicate == null)
            {
                yield break;
            }
            if (Rand.Chance(this.SuccessChance))
            {
                Item duplicated = toDuplicate.CloneOne();
                if (actor != null || user != null)
                {
                    foreach (Instruction instruction in InstructionSets_Actor.AddToInventoryOrSpawnNear(actor ?? user, duplicated))
                    {
                        yield return instruction;
                    }
                    IEnumerator<Instruction> enumerator = null;
                }
                else
                {
                    foreach (Instruction instruction2 in InstructionSets_Entity.Spawn(duplicated, SpawnPositionFinder.Near(target.Position, duplicated, false, false, null), null))
                    {
                        yield return instruction2;
                    }
                    IEnumerator<Instruction> enumerator = null;
                }
                yield return new Instruction_PlayLog("ItemDuplicated".Translate(duplicated));
                yield return new Instruction_Sound(Get.Sound_Enchant, null, 1f, 1f);
                duplicated = null;
            }
            else
            {
                if (toDuplicate.Spawned)
                {
                    foreach (Instruction instruction3 in InstructionSets_Entity.Destroy(toDuplicate, null, null))
                    {
                        yield return instruction3;
                    }
                    IEnumerator<Instruction> enumerator = null;
                }
                else
                {
                    foreach (Instruction instruction4 in InstructionSets_Actor.RemoveOneFromInventory(toDuplicate))
                    {
                        yield return instruction4;
                    }
                    IEnumerator<Instruction> enumerator = null;
                }
                yield return new Instruction_Sound(Get.Sound_DestroyItemInInventory, null, 1f, 1f);
                yield return new Instruction_PlayLog("FailedToDuplicate".Translate(toDuplicate.Spec));
            }
            yield break;
            yield break;
        }

        [Saved]
        private float successChance;
    }
}