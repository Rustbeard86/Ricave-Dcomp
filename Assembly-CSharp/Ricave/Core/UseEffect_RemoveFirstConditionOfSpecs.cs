using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class UseEffect_RemoveFirstConditionOfSpecs : UseEffect
    {
        public List<ConditionSpec> Specs
        {
            get
            {
                return this.specs;
            }
        }

        public override string LabelBase
        {
            get
            {
                return base.Spec.LabelFormat.Formatted(StringUtility.ToCommaListOr(this.specs));
            }
        }

        public override Texture2D Icon
        {
            get
            {
                return this.specs[0].Icon;
            }
        }

        protected UseEffect_RemoveFirstConditionOfSpecs()
        {
        }

        public UseEffect_RemoveFirstConditionOfSpecs(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
            ((UseEffect_RemoveFirstConditionOfSpecs)clone).specs = this.specs;
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
            bool flag = false;
            foreach (ConditionSpec conditionSpec in this.specs)
            {
                Condition firstOfSpec = conditions.GetFirstOfSpec(conditionSpec);
                if (firstOfSpec != null)
                {
                    foreach (Instruction instruction in InstructionSets_Misc.RemoveCondition(firstOfSpec, user != null && user.IsPlayerParty, true))
                    {
                        yield return instruction;
                    }
                    IEnumerator<Instruction> enumerator2 = null;
                    break;
                }
                if (flag)
                {
                    break;
                }
            }
            List<ConditionSpec>.Enumerator enumerator = default(List<ConditionSpec>.Enumerator);
            yield break;
            yield break;
        }

        [Saved(Default.New, true)]
        private List<ConditionSpec> specs = new List<ConditionSpec>();
    }
}