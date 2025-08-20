using System;

namespace Ricave.Core
{
    public static class ActionUtility
    {
        public static bool TargetConcernsPlayer(Target target)
        {
            if (!target.IsValid)
            {
                return false;
            }
            if (Get.NowControlledActor == null)
            {
                return false;
            }
            if (target.IsPlayerParty || target.IsNowControlledActor)
            {
                return true;
            }
            if (Get.NowControlledActor.Sees(target, null))
            {
                return true;
            }
            Actor actor = target.Entity as Actor;
            if (actor != null && Get.NowControlledActor.AIMemory.AwareOf.Contains(actor))
            {
                return true;
            }
            Item item = target.Entity as Item;
            return item != null && Get.NowControlledActor.Inventory.Contains(item);
        }

        public static bool UsableConcernsPlayer(IUsable usable)
        {
            Entity entity = usable as Entity;
            if (entity == null || !ActionUtility.TargetConcernsPlayer(entity))
            {
                NativeWeapon nativeWeapon = usable as NativeWeapon;
                if (nativeWeapon == null || Get.NowControlledActor == null || nativeWeapon.Actor != Get.NowControlledActor)
                {
                    Spell spell = usable as Spell;
                    if ((spell == null || Get.NowControlledActor == null || spell.Parent == null || spell.Parent.Owner != Get.NowControlledActor) && !(usable is PrivateRoomPickUpStructureUsable) && !(usable is PrivateRoomPlaceStructureUsable))
                    {
                        KickUsable kickUsable = usable as KickUsable;
                        if (kickUsable == null || Get.NowControlledActor == null || kickUsable.Actor != Get.NowControlledActor)
                        {
                            return usable.UseEffects != null && Get.NowControlledActor != null && usable.UseEffects.GetWieldingActor() == Get.NowControlledActor;
                        }
                    }
                }
            }
            return true;
        }
    }
}