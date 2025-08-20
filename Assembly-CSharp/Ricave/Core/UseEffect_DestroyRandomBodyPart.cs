using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ricave.Core
{
    public class UseEffect_DestroyRandomBodyPart : UseEffect
    {
        public override GoodBadNeutral GoodBadNeutral
        {
            get
            {
                return GoodBadNeutral.Bad;
            }
        }

        protected UseEffect_DestroyRandomBodyPart()
        {
        }

        public UseEffect_DestroyRandomBodyPart(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            Actor actor = target.Entity as Actor;
            if (actor == null || !actor.Spawned)
            {
                yield break;
            }
            BodyPart bodyPartToDestroy;
            if (!actor.BodyParts.Where<BodyPart>((BodyPart x) => !x.IsMissing).TryGetRandomElement<BodyPart>(out bodyPartToDestroy))
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
            foreach (Instruction instruction2 in InstructionSets_Actor.DestroyBodyPart(bodyPartToDestroy, vector3Int, user != null && user.IsPlayerParty, true, true))
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