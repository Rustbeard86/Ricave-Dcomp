using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class UseEffect_DropLoot : UseEffect
    {
        protected UseEffect_DropLoot()
        {
        }

        public UseEffect_DropLoot(UseEffectSpec spec)
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
                Actor mimic = null;
                Structure structure = entity as Structure;
                if (structure != null)
                {
                    foreach (Entity entity2 in structure.InnerEntities)
                    {
                        if (entity2.Spec == Get.Entity_Mimic)
                        {
                            mimic = (Actor)entity2;
                            break;
                        }
                    }
                }
                foreach (Instruction instruction in InstructionSets_Entity.DropLoot(entity))
                {
                    yield return instruction;
                }
                IEnumerator<Instruction> enumerator2 = null;
                if (mimic != null && mimic.Spawned)
                {
                    yield return new Instruction_Awareness_PreAction(mimic);
                    yield return new Instruction_Awareness_PostAction(mimic);
                    if (user != null && user.IsNowControlledActor)
                    {
                        foreach (Instruction instruction2 in InstructionSets_Misc.ResetTurnsCanRewind())
                        {
                            yield return instruction2;
                        }
                        enumerator2 = null;
                    }
                }
                mimic = null;
            }
            yield break;
            yield break;
        }
    }
}