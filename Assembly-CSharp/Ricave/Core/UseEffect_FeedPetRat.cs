using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class UseEffect_FeedPetRat : UseEffect
    {
        public override GoodBadNeutral GoodBadNeutral
        {
            get
            {
                return GoodBadNeutral.Good;
            }
        }

        protected UseEffect_FeedPetRat()
        {
        }

        public UseEffect_FeedPetRat(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            yield return new Instruction_ChangePetRatSatiation(1);
            yield break;
        }
    }
}