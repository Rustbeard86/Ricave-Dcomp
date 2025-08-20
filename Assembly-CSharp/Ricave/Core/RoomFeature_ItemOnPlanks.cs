using System;
using System.Linq;
using UnityEngine;

namespace Ricave.Core
{
    public class RoomFeature_ItemOnPlanks : RoomFeature
    {
        public override bool TryGenerate(Room room, WorldGenMemory memory)
        {
            if (room.Height <= 3)
            {
                return false;
            }
            Item item;
            if (!memory.UnusedBaseMiscItems.TryGetRandomElement<Item>(out item))
            {
                return false;
            }
            World world = Get.World;
            Vector3Int vector3Int;
            if (!room.FreeOnFloorNonBlocking.Where<Vector3Int>((Vector3Int x) => !world.AnyEntityAt(x.Above())).TryGetRandomElement<Vector3Int>(out vector3Int))
            {
                return false;
            }
            Maker.Make(Get.Entity_Planks, null, false, false, true).Spawn(vector3Int);
            item.Spawn(vector3Int.Above());
            memory.unusedBaseItems.Remove(item);
            return true;
        }
    }
}