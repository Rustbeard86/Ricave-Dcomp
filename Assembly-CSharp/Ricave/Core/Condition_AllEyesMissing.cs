using System;
using System.Collections.Generic;
using System.Linq;

namespace Ricave.Core
{
    public class Condition_AllEyesMissing : Condition
    {
        public override GoodBadNeutral GoodBadNeutral
        {
            get
            {
                return GoodBadNeutral.Bad;
            }
        }

        public override int SeeRangeCap
        {
            get
            {
                return 1;
            }
        }

        public override float MinMissChanceOverride
        {
            get
            {
                return 0.3f;
            }
        }

        protected Condition_AllEyesMissing()
        {
        }

        public Condition_AllEyesMissing(ConditionSpec spec)
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

        public override IEnumerable<Instruction> MakeOnNewlyAffectingActorInstructions()
        {
            foreach (Instruction instruction in base.MakeOnNewlyAffectingActorInstructions())
            {
                yield return instruction;
            }
            IEnumerator<Instruction> enumerator = null;
            foreach (Instruction instruction2 in Condition_SeeRangeOffset.SeeRangeChangedInstructions(base.AffectedActor))
            {
                yield return instruction2;
            }
            enumerator = null;
            yield break;
            yield break;
        }

        public override IEnumerable<Instruction> MakeOnNoLongerAffectingActorInstructions(Actor actor)
        {
            foreach (Instruction instruction in base.MakeOnNoLongerAffectingActorInstructions(actor))
            {
                yield return instruction;
            }
            IEnumerator<Instruction> enumerator = null;
            foreach (Instruction instruction2 in Condition_SeeRangeOffset.SeeRangeChangedInstructions(actor))
            {
                yield return instruction2;
            }
            enumerator = null;
            yield break;
            yield break;
        }
    }
}