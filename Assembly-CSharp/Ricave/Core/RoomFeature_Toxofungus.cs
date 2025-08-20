using System;
using System.Linq;
using UnityEngine;

namespace Ricave.Core
{
    public class RoomFeature_Toxofungus : RoomFeature
    {
        public override bool TryGenerate(Room room, WorldGenMemory memory)
        {
            if (!Rand.ChanceSeeded(0.4f, Calc.CombineHashes<int, int>(memory.config.WorldSeed, 287453691)))
            {
                return false;
            }
            Vector3Int vector3Int;
            if (!room.FreeOnFloorNoEntranceBlocking.Where<Vector3Int>((Vector3Int x) => Get.CellsInfo.AnyFilledImpassableAt(x.Below())).TryGetRandomElement<Vector3Int>(out vector3Int))
            {
                return false;
            }
            Maker.Make(Get.Entity_Toxofungus, null, false, false, true).Spawn(vector3Int);
            return true;
        }
    }
}