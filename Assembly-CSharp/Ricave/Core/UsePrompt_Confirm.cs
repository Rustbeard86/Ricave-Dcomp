using System;
using System.Collections.Generic;
using Ricave.UI;

namespace Ricave.Core
{
    public class UsePrompt_Confirm : UsePrompt_WheelSelector
    {
        protected override IEnumerable<WheelSelector.Option> GetOptions(Action_Use intendedAction)
        {
            Func<Action> <> 9__1;
            yield return new WheelSelector.Option(this.confirmLabel, delegate
            {
                Func<Action> func;
                if ((func = <> 9__1) == null)
                {
                    func = (<> 9__1 = () => intendedAction);
                }
                ActionViaInterfaceHelper.TryDo(func);
            }, null, null);
            yield break;
        }

        [Saved]
        [Translatable]
        private string confirmLabel;
    }
}