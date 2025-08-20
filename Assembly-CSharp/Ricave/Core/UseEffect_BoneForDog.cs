using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class UseEffect_BoneForDog : UseEffect
    {
        public override GoodBadNeutral GoodBadNeutral
        {
            get
            {
                return GoodBadNeutral.Good;
            }
        }

        protected UseEffect_BoneForDog()
        {
        }

        public UseEffect_BoneForDog(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            if (!target.IsEntity || target.Entity.Spec != Get.Entity_Dog || !target.Spawned)
            {
                yield break;
            }
            int num = Math.Min(5, target.Entity.MaxHP - target.Entity.HP);
            IEnumerator<Instruction> enumerator;
            if (num > 0)
            {
                foreach (Instruction instruction in InstructionSets_Entity.Heal(target.Entity, num, user != null && user.IsPlayerParty, true))
                {
                    yield return instruction;
                }
                enumerator = null;
            }
            Condition_DealtDamageFactor condition_DealtDamageFactor = new Condition_DealtDamageFactor(Get.Condition_DealtDamageFactor, 1.5f, Get.DamageType_Physical, 100);
            foreach (Instruction instruction2 in InstructionSets_Misc.AddCondition(condition_DealtDamageFactor, ((Actor)target.Entity).Conditions, user != null && user.IsPlayerParty, true))
            {
                yield return instruction2;
            }
            enumerator = null;
            yield break;
            yield break;
        }
    }
}