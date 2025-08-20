using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class UseEffect_Visual_Eat : UseEffect
    {
        protected UseEffect_Visual_Eat()
        {
        }

        public UseEffect_Visual_Eat(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            yield return new Instruction_Sound(Get.Sound_Eat, new Vector3?(user.Position), 1f, 1f);
            yield break;
        }
    }
}