using System;
using UnityEngine;

namespace Ricave.Core
{
    public class RoomFeature_DevilStatue : RoomFeature
    {
        public override bool TryGenerate(Room room, WorldGenMemory memory)
        {
            Vector3Int vector3Int;
            if (!room.FreeOnFloorNonBlocking.TryGetRandomElement<Vector3Int>(out vector3Int))
            {
                return false;
            }
            Maker.Make(Get.Entity_DevilStatue, null, false, false, true).Spawn(vector3Int);
            return true;
        }
    }
}