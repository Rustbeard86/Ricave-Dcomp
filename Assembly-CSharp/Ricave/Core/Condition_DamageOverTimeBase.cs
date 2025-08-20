using System;
using System.Collections.Generic;
using Ricave.UI;
using UnityEngine;

namespace Ricave.Core
{
    public abstract class Condition_DamageOverTimeBase : Condition
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

        public abstract int IntervalTurns { get; }

        public abstract int Damage { get; }

        public abstract DamageTypeSpec DamageType { get; }

        public override string LabelBase
        {
            get
            {
                return base.Spec.LabelFormat.Formatted(StringUtility.DamagePerTurnsString(this.Damage, this.IntervalTurns));
            }
        }

        public override string Tip
        {
            get
            {
                if (this.IntervalTurns > 1 && base.AffectedActor != null)
                {
                    return base.Tip.AppendedInDoubleNewLine("WillTakeDamageIn".Translate(RichText.Turns(StringUtility.TurnsString(this.IntervalTurns - this.turnsPassed))));
                }
                return base.Tip;
            }
        }

        protected Condition_DamageOverTimeBase()
        {
        }

        public Condition_DamageOverTimeBase(ConditionSpec spec)
            : base(spec)
        {
        }

        public Condition_DamageOverTimeBase(ConditionSpec spec, int turnsLeft)
            : base(spec, turnsLeft)
        {
        }

        public override IEnumerable<Instruction> MakeResolveConditionInstructions()
        {
            if (this.turnsPassed >= this.IntervalTurns - 1)
            {
                if (this.turnsPassed != 0)
                {
                    yield return new Instruction_ChangeDamageOverTimeConditionTurnsPassed(this, -this.turnsPassed);
                }
                Actor actor = base.AffectedActor;
                int finalDamage = DamageUtility.ApplyDamageProtectionAndClamp(actor, this.Damage, this.DamageType);
                foreach (Instruction instruction in InstructionSets_Entity.Damage(actor, finalDamage, this.DamageType, null, null, false, false, null, null, false, true))
                {
                    yield return instruction;
                }
                IEnumerator<Instruction> enumerator = null;
                if (finalDamage > 0)
                {
                    yield return new Instruction_Sound(Get.Sound_LoseHealthGeneric, new Vector3?(actor.Position), 1f, 1f);
                }
                actor = null;
            }
            else
            {
                yield return new Instruction_ChangeDamageOverTimeConditionTurnsPassed(this, 1);
            }
            yield break;
            yield break;
        }

        public override void CopyFieldsTo(Condition clone)
        {
            ((Condition_DamageOverTimeBase)clone).turnsPassed = this.turnsPassed;
        }

        [Saved]
        private int turnsPassed;
    }
}