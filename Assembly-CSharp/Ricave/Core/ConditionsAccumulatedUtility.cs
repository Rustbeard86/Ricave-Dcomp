using System;

namespace Ricave.Core
{
    public static class ConditionsAccumulatedUtility
    {
        public static Actor GetAffectedActor(this Conditions conditions)
        {
            if (conditions == null)
            {
                return null;
            }
            Actor actor = conditions.Parent as Actor;
            if (actor != null)
            {
                return actor;
            }
            Item item = conditions.Parent as Item;
            if (item != null && item.ConditionsEquipped == conditions)
            {
                Inventory parentInventory = item.ParentInventory;
                if (parentInventory != null && parentInventory.Equipped.IsEquipped(item))
                {
                    return parentInventory.Owner;
                }
            }
            return null;
        }
    }
}