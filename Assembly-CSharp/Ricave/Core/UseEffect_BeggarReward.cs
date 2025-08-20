using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class UseEffect_BeggarReward : UseEffect
    {
        public override GoodBadNeutral GoodBadNeutral
        {
            get
            {
                return GoodBadNeutral.Good;
            }
        }

        protected UseEffect_BeggarReward()
        {
        }

        public UseEffect_BeggarReward(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            if (user == null)
            {
                yield break;
            }
            Rand.PushState(Calc.CombineHashes<int, int, int>(Get.WorldSeed, usable.MyStableHash, 742579675));
            Item reward = (Rand.Bool ? ItemGenerator.Ring(true) : ItemGenerator.Amulet(true));
            Rand.PopState();
            foreach (Instruction instruction in InstructionSets_Actor.AddToInventoryOrSpawnNear(user, reward))
            {
                yield return instruction;
            }
            IEnumerator<Instruction> enumerator = null;
            yield return new Instruction_PlayLog("ReceivedItem".Translate(reward));
            yield return new Instruction_Sound(Get.Sound_PickUpItem, null, 1f, 1f);
            yield break;
            yield break;
        }
    }
}