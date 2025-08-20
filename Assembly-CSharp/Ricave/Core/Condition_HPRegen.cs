using System;
using System.Collections.Generic;
using Ricave.UI;

namespace Ricave.Core
{
    public class Condition_HPRegen : Condition
    {
        public override GoodBadNeutral GoodBadNeutral
        {
            get
            {
                return GoodBadNeutral.Good;
            }
        }

        public int IntervalTurns
        {
            get
            {
                int num;
                int num2;
                RampUpUtility.ApplyRampUpToTimesPerInterval(this.amount, this.intervalTurns, base.InheritedRampUp, this.RampUpFactor, out num, out num2);
                Actor affectedActor = base.AffectedActor;
                if (affectedActor != null && affectedActor.IsMainActor && this.native)
                {
                    float nativeHealingRate = Get.TraitManager.NativeHealingRate;
                    if (nativeHealingRate < 1f && nativeHealingRate != 0f)
                    {
                        num2 = Calc.RoundToIntHalfUp((float)num2 / nativeHealingRate);
                    }
                }
                return num2;
            }
        }

        public int Amount
        {
            get
            {
                int num;
                int num2;
                RampUpUtility.ApplyRampUpToTimesPerInterval(this.amount, this.intervalTurns, base.InheritedRampUp, this.RampUpFactor, out num, out num2);
                Actor affectedActor = base.AffectedActor;
                if (affectedActor != null && affectedActor.IsMainActor && this.native)
                {
                    float nativeHealingRate = Get.TraitManager.NativeHealingRate;
                    if (nativeHealingRate > 1f)
                    {
                        num = Calc.RoundToIntHalfUp((float)num * nativeHealingRate);
                    }
                    else if (nativeHealingRate == 0f)
                    {
                        num = 0;
                    }
                    if (Get.Trait_Vampire.IsChosen() && !Get.DayNightCycleManager.IsNightForNightOwl)
                    {
                        num = 0;
                    }
                }
                Actor affectedActor2 = base.AffectedActor;
                if (affectedActor2 != null && affectedActor2.IsMainActor && Get.Skill_FasterHealing.IsUnlocked() && this.native)
                {
                    num *= 2;
                }
                return num;
            }
        }

        public int BaseIntervalTurns
        {
            get
            {
                return this.intervalTurns;
            }
        }

        public int BaseAmount
        {
            get
            {
                return this.amount;
            }
        }

        public bool Native
        {
            get
            {
                return this.native;
            }
        }

        public int Progress
        {
            get
            {
                return this.progress;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.progress = value;
            }
        }

        public int TurnsToNextHeal
        {
            get
            {
                return this.IntervalTurns - this.progress;
            }
        }

        public override string LabelBase
        {
            get
            {
                return base.Spec.LabelFormat.Formatted(StringUtility.NumberPerTurnsString(this.Amount, this.IntervalTurns));
            }
        }

        public bool Disabled
        {
            get
            {
                if (this.native)
                {
                    Actor affectedActor = base.AffectedActor;
                    return affectedActor != null && affectedActor.ConditionsAccumulated.DisableNativeHPRegen;
                }
                return false;
            }
        }

        public float RampUpFactor
        {
            get
            {
                if (!this.usesRampUp)
                {
                    return 1f;
                }
                return 1.2500001f;
            }
        }

        public override string Tip
        {
            get
            {
                if (this.IntervalTurns > 1 && !this.Disabled && base.AffectedActor != null && base.AffectedActor.HP < base.AffectedActor.MaxHP)
                {
                    return base.Tip.AppendedInDoubleNewLine("WillGainHPIn".Translate(RichText.Turns(StringUtility.TurnsString(this.TurnsToNextHeal))));
                }
                return base.Tip;
            }
        }

        protected Condition_HPRegen()
        {
        }

        public Condition_HPRegen(ConditionSpec spec)
            : base(spec)
        {
        }

        public Condition_HPRegen(ConditionSpec spec, int intervalTurns, int amount, bool usesRampUp = false, int turnsLeft = 0)
            : base(spec, turnsLeft)
        {
            this.intervalTurns = intervalTurns;
            this.amount = amount;
            this.usesRampUp = usesRampUp;
        }

        public override IEnumerable<Instruction> MakeResolveConditionInstructions()
        {
            Actor actor = base.AffectedActor;
            if (actor.HP >= actor.MaxHP)
            {
                if (this.progress != 0)
                {
                    yield return new Instruction_ChangeHPRegenProgress(this, -this.progress);
                }
            }
            else if (!this.Disabled)
            {
                yield return new Instruction_ChangeHPRegenProgress(this, 1);
                if (this.progress >= this.IntervalTurns)
                {
                    yield return new Instruction_ChangeHPRegenProgress(this, -this.progress);
                    foreach (Instruction instruction in InstructionSets_Entity.Heal(actor, this.Amount, false, false))
                    {
                        yield return instruction;
                    }
                    IEnumerator<Instruction> enumerator = null;
                }
            }
            yield break;
            yield break;
        }

        public override void CopyFieldsTo(Condition clone)
        {
            Condition_HPRegen condition_HPRegen = (Condition_HPRegen)clone;
            condition_HPRegen.intervalTurns = this.intervalTurns;
            condition_HPRegen.amount = this.amount;
            condition_HPRegen.progress = this.progress;
            condition_HPRegen.native = this.native;
            condition_HPRegen.usesRampUp = this.usesRampUp;
        }

        [Saved]
        private int intervalTurns;

        [Saved]
        private int amount;

        [Saved]
        private bool native;

        [Saved]
        private bool usesRampUp;

        [Saved]
        private int progress;
    }
}