using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class UseEffect_Visual_HitMiss : UseEffect
    {
        protected UseEffect_Visual_HitMiss()
        {
        }

        public UseEffect_Visual_HitMiss(UseEffectSpec spec)
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
                if (user.IsNowControlledActor)
                {
                    yield return new Instruction_VisualEffect(Get.VisualEffect_PlayerHits_Miss, user.Position);
                }
                if (target.IsEntity && target != user)
                {
                    yield return new Instruction_StartStrikeAnimation(user, target.Position);
                }
            }
            yield return new Instruction_Sound(Get.Sound_Miss, new Vector3?(target.Position), 1f, 1f);
            yield break;
        }
    }
}