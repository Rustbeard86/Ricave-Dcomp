using System;
using System.Linq;
using UnityEngine;

namespace Ricave.Core
{
    public class RoomFeature_GildedLockbox : RoomFeature
    {
        public override bool TryGenerate(Room room, WorldGenMemory memory)
        {
            Vector3Int vector3Int;
            if (!room.FreeOnFloorNonBlocking.Where<Vector3Int>((Vector3Int x) => Get.CellsInfo.AnyPermanentFilledImpassableAt(x.Below())).TryGetRandomElement<Vector3Int>(out vector3Int))
            {
                return false;
            }
            Structure structure = Maker.Make<Structure>(Get.Entity_GildedLockbox, null, false, false, true);
            Item item = (Rand.Chance(0.3f) ? ItemGenerator.Reward(false) : ItemGenerator.SmallReward(false));
            structure.InnerEntities.Add(item);
            structure.Spawn(vector3Int);
            return true;
        }
    }
}