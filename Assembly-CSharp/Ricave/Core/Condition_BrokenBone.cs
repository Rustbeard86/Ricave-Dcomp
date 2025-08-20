using System;
using System.Collections.Generic;
using System.Linq;

namespace Ricave.Core
{
    public class Condition_BrokenBone : Condition
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
                return new ValueTuple<float, DamageTypeSpec>(0.85f, Get.DamageType_Physical);
            }
        }

        public override string Description
        {
            get
            {
                return base.Spec.DescriptionFormat.Formatted(0.14999998f.ToStringPercent(false));
            }
        }

        protected Condition_BrokenBone()
        {
        }

        public Condition_BrokenBone(ConditionSpec spec)
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

        private const float Factor = 0.85f;
    }
}