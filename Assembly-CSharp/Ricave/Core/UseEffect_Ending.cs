using System;
using System.Collections.Generic;
using Ricave.Rendering;

namespace Ricave.Core
{
    public class UseEffect_Ending : UseEffect
    {
        protected UseEffect_Ending()
        {
        }

        public UseEffect_Ending(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            yield return new Instruction_Immediate(delegate
            {
                Get.Progress.OnFinishedGame();
            });
            yield return new Instruction_Immediate(delegate
            {
                Get.TextSequenceDrawer.StartFadeOut(TextSequence.Ending);
            });
            yield break;
        }
    }
}