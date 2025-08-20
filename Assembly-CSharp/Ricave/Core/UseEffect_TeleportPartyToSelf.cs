using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class UseEffect_TeleportPartyToSelf : UseEffect
    {
        protected UseEffect_TeleportPartyToSelf()
        {
        }

        public UseEffect_TeleportPartyToSelf(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            if (user == null || !user.Spawned)
            {
                yield break;
            }
            foreach (Actor partyMember in Get.Player.Party.ToTemporaryList<Actor>())
            {
                if (partyMember.Spawned && partyMember != user)
                {
                    Vector3Int vector3Int = SpawnPositionFinder.Near(user.Position, partyMember, false, false, null);
                    foreach (Instruction instruction in InstructionSets_Entity.Move(partyMember, vector3Int, true, true))
                    {
                        yield return instruction;
                    }
                    IEnumerator<Instruction> enumerator2 = null;
                    yield return new Instruction_VisualEffect(Get.VisualEffect_Magic, partyMember.Position);
                    partyMember = null;
                }
            }
            List<Actor>.Enumerator enumerator = default(List<Actor>.Enumerator);
            foreach (Actor partyMember in Get.Player.Followers.ToTemporaryList<Actor>())
            {
                if (partyMember.Spawned && partyMember != user)
                {
                    Vector3Int vector3Int2 = SpawnPositionFinder.Near(user.Position, partyMember, false, false, null);
                    foreach (Instruction instruction2 in InstructionSets_Entity.Move(partyMember, vector3Int2, true, true))
                    {
                        yield return instruction2;
                    }
                    IEnumerator<Instruction> enumerator2 = null;
                    yield return new Instruction_VisualEffect(Get.VisualEffect_Magic, partyMember.Position);
                    partyMember = null;
                }
            }
            enumerator = default(List<Actor>.Enumerator);
            yield return new Instruction_Sound(Get.Sound_Teleport, new Vector3?(user.Position), 1f, 1f);
            yield break;
            yield break;
        }
    }
}