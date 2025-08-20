using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public static class AchievementsUtility
    {
        public static int AchievementsCount
        {
            get
            {
                return Get.Specs.GetAll<AchievementSpec>().Count;
            }
        }

        public static int CompletedAchievementsCount
        {
            get
            {
                int num = 0;
                using (List<AchievementSpec>.Enumerator enumerator = Get.Specs.GetAll<AchievementSpec>().GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        if (enumerator.Current.IsCompleted)
                        {
                            num++;
                        }
                    }
                }
                return num;
            }
        }
    }
}