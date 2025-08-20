using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class UseEffect_DeSpawnSelf : UseEffect
    {
        protected UseEffect_DeSpawnSelf()
        {
        }

        public UseEffect_DeSpawnSelf(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
            ((UseEffect_DeSpawnSelf)clone).fadeOut = this.fadeOut;
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            Entity entity = usable as Entity;
            if (entity != null && entity.Spawned)
            {
                foreach (Instruction instruction in InstructionSets_Entity.DeSpawn(entity, this.fadeOut))
                {
                    yield return instruction;
                }
                IEnumerator<Instruction> enumerator = null;
            }
            yield break;
            yield break;
        }

        [Saved]
        private bool fadeOut;
    }
}