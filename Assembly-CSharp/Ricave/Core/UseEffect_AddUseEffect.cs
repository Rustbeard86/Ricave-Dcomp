using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class UseEffect_AddUseEffect : UseEffect
    {
        public override GoodBadNeutral GoodBadNeutral
        {
            get
            {
                return this.useEffect.GoodBadNeutral;
            }
        }

        public UseEffect UseEffect
        {
            get
            {
                return this.useEffect;
            }
        }

        public override string LabelBase
        {
            get
            {
                return base.Spec.LabelFormat.Formatted(this.UseEffect);
            }
        }

        public override Texture2D Icon
        {
            get
            {
                return this.useEffect.Icon;
            }
        }

        public override Color IconColor
        {
            get
            {
                return this.useEffect.IconColor;
            }
        }

        protected UseEffect_AddUseEffect()
        {
        }

        public UseEffect_AddUseEffect(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
            ((UseEffect_AddUseEffect)clone).useEffect = this.useEffect;
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            Actor actor = target.Entity as Actor;
            UseEffects useEffects;
            if (actor != null && actor.NativeWeapons.Count != 0)
            {
                useEffects = actor.NativeWeapons[0].UseEffects;
            }
            else
            {
                Item item = target.Entity as Item;
                if (item == null)
                {
                    yield break;
                }
                useEffects = item.UseEffects;
            }
            UseEffect useEffect = this.useEffect.Clone();
            foreach (Instruction instruction in InstructionSets_Misc.AddUseEffect(useEffect, useEffects, user != null && user.IsPlayerParty))
            {
                yield return instruction;
            }
            IEnumerator<Instruction> enumerator = null;
            yield break;
            yield break;
        }

        [Saved]
        private UseEffect useEffect;
    }
}