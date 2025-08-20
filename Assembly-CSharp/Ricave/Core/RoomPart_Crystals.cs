using System;
using UnityEngine;

namespace Ricave.Core
{
    public class RoomPart_Crystals : RoomPart
    {
        public override void Generate(Room room, WorldGenMemory memory)
        {
            Vector3Int vector3Int;
            if (Get.World.GetEntitiesOfSpec(Get.Entity_RedCrystal).Count + Get.World.GetEntitiesOfSpec(Get.Entity_GreenCrystal).Count + Get.World.GetEntitiesOfSpec(Get.Entity_BlueCrystal).Count >= 2 || !room.GetNonBlockingPositionsForEntity(Get.Entity_RedCrystal, true, true).TryGetRandomElement<Vector3Int>(out vector3Int))
            {
                return;
            }
            Place place = memory.config.Place;
            EntitySpec entitySpec2;
            if (place != null && place.Crystal != null)
            {
                Place.CrystalType? crystal = memory.config.Place.Crystal;
                EntitySpec entitySpec;
                if (crystal != null)
                {
                    switch (crystal.GetValueOrDefault())
                    {
                        case Place.CrystalType.Red:
                            entitySpec = Get.Entity_RedCrystal;
                            goto IL_00CB;
                        case Place.CrystalType.Green:
                            entitySpec = Get.Entity_GreenCrystal;
                            goto IL_00CB;
                        case Place.CrystalType.Blue:
                            entitySpec = Get.Entity_BlueCrystal;
                            goto IL_00CB;
                    }
                }
                entitySpec = null;
            IL_00CB:
                entitySpec2 = entitySpec;
            }
            else
            {
                entitySpec2 = Rand.Element<EntitySpec>(Get.Entity_RedCrystal, Get.Entity_GreenCrystal, Get.Entity_BlueCrystal);
            }
            Maker.Make<Structure>(entitySpec2, null, false, false, true).Spawn(vector3Int);
        }
    }
}