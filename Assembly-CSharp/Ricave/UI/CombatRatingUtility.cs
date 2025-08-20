using System;
using Ricave.Core;

namespace Ricave.UI
{
    public static class CombatRatingUtility
    {
        public static CombatRating CombatRating
        {
            get
            {
                if (Get.Player.ExpFromKillingEnemies < 5 || Get.Player.ExpectedHPLostFromEnemies == 0f)
                {
                    return CombatRating.Unknown;
                }
                float num = (float)Get.Player.HPLostFromEnemies / Get.Player.ExpectedHPLostFromEnemies;
                if (num < 0.5f)
                {
                    return CombatRating.S;
                }
                if (num < 0.8f)
                {
                    return CombatRating.A;
                }
                if (num < 1.15f)
                {
                    return CombatRating.B;
                }
                if (num < 1.6f)
                {
                    return CombatRating.C;
                }
                return CombatRating.D;
            }
        }

        public static string GetLabel(this CombatRating combatRating)
        {
            switch (combatRating)
            {
                case CombatRating.Unknown:
                    return "CombatRating_Unknown".Translate();
                case CombatRating.S:
                    return "CombatRating_S".Translate();
                case CombatRating.A:
                    return "CombatRating_A".Translate();
                case CombatRating.B:
                    return "CombatRating_B".Translate();
                case CombatRating.C:
                    return "CombatRating_C".Translate();
                case CombatRating.D:
                    return "CombatRating_D".Translate();
                default:
                    return "";
            }
        }
    }
}