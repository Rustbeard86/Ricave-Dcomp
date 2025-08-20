using System;
using System.Collections.Generic;
using System.Linq;

namespace Ricave.Core
{
    public class Condition_ArmMissing : Condition
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
                return new ValueTuple<float, DamageTypeSpec>(0.75f, Get.DamageType_Physical);
            }
        }

        public override string Description
        {
            get
            {
                return base.Spec.DescriptionFormat.Formatted(0.25f.ToStringPercent(false));
            }
        }

        protected Condition_ArmMissing()
        {
        }

        public Condition_ArmMissing(ConditionSpec spec)
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

        private const float Factor = 0.75f;
    }
}