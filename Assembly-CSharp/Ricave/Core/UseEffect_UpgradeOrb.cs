using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class UseEffect_UpgradeOrb : UseEffect
    {
        public override GoodBadNeutral GoodBadNeutral
        {
            get
            {
                return GoodBadNeutral.Good;
            }
        }

        protected UseEffect_UpgradeOrb()
        {
        }

        public UseEffect_UpgradeOrb(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            Actor actor = user ?? (target.Entity as Actor);
            if (actor == null)
            {
                yield break;
            }
            object choice = UsePrompt.Choice;
            if (choice is int)
            {
                int num = (int)choice;
                if (actor.IsNowControlledActor)
                {
                    Condition condition = null;
                    switch (num)
                    {
                        case 0:
                            condition = new Condition_MaxHPOffset(Get.Condition_MaxHPOffset, 1, 0);
                            break;
                        case 1:
                            condition = new Condition_CritChanceFactor(Get.Condition_CritChanceFactor, 1.4f);
                            break;
                        case 2:
                            condition = new Condition_MissChanceFactor(Get.Condition_MissChanceFactor, 0.8f);
                            break;
                        case 3:
                            condition = new Condition_IncomingDamageFactor(Get.Condition_IncomingDamageFactor, 0.5f, Get.DamageType_Fire, 0);
                            break;
                    }
                    foreach (Instruction instruction in InstructionSets_Misc.AddCondition(condition, actor.Conditions, actor != null && actor.IsPlayerParty, true))
                    {
                        yield return instruction;
                    }
                    IEnumerator<Instruction> enumerator = null;
                    yield break;
                }
            }
            yield break;
            yield break;
        }
    }
}