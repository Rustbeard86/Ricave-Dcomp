using System;
using System.Collections.Generic;
using Ricave.UI;

namespace Ricave.Core
{
    public class UsePrompt_Identify : UsePrompt_WheelSelector
    {
        protected override IEnumerable<WheelSelector.Option> GetOptions(Action_Use intendedAction)
        {
            Actor actor = intendedAction.On.Entity as Actor;
            if (actor == null)
            {
                yield break;
            }
            foreach (Item item in actor.Inventory.UnidentifiedItemsInIdentifyOrder)
            {
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