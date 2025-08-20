using System;
using System.Collections.Generic;
using Ricave.UI;

namespace Ricave.Core
{
    public abstract class UsePrompt_WheelSelector : UsePrompt
    {
        protected abstract IEnumerable<WheelSelector.Option> GetOptions(Action_Use intendedAction);

        protected virtual string GetPostProcessedDescription()
        {
            return this.description;
        }

        public override void ShowUsePrompt(Action_Use intendedAction)
        {
            List<WheelSelector.Option> list = new List<WheelSelector.Option>();
            list.AddRange(this.GetOptions(intendedAction));
            list.Add(new WheelSelector.Option("Leave".Translate(), delegate
            {
                Get.Sound_Click.PlayOneShot(null, 1f, 1f);
            }, null, null));
            Get.UI.OpenWheelSelector(list, intendedAction.Usable, this.GetPostProcessedDescription(), false, false, false);
        }

        [Saved]
        [Translatable]
        protected string description;
    }
}