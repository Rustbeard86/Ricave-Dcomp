using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class UseEffect_Blessing : UseEffect
    {
        public override GoodBadNeutral GoodBadNeutral
        {
            get
            {
                return GoodBadNeutral.Good;
            }
        }

        protected UseEffect_Blessing()
        {
        }

        public UseEffect_Blessing(UseEffectSpec spec)
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
            int num = Rand.RangeInclusive(0, 4);
            Condition condition = null;
            Condition conditionToAdd2 = null;
            switch (num)
            {
                case 0:
                    condition = new Condition_Levitating(Get.Condition_Levitating, 1000);
                    break;
                case 1:
                    condition = new Condition_MaxHPOffset(Get.Condition_MaxHPOffset, 8, 1000);
                    break;
                case 2:
                    condition = new Condition_DealtDamageOffset(Get.Condition_DealtDamageOffset, 1, 1, Get.DamageType_Physical, 1000);
                    break;
                case 3:
                    condition = new Condition_SeeRangeOffset(Get.Condition_SeeRangeOffset, 3, 1000);
                    break;
                case 4:
                    condition = new Condition_HPRegen(Get.Condition_HPRegen, 20, 1, false, 1000);
                    break;
            }
            foreach (Instruction instruction in InstructionSets_Misc.AddCondition(condition, user.Conditions, user != null && user.IsPlayerParty, true))
            {
                yield return instruction;
            }
            IEnumerator<Instruction> enumerator = null;
            if (conditionToAdd2 != null)
            {
                foreach (Instruction instruction2 in InstructionSets_Misc.AddCondition(conditionToAdd2, user.Conditions, user != null && user.IsPlayerParty, true))
                {
                    yield return instruction2;
                }
                enumerator = null;
            }
            yield break;
            yield break;
        }
    }
}