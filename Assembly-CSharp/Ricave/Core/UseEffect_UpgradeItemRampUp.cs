using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class UseEffect_UpgradeItemRampUp : UseEffect
    {
        public override GoodBadNeutral GoodBadNeutral
        {
            get
            {
                return GoodBadNeutral.Good;
            }
        }

        protected UseEffect_UpgradeItemRampUp()
        {
        }

        public UseEffect_UpgradeItemRampUp(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            if (user != null && user.IsNowControlledActor)
            {
                object choice = UsePrompt.Choice;
                Item item = choice as Item;
                if (item != null && user.Inventory.Contains(item) && RampUpUtility.AffectedByRampUp(item))
                {
                    int num = item.RampUp + 1;
                    if (RampUpUtility.ResolveNonRepeatedRampUpFor(item, num) <= item.RampUp)
                    {
                        num++;
                    }
                    yield return new Instruction_SetRampUp(item, num);
                    if (ActionUtility.TargetConcernsPlayer(user) || ActionUtility.TargetConcernsPlayer(item))
                    {
                        yield return new Instruction_PlayLog("ItemRampUpUpgraded".Translate(item));
                    }
                    yield break;
                }
            }
            yield break;
        }
    }
}