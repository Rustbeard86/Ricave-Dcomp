using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class UseEffect_Visual_UseFlintlockPistol : UseEffect
    {
        protected UseEffect_Visual_UseFlintlockPistol()
        {
        }

        public UseEffect_Visual_UseFlintlockPistol(UseEffectSpec spec)
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
                yield return new Instruction_Sound(Get.Sound_UseFlintlockPistol, new Vector3?(user.Position), 1f, 1f);
                yield return new Instruction_VisualEffect(Get.VisualEffect_Gunshot, user.Position, (user.Position == target.Position) ? Quaternion.identity : Quaternion.LookRotation((target.Position - user.Position).normalized));
                if (target.IsEntity && target != user)
                {
                    yield return new Instruction_StartStrikeAnimation(user, target.Position);
                }
                Instruction_VisualEffect instruction_VisualEffect = UseEffect_Visual_UseBow.LineBetween(user, usable, target, false);
                if (instruction_VisualEffect != null)
                {
                    yield return instruction_VisualEffect;
                }
            }
            yield break;
        }
    }
}