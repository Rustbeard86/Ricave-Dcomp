using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Ricave.Core
{
    public static class TraitRarityUtility
    {
        [return: TupleElementNames(new string[] { "label", "color" })]
        public static ValueTuple<string, Color> GetLabelAndColor(this TraitRarity rarity)
        {
            switch (rarity)
            {
                case TraitRarity.Common:
                    return new ValueTuple<string, Color>("Common".Translate(), new Color(0.65f, 0.65f, 0.65f));
                case TraitRarity.Uncommon:
                    return new ValueTuple<string, Color>("Uncommon".Translate(), new Color(0.6f, 1f, 0.6f));
                case TraitRarity.Rare:
                    return new ValueTuple<string, Color>("Rare".Translate(), new Color(0.5f, 0.75f, 1f));
                case TraitRarity.Epic:
                    return new ValueTuple<string, Color>("Epic".Translate(), new Color(0.78f, 0.38f, 0.89f));
                default:
                    return new ValueTuple<string, Color>(null, Color.white);
            }
        }
    }
}