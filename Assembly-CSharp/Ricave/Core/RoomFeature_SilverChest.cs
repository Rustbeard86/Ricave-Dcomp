using System;
using System.Linq;
using UnityEngine;

namespace Ricave.Core
{
    public class RoomFeature_SilverChest : RoomFeature
    {
        public override bool TryGenerate(Room room, WorldGenMemory memory)
        {
            if (memory.baseItems.Any<Item>((Item x) => x.Spec == Get.Entity_SilverKey))
            {
                Vector3Int vector3Int;
                if (room.FreeOnFloorNonBlocking.Where<Vector3Int>((Vector3Int x) => x.Below().InBounds() && Get.CellsInfo.AnyPermanentFilledImpassableAt(x.Below())).TryGetRandomElement<Vector3Int>(out vector3Int))
                {
                    Item item;
                    if (Rand.Chance(0.2f))
                    {
                        item = ItemGenerator.Wand();
                    }
                    else
                    {
                        item = ItemGenerator.GoodReward();
                    }
                    Structure structure = Maker.Make<Structure>(Get.Entity_SilverChest, null, false, false, true);
                    structure.InnerEntities.Add(item);
                    structure.Spawn(vector3Int);
                    return true;
                }
            }
            return false;
        }
    }
}