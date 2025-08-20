using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class UseEffect_Visual_BowMiss : UseEffect
    {
        protected UseEffect_Visual_BowMiss()
        {
        }

        public UseEffect_Visual_BowMiss(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            if (user != null)
            {
                yield return new Instruction_Sound(Get.Sound_UseBow, new Vector3?(user.Position), 1f, 1f);
                if (target.IsEntity && target != user)
                {
                    yield return new Instruction_StartStrikeAnimation(user, target.Position);
                }
                Instruction_VisualEffect instruction_VisualEffect = UseEffect_Visual_UseBow.LineBetween(user, usable, target, true);
                if (instruction_VisualEffect != null)
                {
                    yield return instruction_VisualEffect;
                }
            }
            yield break;
        }
    }
}