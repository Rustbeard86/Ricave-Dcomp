using System;
using System.Collections.Generic;
using System.Linq;

namespace Ricave.Core
{
    public class Condition_Wet : Condition
    {
        public override GoodBadNeutral GoodBadNeutral
        {
            get
            {
                return GoodBadNeutral.Neutral;
            }
        }

        public override ValueTuple<float, DamageTypeSpec> IncomingDamageFactor
        {
            get
            {
                return new ValueTuple<float, DamageTypeSpec>(0.5f, Get.DamageType_Fire);
            }
        }

        public override int NativeStaminaRegenIntervalFactor
        {
            get
            {
                return 2;
            }
        }

        protected Condition_Wet()
        {
        }

        public Condition_Wet(ConditionSpec spec)
            : base(spec)
        {
        }

        public override IEnumerable<Instruction> MakeResolveConditionInstructions()
        {
            return Enumerable.Empty<Instruction>();
        }

        public override void CopyFieldsTo(Condition clone)
        {
        }
    }
}