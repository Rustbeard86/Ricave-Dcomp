using System;
using System.Collections.Generic;
using Ricave.UI;

namespace Ricave.Core
{
    public class UsePrompt_Fountain : UsePrompt_Confirm
    {
        protected override IEnumerable<WheelSelector.Option> GetOptions(Action_Use intendedAction)
        {
            foreach (WheelSelector.Option option in base.GetOptions(intendedAction))
            {
                yield return option;
            }
            IEnumerator<WheelSelector.Option> enumerator = null;
            yield return new WheelSelector.Option("ThrowGold".Translate(50), delegate
            {
                if (Get.Player.Gold >= 50)
                {
                    this.TryDoActionWithChoice(intendedAction, true);
                    return;
                }
                Get.Sound_Click.PlayOneShot(null, 1f, 1f);
            }, null, null);
            yield break;
            yield break;
        }
    }
}