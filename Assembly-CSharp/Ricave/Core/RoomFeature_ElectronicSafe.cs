using System;
using System.Linq;
using UnityEngine;

namespace Ricave.Core
{
    public class RoomFeature_ElectronicSafe : RoomFeature
    {
        public override bool TryGenerate(Room room, WorldGenMemory memory)
        {
            Vector3Int vector3Int;
            if (!room.FreeOnFloorNonBlocking.Where<Vector3Int>((Vector3Int x) => Get.CellsInfo.AnyPermanentFilledImpassableAt(x.Below())).TryGetRandomElement<Vector3Int>(out vector3Int))
            {
                return false;
            }
            Item item = ItemGenerator.SmallReward(false);
            Structure structure = Maker.Make<Structure>(Get.Entity_ElectronicSafe, null, false, false, true);
            structure.InnerEntities.Add(item);
            structure.Spawn(vector3Int);
            return true;
        }
    }
}