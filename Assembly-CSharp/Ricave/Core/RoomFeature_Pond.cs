using System;
using System.Linq;
using UnityEngine;

namespace Ricave.Core
{
    public class RoomFeature_Pond : RoomFeature
    {
        public override bool TryGenerate(Room room, WorldGenMemory memory)
        {
            Vector3Int vector3Int;
            if (!room.FreeOnFloorNonBlocking.Where<Vector3Int>((Vector3Int x) => Get.CellsInfo.AnyFilledImpassableAt(x.Below())).TryGetRandomElement<Vector3Int>(out vector3Int))
            {
                return false;
            }
            Maker.Make(Get.Entity_Pond, null, false, false, true).Spawn(vector3Int);
            return true;
        }
    }
}