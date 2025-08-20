using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class UseEffect_Visual_PullLever : UseEffect
    {
        protected UseEffect_Visual_PullLever()
        {
        }

        public UseEffect_Visual_PullLever(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            Structure lever = usable as Structure;
            if (lever == null)
            {
                yield break;
            }
            yield return new Instruction_Sound(Get.Sound_PullLever, new Vector3?(lever.Position), 1f, 1f);
            yield return new Instruction_Immediate(delegate
            {
                Get.VisualEffectsManager.DoOneShot(Get.VisualEffect_PullLeverAnimation, lever.Position, lever.Rotation, Vector3.one, null, null, null);
            });
            yield break;
        }
    }
}