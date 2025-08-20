using System;
using UnityEngine;

namespace Ricave.Core
{
    public class RoomPart_FloorUnderWallsOnly : RoomPart
    {
        public override void Generate(Room room, WorldGenMemory memory)
        {
            World world = Get.World;
            foreach (Vector3Int vector3Int in room.Shape.BottomSurfaceCuboid.EdgeCellsXZ)
            {
                if (!world.AnyEntityAt(vector3Int))
                {
                    Maker.Make(Get.Entity_Floor, null, false, false, true).Spawn(vector3Int);
                }
            }
        }
    }
}