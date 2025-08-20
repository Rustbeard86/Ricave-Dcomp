using System;
using System.Linq;
using UnityEngine;

namespace Ricave.Core
{
    public class RoomFeature_ManaCrystal : RoomFeature
    {
        public override bool TryGenerate(Room room, WorldGenMemory memory)
        {
            World world = Get.World;
            int count = world.GetEntitiesOfSpec(Get.Entity_ManaCrystal).Count;
            int count2 = world.GetEntitiesOfSpec(Get.Entity_SmallManaCrystal).Count;
            if (count >= Get.Difficulty.ManaCrystalsPerFloor && count2 >= Get.Difficulty.SmallManaCrystalsPerFloor)
            {
                return false;
            }
            Vector3Int vector3Int;
            if (!room.FreeOnFloorNonBlocking.Where<Vector3Int>((Vector3Int x) => world.CellsInfo.AnyPermanentFilledImpassableAt(x.Below())).TryGetRandomElement<Vector3Int>(out vector3Int))
            {
                return false;
            }
            EntitySpec entitySpec;
            if (count < Get.Difficulty.ManaCrystalsPerFloor)
            {
                if (count2 < Get.Difficulty.SmallManaCrystalsPerFloor)
                {
                    entitySpec = (Rand.Bool ? Get.Entity_ManaCrystal : Get.Entity_SmallManaCrystal);
                }
                else
                {
                    entitySpec = Get.Entity_ManaCrystal;
                }
            }
            else
            {
                entitySpec = Get.Entity_SmallManaCrystal;
            }
            Maker.Make(entitySpec, null, false, false, true).Spawn(vector3Int);
            return true;
        }
    }
}