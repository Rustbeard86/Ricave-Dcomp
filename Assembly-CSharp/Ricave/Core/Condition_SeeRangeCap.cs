using System;
using System.Collections.Generic;
using System.Linq;

namespace Ricave.Core
{
    public class Condition_SeeRangeCap : Condition
    {
        public override GoodBadNeutral GoodBadNeutral
        {
            get
            {
                return GoodBadNeutral.Bad;
            }
        }

        public int Cap
        {
            get
            {
                return this.cap;
            }
        }

        public override string LabelBase
        {
            get
            {
                return base.Spec.LabelFormat.Formatted(this.cap);
            }
        }

        public override int SeeRangeCap
        {
            get
            {
                return this.cap;
            }
        }

        protected Condition_SeeRangeCap()
        {
        }

        public Condition_SeeRangeCap(ConditionSpec spec)
            : base(spec)
        {
        }

        public Condition_SeeRangeCap(ConditionSpec spec, int cap, int turnsLeft = 0)
            : base(spec, turnsLeft)
        {
            this.cap = cap;
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

        public override void CopyFieldsTo(Condition clone)
        {
            ((Condition_SeeRangeCap)clone).cap = this.cap;
        }

        [Saved]
        private int cap;
    }
}