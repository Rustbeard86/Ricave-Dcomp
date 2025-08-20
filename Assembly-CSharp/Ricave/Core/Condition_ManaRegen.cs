using System;
using System.Collections.Generic;
using Ricave.UI;

namespace Ricave.Core
{
    public class Condition_ManaRegen : Condition
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
                    return affectedActor != null && affectedActor.ConditionsAccumulated.DisableNativeManaRegen;
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
                if (this.IntervalTurns > 1 && !this.Disabled && base.AffectedActor != null && base.AffectedActor.Mana < base.AffectedActor.MaxMana)
                {
                    return base.Tip.AppendedInDoubleNewLine("WillGainManaIn".Translate(RichText.Turns(StringUtility.TurnsString(this.TurnsToNextRegen))));
                }
                return base.Tip;
            }
        }

        protected Condition_ManaRegen()
        {
        }

        public Condition_ManaRegen(ConditionSpec spec)
            : base(spec)
        {
        }

        public Condition_ManaRegen(ConditionSpec spec, int intervalTurns, int amount, bool usesRampUp = false)
            : base(spec)
        {
            this.intervalTurns = intervalTurns;
            this.amount = amount;
            this.usesRampUp = usesRampUp;
        }

        public override IEnumerable<Instruction> MakeResolveConditionInstructions()
        {
            Actor actor = base.AffectedActor;
            if (actor.Mana >= actor.MaxMana)
            {
                if (this.progress != 0)
                {
                    yield return new Instruction_ChangeManaRegenProgress(this, -this.progress);
                }
            }
            else if (!this.Disabled)
            {
                yield return new Instruction_ChangeManaRegenProgress(this, 1);
                if (this.progress >= this.IntervalTurns)
                {
                    yield return new Instruction_ChangeManaRegenProgress(this, -this.progress);
                    foreach (Instruction instruction in InstructionSets_Actor.RestoreMana(actor, this.Amount, false, false, false))
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
            Condition_ManaRegen condition_ManaRegen = (Condition_ManaRegen)clone;
            condition_ManaRegen.intervalTurns = this.intervalTurns;
            condition_ManaRegen.amount = this.amount;
            condition_ManaRegen.progress = this.progress;
            condition_ManaRegen.native = this.native;
            condition_ManaRegen.usesRampUp = this.usesRampUp;
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