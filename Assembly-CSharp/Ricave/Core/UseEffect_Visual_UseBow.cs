using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class UseEffect_Visual_UseBow : UseEffect
    {
        protected UseEffect_Visual_UseBow()
        {
        }

        public UseEffect_Visual_UseBow(UseEffectSpec spec)
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
                yield return new Instruction_Sound(Get.Sound_UseBow, new Vector3?(user.Position), 1f, 1f);
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

        public static Instruction_VisualEffect LineBetween(Actor user, IUsable usable, Target target, bool miss = false)
        {
            return UseEffect_Visual_UseBow.LineBetween(UseEffect_Sound.GetSourcePos(user, usable, target), target, miss);
        }

        public static Instruction_VisualEffect LineBetween(Vector3Int source, Target target, bool miss = false)
        {
            if (source == target.Position)
            {
                return null;
            }
            Vector3 vector = target.Position;
            if (miss)
            {
                vector += Calc.RandomPerpendicularVector(vector - source) * 0.4f;
            }
            Vector3 vector2 = (source + vector) / 2f;
            return new Instruction_VisualEffect(Get.VisualEffect_LineAnimation, vector2, Quaternion.LookRotation((vector - source).normalized), new Vector3(1f, 1f, (source - vector).magnitude));
        }
    }
}