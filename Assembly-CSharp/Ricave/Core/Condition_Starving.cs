using System;

namespace Ricave.Core
{
    public class Condition_Starving : Condition_DamageOverTimeBase
    {
        public override GoodBadNeutral GoodBadNeutral
        {
            get
            {
                return GoodBadNeutral.Bad;
            }
        }

        public override bool StopIdentification
        {
            get
            {
                return true;
            }
        }

        public override bool DisableNativeHPRegen
        {
            get
            {
                return true;
            }
        }

        public override bool DisableNativeStaminaRegen
        {
            get
            {
                return true;
            }
        }

        public override bool DisableNativeManaRegen
        {
            get
            {
                return true;
            }
        }

        public override int IntervalTurns
        {
            get
            {
                return 50;
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
                return Get.DamageType_Starvation;
            }
        }

        protected Condition_Starving()
        {
        }

        public Condition_Starving(ConditionSpec spec)
            : base(spec)
        {
        }

        public override void CopyFieldsTo(Condition clone)
        {
        }
    }
}