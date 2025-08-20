using System;
using System.Collections.Generic;
using Ricave.Core;
using UnityEngine;

namespace Ricave.Rendering
{
    public static class RaycastUtility
    {
        private static Vector2 ScreenCenterForRaycasts
        {
            get
            {
                return new Vector2((float)Sys.Resolution.x / 2f, (float)Sys.Resolution.y / 2f);
            }
        }

        public static GameObject RaycastFromCamera(float distance, out Vector3Int hitDirection, out Vector3 hitPoint, bool ignoreFloorMarkers = true, bool includeInspectModeOnly = false, bool canReorder = true, Predicate<GameObject> shouldIgnore = null)
        {
            Ray ray = Get.Camera.ScreenPointToRay(RaycastUtility.ScreenCenterForRaycasts);
            return RaycastUtility.Raycast(ray.origin, ray.direction, distance, out hitDirection, out hitPoint, true, ignoreFloorMarkers, includeInspectModeOnly, canReorder, shouldIgnore);
        }

        public static GameObject RaycastFromMousePosition(float distance, out Vector3Int hitDirection, out Vector3 hitPoint, bool ignoreFloorMarkers = true, bool includeInspectModeOnly = false, bool canReorder = true, Predicate<GameObject> shouldIgnore = null)
        {
            Ray ray = Get.Camera.ScreenPointToRay(Input.mousePosition);
            return RaycastUtility.Raycast(ray.origin, ray.direction, distance, out hitDirection, out hitPoint, true, ignoreFloorMarkers, includeInspectModeOnly, canReorder, shouldIgnore);
        }

        public static GameObject Raycast(Vector3 origin, Vector3 direction, float distance, out Vector3Int hitDirection, out Vector3 hitPoint, bool ignorePlayer = false, bool ignoreFloorMarkers = true, bool includeInspectModeOnly = false, bool canReorder = true, Predicate<GameObject> shouldIgnore = null)
        {
            Profiler.Begin("Raycast");
            GameObject gameObject;
            try
            {
                Ray ray = new Ray(origin, direction);
                int num = -1;
                num ^= Get.IgnoreRaycastMask;
                if (ignoreFloorMarkers)
                {
                    num ^= Get.FloorMarkersMask;
                }
                if (!includeInspectModeOnly)
                {
                    num ^= Get.InspectModeOnlyMask;
                }
                int num2 = Physics.RaycastNonAlloc(ray, RaycastUtility.tmpHits, distance, num);
                if (num2 <= 0)
                {
                    hitDirection = default(Vector3Int);
                    hitPoint = default(Vector3);
                    gameObject = null;
                }
                else
                {
                    if (num2 >= 2)
                    {
                        RaycastUtility.tmpForSort.Clear();
                        for (int i = 0; i < num2; i++)
                        {
                            RaycastUtility.tmpForSort.Add(RaycastUtility.tmpHits[i]);
                        }
                        RaycastUtility.tmpForSort.Sort(RaycastUtility.ByRaycastHitDistance);
                        RaycastUtility.tmpForSort.CopyTo(RaycastUtility.tmpHits);
                    }
                    for (int j = 0; j < num2; j++)
                    {
                        RaycastHit raycastHit = RaycastUtility.tmpHits[j];
                        if (!RaycastUtility.ShouldIgnoreHit(raycastHit, ignorePlayer))
                        {
                            GameObject gameObject2 = raycastHit.collider.gameObject;
                            if (shouldIgnore == null || !shouldIgnore(gameObject2))
                            {
                                hitDirection = raycastHit.normal.ToCardinalVector3IntDir();
                                hitPoint = raycastHit.point;
                                if (canReorder)
                                {
                                    Profiler.Begin("Raycast GetComponentInParent");
                                    EntityGOC componentInParent = gameObject2.GetComponentInParent<EntityGOC>();
                                    Entity entity = ((componentInParent != null) ? componentInParent.Entity : null);
                                    Profiler.End();
                                    GameObject gameObject3 = RaycastUtility.AdjustEntityRaycastResult(entity, origin, direction, distance, ref hitDirection, ref hitPoint, RaycastUtility.tmpHits, num2, shouldIgnore, ignorePlayer);
                                    if (gameObject3 != null)
                                    {
                                        gameObject2 = gameObject3;
                                    }
                                }
                                return gameObject2;
                            }
                        }
                    }
                    hitDirection = default(Vector3Int);
                    hitPoint = default(Vector3);
                    gameObject = null;
                }
            }
            finally
            {
                Profiler.End();
            }
            return gameObject;
        }

