using System;
using System.Linq;
using UnityEngine;

namespace Ricave.Core
{
    public class RoomFeature_Platform : RoomFeature
    {
        public override bool TryGenerate(Room room, WorldGenMemory memory)
        {
            if (room.Height <= 4)
            {
                return false;
            }
            Vector3Int vector3Int;
            if (!room.Shape.InnerCuboid(1).TopSurfaceCuboid.EdgeCellsXZ.Select<Vector3Int, Vector3Int>((Vector3Int x) => x.Below()).Where<Vector3Int>(delegate (Vector3Int x)
            {
                if (!Get.World.AnyEntityAt(x) && !Get.World.AnyEntityAt(x.Above()) && !room.IsEntranceCellToAvoidBlocking(x, true))
                {
                    if (x.Above().AdjacentCardinalCellsXZ().Any<Vector3Int>((Vector3Int y) => y.InBounds() && Get.CellsInfo.CanPassThroughNoActors(y)))
                    {
                        return ItemOrStructureFallUtility.WouldHaveSupport(Get.Entity_Platform, x, null);
                    }
                }
                return false;
            }).TryGetRandomElement<Vector3Int>(out vector3Int))
            {
                return false;
            }
            Maker.Make(Get.Entity_Platform, null, false, false, true).Spawn(vector3Int);
            ItemGenerator.SmallReward(true).Spawn(vector3Int.Above());
            return true;
        }
    }
}