using System;
using UnityEngine;

namespace Ricave.Core
{
    public class RoomFeature_TableAndChairs : RoomFeature
    {
        public override bool TryGenerate(Room room, WorldGenMemory memory)
        {
            World world = Get.World;
            CellCuboid cellCuboid;
            if (!BiggestRectFinder.TryFindRectOfSize(room.Shape.InnerCuboid(1).BottomSurfaceCuboid, (Vector3Int x) => !world.AnyEntityAt(x) && world.CellsInfo.IsFloorUnderNoActors(x) && !room.IsEntranceCellToAvoidBlockingOnlyFromBelow(x), 3, out cellCuboid))
            {
                return false;
            }
            Maker.Make(Get.Entity_Table, null, false, false, true).Spawn(cellCuboid.Center);
            if (Rand.Bool)
            {
                Maker.Make(Get.Entity_Chair, null, false, false, true).Spawn(cellCuboid.Position + new Vector3Int(0, 0, 1), Vector3IntUtility.Right);
                Maker.Make(Get.Entity_Chair, null, false, false, true).Spawn(cellCuboid.Position + new Vector3Int(2, 0, 1), Vector3IntUtility.Left);
            }
            else
            {
                Maker.Make(Get.Entity_Chair, null, false, false, true).Spawn(cellCuboid.Position + new Vector3Int(1, 0, 0), Vector3IntUtility.Forward);
                Maker.Make(Get.Entity_Chair, null, false, false, true).Spawn(cellCuboid.Position + new Vector3Int(1, 0, 2), Vector3IntUtility.Back);
            }
            return true;
        }
    }
}