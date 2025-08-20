using System;

namespace Ricave.Core
{
    public static class LevelUtility
    {
        public static int MaxHPPerLevelAdjusted
        {
            get
            {
                return Calc.RoundToIntHalfUp((float)(5 + Get.TraitManager.MaxHPOffsetPerLevel) * Get.TraitManager.MaxHPFactor);
            }
        }

        public static int GetLevel(int experience)
        {
            int num = 1;
            while (experience >= LevelUtility.GetTotalExperienceRequired(num))
            {
                num++;
            }
            return num - 1;
        }

        public static int GetTotalExperienceRequired(int level)
        {
            level--;
            return level * (3 * level + 7) / 2;
        }

        public static int GetExperienceSinceLeveling(int experience)
        {
            return experience - LevelUtility.GetTotalExperienceRequired(LevelUtility.GetLevel(experience));
        }

        public const int MaxHPPerLevel = 5;

        public const int MaxHPPerLevel_OtherPartyMember = 3;
    }
}