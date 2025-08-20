using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class UseEffect_ResetTurnsCanRewind : UseEffect
    {
        public override GoodBadNeutral GoodBadNeutral
        {
            get
            {
                return GoodBadNeutral.Bad;
            }
        }

        protected UseEffect_ResetTurnsCanRewind()
        {
        }

        public UseEffect_ResetTurnsCanRewind(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            foreach (Instruction instruction in InstructionSets_Misc.ResetTurnsCanRewind())
            {
                yield return instruction;
            }
            IEnumerator<Instruction> enumerator = null;
            yield break;
            yield break;
        }
    }
}