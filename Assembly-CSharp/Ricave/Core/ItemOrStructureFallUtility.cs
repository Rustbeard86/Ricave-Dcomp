using System;
using UnityEngine;

namespace Ricave.Core
{
    public static class ItemOrStructureFallUtility
    {
        public static bool HasSupport(Entity entity)
        {
            return !entity.Spawned || ItemOrStructureFallUtility.WouldHaveSupport(entity.Spec, entity.Position, new Vector3Int?(entity.DirectionCardinal));
        }

        public static bool WouldHaveSupport(EntitySpec entity, Vector3Int at, Vector3Int? entityDir)
        {
            if (entityDir == null)
            {
                entityDir = new Vector3Int?(SpawnPositionFinder.GetExpectedDir(entity, at));
            }
            Vector3Int vector3Int = at.Below();
            if (entity.IsItem)
            {
                return Get.CellsInfo.AnyGivesGravitySupportInside(at) || !vector3Int.InBounds() || !Get.CellsInfo.CanPassThroughNoActors(vector3Int);
            }
            if (!entity.IsStructure)
            {
                return true;
            }
            if (entity.Structure.FallBehavior == StructureFallBehavior.None)
            {
                return true;
            }
            if (!entity.Structure.AttachesToAnything)
            {
                return Get.CellsInfo.AnyGivesGravitySupportInside(at) || !vector3Int.InBounds() || !Get.CellsInfo.CanPassThroughNoActors(vector3Int);
            }
            Vector3Int vector3Int2 = at.Above();
            if (entity.Structure.AttachesToCeiling && vector3Int2.InBounds() && Get.CellsInfo.AnyFilledImpassableAt(vector3Int2))
            {
                return true;
            }
            Vector3Int vector3Int3 = at - entityDir.Value;
            return (entity.Structure.AttachesToBack && vector3Int3.InBounds() && Get.CellsInfo.AnyFilledImpassableAt(vector3Int3)) || (entity.Structure.AttachesToSameSpecAbove && vector3Int2.InBounds() && Get.World.AnyEntityOfSpecAt(vector3Int2, entity));
        }

        public static bool CanAnythingEverBeAttachedToOrAffectsGravity(EntitySpec spec)
        {
            return !spec.CanPassThrough || (spec.IsStructure && spec.Structure.IsFilled) || (spec.IsStructure && spec.Structure.AttachesToSameSpecAbove) || (spec.IsStructure && spec.Structure.GivesGravitySupportInside);
        }

        public static readonly Vector3Int[] DirectionsCardinalInFallCheckOrder = new Vector3Int[]
        {
            Vector3IntUtility.Left,
            Vector3IntUtility.Right,
            Vector3IntUtility.Forward,
            Vector3IntUtility.Back,
            Vector3IntUtility.Down,
            Vector3Int.zero,
            Vector3IntUtility.Up
        };
    }
}