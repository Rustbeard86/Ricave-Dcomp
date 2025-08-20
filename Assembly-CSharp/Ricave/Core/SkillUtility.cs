using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public static class SkillUtility
    {
        public static bool IsUnlocked(this SkillSpec skillSpec)
        {
            return Get.SkillManager.IsUnlocked(skillSpec);
        }

        public static bool TwoAdjacentEnemiesToPlayer
        {
            get
            {
                int num = 0;
                foreach (Vector3Int vector3Int in Get.MainActor.Position.GetCellsWithin(1))
                {
                    if (vector3Int.InBounds())
                    {
                        foreach (Entity entity in Get.World.GetEntitiesAt(vector3Int))
                        {
                            Actor actor = entity as Actor;
                            if (actor != null && actor != Get.MainActor && actor.IsHostile(Get.MainActor) && (LineOfSight.IsLineOfSight(Get.MainActor.Position, actor.Position) || LineOfSight.IsLineOfFire(Get.MainActor.Position, actor.Position)))
                            {
                                num++;
                                if (num >= 2)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
                return false;
            }
        }

        public static Actor GetActorToDeflectBlockedDamageOnto(Actor damageFromActor)
        {
            List<Actor> list = null;
            Actor actor3;
            try
            {
                foreach (Vector3Int vector3Int in Get.MainActor.Position.GetCellsWithin(1))
                {
                    if (vector3Int.InBounds())
                    {
                        foreach (Entity entity in Get.World.GetEntitiesAt(vector3Int))
                        {
                            Actor actor = entity as Actor;
                            if (actor != null && actor != Get.MainActor && actor != damageFromActor && actor.IsHostile(Get.MainActor) && LineOfSight.IsLineOfFire(Get.MainActor.Position, actor.Position))
                            {
                                if (list == null)
                                {
                                    list = Pool<List<Actor>>.Get();
                                    list.Clear();
                                }
                                list.Add(actor);
                            }
                        }
                    }
                }
                Actor actor2;
                if (list != null && list.TryGetRandomElement<Actor>(out actor2))
                {
                    actor3 = actor2;
                }
                else
                {
                    actor3 = null;
                }
            }
            finally
            {
                if (list != null)
                {
                    list.Clear();
                    Pool<List<Actor>>.Return(list);
                }
            }
            return actor3;
        }

        public static bool IsLockpickable(Structure structure)
        {
            return structure.Spec == Get.Entity_GreenChest || structure.Spec == Get.Entity_AncientContainer || structure.Spec == Get.Entity_RoyalDoor;
        }
    }
}