        private static GameObject AdjustEntityRaycastResult(Entity originalHitEntity, Vector3 origin, Vector3 direction, float distance, ref Vector3Int hitDirection, ref Vector3 hitPoint, RaycastHit[] allHits, int allHitsCount, Predicate<GameObject> shouldIgnore = null, bool ignorePlayer = false)
        {
            if (!(originalHitEntity is Actor))
            {
                Structure structure = originalHitEntity as Structure;
                if ((structure == null || !structure.Spec.Structure.IsSpecial) && (Get.NowControlledActor.Inventory.EquippedWeapon != null || Get.NowControlledActor.NativeWeapons.Any() || Get.UseOnTargetUI.TargetingUsable != null))
                {
                    for (int i = 0; i < allHitsCount; i++)
                    {
                        RaycastHit raycastHit = allHits[i];
                        ActorGOC componentInParent = raycastHit.collider.gameObject.GetComponentInParent<ActorGOC>();
                        if (componentInParent != null && componentInParent.Actor != null && !RaycastUtility.ShouldIgnoreHit(raycastHit, ignorePlayer) && (shouldIgnore == null || !shouldIgnore(raycastHit.collider.gameObject)))
                        {
                            hitDirection = raycastHit.normal.ToCardinalVector3IntDir();
                            hitPoint = raycastHit.point;
                            return raycastHit.collider.gameObject;
                        }
                    }
                }
            }
            if (originalHitEntity == null)
            {
                return null;
            }
            if (RaycastUtility.GetRaycastPriority(originalHitEntity) == RaycastUtility.RaycastPriority.High)
            {
                return null;
            }
            GameObject gameObject = null;
            RaycastHit raycastHit2 = default(RaycastHit);
            RaycastUtility.RaycastPriority raycastPriority = RaycastUtility.RaycastPriority.Low;
            for (int j = 0; j < allHitsCount; j++)
            {
                RaycastHit raycastHit3 = RaycastUtility.tmpHits[j];
                Profiler.Begin("Raycast GetComponentInParent");
                EntityGOC componentInParent2 = raycastHit3.collider.gameObject.GetComponentInParent<EntityGOC>();
                Entity entity = ((componentInParent2 != null) ? componentInParent2.Entity : null);
                Profiler.End();
                if (entity != null && entity != originalHitEntity && !(entity.Position != originalHitEntity.Position) && entity != Get.NowControlledActor)
                {
                    RaycastUtility.RaycastPriority raycastPriority2 = RaycastUtility.GetRaycastPriority(entity);
                    if (raycastPriority2 > raycastPriority && !RaycastUtility.ShouldIgnoreHit(raycastHit3, ignorePlayer) && (shouldIgnore == null || !shouldIgnore(raycastHit3.collider.gameObject)))
                    {
                        gameObject = raycastHit3.collider.gameObject;
                        raycastHit2 = raycastHit3;
                        raycastPriority = raycastPriority2;
                    }
                }
            }
            if (gameObject != null)
            {
                hitDirection = raycastHit2.normal.ToCardinalVector3IntDir();
                hitPoint = raycastHit2.point;
                return gameObject;
            }
            return null;
        }

        private static RaycastUtility.RaycastPriority GetRaycastPriority(Entity entity)
        {
            if (entity is Actor)
            {
                return RaycastUtility.RaycastPriority.High;
            }
            if (!(entity is Item) && (!(entity is Structure) || !entity.Spec.Structure.IsSpecial))
            {
                return RaycastUtility.RaycastPriority.Low;
            }
            if (Get.World.AnyEntityOfSpecAt(entity.Position, Get.Entity_Cage) || Get.World.AnyEntityOfSpecAt(entity.Position, Get.Entity_HangingCage) || Get.World.AnyEntityOfSpecAt(entity.Position, Get.Entity_Vines))
            {
                return RaycastUtility.RaycastPriority.Low;
            }
            return RaycastUtility.RaycastPriority.Medium;
        }

        private static bool ShouldIgnoreHit(RaycastHit hit, bool ignorePlayer)
        {
            Profiler.Begin("Raycast GetComponentInParent");
            EntityGOC componentInParent = hit.collider.gameObject.GetComponentInParent<EntityGOC>();
            Entity entity = ((componentInParent != null) ? componentInParent.Entity : null);
            Profiler.End();
            if (entity != null && entity.IsNowControlledActor && ignorePlayer)
            {
                return true;
            }
            Actor actor = entity as Actor;
            return actor != null && !BodyPartUtility.DidRaycastHitActor(actor, hit.point, true);
        }

        private static RaycastHit[] tmpHits = new RaycastHit[500];

        private static List<RaycastHit> tmpForSort = new List<RaycastHit>();

        private static readonly Comparison<RaycastHit> ByRaycastHitDistance = (RaycastHit a, RaycastHit b) => a.distance.CompareTo(b.distance);

        private enum RaycastPriority
        {
            Low,

            Medium,

            High
        }
    }
}