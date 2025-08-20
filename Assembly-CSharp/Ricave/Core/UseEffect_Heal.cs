using System;
using System.Collections.Generic;
using Ricave.UI;

namespace Ricave.Core
{
    public class UseEffect_Heal : UseEffect
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
                    int num = Calc.RoundToIntHalfUp((float)Get.NowControlledActor.MaxHP * this.percent.Value);
                    return "{0} ({1})".Formatted(text, "RestoredAmountForPlayer".Translate(num));
                }
                string text2 = base.Spec.LabelFormat.Formatted(StringUtility.RangeToString(this.from, this.to));
                ValueTuple<int, int> valueTuple = this.AdjustedFromToFor(Get.NowControlledActor);
                if (valueTuple.Item1 != this.from || valueTuple.Item2 != this.to)
                {
                    string text3 = "PostProcessedPlayerHealAmount".Translate(StringUtility.RangeToString(valueTuple.Item1, valueTuple.Item2));
                    text2 = text2.AppendedWithSpace(RichText.Grayed("({0})".Formatted(text3)));
                }
                return text2;
            }
        }

        protected UseEffect_Heal()
        {
        }

        public UseEffect_Heal(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
            UseEffect_Heal useEffect_Heal = (UseEffect_Heal)clone;
            useEffect_Heal.from = this.from;
            useEffect_Heal.to = this.to;
            useEffect_Heal.fully = this.fully;
            useEffect_Heal.percent = this.percent;
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            if (!target.IsEntity || !target.Spawned)
            {
                yield break;
            }
            int num;
            if (this.fully)
            {
                num = target.Entity.MaxHP - target.Entity.HP;
            }
            else if (this.percent != null)
            {
                num = Calc.RoundToIntHalfUp((float)target.Entity.MaxHP * this.percent.Value);
            }
            else
            {
                ValueTuple<int, int> valueTuple = this.AdjustedFromToFor(target);
                num = Rand.RangeInclusive(valueTuple.Item1, valueTuple.Item2);
            }
            num = Math.Min(num, target.Entity.MaxHP - target.Entity.HP);
            if (num > 0)
            {
                foreach (Instruction instruction in InstructionSets_Entity.Heal(target.Entity, num, user != null && user.IsPlayerParty, true))
                {
                    yield return instruction;
                }
                IEnumerator<Instruction> enumerator = null;
            }
            yield break;
            yield break;
        }

        public ValueTuple<int, int> AdjustedFromToFor(Target target)
        {
            int num = this.from;
            int num2 = this.to;
            if (target.IsMainActor && Get.Skill_MoreHealthFromFood.IsUnlocked())
            {
                UseEffects parent = base.Parent;
                Item item = ((parent != null) ? parent.Parent : null) as Item;
                if (item != null && item.Spec.Item.IsFood)
                {
                    num *= 2;
                    num2 *= 2;
                }
            }
            return new ValueTuple<int, int>(num, num2);
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