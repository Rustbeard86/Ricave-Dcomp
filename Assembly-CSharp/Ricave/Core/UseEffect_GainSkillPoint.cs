using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class UseEffect_GainSkillPoint : UseEffect
    {
        protected UseEffect_GainSkillPoint()
        {
        }

        public UseEffect_GainSkillPoint(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            if (user != null && user.IsPlayerParty)
            {
                yield return new Instruction_ChangePlayerSkillPoints(1);
                yield return new Instruction_PlayLog("YouGainedSkillPoint".Translate());
                yield return new Instruction_VisualEffect(Get.VisualEffect_GainedSkillPoint, user.Position);
            }
            yield break;
        }
    }
}