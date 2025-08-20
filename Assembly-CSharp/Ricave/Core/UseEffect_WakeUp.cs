using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class UseEffect_WakeUp : UseEffect
    {
        public string Reason
        {
            get
            {
                return this.reason;
            }
        }

        protected UseEffect_WakeUp()
        {
        }

        public UseEffect_WakeUp(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
            ((UseEffect_WakeUp)clone).reason = this.reason;
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            if (target.IsEntity && target.Spawned)
            {
                Actor actor = target.Entity as Actor;
                if (actor != null)
                {
                    foreach (Instruction instruction in InstructionSets_Actor.WakeUp(actor, this.reason))
                    {
                        yield return instruction;
                    }
                    IEnumerator<Instruction> enumerator = null;
                    yield break;
                }
            }
            yield break;
            yield break;
        }

        [Saved]
        [Translatable]
        private string reason;
    }
}