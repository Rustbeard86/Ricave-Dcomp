using System;
using UnityEngine;

namespace Ricave.Core
{
    public class RoomFeature_Bonfire : RoomFeature
    {
        public override bool TryGenerate(Room room, WorldGenMemory memory)
        {
            Vector3Int vector3Int;
            if (memory.config.Floor == 1 || memory.config.Floor % 2 != 1 || !room.FreeOnFloorNonBlocking.TryGetRandomElement<Vector3Int>(out vector3Int))
            {
                return false;
            }
            Maker.Make(Get.Entity_Bonfire, null, false, false, true).Spawn(vector3Int);
            return true;
        }
    }
}