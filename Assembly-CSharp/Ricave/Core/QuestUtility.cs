using System;

namespace Ricave.Core
{
    public static class QuestUtility
    {
        public static bool IsCompleted(this QuestSpec questSpec)
        {
            return Get.QuestManager.IsCompleted(questSpec) || Get.ThisRunCompletedQuests.IsMarkedCompleted(questSpec);
        }

        public static bool IsCompletedAndClaimed(this QuestSpec questSpec)
        {
            return Get.QuestManager.IsRewardClaimed(questSpec);
        }

        public static bool IsActive(this QuestSpec questSpec)
        {
            return Get.QuestManager.IsActive(questSpec) && !Get.ThisRunCompletedQuests.IsMarkedCompleted(questSpec);
        }
    }
}