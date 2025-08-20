using System;
using UnityEngine;

namespace Ricave.Core
{
    public class RoomFeature_MagicFire : RoomFeature
    {
        public override bool TryGenerate(Room room, WorldGenMemory memory)
        {
            Vector3Int vector3Int;
            if (Get.Quest_MemoryPiece4.IsActive() && memory.config.Floor == 8 && room.FreeOnFloorNonBlocking.TryGetRandomElement<Vector3Int>(out vector3Int))
            {
                Maker.Make(Get.Entity_MagicFire, null, false, false, true).Spawn(vector3Int);
                return true;
            }
            return false;
        }
    }
}