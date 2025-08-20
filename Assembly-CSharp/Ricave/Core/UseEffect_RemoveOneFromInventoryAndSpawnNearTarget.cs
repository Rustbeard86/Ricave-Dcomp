using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class UseEffect_RemoveOneFromInventoryAndSpawnNearTarget : UseEffect
    {
        protected UseEffect_RemoveOneFromInventoryAndSpawnNearTarget()
        {
        }

        public UseEffect_RemoveOneFromInventoryAndSpawnNearTarget(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            Item usableItem = usable as Item;
            if (usableItem != null)
            {
                Item toSpawn = null;
                IEnumerator<Instruction> enumerator;
                if (usableItem.StackCount <= 1)
                {
                    foreach (Instruction instruction in InstructionSets_Actor.RemoveFromInventory(usableItem))
                    {
                        yield return instruction;
                    }
                    enumerator = null;
                    toSpawn = usableItem;
                }
                else
                {
                    toSpawn = usableItem.SplitOff(1);
                    yield return new Instruction_ChangeStackCount(usableItem, -1);
                }
                foreach (Instruction instruction2 in InstructionSets_Entity.Spawn(toSpawn, SpawnPositionFinder.Near(target.Position, toSpawn, false, false, null), null))
                {
                    yield return instruction2;
                }
                enumerator = null;
                yield return new Instruction_StartItemJumpAnimation(toSpawn);
                toSpawn = null;
            }
            yield break;
            yield break;
        }
    }
}