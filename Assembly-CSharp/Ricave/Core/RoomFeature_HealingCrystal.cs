using System;
using System.Linq;
using UnityEngine;

namespace Ricave.Core
{
    public class RoomFeature_HealingCrystal : RoomFeature
    {
        public override bool TryGenerate(Room room, WorldGenMemory memory)
        {
            World world = Get.World;
            int count = world.GetEntitiesOfSpec(Get.Entity_HealingCrystal).Count;
            int count2 = world.GetEntitiesOfSpec(Get.Entity_SmallHealingCrystal).Count;
            if (count >= Get.Difficulty.HealingCrystalsPerFloor && count2 >= Get.Difficulty.SmallHealingCrystalsPerFloor)
            {
                return false;
            }
            Vector3Int vector3Int;
            if (!room.FreeOnFloorNonBlocking.Where<Vector3Int>((Vector3Int x) => world.CellsInfo.AnyPermanentFilledImpassableAt(x.Below())).TryGetRandomElement<Vector3Int>(out vector3Int))
            {
                return false;
            }
            EntitySpec entitySpec;
            if (count < Get.Difficulty.HealingCrystalsPerFloor)
            {
                if (count2 < Get.Difficulty.SmallHealingCrystalsPerFloor)
                {
                    entitySpec = (Rand.Bool ? Get.Entity_HealingCrystal : Get.Entity_SmallHealingCrystal);
                }
                else
                {
                    entitySpec = Get.Entity_HealingCrystal;
                }
            }
            else
            {
                entitySpec = Get.Entity_SmallHealingCrystal;
            }
            Maker.Make(entitySpec, null, false, false, true).Spawn(vector3Int);
            return true;
        }
    }
}