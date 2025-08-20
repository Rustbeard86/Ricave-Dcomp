using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class UseEffect_DropReward : UseEffect
    {
        public override GoodBadNeutral GoodBadNeutral
        {
            get
            {
                return GoodBadNeutral.Good;
            }
        }

        protected UseEffect_DropReward()
        {
        }

        public UseEffect_DropReward(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            Item reward = ItemGenerator.Reward(true);
            Vector3Int sourcePos = UseEffect_Sound.GetSourcePos(user, usable, target);
            foreach (Instruction instruction in InstructionSets_Entity.Spawn(reward, SpawnPositionFinder.Near(sourcePos, reward, false, false, null), null))
            {
                yield return instruction;
            }
            IEnumerator<Instruction> enumerator = null;
            yield return new Instruction_StartItemJumpAnimation(reward);
            yield break;
            yield break;
        }
    }
}