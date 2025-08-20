using System;
using System.Collections.Generic;
using System.Linq;

namespace Ricave.Core
{
    public class Condition_Berserker : Condition
    {
        public override GoodBadNeutral GoodBadNeutral
        {
            get
            {
                return GoodBadNeutral.Neutral;
            }
        }

        public override ValueTuple<float, DamageTypeSpec> DealtDamageFactor
        {
            get
            {
                return new ValueTuple<float, DamageTypeSpec>(1.25f, Get.DamageType_Physical);
            }
        }

        public override ValueTuple<float, DamageTypeSpec> IncomingDamageFactor
        {
            get
            {
                return new ValueTuple<float, DamageTypeSpec>(1.25f, Get.DamageType_Physical);
            }
        }

        protected Condition_Berserker()
        {
        }

        public Condition_Berserker(ConditionSpec spec)
            : base(spec)
        {
        }

        public Condition_Berserker(ConditionSpec spec, int turnsLeft)
            : base(spec, turnsLeft)
        {
        }

        public override void CopyFieldsTo(Condition clone)
        {
        }

        public override IEnumerable<Instruction> MakeResolveConditionInstructions()
        {
            return Enumerable.Empty<Instruction>();
        }
    }
}