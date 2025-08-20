using System;

namespace Ricave.Core
{
    public static class CosmeticsUtility
    {
        public static bool IsUnlocked(this CosmeticSpec cosmeticSpec)
        {
            return Get.CosmeticsManager.IsUnlocked(cosmeticSpec);
        }

        public static bool IsChosen(this CosmeticSpec cosmeticSpec)
        {
            return Get.CosmeticsManager.IsChosen(cosmeticSpec);
        }
    }
}