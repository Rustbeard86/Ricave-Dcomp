using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class UseEffect_RestoreMana : UseEffect
    {
        public override GoodBadNeutral GoodBadNeutral
        {
            get
            {
                return GoodBadNeutral.Good;
            }
        }

        public int From
        {
            get
            {
                return this.from;
            }
        }

        public int To
        {
            get
            {
                return this.to;
            }
        }

        public bool Fully
        {
            get
            {
                return this.fully;
            }
        }

        public float? Percent
        {
            get
            {
                return this.percent;
            }
        }

        public override string LabelBase
        {
            get
            {
                if (this.fully)
                {
                    return base.Spec.LabelFormat.Formatted("RestoreAll".Translate());
                }
                if (this.percent != null)
                {
                    string text = base.Spec.LabelFormat.Formatted(this.percent.Value.ToStringPercent(false));
                    int num = Calc.RoundToIntHalfUp((float)Get.NowControlledActor.MaxMana * this.percent.Value);
                    return "{0} ({1})".Formatted(text, "RestoredAmountForPlayer".Translate(num));
                }
                return base.Spec.LabelFormat.Formatted(StringUtility.RangeToString(this.from, this.to));
            }
        }

        protected UseEffect_RestoreMana()
        {
        }

        public UseEffect_RestoreMana(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
            UseEffect_RestoreMana useEffect_RestoreMana = (UseEffect_RestoreMana)clone;
            useEffect_RestoreMana.from = this.from;
            useEffect_RestoreMana.to = this.to;
            useEffect_RestoreMana.fully = this.fully;
            useEffect_RestoreMana.percent = this.percent;
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            Actor actor = target.Entity as Actor;
            if (actor == null)
            {
                yield break;
            }
            int num;
            if (this.fully)
            {
                num = actor.MaxMana - actor.Mana;
            }
            else if (this.percent != null)
            {
                num = Calc.RoundToIntHalfUp((float)actor.MaxMana * this.percent.Value);
            }
            else
            {
                num = Rand.RangeInclusive(this.from, this.to);
            }
            num = Math.Min(num, actor.MaxMana - actor.Mana);
            if (num > 0)
            {
                foreach (Instruction instruction in InstructionSets_Actor.RestoreMana(actor, num, user != null && user.IsPlayerParty, true, true))
                {
                    yield return instruction;
                }
                IEnumerator<Instruction> enumerator = null;
            }
            yield break;
            yield break;
        }

        [Saved]
        private int from;

        [Saved]
        private int to;

        [Saved]
        private bool fully;

        [Saved]
        private float? percent;
    }
}