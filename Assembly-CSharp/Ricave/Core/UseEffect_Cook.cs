using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class UseEffect_Cook : UseEffect
    {
        public override GoodBadNeutral GoodBadNeutral
        {
            get
            {
                return GoodBadNeutral.Neutral;
            }
        }

        protected UseEffect_Cook()
        {
        }

        public UseEffect_Cook(UseEffectSpec spec)
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
                if (item != null && user.Inventory.Contains(item) && item.Spec.Item.CookedItemSpec != null)
                {
                    foreach (Instruction instruction in InstructionSets_Actor.RemoveOneFromInventory(item))
                    {
                        yield return instruction;
                    }
                    IEnumerator<Instruction> enumerator = null;
                    Item cooked = Maker.Make<Item>(item.Spec.Item.CookedItemSpec, null, false, false, true);
                    foreach (Instruction instruction2 in InstructionSets_Actor.AddToInventoryOrSpawnNear(user, cooked))
                    {
                        yield return instruction2;
                    }
                    enumerator = null;
                    yield return new Instruction_PlayLog("ReceivedItem".Translate(cooked));
                    yield break;
                }
            }
            yield break;
            yield break;
        }
    }
}