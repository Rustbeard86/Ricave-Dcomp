using System;
using UnityEngine;

namespace Ricave.Core
{
    public class RoomFeature_WaspNest : RoomFeature
    {
        public override bool TryGenerate(Room room, WorldGenMemory memory)
        {
            if (!Get.RunSpec.AllowMobSpawners)
            {
                return false;
            }
            Vector3Int vector3Int;
            if (room.Height != 4 || !room.FreeBelowCeilingNoEntranceBlocking.TryGetRandomElement<Vector3Int>(out vector3Int))
            {
                return false;
            }
            Structure structure = Maker.Make<Structure>(Get.Entity_WaspNest, null, false, false, true);
            if (Rand.Chance(0.3f))
            {
                structure.InnerEntities.Add(Maker.Make(Get.Entity_Honey, null, false, false, true));
            }
            structure.Spawn(vector3Int);
            return true;
        }
    }
}