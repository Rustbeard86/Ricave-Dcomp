using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class UseEffect_ClearAttackTarget : UseEffect
    {
        protected UseEffect_ClearAttackTarget()
        {
        }

        public UseEffect_ClearAttackTarget(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            Actor targetActor = target.Entity as Actor;
            if (targetActor == null)
            {
                yield break;
            }
            yield return new Instruction_SetAttackTarget(targetActor, null);
            yield return new Instruction_Awareness_Clear(targetActor);
            yield break;
        }
    }
}