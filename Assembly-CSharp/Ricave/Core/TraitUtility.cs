using System;

namespace Ricave.Core
{
    public static class TraitUtility
    {
        public static bool IsUnlocked(this TraitSpec traitSpec)
        {
            return Get.TraitManager.IsUnlocked(traitSpec);
        }

        public static bool IsChosen(this TraitSpec traitSpec)
        {
            return Get.TraitManager.IsChosen(traitSpec);
        }
    }
}