using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class UseEffect_FinishTutorial : UseEffect
    {
        protected UseEffect_FinishTutorial()
        {
        }

        public UseEffect_FinishTutorial(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            yield return new Instruction_Sound(Get.Sound_Ending, null, 1f, 1f);
            yield return new Instruction_Immediate(delegate
            {
                Get.ScreenFader.FadeOutAndExecute(delegate
                {
                    Get.LessonManager.FinishIfCurrent(Get.Lesson_Lever);
                    Get.LessonManager.OnTutorialAreaPassed();
                    Get.Progress.ApplyCurrentRunProgress();
                    Root.LoadPlayScene(RootOnSceneChanged.StartNewRun(new RunConfig(Get.Run_Lobby, Rand.Int, Get.Difficulty_Normal, null, "Current", null, false, null, false, false, null, null)));
                }, new float?(6f), false, true, false);
            });
            if (!Get.Achievement_FinishTutorial.IsCompleted)
            {
                yield return new Instruction_CompleteAchievement(Get.Achievement_FinishTutorial);
            }
            yield break;
        }
    }
}