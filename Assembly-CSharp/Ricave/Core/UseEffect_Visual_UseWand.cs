using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class UseEffect_Visual_UseWand : UseEffect
    {
        protected UseEffect_Visual_UseWand()
        {
        }

        public UseEffect_Visual_UseWand(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            yield return new Instruction_Sound(Get.Sound_UseWand, new Vector3?(user.Position), 1f, 1f);
            yield return new Instruction_VisualEffect(Get.VisualEffect_Magic, target.Position);
            if (user != null && target.IsEntity && target != user)
            {
                yield return new Instruction_StartStrikeAnimation(user, target.Position);
            }
            yield break;
        }
    }
}