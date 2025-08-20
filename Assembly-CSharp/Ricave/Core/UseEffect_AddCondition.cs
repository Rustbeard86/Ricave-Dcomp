using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ricave.Core
{
    public class UseEffect_AddCondition : UseEffect
    {
        public override GoodBadNeutral GoodBadNeutral
        {
            get
            {
                return this.condition.GoodBadNeutral;
            }
        }

        public Condition Condition
        {
            get
            {
                return this.condition;
            }
        }

        public override string LabelBase
        {
            get
            {
                return base.Spec.LabelFormat.Formatted(this.Condition);
            }
        }

        public override Texture2D Icon
        {
            get
            {
                return this.condition.Icon;
            }
        }

        public override Color IconColor
        {
            get
            {
                return this.condition.IconColor;
            }
        }

        protected UseEffect_AddCondition()
        {
        }

        public UseEffect_AddCondition(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public UseEffect_AddCondition(UseEffectSpec spec, Condition condition, float chance = 1f, UseEffect_AddCondition.StackMode stackMode = UseEffect_AddCondition.StackMode.CanStack, int usesLeft = 0)
            : base(spec, chance, usesLeft, null, false)
        {
            this.condition = condition;
            this.stackMode = stackMode;
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
            UseEffect_AddCondition useEffect_AddCondition = (UseEffect_AddCondition)clone;
            useEffect_AddCondition.condition = this.condition;
            useEffect_AddCondition.stackMode = this.stackMode;
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            Actor actor = target.Entity as Actor;
            Conditions conditions;
            if (actor != null)
            {
                conditions = actor.Conditions;
            }
            else
            {
                Item item = target.Entity as Item;
                if (item == null)
                {
                    yield break;
                }
                conditions = item.ConditionsEquipped;
            }
            if (this.condition is Condition_Poisoned)
            {
                Actor actor2 = target.Entity as Actor;
                if (actor2 != null && actor2.ImmuneToPoison)
                {
                    yield break;
                }
            }
            if (this.condition is Condition_Burning)
            {
                Actor actor3 = target.Entity as Actor;
                if (actor3 != null && actor3.ImmuneToFire)
                {
                    yield break;
                }
            }
            if (this.condition is Condition_Burning && target.Entity is Actor && Get.CellsInfo.AnyWaterAt(target.Position))
            {
                yield break;
            }
            if (this.condition is Condition_Disease)
            {
                Actor actor4 = target.Entity as Actor;
                if (actor4 != null && actor4.ImmuneToDisease)
                {
                    yield break;
                }
            }
            Actor actor5 = target.Entity as Actor;
            if (actor5 != null && actor5.IsMainActor)
            {
                using (HashSet<TraitSpec>.Enumerator enumerator = Get.TraitManager.Chosen.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        if (enumerator.Current.ImmuneToConditionsFromDamage.Contains(this.condition.Spec))
                        {
                            yield break;
                        }
                    }
                }
            }
            if (this.condition is Condition_Bleeding)
            {
                Actor actor6 = target.Entity as Actor;
                if (actor6 != null && !actor6.Spec.Actor.CanBleed)
                {
                    yield break;
                }
            }
            Condition condition = null;
            if (this.stackMode == UseEffect_AddCondition.StackMode.NeverSameSpec)
            {
                condition = conditions.All.FirstOrDefault<Condition>((Condition x) => x.Spec == this.condition.Spec && x.OriginUseEffect == this) ?? conditions.All.FirstOrDefault<Condition>((Condition x) => x.Spec == this.condition.Spec);
            }
            else if (this.stackMode == UseEffect_AddCondition.StackMode.NeverSameSpecAndOrigin)
            {
                condition = conditions.All.FirstOrDefault<Condition>((Condition x) => x.Spec == this.condition.Spec && x.OriginUseEffect == this);
            }
            if (condition != null)
            {
                foreach (Instruction instruction in InstructionSets_Misc.TryRenewCondition(condition, this.condition))
                {
                    yield return instruction;
                }
                IEnumerator<Instruction> enumerator2 = null;
            }
            else
            {
                Condition condition2 = this.condition.Clone();
                condition2.OriginUseEffect = this;
                foreach (Instruction instruction2 in InstructionSets_Misc.AddCondition(condition2, conditions, user != null && user.IsPlayerParty, true))
                {
                    yield return instruction2;
                }
                IEnumerator<Instruction> enumerator2 = null;
            }
            yield break;
            yield break;
        }

        [Saved]
        private Condition condition;

        [Saved(UseEffect_AddCondition.StackMode.CanStack, false)]
        private UseEffect_AddCondition.StackMode stackMode;

        public enum StackMode
        {
            CanStack,

            NeverSameSpec,

            NeverSameSpecAndOrigin
        }
    }
}