using System;
using UnityEngine;

namespace Ricave.Core
{
    public class RoomPart_Items : RoomPart
    {
        public override void Generate(Room room, WorldGenMemory memory)
        {
            World world = Get.World;
            Item item;
            Vector3Int vector3Int;
            if (memory.UnusedBaseGear.TryGetRandomElement<Item>(out item) && room.FreeOnFloorNonBlocking.TryGetRandomElement<Vector3Int>(out vector3Int))
            {
                Structure structure = Maker.Make<Structure>(Get.Entity_Chest, null, false, false, true);
                structure.InnerEntities.Add(item);
                structure.Spawn(vector3Int);
                memory.unusedBaseItems.Remove(item);
            }
            Item item2;
            Vector3Int vector3Int2;
            if (memory.UnusedBaseLobbyItems.TryGetRandomElement<Item>(out item2) && room.FreeOnFloorNoEntranceBlocking.TryGetRandomElement<Vector3Int>(out vector3Int2))
            {
                item2.Spawn(vector3Int2);
                memory.unusedBaseItems.Remove(item2);
            }
        }
    }
}