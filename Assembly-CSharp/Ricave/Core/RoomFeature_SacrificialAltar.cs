using System;
using System.Linq;
using UnityEngine;

namespace Ricave.Core
{
    public class RoomFeature_SacrificialAltar : RoomFeature
    {
        public override bool TryGenerate(Room room, WorldGenMemory memory)
        {
            Vector3Int vector3Int;
            if (!room.FreeOnFloorNoEntranceBlocking.Where<Vector3Int>((Vector3Int x) => x.AdjacentCardinalCellsXZ().All<Vector3Int>((Vector3Int y) => Get.CellsInfo.CanPassThroughNoActors(y) && Get.CellsInfo.IsFloorUnderNoActors(y))).TryGetRandomElement<Vector3Int>(out vector3Int))
            {
                return false;
            }
            Entity entity = Maker.Make(Get.Entity_SacrificialAltar, null, false, false, true);
            Item item = ItemGenerator.Reward(true);
            entity.GetComp<SacrificialAltarComp>().Reward = item;
            entity.Spawn(vector3Int);
            return true;
        }
    }
}