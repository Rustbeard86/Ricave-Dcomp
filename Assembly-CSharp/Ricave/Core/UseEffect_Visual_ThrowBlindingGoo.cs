using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class UseEffect_Visual_ThrowBlindingGoo : UseEffect
    {
        protected UseEffect_Visual_ThrowBlindingGoo()
        {
        }

        public UseEffect_Visual_ThrowBlindingGoo(UseEffectSpec spec)
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
                yield return new Instruction_Sound(Get.Sound_ThrowWeapon, new Vector3?(user.Position), 1f, 1f);
            }
            yield return new Instruction_VisualEffect(Get.VisualEffect_BlindingGooSplat, target.RenderPosition, target.RenderPosition.QuatFromCamera());
            if (user != null && target.IsEntity && target != user)
            {
                yield return new Instruction_StartStrikeAnimation(user, target.Position);
            }
            yield break;
        }
    }
}