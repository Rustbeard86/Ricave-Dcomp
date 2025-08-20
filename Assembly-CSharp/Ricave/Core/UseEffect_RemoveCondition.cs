using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ricave.Core
{
    public class UseEffect_RemoveCondition : UseEffect
    {
        public ConditionSpec SpecToRemove
        {
            get
            {
                return this.specToRemove;
            }
        }

        public bool RemoveAll
        {
            get
            {
                return this.removeAll;
            }
        }

        public override string LabelBase
        {
            get
            {
                return base.Spec.LabelFormat.Formatted(this.specToRemove);
            }
        }

        public override Texture2D Icon
        {
            get
            {
                return this.specToRemove.Icon;
            }
        }

        protected UseEffect_RemoveCondition()
        {
        }

        public UseEffect_RemoveCondition(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
            UseEffect_RemoveCondition useEffect_RemoveCondition = (UseEffect_RemoveCondition)clone;
            useEffect_RemoveCondition.specToRemove = this.specToRemove;
            useEffect_RemoveCondition.removeAll = this.removeAll;
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            Actor actor = target.Entity as Actor;
            Conditions removeFrom;
            if (actor != null)
            {
                removeFrom = actor.Conditions;
            }
            else
            {
                Item item = target.Entity as Item;
                if (item == null)
                {
                    yield break;
                }
                removeFrom = item.ConditionsEquipped;
            }
            foreach (Condition condition in removeFrom.All.Where<Condition>((Condition x) => x.Spec == this.specToRemove).ToTemporaryList<Condition>())
            {
                if (removeFrom.Contains(condition))
                {
                    foreach (Instruction instruction in InstructionSets_Misc.RemoveCondition(condition, user != null && user.IsPlayerParty, true))
                    {
                        yield return instruction;
                    }
                    IEnumerator<Instruction> enumerator2 = null;
                    if (!this.removeAll)
                    {
                        break;
                    }
                }
            }
            List<Condition>.Enumerator enumerator = default(List<Condition>.Enumerator);
            yield break;
            yield break;
        }

        [Saved]
        private ConditionSpec specToRemove;

        [Saved]
        private bool removeAll;
    }
}