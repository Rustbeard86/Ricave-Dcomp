using System;
using System.Collections.Generic;
using System.Linq;

namespace Ricave.Core
{
    public class Condition_AllArmsMissing : Condition
    {
        public override GoodBadNeutral GoodBadNeutral
        {
            get
            {
                return GoodBadNeutral.Bad;
            }
        }

        public override ValueTuple<float, DamageTypeSpec> DealtDamageFactor
        {
            get
            {
                return new ValueTuple<float, DamageTypeSpec>(0.5f, Get.DamageType_Physical);
            }
        }

        public override string Description
        {
            get
            {
                return base.Spec.DescriptionFormat.Formatted(0.5f.ToStringPercent(false));
            }
        }

        protected Condition_AllArmsMissing()
        {
        }

        public Condition_AllArmsMissing(ConditionSpec spec)
            : base(spec)
        {
        }

        public override void CopyFieldsTo(Condition clone)
        {
        }

        public override IEnumerable<Instruction> MakeResolveConditionInstructions()
        {
            return Enumerable.Empty<Instruction>();
        }

        private const float Factor = 0.5f;
    }
}