using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class UseEffect_CollectBossTrophyStardust : UseEffect
    {
        public override GoodBadNeutral GoodBadNeutral
        {
            get
            {
                return GoodBadNeutral.Good;
            }
        }

        public override Texture2D Icon
        {
            get
            {
                return Get.Entity_Stardust.IconAdjusted;
            }
        }

        public override Color IconColor
        {
            get
            {
                return Get.Entity_Stardust.IconColorAdjusted;
            }
        }

        public override string LabelBase
        {
            get
            {
                UseEffects parent = base.Parent;
                BossTrophy bossTrophy = ((parent != null) ? parent.Parent : null) as BossTrophy;
                if (bossTrophy != null && bossTrophy.Boss != null)
                {
                    return base.Spec.LabelFormat.Formatted(bossTrophy.Boss.KilledOnFloor);
                }
                return base.LabelBase;
            }
        }

        protected UseEffect_CollectBossTrophyStardust()
        {
        }

        public UseEffect_CollectBossTrophyStardust(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            BossTrophy bossTrophy = usable as BossTrophy;
            if (bossTrophy == null || !bossTrophy.Boss.TrophyHasStardust || bossTrophy.Boss.KilledOnFloor <= 0 || user == null)
            {
                yield break;
            }
            yield return new Instruction_Sound(Get.Sound_PickUpSpecialItem, new Vector3?(user.Position), 1f, 1f);
            if (user.IsPlayerParty)
            {
                yield return new Instruction_ChangeLobbyItems(Get.Entity_Stardust, bossTrophy.Boss.KilledOnFloor);
            }
            yield return new Instruction_ChangeBossTrophyHasStardust(bossTrophy.BossIndex, false);
            if (user.IsPlayerParty)
            {
                yield return new Instruction_PlayLog("PlayerCollectedBossTrophyStardust".Translate(Get.Entity_Stardust, bossTrophy.Boss.KilledOnFloor.ToStringCached()));
            }
            yield break;
        }
    }
}