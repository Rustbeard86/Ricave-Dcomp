using System;
using System.Collections.Generic;
using Ricave.UI;

namespace Ricave.Core
{
    public class UsePrompt_UpgradeOrb : UsePrompt_WheelSelector
    {
        protected override IEnumerable<WheelSelector.Option> GetOptions(Action_Use intendedAction)
        {
            yield return new WheelSelector.Option("UpgradeOrb_MaxHP".Translate(1), delegate
            {
                this.TryDoActionWithChoice(intendedAction, 0);
            }, null, null);
            yield return new WheelSelector.Option("UpgradeOrb_Crit".Translate(2), delegate
            {
                this.TryDoActionWithChoice(intendedAction, 1);
            }, null, null);
            yield return new WheelSelector.Option("UpgradeOrb_Miss".Translate(2), delegate
            {
                this.TryDoActionWithChoice(intendedAction, 2);
            }, null, null);
            yield return new WheelSelector.Option("UpgradeOrb_Fire".Translate(50), delegate
            {
                this.TryDoActionWithChoice(intendedAction, 3);
            }, null, null);
            yield break;
        }
    }
}