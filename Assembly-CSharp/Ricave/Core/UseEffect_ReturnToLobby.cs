using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class UseEffect_ReturnToLobby : UseEffect
    {
        protected UseEffect_ReturnToLobby()
        {
        }

        public UseEffect_ReturnToLobby(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            Actor actor = target.Entity as Actor;
            if (actor == null)
            {
                yield break;
            }
            if (actor.IsPlayerParty)
            {
                yield return new Instruction_SetReturnedToLobby(true);
                foreach (Actor actor2 in Get.PlayerParty.ToTemporaryList<Actor>())
                {
                    if (actor2.Spawned)
                    {
                        foreach (Instruction instruction in InstructionSets_Entity.DeSpawn(actor2, false))
                        {
                            yield return instruction;
                        }
                        IEnumerator<Instruction> enumerator2 = null;
                    }
                }
                List<Actor>.Enumerator enumerator = default(List<Actor>.Enumerator);
            }
            else
            {
                foreach (Instruction instruction2 in InstructionSets_Entity.DeSpawn(actor, false))
                {
                    yield return instruction2;
                }
                IEnumerator<Instruction> enumerator2 = null;
            }
            yield break;
            yield break;
        }
    }
}