using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class Condition_Burning : Condition_DamageOverTimeBase
    {
        public override GoodBadNeutral GoodBadNeutral
        {
            get
            {
                return GoodBadNeutral.Bad;
            }
        }

        public override int IntervalTurns
        {
            get
            {
                return 1;
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
                return Get.DamageType_Fire;
            }
        }

        protected Condition_Burning()
        {
        }

        public Condition_Burning(ConditionSpec spec)
            : base(spec)
        {
        }

        public Condition_Burning(ConditionSpec spec, int damage, int turnsLeft)
            : base(spec, turnsLeft)
        {
            this.damage = damage;
        }

        public override IEnumerable<Instruction> MakeResolveConditionInstructions()
        {
            if (base.AffectedActor.ImmuneToFire)
            {
                yield break;
            }
            foreach (Instruction instruction in base.MakeResolveConditionInstructions())
            {
                yield return instruction;
            }
            IEnumerator<Instruction> enumerator = null;
            yield break;
            yield break;
        }

        public override void CopyFieldsTo(Condition clone)
        {
            base.CopyFieldsTo(clone);
            ((Condition_Burning)clone).damage = this.damage;
        }

        [Saved(1, false)]
        private int damage = 1;
    }
}