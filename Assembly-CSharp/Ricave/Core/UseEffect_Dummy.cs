using System;
using System.Collections.Generic;
using System.Linq;

namespace Ricave.Core
{
    public class UseEffect_Dummy : UseEffect
    {
        protected UseEffect_Dummy()
        {
        }

        public UseEffect_Dummy(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            return Enumerable.Empty<Instruction>();
        }
    }
}