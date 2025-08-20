using System;
using Ricave.UI;

namespace Ricave.Core
{
    public class UsePrompt_TrainingRoom : UsePrompt
    {
        public override void ShowUsePrompt(Action_Use intendedAction)
        {
            ((Window_TrainingRoom)Get.WindowManager.Open(Get.Window_TrainingRoom, true)).OnAccept = delegate (EntitySpec actorSpec, bool boss)
            {
                this.TryDoActionWithChoice(intendedAction, new RunConfig(Get.Run_Training, Rand.Int, Get.Difficulty_Normal, Get.Progress.LastChosenClass, "Current", null, true, actorSpec, boss, false, null, null));
            };
        }
    }
}