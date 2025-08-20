using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class UseEffect_Visual_Hit : UseEffect
    {
        protected UseEffect_Visual_Hit()
        {
        }

        public UseEffect_Visual_Hit(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
            ((UseEffect_Visual_Hit)clone).playSound = this.playSound;
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            if (user != null)
            {
                if (user.IsNowControlledActor)
                {
                    yield return new Instruction_VisualEffect(Get.VisualEffect_PlayerHits, user.Position);
                }
                if (target.IsEntity && target != user)
                {
                    yield return new Instruction_StartStrikeAnimation(user, target.Position);
                }
            }
            if (this.playSound)
            {
                yield return new Instruction_Sound(UseEffect_Visual_Hit.GetHitSound(target), new Vector3?(target.Position), (user.ChargedAttack > 0) ? 0.825f : 1f, 1f);
            }
            yield break;
        }

        public static SoundSpec GetHitSound(Target target)
        {
            return Get.Sound_Hit;
        }

        [Saved(true, false)]
        private bool playSound = true;
    }
}