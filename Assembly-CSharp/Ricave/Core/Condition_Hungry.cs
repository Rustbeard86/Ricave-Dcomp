using System;
using System.Collections.Generic;
using System.Linq;

namespace Ricave.Core
{
    public class Condition_Hungry : Condition
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

        public override bool DisableNativeManaRegen
        {
            get
            {
                return true;
            }
        }

        protected Condition_Hungry()
        {
        }

        public Condition_Hungry(ConditionSpec spec)
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
    }
}