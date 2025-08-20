using System;
using UnityEngine;

namespace Ricave.Core
{
    public class RoomFeature_BigMagicTrapsWithRareItem : RoomFeature
    {
        public override bool TryGenerate(Room room, WorldGenMemory memory)
        {
            if (memory.config.Floor != 1 && memory.config.Floor % 2 == 1)
            {
                return false;
            }
            if (room.Height <= 3)
            {
                return false;
            }
            if (memory.config.Floor >= 5)
            {
                return false;
            }
            World world = Get.World;
            CellCuboid cellCuboid;
            if (!BiggestRectFinder.TryFindRectOfSize(room.Shape.InnerCuboid(1).BottomSurfaceCuboid, (Vector3Int x) => !world.AnyEntityAt(x) && world.CellsInfo.IsFloorUnderNoActors(x) && world.CellsInfo.AnyPermanentFilledImpassableAt(x.Below()) && !world.AnyEntityAt(x.Above()) && !room.IsEntranceCellToAvoidBlockingOnlyFromBelow(x), 5, out cellCuboid))
            {
                return false;
            }
            foreach (Vector3Int vector3Int in cellCuboid.InnerCuboidXZ(1).EdgeCellsXZ)
            {
                Maker.Make(Get.Entity_BigMagicTrap, null, false, false, true).Spawn(vector3Int);
            }
            Maker.Make(Get.Entity_MagicColumns, null, false, false, true).Spawn(cellCuboid.Center);
            Maker.Make(Get.Entity_WoodenFloor, null, false, false, true).Spawn(cellCuboid.Center.Above());
            ItemGenerator.Reward(true).Spawn(cellCuboid.Center);
            return true;
        }
    }
}