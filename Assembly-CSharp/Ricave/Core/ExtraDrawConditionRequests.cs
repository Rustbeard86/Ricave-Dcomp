using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public static class ExtraDrawConditionRequests
    {
        public static void GetExtraConditionDrawRequests(Actor actor, List<ConditionDrawRequest> outDrawRequests)
        {
            if (Get.NowControlledActor != null && !actor.IsNowControlledActor && !actor.Sees(Get.NowControlledActor, null) && actor.Spawned)
            {
                outDrawRequests.Add(new ConditionDrawRequest("CantSeePlayer".Translate(), ExtraDrawConditionRequests.CantSeePlayerTipGetter, ExtraDrawConditionRequests.CantSeePlayerIcon, Color.white, -99999f));
            }
            if (Get.NowControlledActor != null && !actor.IsNowControlledActor && Get.NowControlledActor.CanSurpriseAttack(actor) && Get.VisibilityCache.PlayerSees(actor))
            {
                outDrawRequests.Add(new ConditionDrawRequest("CanSurpriseAttack".Translate(), ExtraDrawConditionRequests.CanSurpriseAttackTipGetter, ExtraDrawConditionRequests.SurpriseAttackIcon, Color.white, -99999f));
            }
            if (Get.NowControlledActor != null && !actor.IsNowControlledActor)
            {
                if (GravityUtility.IsAltitudeLower(actor.Position, Get.NowControlledActor.Position, actor.Gravity))
                {
                    outDrawRequests.Add(new ConditionDrawRequest("BelowYou".Translate(), ExtraDrawConditionRequests.BelowYouTipGetter, ExtraDrawConditionRequests.BelowYouIcon, Color.white, -99999f));
                }
                else if (GravityUtility.IsAltitudeLower(Get.NowControlledActor.Position, actor.Position, Get.NowControlledActor.Gravity))
                {
                    outDrawRequests.Add(new ConditionDrawRequest("AboveYou".Translate(), ExtraDrawConditionRequests.AboveYouTipGetter, ExtraDrawConditionRequests.AboveYouIcon, Color.white, -99999f));
                }
            }
            if (!actor.IsNowControlledActor && Get.CellsInfo.IsFallingAt(actor.Position, actor, false))
            {
                outDrawRequests.Add(new ConditionDrawRequest("Falling".Translate(), ExtraDrawConditionRequests.FallingTipGetter, ExtraDrawConditionRequests.FallingIcon, Color.white, -99999f));
            }
            if (!actor.IsNowControlledActor && Get.NowControlledActor == Get.MainActor && Get.Skill_SurroundedBonus.IsUnlocked() && SkillUtility.TwoAdjacentEnemiesToPlayer)
            {
                outDrawRequests.Add(new ConditionDrawRequest(Get.Skill_SurroundedBonus.LabelCap, ExtraDrawConditionRequests.SurroundedBonusTipGetter, Get.Skill_SurroundedBonus.Icon, Color.white, -99999f));
            }
        }

        private static readonly Texture2D SurpriseAttackIcon = Assets.Get<Texture2D>("Textures/UI/Conditions/CanSurpriseAttack");

        private static readonly Texture2D CantSeePlayerIcon = Assets.Get<Texture2D>("Textures/UI/Conditions/CantSeePlayer");

        private static readonly Texture2D FallingIcon = Assets.Get<Texture2D>("Textures/UI/Conditions/Falling");

        private static readonly Texture2D BelowYouIcon = Assets.Get<Texture2D>("Textures/UI/Conditions/BelowYou");

        private static readonly Texture2D AboveYouIcon = Assets.Get<Texture2D>("Textures/UI/Conditions/AboveYou");

        private static readonly Func<string> CantSeePlayerTipGetter = () => "CantSeePlayerDesc".Translate();

        private static readonly Func<string> CanSurpriseAttackTipGetter = () => "CanSurpriseAttackDesc".Translate(0.39999998f.ToStringPercent(false));

        private static readonly Func<string> FallingTipGetter = () => "FallingDesc".Translate();

        private static readonly Func<string> BelowYouTipGetter = () => "BelowYouDesc".Translate(1);

        private static readonly Func<string> AboveYouTipGetter = () => "AboveYouDesc".Translate(1);

        private static readonly Func<string> SurroundedBonusTipGetter = () => "SurroundedBonusDesc".Translate(0.25f.ToStringPercent(false));
    }
}