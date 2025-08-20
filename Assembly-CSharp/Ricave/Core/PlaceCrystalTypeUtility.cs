using System;

namespace Ricave.Core
{
    public static class PlaceCrystalTypeUtility
    {
        public static string GetLabel(this Place.CrystalType crystal)
        {
            switch (crystal)
            {
                case Place.CrystalType.Red:
                    return "CrystalType_Red".Translate();
                case Place.CrystalType.Green:
                    return "CrystalType_Green".Translate();
                case Place.CrystalType.Blue:
                    return "CrystalType_Blue".Translate();
                default:
                    return "";
            }
        }

        public static string GetLabelCap(this Place.CrystalType crystal)
        {
            return crystal.GetLabel().CapitalizeFirst();
        }
    }
}