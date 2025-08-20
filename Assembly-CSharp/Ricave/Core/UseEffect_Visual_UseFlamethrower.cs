using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class UseEffect_Visual_UseFlamethrower : UseEffect
    {
        protected UseEffect_Visual_UseFlamethrower()
        {
        }

        public UseEffect_Visual_UseFlamethrower(UseEffectSpec spec)
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
                yield return new Instruction_Sound(Get.Sound_Flamethrower, new Vector3?(user.Position), 1f, 1f);
                Vector3Int sourcePos = UseEffect_Sound.GetSourcePos(user, usable, target);
                Vector3 vector = (sourcePos + target.Position) / 2f;
                yield return new Instruction_VisualEffect(Get.VisualEffect_Flamethrower, vector, (sourcePos == target.Position) ? Quaternion.identity : Quaternion.LookRotation((target.Position - sourcePos).normalized));
                if (target.IsEntity && target != user)
                {
                    yield return new Instruction_StartStrikeAnimation(user, target.Position);
                }
            }
            yield break;
        }
    }
}