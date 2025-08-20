using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class TargetFilter
    {
        public bool Allows(Target target, Actor user = null)
        {
            if (this.disallowMainActor && target.IsMainActor)
            {
                return false;
            }
            if (this.disallowPlayerParty && target.IsPlayerParty)
            {
                return false;
            }
            if (this.disallowUser && target == user)
            {
                return false;
            }
            if (this.disallowFlyingActors)
            {
                Actor actor = target.Entity as Actor;
                if (actor != null && actor.CanFly)
                {
                    return false;
                }
            }
            if (this.disallowActorsWithCondition != null)
            {
                Actor actor2 = target.Entity as Actor;
                if (actor2 != null)
                {
                    List<Condition> allConditions = actor2.ConditionsAccumulated.AllConditions;
                    for (int i = 0; i < allConditions.Count; i++)
                    {
                        if (allConditions[i].Spec == this.disallowActorsWithCondition)
                        {
                            return false;
                        }
                    }
                }
            }
            if (this.disallowOutsidePrivateRoom)
            {
                RetainedRoomInfo.RoomInfo roomOfSpecAt = Get.RetainedRoomInfo.GetRoomOfSpecAt(target.Position, Get.Room_PrivateRoom);
                if (roomOfSpecAt == null || !roomOfSpecAt.Shape.InnerCuboid(1).Contains(target.Position))
                {
                    return false;
                }
            }
            if (this.disallowSleeping)
            {
                Actor actor3 = target.Entity as Actor;
                if (actor3 != null && actor3.ConditionsAccumulated.AnyOfSpec(Get.Condition_Sleeping))
                {
                    return false;
                }
            }
            if (this.disallowNonHostile && user != null)
            {
                Actor actor4 = target.Entity as Actor;
                if (actor4 != null && !user.IsHostile(actor4))
                {
                    return false;
                }
            }
            if (this.allowActors && target.Entity is Actor)
            {
                return true;
            }
            if (this.allowAttackables && target.IsEntity && target.Entity.MaxHP != 0)
            {
                return true;
            }
            if (this.allowMainActor && target.IsMainActor)
            {
                return true;
            }
            if (this.allowPlayerParty && target.IsPlayerParty)
            {
                return true;
            }
            if (this.allowUser && target == user)
            {
                return true;
            }
            if (this.allowFlammable && target.IsEntity && (target.Entity.Spec.IsActor || (target.Entity.Spec.IsStructure && target.Entity.Spec.Structure.Flammable)))
            {
                return true;
            }
            if (this.allowFragile && target.IsEntity && target.Entity.Spec.IsStructure && target.Entity.Spec.Structure.Fragile)
            {
                return true;
            }
            if (this.allowLocations && target.IsLocation)
            {
                return true;
            }
            if (this.allowNonFilledLocations && target.IsLocation && !Get.CellsInfo.IsFilled(target.Position))
            {
                return true;
            }
            if (this.allowPassableLocations && target.IsLocation && Get.CellsInfo.CanPassThrough(target.Position))
            {
                return true;
            }
            if (this.allowPrivateRoomStructures && target.IsEntity)
            {
                Structure structure = target.Entity as Structure;
                if (structure != null && Get.PrivateRoom.IsPlacedStructure(structure))
                {
                    return true;
                }
            }
            if (this.allowWater && target.IsEntity && target.Entity is Structure && target.Entity.Spec.Structure.IsWater)
            {
                return true;
            }
            if (this.allowFluidSources && target.IsEntity && target.Entity is Structure && target.Entity.Spec.Structure.IsFluidSource)
            {
                return true;
            }
            if (this.allowFishingable && target.IsEntity && target.Entity is Structure && target.Entity.Spec.Structure.Fishingable)
            {
                return true;
            }
            if (this.allowExtinguishable && target.IsEntity)
            {
                Actor actor5 = target.Entity as Actor;
                if (actor5 == null || !actor5.Conditions.AnyOfSpec(Get.Condition_Burning))
                {
                    Structure structure2 = target.Entity as Structure;
                    if (structure2 == null || !structure2.Spec.Structure.Extinguishable)
                    {
                        goto IL_03E4;
                    }
                }
                return true;
            }
        IL_03E4:
            if (this.allowMineable && target.IsEntity && target.Entity is Structure && target.Entity.Spec.Structure.IsMineable)
            {
                return true;
            }
            if (this.allowDiggable && target.IsEntity && target.Entity is Structure && target.Entity.Spec.Structure.IsDiggable)
            {
                return true;
            }
            if (this.allowGlass && target.IsEntity && target.Entity is Structure && target.Entity.Spec.Structure.IsGlass)
            {
                return true;
            }
            if (this.allowCookable && target.IsEntity && target.Entity is Item && target.Entity.Spec.Item.CookedItemSpec != null)
            {
                return true;
            }
            if (this.allowActorsWithAnythingStealable)
            {
                Actor actor6 = target.Entity as Actor;
                if (actor6 != null && this.HasAnythingStealable(actor6))
                {
                    return true;
                }
            }
            if (this.allowActorsWithHunger)
            {
                Actor actor7 = target.Entity as Actor;
                if (actor7 != null && actor7.Conditions.AnyOfSpec(Get.Condition_Hunger))
                {
                    return true;
                }
            }
            Vector3Int vector3Int;
            Vector3Int vector3Int2;
            return (this.allowLocationsWhereCanSpawn != null && target.IsLocation && SpawnPositionFinder.CanSpawnAt(this.allowLocationsWhereCanSpawn, target.Position, null, false, null)) || (this.allowLocationsWhereCanSpawnUntilCeilingAndFloor != null && target.IsLocation && UseEffect_SpawnUntilCeilingAndFloor.TryFindStartSpawnPos(this.allowLocationsWhereCanSpawnUntilCeilingAndFloor, target.Position, out vector3Int, out vector3Int2)) || (this.allowEntity != null && target.IsEntity && target.Entity.Spec == this.allowEntity) || (this.allowEntities != null && target.IsEntity && this.allowEntities.Contains(target.Entity.Spec));
        }

        public static TargetFilter ForPrivateRoomPlaceStructure(EntitySpec structureSpec)
        {
            return new TargetFilter
            {
                disallowOutsidePrivateRoom = true,
                allowLocationsWhereCanSpawn = structureSpec
            };
        }

        public static TargetFilter ForPrivateRoomPickUpStructure()
        {
            if (TargetFilter.forPrivateRoomPickUpStructure == null)
            {
                TargetFilter.forPrivateRoomPickUpStructure = new TargetFilter
                {
                    allowPrivateRoomStructures = true
                };
            }
            return TargetFilter.forPrivateRoomPickUpStructure;
        }

        public static TargetFilter ForKick()
        {
            if (TargetFilter.forKick == null)
            {
                TargetFilter.forKick = new TargetFilter
                {
                    allowEntity = Get.Entity_Door
                };
            }
            return TargetFilter.forKick;
        }

        private bool HasAnythingStealable(Actor actor)
        {
            foreach (Item item in actor.Inventory.AllItems)
            {
                if (!item.Cursed || !actor.Inventory.Equipped.IsEquipped(item))
                {
                    return true;
                }
            }
            return false;
        }

        [Saved]
        private bool disallowMainActor;

        [Saved]
        private bool disallowPlayerParty;

        [Saved]
        private bool disallowUser;

        [Saved]
        private bool disallowFlyingActors;

        [Saved]
        private ConditionSpec disallowActorsWithCondition;

        [Saved]
        private bool disallowOutsidePrivateRoom;

        [Saved]
        private bool disallowSleeping;

        [Saved]
        private bool disallowNonHostile;

        [Saved]
        private bool allowActors;

        [Saved]
        private bool allowAttackables;

        [Saved]
        private bool allowMainActor;

        [Saved]
        private bool allowPlayerParty;

        [Saved]
        private bool allowUser;

        [Saved]
        private bool allowFlammable;

        [Saved]
        private bool allowFragile;

        [Saved]
        private bool allowLocations;

        [Saved]
        private bool allowNonFilledLocations;

        [Saved]
        private bool allowPassableLocations;

        [Saved]
        private bool allowPrivateRoomStructures;

        [Saved]
        private bool allowWater;

        [Saved]
        private bool allowFluidSources;

        [Saved]
        private bool allowFishingable;

        [Saved]
        private bool allowExtinguishable;

        [Saved]
        private bool allowMineable;

        [Saved]
        private bool allowDiggable;

        [Saved]
        private bool allowCookable;

        [Saved]
        private bool allowGlass;

        [Saved]
        private bool allowActorsWithAnythingStealable;

        [Saved]
        private bool allowActorsWithHunger;

        [Saved]
        private EntitySpec allowLocationsWhereCanSpawn;

        [Saved]
        private EntitySpec allowLocationsWhereCanSpawnUntilCeilingAndFloor;

        [Saved]
        private EntitySpec allowEntity;

        [Saved]
        private List<EntitySpec> allowEntities;

        private static TargetFilter forPrivateRoomPickUpStructure;

        private static TargetFilter forKick;
    }
}