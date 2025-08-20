using System;
using UnityEngine;

namespace Ricave.Core
{
    public static class UseEffectDrawRequestUtility
    {
        public static UseEffectDrawRequest? ToDrawRequestOrNull(this UseEffect useEffect)
        {
            if (useEffect == null || useEffect.Hidden)
            {
                return null;
            }
            if (useEffect.Parent != null)
            {
                Item item = useEffect.Parent.Parent as Item;
                if (item != null && !item.Identified)
                {
                    if (item.ParentInventory != null && item.ParentInventory.Equipped.IsEquipped(item))
                    {
                        return new UseEffectDrawRequest?(new UseEffectDrawRequest("UnidentifiedUseEffect".Translate(), UseEffectDrawRequestUtility.UnidentifiedUseEffectTipGetter, UseEffectDrawRequestUtility.UnidentifiedUseEffectIcon, Color.white));
                    }
                    return null;
                }
            }
            return new UseEffectDrawRequest?(new UseEffectDrawRequest(useEffect));
        }

        private static readonly Texture2D UnidentifiedUseEffectIcon = Assets.Get<Texture2D>("Textures/UI/UseEffects/Unidentified");

        private static readonly Func<string> UnidentifiedUseEffectTipGetter = () => "UnidentifiedUseEffectDesc".Translate();
    }
}