using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class UseEffect_SurgicalBed : UseEffect
    {
        public override GoodBadNeutral GoodBadNeutral
        {
            get
            {
                return GoodBadNeutral.Good;
            }
        }

        protected UseEffect_SurgicalBed()
        {
        }

        public UseEffect_SurgicalBed(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            if (user == null)
            {
                yield break;
            }
            Actor actor = target.Entity as Actor;
            if (actor == null)
            {
                yield break;
            }
            foreach (BodyPart bodyPart in actor.BodyParts)
            {
                if (bodyPart.IsMissing)
                {
                    foreach (Instruction instruction in InstructionSets_Actor.RestoreMissingBodyPart(bodyPart, user != null && user.IsPlayerParty))
                    {
                        yield return instruction;
                    }
                    IEnumerator<Instruction> enumerator2 = null;
                }
            }
            List<BodyPart>.Enumerator enumerator = default(List<BodyPart>.Enumerator);
            yield break;
            yield break;
        }
    }
}