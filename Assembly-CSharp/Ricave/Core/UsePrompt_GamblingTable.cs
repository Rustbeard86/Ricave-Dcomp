using System;
using System.Collections.Generic;
using Ricave.UI;

namespace Ricave.Core
{
    public class UsePrompt_GamblingTable : UsePrompt_WheelSelector
    {
        protected override IEnumerable<WheelSelector.Option> GetOptions(Action_Use intendedAction)
        {
            foreach (int num in UsePrompt_GamblingTable.Amounts)
            {
                if (Get.Player.Gold >= num)
                {
                    int localAmount = num;
                    yield return new WheelSelector.Option("BetAmount".Translate(localAmount), delegate
                    {
                        this.TryDoActionWithChoice(intendedAction, localAmount);
                    }, null, null);
                }
            }
            int[] array = null;
            yield break;
        }

        private static readonly int[] Amounts = new int[] { 10, 50, 100 };
    }
}