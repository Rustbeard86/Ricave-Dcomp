using System;
using UnityEngine;

namespace Ricave.Core
{
    public class RoomFeature_MetalCrate : RoomFeature
    {
        public override bool TryGenerate(Room room, WorldGenMemory memory)
        {
            Item item;
            Vector3Int vector3Int;
            if (!memory.UnusedBaseMiscItems.TryGetRandomElement<Item>(out item) || !room.FreeOnFloorNonBlocking.TryGetRandomElement<Vector3Int>(out vector3Int))
            {
                return false;
            }
            Structure structure = Maker.Make<Structure>(Get.Entity_MetalCrate, null, false, false, true);
            structure.InnerEntities.Add(item);
            structure.Spawn(vector3Int);
            memory.unusedBaseItems.Remove(item);
            return true;
        }
    }
}