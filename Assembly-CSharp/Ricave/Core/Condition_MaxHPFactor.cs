using System;
using System.Collections.Generic;
using System.Linq;

namespace Ricave.Core
{
    public class Condition_MaxHPFactor : Condition
    {
        public override GoodBadNeutral GoodBadNeutral
        {
            get
            {
                if (this.factor > 1f)
                {
                    return GoodBadNeutral.Good;
                }
                if (this.factor >= 1f)
                {
                    return GoodBadNeutral.Neutral;
                }
                return GoodBadNeutral.Bad;
            }
        }

        public float Factor
        {
            get
            {
                return this.factor;
            }
        }

        public override string LabelBase
        {
            get
            {
                return base.Spec.LabelFormat.Formatted(this.factor.ToStringFactor());
            }
        }

        public override float MaxHPFactor
        {
            get
            {
                return this.factor;
            }
        }

        public override float ActorScaleFactor
        {
            get
            {
                return this.actorScaleFactor;
            }
        }

        protected Condition_MaxHPFactor()
        {
        }

        public Condition_MaxHPFactor(ConditionSpec spec)
            : base(spec)
        {
        }

        public override IEnumerable<Instruction> MakeResolveConditionInstructions()
        {
            return Enumerable.Empty<Instruction>();
        }

        public override IEnumerable<Instruction> MakePostAddInstructions()
        {
            foreach (Instruction instruction in base.MakePostAddInstructions())
            {
                yield return instruction;
            }
            IEnumerator<Instruction> enumerator = null;
            Actor affectedActor = base.AffectedActor;
            if (affectedActor != null && this.Factor > 1f && affectedActor.HP > 0)
            {
                int num = Calc.RoundToIntHalfUp((float)affectedActor.MaxHP - (float)affectedActor.MaxHP / this.Factor);
                if (num >= 1)
                {
                    yield return new Instruction_ChangeHP(affectedActor, num);
                }
            }
            yield break;
            yield break;
        }

        public override void CopyFieldsTo(Condition clone)
        {
            Condition_MaxHPFactor condition_MaxHPFactor = (Condition_MaxHPFactor)clone;
            condition_MaxHPFactor.factor = this.factor;
            condition_MaxHPFactor.actorScaleFactor = this.actorScaleFactor;
        }

        [Saved(1f, false)]
        private float factor = 1f;

        [Saved(1f, false)]
        private float actorScaleFactor = 1f;
    }
}