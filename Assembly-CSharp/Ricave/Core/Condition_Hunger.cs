using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class Condition_Hunger : Condition
    {
        public override GoodBadNeutral GoodBadNeutral
        {
            get
            {
                return GoodBadNeutral.Bad;
            }
        }

        public int TurnsPassed
        {
            get
            {
                return this.turnsPassed;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.turnsPassed = value;
            }
        }

        protected Condition_Hunger()
        {
        }

        public Condition_Hunger(ConditionSpec spec)
            : base(spec)
        {
        }

        public override void CopyFieldsTo(Condition clone)
        {
        }

        public override IEnumerable<Instruction> MakeResolveConditionInstructions()
        {
            Actor actor = base.AffectedActor;
            if (!actor.IsMainActor)
            {
                yield break;
            }
            if (Get.DungeonModifier_NoHunger.IsActiveAndAppliesToCurrentRun())
            {
                yield break;
            }
            int offsetPerInterval = 1;
            int interval = 1;
            Calc.MultiplyTimesPerInterval(ref offsetPerInterval, ref interval, actor.HungerRateMultiplier);
            if (offsetPerInterval <= 0)
            {
                yield break;
            }
            if (Get.Player.Satiation <= 0)
            {
                if (this.turnsPassed != 0)
                {
                    yield return new Instruction_ChangeHungerTurnsPassed(this, -this.turnsPassed);
                }
            }
            else
            {
                yield return new Instruction_ChangeHungerTurnsPassed(this, 1);
                if (this.turnsPassed >= interval)
                {
                    yield return new Instruction_ChangeHungerTurnsPassed(this, -this.turnsPassed);
                    int num = -Math.Min(offsetPerInterval, Get.Player.Satiation);
                    if (num != 0)
                    {
                        yield return new Instruction_ChangeSatiation(num);
                        Condition conditionToAdd = null;
                        if (Get.Player.Satiation <= 0)
                        {
                            if (!actor.Conditions.AnyOfSpec(Get.Condition_Starving))
                            {
                                yield return new Instruction_PlayLog("YouAreStarving".Translate());
                                yield return new Instruction_Sound(Get.Sound_BecameHungry, null, 1f, 1f);
                                conditionToAdd = new Condition_Starving(Get.Condition_Starving);
                                Condition firstOfSpec = actor.Conditions.GetFirstOfSpec(Get.Condition_Hungry);
                                if (firstOfSpec != null)
                                {
                                    foreach (Instruction instruction in InstructionSets_Misc.RemoveCondition(firstOfSpec, false, false))
                                    {
                                        yield return instruction;
                                    }
                                    IEnumerator<Instruction> enumerator = null;
                                }
                            }
                        }
                        else if (Get.Player.Satiation <= 250 && !actor.Conditions.AnyOfSpec(Get.Condition_Hungry))
                        {
                            yield return new Instruction_PlayLog("YouAreHungry".Translate());
                            yield return new Instruction_Sound(Get.Sound_BecameHungry, null, 1f, 1f);
                            conditionToAdd = new Condition_Hungry(Get.Condition_Hungry);
                        }
                        if (conditionToAdd != null)
                        {
                            foreach (Instruction instruction2 in InstructionSets_Misc.AddCondition(conditionToAdd, actor.Conditions, false, false))
                            {
                                yield return instruction2;
                            }
                            IEnumerator<Instruction> enumerator = null;
                            if (actor.IsNowControlledActor)
                            {
                                yield return new Instruction_ShowNewImportantCondition(conditionToAdd);
                            }
                        }
                        conditionToAdd = null;
                    }
                }
            }
            yield break;
            yield break;
        }

        [Saved]
        private int turnsPassed;

        public const int HungryAtSatiation = 250;

        public const int StarvingAtSatiation = 0;

        public const int MaxSatiation = 600;
    }
}