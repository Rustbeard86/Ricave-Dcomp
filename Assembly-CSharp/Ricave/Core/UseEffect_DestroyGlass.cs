using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class UseEffect_DestroyGlass : UseEffect
    {
        protected UseEffect_DestroyGlass()
        {
        }

        public UseEffect_DestroyGlass(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            Entity entity = target.Entity;
            Structure structure = entity as Structure;
            if (structure == null || !structure.Spec.Structure.IsGlass || !structure.Spawned)
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
            foreach (Instruction instruction2 in InstructionSets_Entity.Destroy(structure, vector3Int, user))
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