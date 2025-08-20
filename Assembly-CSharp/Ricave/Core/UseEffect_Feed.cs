using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class UseEffect_Feed : UseEffect
    {
        public override GoodBadNeutral GoodBadNeutral
        {
            get
            {
                return GoodBadNeutral.Good;
            }
        }

        public int Satiation
        {
            get
            {
                return this.satiation;
            }
        }

        public override string LabelBase
        {
            get
            {
                return base.Spec.LabelFormat.Formatted(this.satiation.ToStringOffset(true));
            }
        }

        protected UseEffect_Feed()
        {
        }

        public UseEffect_Feed(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
            ((UseEffect_Feed)clone).satiation = this.satiation;
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            Actor actor = target.Entity as Actor;
            if (actor != null)
            {
                foreach (Instruction instruction in UseEffect_Feed.FeedInstructions(actor, this.satiation))
                {
                    yield return instruction;
                }
                IEnumerator<Instruction> enumerator = null;
            }
            yield break;
            yield break;
        }

        public static IEnumerable<Instruction> FeedInstructions(Actor actor, int satiation)
        {
            if (actor.IsMainActor)
            {
                int num = Calc.Clamp(600 - Get.Player.Satiation, 0, satiation);
                if (num > 0)
                {
                    yield return new Instruction_ChangeSatiation(num);
                }
                if (Get.Player.Satiation > 250)
                {
                    foreach (Instruction instruction in UseEffect_Feed.CheckRemoveCondition(Get.Condition_Hungry, actor))
                    {
                        yield return instruction;
                    }
                    IEnumerator<Instruction> enumerator = null;
                }
                if (Get.Player.Satiation > 0)
                {
                    foreach (Instruction instruction2 in UseEffect_Feed.CheckRemoveCondition(Get.Condition_Starving, actor))
                    {
                        yield return instruction2;
                    }
                    IEnumerator<Instruction> enumerator = null;
                    if (Get.Player.Satiation <= 250 && !actor.Conditions.AnyOfSpec(Get.Condition_Hungry))
                    {
                        foreach (Instruction instruction3 in InstructionSets_Misc.AddCondition(new Condition_Hungry(Get.Condition_Hungry), actor.Conditions, false, false))
                        {
                            yield return instruction3;
                        }
                        enumerator = null;
                    }
                }
            }
            yield break;
            yield break;
        }

        private static IEnumerable<Instruction> CheckRemoveCondition(ConditionSpec conditionSpec, Actor actor)
        {
            Condition firstOfSpec = actor.Conditions.GetFirstOfSpec(conditionSpec);
            if (firstOfSpec == null)
            {
                yield break;
            }
            foreach (Instruction instruction in InstructionSets_Misc.RemoveCondition(firstOfSpec, false, false))
            {
                yield return instruction;
            }
            IEnumerator<Instruction> enumerator = null;
            yield break;
            yield break;
        }

        [Saved]
        private int satiation;
    }
}