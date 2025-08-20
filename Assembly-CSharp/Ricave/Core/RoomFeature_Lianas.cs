using System;
using UnityEngine;

namespace Ricave.Core
{
    public class RoomFeature_Lianas : RoomFeature
    {
        public override bool TryGenerate(Room room, WorldGenMemory memory)
        {
            if (room.RoomAbove != null)
            {
                return false;
            }
            World world = Get.World;
            CellCuboid cellCuboid;
            if (!BiggestRectFinder.TryFindRectOfSize(room.Shape.InnerCuboid(1).BottomSurfaceCuboid, (Vector3Int x) => !world.AnyEntityAt(x) && room.AllCellsToOneBelowCeilingFree(x), 4, out cellCuboid))
            {
                return false;
            }
            cellCuboid = cellCuboid.InnerCuboidXZ(1);
            foreach (Vector3Int vector3Int in cellCuboid)
            {
                int num = room.Height - 2;
                int num2 = Rand.RangeInclusive(1, num);
                for (int i = 0; i < num2; i++)
                {
                    Maker.Make(Get.Entity_Liana, null, false, false, true).Spawn(vector3Int.WithAddedY(num - i - 1));
                }
            }
            return true;
        }
    }
}