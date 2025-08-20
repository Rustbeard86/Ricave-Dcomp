using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class UseEffect_CutsceneTrigger : UseEffect
    {
        protected UseEffect_CutsceneTrigger()
        {
        }

        public UseEffect_CutsceneTrigger(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            yield return new Instruction_SetFinishedRun(true);
            yield return new Instruction_Immediate(delegate
            {
                Get.ScreenFader.FadeOutAndExecute(delegate
                {
                    Get.Progress.ApplyCurrentRunProgress();
                    Root.LoadPlayScene(RootOnSceneChanged.StartNewRun(new RunConfig(Get.Run_Lobby, Rand.Int, Get.Difficulty_Normal, null, "Current", null, false, null, false, false, null, null)));
                }, new float?(0.62f), false, true, false);
            });
            yield break;
        }
    }
}