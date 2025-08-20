using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class UseEffect_Visual_FadeFromBlack : UseEffect
    {
        protected UseEffect_Visual_FadeFromBlack()
        {
        }

        public UseEffect_Visual_FadeFromBlack(UseEffectSpec spec)
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
                Get.ScreenFader.FadeFromBlack();
            });
            yield break;
        }
    }
}