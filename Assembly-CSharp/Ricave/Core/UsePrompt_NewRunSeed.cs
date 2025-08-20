using System;
using Ricave.UI;

namespace Ricave.Core
{
    public class UsePrompt_NewRunSeed : UsePrompt
    {
        public override void ShowUsePrompt(Action_Use intendedAction)
        {
            ((Window_NewRunWithSeed)Get.WindowManager.Open(Get.Window_NewRunWithSeed, true)).OnAccept = delegate (string requestedSeed)
            {
                int num;
                if (!int.TryParse(requestedSeed, out num))
                {
                    num = requestedSeed.StableHashCode();
                }
                this.TryDoActionWithChoice(intendedAction, new RunConfig(Get.Run_Main9, num, Get.Difficulty_Normal, null, "Current", null, true, null, false, false, null, Get.Progress.LastChosenPartyMember));
            };
        }
    }
}