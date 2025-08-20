using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class UseEffect_IncrementChestsOpened : UseEffect
    {
        protected UseEffect_IncrementChestsOpened()
        {
        }

        public UseEffect_IncrementChestsOpened(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            if (user != null && user.IsPlayerParty)
            {
                yield return new Instruction_ChangeChestsOpened(1);
            }
            yield break;
        }
    }
}