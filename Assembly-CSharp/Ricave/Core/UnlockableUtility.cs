using System;

namespace Ricave.Core
{
    public static class UnlockableUtility
    {
        public static bool IsUnlocked(this UnlockableSpec unlockableSpec)
        {
            return Get.UnlockableManager.IsUnlocked(unlockableSpec, null);
        }
    }
}