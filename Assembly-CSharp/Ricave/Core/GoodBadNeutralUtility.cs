using System;
using UnityEngine;

namespace Ricave.Core
{
    public static class GoodBadNeutralUtility
    {
        public static Color GetColor(this GoodBadNeutral status)
        {
            switch (status)
            {
                case GoodBadNeutral.Neutral:
                    return Color.white;
                case GoodBadNeutral.Good:
                    return GoodBadNeutralUtility.GoodEffectColor;
                case GoodBadNeutral.Bad:
                    return GoodBadNeutralUtility.BadEffectColor;
                default:
                    return Color.white;
            }
        }

        public static readonly Color GoodEffectColor = new Color(0.3f, 1f, 0.3f);

        public static readonly Color BadEffectColor = new Color(1f, 0.25f, 0.25f);
    }
}