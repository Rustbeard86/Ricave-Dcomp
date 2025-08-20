using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public static class RunSpecUtility
    {
        public static bool IsUnlocked(this RunSpec runSpec)
        {
            if (!runSpec.IsMain)
            {
                return true;
            }
            RunSpec previousMainDungeon = runSpec.GetPreviousMainDungeon();
            return previousMainDungeon == null || Get.Progress.IsRunCompleted(previousMainDungeon);
        }

        public static RunSpec GetNextMainDungeon(this RunSpec runSpec)
        {
            if (!runSpec.IsMain)
            {
                return null;
            }
            List<RunSpec> mainDungeonsInOrder = Get.Progress.MainDungeonsInOrder;
            int num = mainDungeonsInOrder.IndexOf(runSpec);
            if (num < 0 || num >= mainDungeonsInOrder.Count - 1)
            {
                return null;
            }
            return mainDungeonsInOrder[num + 1];
        }

        public static RunSpec GetPreviousMainDungeon(this RunSpec runSpec)
        {
            if (!runSpec.IsMain)
            {
                return null;
            }
            List<RunSpec> mainDungeonsInOrder = Get.Progress.MainDungeonsInOrder;
            int num = mainDungeonsInOrder.IndexOf(runSpec);
            if (num <= 0)
            {
                return null;
            }
            return mainDungeonsInOrder[num - 1];
        }
    }
}