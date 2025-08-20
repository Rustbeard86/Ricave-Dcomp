using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class UseEffect_Visual_BigMagnet : UseEffect
    {
        protected UseEffect_Visual_BigMagnet()
        {
        }

        public UseEffect_Visual_BigMagnet(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            Structure bigMagnet = usable as Structure;
            if (bigMagnet == null)
            {
                yield break;
            }
            yield return new Instruction_Sound(Get.Sound_BigMagnetPull, new Vector3?(target.Position), 1f, 1f);
            yield return new Instruction_VisualEffect(Get.VisualEffect_BigMagnetPull, (target.RenderPosition + bigMagnet.RenderPosition) / 2f, Quaternion.LookRotation((target.RenderPosition - bigMagnet.RenderPosition).normalized));
            yield break;
        }
    }
}