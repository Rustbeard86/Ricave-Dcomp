using System;
using System.Collections.Generic;
using System.Linq;
using Ricave.UI;

namespace Ricave.Core
{
    public class UseEffect_IdentifyTurns : UseEffect
    {
        public override GoodBadNeutral GoodBadNeutral
        {
            get
            {
                return GoodBadNeutral.Good;
            }
        }

        public int IdentifyTurns
        {
            get
            {
                return this.identifyTurns;
            }
        }

        public override string LabelBase
        {
            get
            {
                return base.Spec.LabelFormat.Formatted(RichText.Turns("({0})".Formatted("WorthOfTurns".Translate(this.identifyTurns))));
            }
        }

        protected UseEffect_IdentifyTurns()
        {
        }

        public UseEffect_IdentifyTurns(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
            ((UseEffect_IdentifyTurns)clone).identifyTurns = this.identifyTurns;
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            Actor targetActor = target.Entity as Actor;
            if (targetActor == null)
            {
                yield break;
            }
            int leftTurns = this.identifyTurns;
            Func<Item, bool> <> 9__0;
            while (leftTurns > 0)
            {
                IEnumerable<Item> unidentifiedItemsInIdentifyOrder = targetActor.Inventory.UnidentifiedItemsInIdentifyOrder;
                Func<Item, bool> func;
                if ((func = <> 9__0) == null)
                {
                    func = (<> 9__0 = (Item x) => x != usable);
                }
                Item item = unidentifiedItemsInIdentifyOrder.FirstOrDefault<Item>(func);
                if (item == null)
                {
                    break;
                }
                int num = Math.Min(leftTurns, item.TurnsLeftToIdentify);
                leftTurns -= num;
                foreach (Instruction instruction in InstructionSets_Entity.Identify(item, num, true))
                {
                    yield return instruction;
                }
                IEnumerator<Instruction> enumerator = null;
            }
            yield break;
            yield break;
        }

        [Saved]
        private int identifyTurns;
    }
}