using System;

namespace Ricave.Core
{
    public static class UseEffectsUtility
    {
        public static Actor GetWieldingActor(this UseEffects useEffects)
        {
            if (useEffects == null)
            {
                return null;
            }
            Item item = useEffects.Parent as Item;
            if (item != null)
            {
                Inventory parentInventory = item.ParentInventory;
                if (parentInventory != null && parentInventory.Equipped.IsEquipped(item))
                {
                    return parentInventory.Owner;
                }
            }
            else
            {
                NativeWeapon nativeWeapon = useEffects.Parent as NativeWeapon;
                if (nativeWeapon != null)
                {
                    return nativeWeapon.Actor;
                }
                Spell spell = useEffects.Parent as Spell;
                if (spell != null)
                {
                    Spells parent = spell.Parent;
                    if (parent == null)
                    {
                        return null;
                    }
                    return parent.Owner;
                }
                else
                {
                    GenericUsable genericUsable = useEffects.Parent as GenericUsable;
                    if (genericUsable != null)
                    {
                        return genericUsable.ParentEntity as Actor;
                    }
                    if (useEffects.Parent is PrivateRoomPickUpStructureUsable)
                    {
                        return Get.MainActor;
                    }
                    if (useEffects.Parent is PrivateRoomPlaceStructureUsable)
                    {
                        return Get.MainActor;
                    }
                    KickUsable kickUsable = useEffects.Parent as KickUsable;
                    if (kickUsable != null)
                    {
                        return kickUsable.Actor;
                    }
                }
            }
            return null;
        }

        public static Entity GetWieldingActorOrStructure(this UseEffects useEffects)
        {
            if (useEffects == null)
            {
                return null;
            }
            Actor wieldingActor = useEffects.GetWieldingActor();
            if (wieldingActor != null)
            {
                return wieldingActor;
            }
            Structure structure = useEffects.Parent as Structure;
            if (structure != null)
            {
                return structure;
            }
            return null;
        }
    }
}