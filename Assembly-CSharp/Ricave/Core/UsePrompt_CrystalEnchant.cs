using System;
using System.Collections.Generic;
using Ricave.UI;

namespace Ricave.Core
{
    public class UsePrompt_CrystalEnchant : UsePrompt_WheelSelector
    {
        protected override IEnumerable<WheelSelector.Option> GetOptions(Action_Use intendedAction)
        {
            EntitySpec crystalSpec = ((Item)intendedAction.Usable).Spec;
            foreach (Item item in Get.NowControlledActor.Inventory.AllItems)
            {
                if (crystalSpec == Get.Entity_MinedRedCrystal)
                {
                    if (!item.Spec.Item.IsEquippableWeapon)
                    {
                        continue;
                    }
                    if (!item.UseEffects.AnyOfSpec(Get.UseEffect_Damage))
                    {
                        continue;
                    }
                }
                else if (crystalSpec == Get.Entity_MinedGreenCrystal)
                {
                    if (!item.Spec.Item.IsEquippableNonWeapon || item.Spec.Item.ItemSlot != Get.ItemSlot_Armor)
                    {
                        continue;
                    }
                    if (!item.ConditionsEquipped.AnyOfSpec(Get.Condition_IncomingDamageOffset))
                    {
                        continue;
                    }
                }
                else if (crystalSpec == Get.Entity_MinedBlueCrystal && (!item.Spec.Item.IsEquippable || (item.Spec.Item.ItemSlot != Get.ItemSlot_Weapon && item.Spec.Item.ItemSlot != Get.ItemSlot_Armor)))
                {
                    continue;
                }
                Item localItem = item;
                yield return new WheelSelector.Option(RichText.Label(item), delegate
                {
                    this.TryDoActionWithChoice(intendedAction, localItem);
                }, null, localItem);
            }
            List<Item>.Enumerator enumerator = default(List<Item>.Enumerator);
            yield break;
            yield break;
        }
    }
}