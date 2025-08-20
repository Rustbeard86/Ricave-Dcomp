using System;
using UnityEngine;

namespace Ricave.Core
{
    public class RoomFeature_Vines : RoomFeature
    {
        public override bool TryGenerate(Room room, WorldGenMemory memory)
        {
            if (!Get.Quest_MemoryPiece1.IsActive() || memory.config.Floor != 3)
            {
                return false;
            }
            int num = Rand.RangeInclusive(1, 3);
            bool flag = false;
            int num2 = 0;
            Vector3Int vector3Int;
            while (num2 < num && room.FreeOnFloor.TryGetRandomElement<Vector3Int>(out vector3Int))
            {
                Maker.Make(Get.Entity_Vines, null, false, false, true).Spawn(vector3Int);
                flag = true;
                num2++;
            }
            return flag;
        }
    }
}