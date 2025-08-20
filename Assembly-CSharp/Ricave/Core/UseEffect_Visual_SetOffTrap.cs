using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class UseEffect_Visual_SetOffTrap : UseEffect
    {
        protected UseEffect_Visual_SetOffTrap()
        {
        }

        public UseEffect_Visual_SetOffTrap(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            Entity entity = usable as Entity;
            if (entity != null)
            {
                yield return new Instruction_Sound(Get.Sound_SetOffTrap, new Vector3?(entity.Position), 1f, 1f);
            }
            yield break;
        }
    }
}