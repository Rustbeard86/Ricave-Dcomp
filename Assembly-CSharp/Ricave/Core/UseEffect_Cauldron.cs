using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class UseEffect_Cauldron : UseEffect
    {
        protected UseEffect_Cauldron()
        {
        }

        public UseEffect_Cauldron(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            if (user != null && user.IsNowControlledActor)
            {
                Item item = UsePrompt.Choice as Item;
                if (item != null && user.Inventory.Contains(item))
                {
                    foreach (Instruction instruction in InstructionSets_Actor.RemoveOneFromInventory(item))
                    {
                        yield return instruction;
                    }
                    IEnumerator<Instruction> enumerator = null;
                    Rand.PushState(Calc.CombineHashes<int, int, int>(Get.WorldSeed, usable.MyStableHash, 813256473));
                    Item scroll = ItemGenerator.Scroll(true);
                    Rand.PopState();
                    if (!scroll.Identified)
                    {
                        foreach (Instruction instruction2 in InstructionSets_Entity.Identify(scroll, scroll.TurnsLeftToIdentify, false))
                        {
                            yield return instruction2;
                        }
                        enumerator = null;
                    }
                    foreach (Instruction instruction3 in InstructionSets_Actor.AddToInventoryOrSpawnNear(user, scroll))
                    {
                        yield return instruction3;
                    }
                    enumerator = null;
                    yield return new Instruction_PlayLog("ReceivedItem".Translate(scroll));
                    yield break;
                }
            }
            yield break;
            yield break;
        }
    }
}