using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class UseEffect_RemoveOneFromInventory : UseEffect
    {
        protected UseEffect_RemoveOneFromInventory()
        {
        }

        public UseEffect_RemoveOneFromInventory(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            Item item = usable as Item;
            if (item != null)
            {
                foreach (Instruction instruction in InstructionSets_Actor.RemoveOneFromInventory(item))
                {
                    yield return instruction;
                }
                IEnumerator<Instruction> enumerator = null;
            }
            yield break;
            yield break;
        }
    }
}