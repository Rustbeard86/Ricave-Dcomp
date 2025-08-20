using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class UseEffect_Destroy : UseEffect
    {
        protected UseEffect_Destroy()
        {
        }

        public UseEffect_Destroy(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            if (!target.IsEntity)
            {
                yield break;
            }
            IEnumerator<Instruction> enumerator;
            if (user != null)
            {
                foreach (Instruction instruction in InstructionSets_Entity.PreViolentlyAttackedBy(target.Entity, user))
                {
                    yield return instruction;
                }
                enumerator = null;
            }
            Vector3Int? vector3Int = DamageUtility.DeduceImpactSource(user, target, originalTarget);
            foreach (Instruction instruction2 in InstructionSets_Entity.Destroy(target.Entity, vector3Int, user))
            {
                yield return instruction2;
            }
            enumerator = null;
            if (user != null)
            {
                foreach (Instruction instruction3 in InstructionSets_Entity.PostViolentlyAttackedBy(target.Entity, user))
                {
                    yield return instruction3;
                }
                enumerator = null;
            }
            yield break;
            yield break;
        }
    }
}