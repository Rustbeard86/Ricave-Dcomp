using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class UseEffect_DestroySelf : UseEffect
    {
        protected UseEffect_DestroySelf()
        {
        }

        public UseEffect_DestroySelf(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            Entity entity = usable as Entity;
            if (entity != null)
            {
                if (!entity.Spawned)
                {
                    Item item = entity as Item;
                    if (item == null || item.ParentInventory == null)
                    {
                        goto IL_00BC;
                    }
                }
                foreach (Instruction instruction in InstructionSets_Entity.Destroy(entity, null, user))
                {
                    yield return instruction;
                }
                IEnumerator<Instruction> enumerator = null;
            }
        IL_00BC:
            yield break;
            yield break;
        }
    }
}