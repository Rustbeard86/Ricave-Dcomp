using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class UseEffect_Visual_UseStaircase : UseEffect
    {
        protected UseEffect_Visual_UseStaircase()
        {
        }

        public UseEffect_Visual_UseStaircase(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            Structure staircase = usable as Structure;
            if (staircase == null)
            {
                yield break;
            }
            yield return new Instruction_Sound(Get.Sound_UseStaircase, new Vector3?(staircase.Position), 1f, 1f);
            yield return new Instruction_Immediate(delegate
            {
                Get.CameraEffects.StartWalkDownStaircaseAnimation(staircase.Position, staircase.DirectionCardinal);
            });
            yield break;
        }
    }
}