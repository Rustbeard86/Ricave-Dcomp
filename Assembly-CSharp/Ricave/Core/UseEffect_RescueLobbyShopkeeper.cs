using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class UseEffect_RescueLobbyShopkeeper : UseEffect
    {
        protected UseEffect_RescueLobbyShopkeeper()
        {
        }

        public UseEffect_RescueLobbyShopkeeper(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            if (Get.Quest_RescueLobbyShopkeeper.IsActive())
            {
                yield return new Instruction_CompleteQuest(Get.Quest_RescueLobbyShopkeeper);
            }
            yield break;
        }
    }
}