using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class UseEffect_SetSpiritFree : UseEffect
    {
        public override GoodBadNeutral GoodBadNeutral
        {
            get
            {
                return GoodBadNeutral.Good;
            }
        }

        protected UseEffect_SetSpiritFree()
        {
        }

        public UseEffect_SetSpiritFree(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
        }

        public override bool PreventEntireUse(Actor user, IUsable usable, Target target, StringSlot outReason = null)
        {
            Actor actor;
            if (!this.TryFindSpirit(user, out actor))
            {
                if (outReason != null)
                {
                    outReason.Set("NeedSpirit".Translate());
                }
                return true;
            }
            return false;
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            Actor spirit;
            if (user == null || !this.TryFindSpirit(user, out spirit))
            {
                yield break;
            }
            foreach (Instruction instruction in InstructionSets_Entity.DeSpawn(spirit, false))
            {
                yield return instruction;
            }
            IEnumerator<Instruction> enumerator = null;
            List<Entity> rewards = new List<Entity>();
            Item item = ItemGenerator.Gold(15);
            rewards.Add(item);
            foreach (Instruction instruction2 in InstructionSets_Actor.AddToInventoryOrSpawnNear(user, item))
            {
                yield return instruction2;
            }
            enumerator = null;
            if (!Get.RunConfig.ProgressDisabled)
            {
                Item item2 = ItemGenerator.Stardust(5);
                rewards.Add(item2);
                foreach (Instruction instruction3 in InstructionSets_Actor.AddToInventoryOrSpawnNear(user, item2))
                {
                    yield return instruction3;
                }
                enumerator = null;
            }
            yield return new Instruction_ChangeSpiritsSetFree(1);
            yield return new Instruction_PlayLog("SetSpiritFree".Translate(spirit, StringUtility.ToCommaListAnd(rewards)));
            if (!Get.Achievement_SetSpiritFree.IsCompleted)
            {
                yield return new Instruction_CompleteAchievement(Get.Achievement_SetSpiritFree);
            }
            yield break;
            yield break;
        }

        private bool TryFindSpirit(Actor user, out Actor spirit)
        {
            if (!user.IsPlayerParty)
            {
                spirit = null;
                return false;
            }
            List<Actor> followers = Get.Player.Followers;
            for (int i = 0; i < followers.Count; i++)
            {
                Actor actor = followers[i];
                if (actor.Spec == Get.Entity_Spirit && Get.Player.FollowerCanFollowPlayerIntoNextFloor(actor, true))
                {
                    spirit = actor;
                    return true;
                }
            }
            spirit = null;
            return false;
        }
    }
}