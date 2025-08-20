using System;
using System.Collections.Generic;
using System.Linq;

namespace Ricave.Core
{
    public class UseEffect_Identify : UseEffect
    {
        public override GoodBadNeutral GoodBadNeutral
        {
            get
            {
                return GoodBadNeutral.Good;
            }
        }

        protected UseEffect_Identify()
        {
        }

        public UseEffect_Identify(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            Actor actor = target.Entity as Actor;
            Item item = target.Entity as Item;
            Item item2;
            if (item != null && !item.Identified)
            {
                item2 = item;
            }
            else
            {
                if (actor != null)
                {
                    Item item3 = UsePrompt.Choice as Item;
                    if (item3 != null && actor.Inventory.UnidentifiedItemsInIdentifyOrder.Contains(item3))
                    {
                        item2 = item3;
                        goto IL_0103;
                    }
                }
                if (actor != null)
                {
                    item2 = actor.Inventory.UnidentifiedItemsInIdentifyOrder.Where<Item>((Item x) => x.Spec.Item.IsEquippable).FirstOrDefault<Item>((Item x) => x != usable);
                    if (item2 == null)
                    {
                        item2 = actor.Inventory.UnidentifiedItemsInIdentifyOrder.FirstOrDefault<Item>((Item x) => x != usable);
                    }
                }
                else
                {
                    item2 = null;
                }
            }
        IL_0103:
            if (item2 == null || item2.Identified)
            {
                yield break;
            }
            foreach (Instruction instruction in InstructionSets_Entity.Identify(item2, item2.TurnsLeftToIdentify, true))
            {
                yield return instruction;
            }
            IEnumerator<Instruction> enumerator = null;
            yield break;
            yield break;
        }
    }
}