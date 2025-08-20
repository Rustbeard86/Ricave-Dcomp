using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class UseEffect_Visual_SetTrap : UseEffect
    {
        protected UseEffect_Visual_SetTrap()
        {
        }

        public UseEffect_Visual_SetTrap(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            yield return new Instruction_Sound(Get.Sound_SetTrap, new Vector3?(target.Position), 1f, 1f);
            yield break;
        }
    }
}