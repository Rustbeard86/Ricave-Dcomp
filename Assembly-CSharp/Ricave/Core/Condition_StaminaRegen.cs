using System;
using System.Collections.Generic;
using Ricave.UI;

namespace Ricave.Core
{
    public class Condition_StaminaRegen : Condition
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
                    num2 *= Get.TraitManager.NativeStaminaRegenIntervalFactor;
                }
                if (this.native && base.AffectedActor != null)
                {
                    num2 *= base.AffectedActor.ConditionsAccumulated.NativeStaminaRegenIntervalFactor;
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
                if (affectedActor != null && affectedActor.IsMainActor && Get.Skill_FasterStaminaRegen.IsUnlocked() && this.native)
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

        public int TurnsToNextRegen
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
                    if (affectedActor != null && affectedActor.ConditionsAccumulated.DisableNativeStaminaRegen)
                    {
                        return true;
                    }
                }
                return this.DisabledBecauseNotTouchingGround;
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
                if (this.IntervalTurns <= 1 || this.Disabled || base.AffectedActor == null || base.AffectedActor.Stamina >= base.AffectedActor.MaxStamina)
                {
                    string text = base.Tip;
                    if (this.DisabledBecauseNotTouchingGround)
                    {
                        text = text.AppendedInDoubleNewLine("StaminaRegenDisabledBecauseNotTouchingGround".Translate());
                    }
                    return text;
                }
                return base.Tip.AppendedInDoubleNewLine("WillGainStaminaIn".Translate(RichText.Turns(StringUtility.TurnsString(this.TurnsToNextRegen))));
            }
        }

        public bool DisabledBecauseNotTouchingGround
        {
            get
            {
                if (!this.native)
                {
                    return false;
                }
                Actor affectedActor = base.AffectedActor;
                return affectedActor != null && Get.CellsInfo.IsFallingAt(affectedActor.Position, affectedActor.Gravity, false, affectedActor.CanUseLadders, false);
            }
        }

        protected Condition_StaminaRegen()
        {
        }

        public Condition_StaminaRegen(ConditionSpec spec)
            : base(spec)
        {
        }

        public Condition_StaminaRegen(ConditionSpec spec, int intervalTurns, int amount, bool usesRampUp = false)
            : base(spec)
        {
            this.intervalTurns = intervalTurns;
            this.amount = amount;
            this.usesRampUp = usesRampUp;
        }

        public override IEnumerable<Instruction> MakeResolveConditionInstructions()
        {
            Actor actor = base.AffectedActor;
            if (actor.Stamina >= actor.MaxStamina)
            {
                if (this.progress != 0)
                {
                    yield return new Instruction_ChangeStaminaRegenProgress(this, -this.progress);
                }
            }
            else if (!this.Disabled)
            {
                yield return new Instruction_ChangeStaminaRegenProgress(this, 1);
                if (this.progress >= this.IntervalTurns)
                {
                    yield return new Instruction_ChangeStaminaRegenProgress(this, -this.progress);
                    foreach (Instruction instruction in InstructionSets_Actor.RestoreStamina(actor, this.Amount, false, false, false))
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
            Condition_StaminaRegen condition_StaminaRegen = (Condition_StaminaRegen)clone;
            condition_StaminaRegen.intervalTurns = this.intervalTurns;
            condition_StaminaRegen.amount = this.amount;
            condition_StaminaRegen.progress = this.progress;
            condition_StaminaRegen.native = this.native;
            condition_StaminaRegen.usesRampUp = this.usesRampUp;
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