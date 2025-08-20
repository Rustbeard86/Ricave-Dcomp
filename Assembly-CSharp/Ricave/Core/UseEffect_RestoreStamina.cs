using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class UseEffect_RestoreStamina : UseEffect
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

        public override string LabelBase
        {
            get
            {
                return base.Spec.LabelFormat.Formatted(this.fully ? "RestoreAll".Translate() : StringUtility.RangeToString(this.from, this.to));
            }
        }

        protected UseEffect_RestoreStamina()
        {
        }

        public UseEffect_RestoreStamina(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
            UseEffect_RestoreStamina useEffect_RestoreStamina = (UseEffect_RestoreStamina)clone;
            useEffect_RestoreStamina.from = this.from;
            useEffect_RestoreStamina.to = this.to;
            useEffect_RestoreStamina.fully = this.fully;
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            Actor actor = target.Entity as Actor;
            if (actor == null)
            {
                yield break;
            }
            int num = (this.fully ? (actor.MaxStamina - actor.Stamina) : Rand.RangeInclusive(this.from, this.to));
            num = Math.Min(num, actor.MaxStamina - actor.Stamina);
            if (num > 0)
            {
                foreach (Instruction instruction in InstructionSets_Actor.RestoreStamina(actor, num, user != null && user.IsPlayerParty, true, true))
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
    }
}