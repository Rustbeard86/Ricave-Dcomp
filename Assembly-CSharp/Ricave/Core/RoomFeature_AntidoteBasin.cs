using System;
using System.Linq;
using UnityEngine;

namespace Ricave.Core
{
    public class RoomFeature_AntidoteBasin : RoomFeature
    {
        public override bool TryGenerate(Room room, WorldGenMemory memory)
        {
            Vector3Int vector3Int;
            if (!room.FreeOnFloor.Where<Vector3Int>(delegate (Vector3Int x)
            {
                if (room.Shape.InnerCuboid(1).IsOnEdgeXZ(x) && ItemOrStructureFallUtility.WouldHaveSupport(Get.Entity_AntidoteBasin, x, null))
                {
                    return x.AdjacentCardinalCellsXZ().Any<Vector3Int>((Vector3Int y) => Get.CellsInfo.AnyFilledImpassableAt(y));
                }
                return false;
            }).TryGetRandomElement<Vector3Int>(out vector3Int))
            {
                return false;
            }
            Maker.Make(Get.Entity_AntidoteBasin, null, false, false, true).Spawn(vector3Int);
            return true;
        }
    }
}