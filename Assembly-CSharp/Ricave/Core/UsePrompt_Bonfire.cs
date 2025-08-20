using System;
using System.Collections.Generic;
using Ricave.UI;

namespace Ricave.Core
{
    public class UsePrompt_Bonfire : UsePrompt_WheelSelector
    {
        protected override IEnumerable<WheelSelector.Option> GetOptions(Action_Use intendedAction)
        {
            foreach (Item item in Get.NowControlledActor.Inventory.AllItems)
            {
                if (item.Spec.Item.CookedItemSpec != null)
                {
                    Item localItem = item;
                    yield return new WheelSelector.Option(RichText.Label(item), delegate
                    {
                        this.TryDoActionWithChoice(intendedAction, localItem);
                    }, null, localItem);
                }
            }
            List<Item>.Enumerator enumerator = default(List<Item>.Enumerator);
            yield break;
            yield break;
        }
    }
}