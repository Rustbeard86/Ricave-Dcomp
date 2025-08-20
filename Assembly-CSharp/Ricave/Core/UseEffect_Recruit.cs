using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class UseEffect_Recruit : UseEffect
    {
        public override GoodBadNeutral GoodBadNeutral
        {
            get
            {
                return GoodBadNeutral.Good;
            }
        }

        protected UseEffect_Recruit()
        {
        }

        public UseEffect_Recruit(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            GenericUsable genericUsable = usable as GenericUsable;
            Actor targetActor = ((genericUsable != null) ? genericUsable.ParentEntity : null) as Actor;
            if (targetActor == null)
            {
                yield break;
            }
            if (targetActor.Faction != Get.Player.Faction && Get.Player.Faction != null)
            {
                yield return new Instruction_SetFaction(targetActor, Get.Player.Faction);
            }
            if (!targetActor.IsPlayerParty)
            {
                yield return new Instruction_AddPartyMember(targetActor);
            }
            yield break;
        }
    }
}