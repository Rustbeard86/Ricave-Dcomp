using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class UseEffect_GiveLoot : UseEffect
    {
        protected UseEffect_GiveLoot()
        {
        }

        public UseEffect_GiveLoot(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            Structure structure = usable as Structure;
            if (structure != null)
            {
                foreach (Entity loot in structure.InnerEntities.ToTemporaryList<Entity>())
                {
                    yield return new Instruction_RemoveFromInnerEntities(loot, structure);
                    Item item = loot as Item;
                    if (item != null && user != null)
                    {
                        foreach (Instruction instruction in InstructionSets_Actor.AddToInventoryOrSpawnNear(user, item))
                        {
                            yield return instruction;
                        }
                        IEnumerator<Instruction> enumerator2 = null;
                        if (ActionUtility.TargetConcernsPlayer(user))
                        {
                            yield return new Instruction_PlayLog("ActorReceivesItem".Translate(user, item));
                        }
                    }
                    else
                    {
                        foreach (Instruction instruction2 in InstructionSets_Entity.Spawn(loot, SpawnPositionFinder.Near(structure.Position, loot, false, false, null), null))
                        {
                            yield return instruction2;
                        }
                        IEnumerator<Instruction> enumerator2 = null;
                    }
                    item = null;
                    loot = null;
                }
                List<Entity>.Enumerator enumerator = default(List<Entity>.Enumerator);
            }
            yield break;
            yield break;
        }
    }
}