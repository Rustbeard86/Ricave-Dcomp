using System;
using System.Linq;
using UnityEngine;

namespace Ricave.Core
{
    public class RoomFeature_UpgradeOrb : RoomFeature
    {
        public override bool TryGenerate(Room room, WorldGenMemory memory)
        {
            if (!Rand.ChanceSeeded(0.8f, Calc.CombineHashes<int, int>(Get.RunConfig.RunSeed, 634213245)))
            {
                return false;
            }
            int num = Calc.HashToRangeInclusive(Calc.CombineHashes<int, int>(Get.RunConfig.RunSeed, 98074365), 1, Get.RunSpec.FloorCount ?? 10);
            if (num == 5 || num == 9)
            {
                num--;
            }
            if (Get.Floor != num)
            {
                return false;
            }
            Vector3Int vector3Int;
            if (!room.FreeOnFloorNonBlocking.Where<Vector3Int>((Vector3Int x) => Get.World.CellsInfo.AnyPermanentFilledImpassableAt(x.Below())).TryGetRandomElement<Vector3Int>(out vector3Int))
            {
                return false;
            }
            Maker.Make(Get.Entity_UpgradeOrb, null, false, false, true).Spawn(vector3Int);
            return true;
        }
    }
}