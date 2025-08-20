using System;
using UnityEngine;

namespace Ricave.Core
{
    public class RoomFeature_Anvil : RoomFeature
    {
        public override bool TryGenerate(Room room, WorldGenMemory memory)
        {
            if (Get.RunInfo.GeneratedAnvil)
            {
                return false;
            }
            Vector3Int vector3Int;
            if (!room.FreeOnFloorNonBlocking.TryGetRandomElement<Vector3Int>(out vector3Int))
            {
                return false;
            }
            Maker.Make(Get.Entity_Anvil, null, false, false, true).Spawn(vector3Int);
            Get.RunInfo.GeneratedAnvil = true;
            return true;
        }
    }
}