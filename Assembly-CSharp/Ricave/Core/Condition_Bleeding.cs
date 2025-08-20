using System;

namespace Ricave.Core
{
    public class Condition_Bleeding : Condition_DamageOverTimeBase
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
                return 20;
            }
        }

        public override int Damage
        {
            get
            {
                return 1;
            }
        }

        public override DamageTypeSpec DamageType
        {
            get
            {
                return Get.DamageType_Bleeding;
            }
        }

        protected Condition_Bleeding()
        {
        }

        public Condition_Bleeding(ConditionSpec spec)
            : base(spec)
        {
        }

        public Condition_Bleeding(ConditionSpec spec, int turnsLeft)
            : base(spec, turnsLeft)
        {
        }
    }
}