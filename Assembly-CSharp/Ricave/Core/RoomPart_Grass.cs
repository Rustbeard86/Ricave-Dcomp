using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class RoomPart_Grass : RoomPart
    {
        public override void Generate(Room room, WorldGenMemory memory)
        {
            if (!Rand.Chance(0.45f))
            {
                return;
            }
            CellCuboid bottomSurfaceCuboid = room.Shape.InnerCuboid(1).BottomSurfaceCuboid;
            if (bottomSurfaceCuboid.Volume < 9)
            {
                return;
            }
            int num = ((bottomSurfaceCuboid.Volume <= 16) ? 4 : 9);
            int num2 = Rand.RangeInclusive(1, num);
            List<Entity> list = RoomFillerUtility.DoPatchOf(room, memory, Get.Entity_Grass, num2, true, true, true, null);
            Entity entity;
            if (!memory.generatedGrassWithVineSeed && Rand.Chance(0.22f) && list.TryGetRandomElement<Entity>(out entity))
            {
                memory.generatedGrassWithVineSeed = true;
                ((Structure)entity).InnerEntities.Add(Maker.Make(Get.Entity_VineSeed, null, false, false, true));
            }
        }
    }
}