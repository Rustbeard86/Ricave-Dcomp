using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class UseEffect_RestoreMissingBodyParts : UseEffect
    {
        public override GoodBadNeutral GoodBadNeutral
        {
            get
            {
                return GoodBadNeutral.Good;
            }
        }

        protected UseEffect_RestoreMissingBodyParts()
        {
        }

        public UseEffect_RestoreMissingBodyParts(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            Actor actor = target.Entity as Actor;
            if (actor == null)
            {
                yield break;
            }
            foreach (BodyPart bodyPart in actor.BodyParts)
            {
                if (bodyPart.IsMissing)
                {
                    foreach (Instruction instruction in InstructionSets_Actor.RestoreMissingBodyPart(bodyPart, false))
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