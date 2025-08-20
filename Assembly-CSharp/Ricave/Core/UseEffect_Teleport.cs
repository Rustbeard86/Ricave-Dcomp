using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class UseEffect_Teleport : UseEffect
    {
        protected UseEffect_Teleport()
        {
        }

        public UseEffect_Teleport(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            if (user == null || !target.IsLocation || !Get.CellsInfo.CanPassThrough(target.Position))
            {
                yield break;
            }
            foreach (Instruction instruction in InstructionSets_Entity.Move(user, target.Position, true, true))
            {
                yield return instruction;
            }
            IEnumerator<Instruction> enumerator = null;
            Spell spell = usable as Spell;
            if (spell == null || !spell.Spec.IsAbilityLike)
            {
                yield return new Instruction_Sound(Get.Sound_Teleport, new Vector3?(target.Position), 1f, 1f);
            }
            yield break;
            yield break;
        }
    }
}