using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class RoomFeature_TimedSpikes : RoomFeature
    {
        public override bool TryGenerate(Room room, WorldGenMemory memory)
        {
            List<Entity> list = RoomFillerUtility.DoPatchOf(room, memory, Get.Entity_TimedSpikes, Rand.RangeInclusive(1, 3), true, false, false, (Vector3Int x) => Get.CellsInfo.AnyFilledImpassableAt(x.Below()));
            if (list.Count == 0)
            {
                return false;
            }
            ItemGenerator.SmallReward(true).Spawn(list[0].Position);
            return true;
        }
    }
}