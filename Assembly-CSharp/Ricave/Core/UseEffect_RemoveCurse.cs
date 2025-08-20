using System;
using System.Collections.Generic;
using System.Linq;
using Ricave.UI;

namespace Ricave.Core
{
    public class UseEffect_RemoveCurse : UseEffect
    {
        public override GoodBadNeutral GoodBadNeutral
        {
            get
            {
                return GoodBadNeutral.Good;
            }
        }

        protected UseEffect_RemoveCurse()
        {
        }

        public UseEffect_RemoveCurse(UseEffectSpec spec)
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
            Item itemToUncurse;
            if (item != null && item.Cursed)
            {
                itemToUncurse = item;
            }
            else
            {
                if (actor != null)
                {
                    Item item2 = UsePrompt.Choice as Item;
                    if (item2 != null && actor.Inventory.Contains(item2))
                    {
                        itemToUncurse = item2;
                        goto IL_0139;
                    }
                }
                if (actor != null)
                {
                    if (actor.Inventory.EquippedWeapon != null && actor.Inventory.EquippedWeapon.Cursed)
                    {
                        itemToUncurse = actor.Inventory.EquippedWeapon;
                    }
                    else
                    {
                        itemToUncurse = actor.Inventory.EquippedItems.FirstOrDefault<Item>((Item x) => x.Cursed);
                        if (itemToUncurse == null)
                        {
                            itemToUncurse = actor.Inventory.AllItems.FirstOrDefault<Item>((Item x) => x.Cursed);
                        }
                    }
                }
                else
                {
                    itemToUncurse = null;
                }
            }
        IL_0139:
            if (itemToUncurse == null || !itemToUncurse.Cursed)
            {
                yield break;
            }
            yield return new Instruction_SetCursed(itemToUncurse, false);
            yield return new Instruction_PlayLog("CurseRemoved".Translate(RichText.Label(itemToUncurse)));
            yield break;
        }
    }
}