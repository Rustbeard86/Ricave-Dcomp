using System;
using UnityEngine;

namespace Ricave.Core
{
    public class RoomFeature_Portal : RoomFeature
    {
        public override bool TryGenerate(Room room, WorldGenMemory memory)
        {
            if (memory.config.Floor <= 1)
            {
                return false;
            }
            World world = Get.World;
            CellCuboid cellCuboid;
            if (!BiggestRectFinder.TryFindRectOfSize(room.Shape.InnerCuboid(1).BottomSurfaceCuboid, (Vector3Int x) => !world.AnyEntityAt(x) && world.CellsInfo.IsFloorUnderNoActors(x) && world.CellsInfo.IsFilled(x.Below()) && !room.IsEntranceCellToAvoidBlockingOnlyFromBelow(x) && !world.AnyEntityNearby(x, 2, Get.Entity_ChainPost), 3, out cellCuboid))
            {
                return false;
            }
            foreach (Vector3Int vector3Int in cellCuboid)
            {
                Get.TiledDecals.SetForced(Get.TiledDecals_Slab, vector3Int);
            }
            Maker.Make(Get.Entity_Portal, null, false, false, true).Spawn(cellCuboid.Center);
            return true;
        }
    }
}