using System;

namespace Ricave.Core
{
    public class Condition_DamageOverTime : Condition_DamageOverTimeBase
    {
        public override int IntervalTurns
        {
            get
            {
                return this.intervalTurns;
            }
        }

        public override int Damage
        {
            get
            {
                return this.damage;
            }
        }

        public override DamageTypeSpec DamageType
        {
            get
            {
                return this.damageType;
            }
        }

        protected Condition_DamageOverTime()
        {
        }

        public Condition_DamageOverTime(ConditionSpec spec)
            : base(spec)
        {
        }

        public Condition_DamageOverTime(ConditionSpec spec, int intervalTurns, int damage, DamageTypeSpec damageType, int turnsLeft = 0)
            : base(spec, turnsLeft)
        {
            this.damage = damage;
            this.intervalTurns = intervalTurns;
            this.damageType = damageType;
        }

        public override void CopyFieldsTo(Condition clone)
        {
            base.CopyFieldsTo(clone);
            Condition_DamageOverTime condition_DamageOverTime = (Condition_DamageOverTime)clone;
            condition_DamageOverTime.damage = this.damage;
            condition_DamageOverTime.intervalTurns = this.intervalTurns;
            condition_DamageOverTime.damageType = this.damageType;
        }

        [Saved]
        private int damage;

        [Saved]
        private int intervalTurns;

        [Saved]
        private DamageTypeSpec damageType;
    }
}