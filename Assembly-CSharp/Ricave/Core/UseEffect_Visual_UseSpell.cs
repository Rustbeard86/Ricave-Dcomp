using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class UseEffect_Visual_UseSpell : UseEffect
    {
        protected UseEffect_Visual_UseSpell()
        {
        }

        public UseEffect_Visual_UseSpell(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            Spell spell = usable as Spell;
            if (spell != null && spell.Spec.IsAbilityLike)
            {
                yield return new Instruction_Sound(Get.Sound_UseRecipe, new Vector3?(target.Position), 1.7f, 1f);
                yield return new Instruction_VisualEffect(Get.VisualEffect_Ability, target.Position);
            }
            else
            {
                yield return new Instruction_Sound(Get.Sound_UseSpell, new Vector3?(target.Position), 1f, 1f);
                yield return new Instruction_VisualEffect(Get.VisualEffect_Magic, target.Position);
            }
            if (user != null && target.IsEntity && target != user)
            {
                yield return new Instruction_StartStrikeAnimation(user, target.Position);
            }
            yield break;
        }
    }
}