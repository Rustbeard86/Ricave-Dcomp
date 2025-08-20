using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ricave.Core
{
    public class UseEffect_Extinguish : UseEffect
    {
        protected UseEffect_Extinguish()
        {
        }

        public UseEffect_Extinguish(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            Entity entity = target.Entity;
            Actor targetActor = entity as Actor;
            if (targetActor != null)
            {
                foreach (Condition condition in targetActor.Conditions.All.Where<Condition>((Condition x) => x.Spec == Get.Condition_Burning).ToTemporaryList<Condition>())
                {
                    if (targetActor.Conditions.Contains(condition))
                    {
                        foreach (Instruction instruction in InstructionSets_Misc.RemoveCondition(condition, user != null && user.IsPlayerParty, true))
                        {
                            yield return instruction;
                        }
                        IEnumerator<Instruction> enumerator2 = null;
                    }
                }
                List<Condition>.Enumerator enumerator = default(List<Condition>.Enumerator);
            }
            else
            {
                Structure structure = target.Entity as Structure;
                if (structure != null && structure.Spec.Structure.Extinguishable)
                {
                    Vector3Int? vector3Int = DamageUtility.DeduceImpactSource(user, target, originalTarget);
                    foreach (Instruction instruction2 in InstructionSets_Entity.Destroy(structure, vector3Int, user))
                    {
                        yield return instruction2;
                    }
                    IEnumerator<Instruction> enumerator2 = null;
                }
            }
            yield break;
            yield break;
        }
    }
}