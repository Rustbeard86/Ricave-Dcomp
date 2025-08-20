using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class UseEffect_Research : UseEffect
    {
        protected UseEffect_Research()
        {
        }

        public UseEffect_Research(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            yield return new Instruction_ChangeAncientDevicesResearched(1);
            Entity usableEntity = usable as Entity;
            if (usableEntity == null)
            {
                yield break;
            }
            if (Get.Player.AncientDevicesResearched >= 3)
            {
                yield return new Instruction_VisualEffect(Get.VisualEffect_Sacrifice, usableEntity.Position);
                Item reward = this.GenerateReward();
                Vector3Int vector3Int = SpawnPositionFinder.Near(usableEntity.Position, reward, false, false, null);
                foreach (Instruction instruction in InstructionSets_Entity.Spawn(reward, vector3Int, null))
                {
                    yield return instruction;
                }
                IEnumerator<Instruction> enumerator = null;
                yield return new Instruction_StartItemJumpAnimation(reward);
                reward = null;
            }
            yield return new Instruction_AddFloatingText(usableEntity, "{0}/{1}".Formatted(Get.Player.AncientDevicesResearched, 3), new Color(0.64f, 0.45f, 0.9f), 0.32f, 0f, -0.27f, null);
            yield break;
            yield break;
        }

        private Item GenerateReward()
        {
            Rand.PushState(Calc.CombineHashes<int, int>(Get.WorldSeed, 713265772));
            Item item = WorldGenUtility.GenerateMiniChallengeReward();
            Rand.PopState();
            return item;
        }

        public const int CountToResearch = 3;
    }
}