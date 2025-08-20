using System;
using Ricave.UI;

namespace Ricave.Core
{
    public class UsePrompt_NewRun : UsePrompt
    {
        public override void ShowUsePrompt(Action_Use intendedAction)
        {
            ((Window_NewRun)Get.WindowManager.Open(Get.Window_NewRun, true)).OnAccept = delegate (RunConfig x)
            {
                this.TryDoActionWithChoice(intendedAction, x);
            };
        }
    }
}