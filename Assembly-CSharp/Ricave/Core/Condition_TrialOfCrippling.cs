using System;
using System.Collections.Generic;
using System.Linq;

namespace Ricave.Core
{
    public class Condition_TrialOfCrippling : Condition
    {
        public override GoodBadNeutral GoodBadNeutral
        {
            get
            {
                return GoodBadNeutral.Neutral;
            }
        }

        public override string LabelBase
        {
            get
            {
                if (base.AffectedActor == null)
                {
                    return base.LabelBase;
                }
                return base.Spec.LabelFormat.Formatted(this.bodyPartsDestroyed, 6);
            }
        }

        public override string Description
        {
            get
            {
                return base.Spec.DescriptionFormat.Formatted(6, 120);
            }
        }

        public int BodyPartsDestroyed
        {
            get
            {
                return this.bodyPartsDestroyed;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.bodyPartsDestroyed = value;
            }
        }

        protected Condition_TrialOfCrippling()
        {
        }

        public Condition_TrialOfCrippling(ConditionSpec spec)
            : base(spec, 120)
        {
        }

        public override IEnumerable<Instruction> MakeResolveConditionInstructions()
        {
            return Enumerable.Empty<Instruction>();
        }

        public override IEnumerable<Instruction> MakeOtherActorBodyPartDestroyedInstructions(BodyPart bodyPart)
        {
            yield return new Instruction_ChangeTrialOfCripplingBodyPartsDestroyed(this, 1);
            if (this.bodyPartsDestroyed >= 6)
            {
                foreach (Instruction instruction in Condition_TrialOfSwiftKiller.TrialCompletedInstructions(this))
                {
                    yield return instruction;
                }
                IEnumerator<Instruction> enumerator = null;
            }
            yield break;
            yield break;
        }

        public override void CopyFieldsTo(Condition clone)
        {
            ((Condition_TrialOfCrippling)clone).bodyPartsDestroyed = this.bodyPartsDestroyed;
        }

        [Saved]
        private int bodyPartsDestroyed;

        private const int BodyPartsToDestroy = 6;

        private const int DurationTurns = 120;
    }
}