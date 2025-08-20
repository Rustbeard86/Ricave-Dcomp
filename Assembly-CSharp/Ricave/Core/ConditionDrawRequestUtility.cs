using System;
using UnityEngine;

namespace Ricave.Core
{
    public static class ConditionDrawRequestUtility
    {
        public static ConditionDrawRequest? ToDrawRequestOrNull(this Condition condition)
        {
            if (condition == null || condition.Hidden)
            {
                return null;
            }
            if (condition.Parent != null)
            {
                Item item = condition.Parent.Parent as Item;
                if (item != null && !item.Identified)
                {
                    if (item.ParentInventory != null && item.ParentInventory.Equipped.IsEquipped(item))
                    {
                        return new ConditionDrawRequest?(new ConditionDrawRequest("UnidentifiedCondition".Translate(), ConditionDrawRequestUtility.UnidentifiedConditionTipGetter, ConditionDrawRequestUtility.UnidentifiedConditionIcon, Color.white, condition.TimeStartedAffectingActor));
                    }
                    return null;
                }
            }
            return new ConditionDrawRequest?(new ConditionDrawRequest(condition));
        }

        private static readonly Texture2D UnidentifiedConditionIcon = Assets.Get<Texture2D>("Textures/UI/Conditions/Unidentified");

        private static readonly Func<string> UnidentifiedConditionTipGetter = () => "UnidentifiedConditionDesc".Translate();
    }
}