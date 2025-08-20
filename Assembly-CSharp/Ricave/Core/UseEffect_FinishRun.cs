using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class UseEffect_FinishRun : UseEffect
    {
        protected UseEffect_FinishRun()
        {
        }

        public UseEffect_FinishRun(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            yield return new Instruction_Sound(Get.Sound_Ending, null, 1f, 1f);
            yield return new Instruction_SetFinishedRun(true);
            List<Actor> followers = Get.Player.Followers;
            int num;
            for (int i = 0; i < followers.Count; i = num + 1)
            {
                Actor actor = followers[i];
                if (actor.Spec == Get.Entity_Spirit && actor.Spawned)
                {
                    yield return new Instruction_ChangeSpiritsSetFree(1);
                }
                num = i;
            }
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
            if (!Get.Achievement_FinishFirstDungeon.IsCompleted)
            {
                yield return new Instruction_CompleteAchievement(Get.Achievement_FinishFirstDungeon);
            }
            yield break;
            yield break;
        }
    }
}