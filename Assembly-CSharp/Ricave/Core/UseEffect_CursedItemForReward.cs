using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class UseEffect_CursedItemForReward : UseEffect
    {
        public override GoodBadNeutral GoodBadNeutral
        {
            get
            {
                return GoodBadNeutral.Good;
            }
        }

        protected UseEffect_CursedItemForReward()
        {
        }

        public UseEffect_CursedItemForReward(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            if (user != null && user.IsNowControlledActor)
            {
                object choice = UsePrompt.Choice;
                Item item = choice as Item;
                if (item != null && user.Inventory.Contains(item))
                {
                    if (!item.Cursed || !item.Identified || user.Inventory.Equipped.IsEquipped(item))
                    {
                        yield break;
                    }
                    foreach (Instruction instruction in InstructionSets_Actor.RemoveOneFromInventory(item))
                    {
                        yield return instruction;
                    }
                    IEnumerator<Instruction> enumerator = null;
                    Rand.PushState(Calc.CombineHashes<int, int, int>(Get.WorldSeed, usable.MyStableHash, 346581230));
                    Item reward = ItemGenerator.SmallReward(true);
                    Rand.PopState();
                    foreach (Instruction instruction2 in InstructionSets_Actor.AddToInventoryOrSpawnNear(user, reward))
                    {
                        yield return instruction2;
                    }
                    enumerator = null;
                    yield return new Instruction_PlayLog("DevilStatueAcceptedItem".Translate(item, reward));
                    yield break;
                }
            }
            yield break;
            yield break;
        }
    }
}