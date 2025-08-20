using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class UseEffect_RemoveAllUseEffects : UseEffect
    {
        protected UseEffect_RemoveAllUseEffects()
        {
        }

        public UseEffect_RemoveAllUseEffects(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            foreach (UseEffect useEffect in usable.UseEffects.All.ToTemporaryList<UseEffect>())
            {
                if (usable.UseEffects.Contains(useEffect))
                {
                    foreach (Instruction instruction in InstructionSets_Misc.RemoveUseEffect(useEffect, false, false))
                    {
                        yield return instruction;
                    }
                    IEnumerator<Instruction> enumerator2 = null;
                }
            }
            List<UseEffect>.Enumerator enumerator = default(List<UseEffect>.Enumerator);
            yield break;
            yield break;
        }
    }
}