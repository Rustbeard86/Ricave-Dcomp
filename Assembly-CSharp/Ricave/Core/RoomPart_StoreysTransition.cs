using System;
using System.Linq;
using UnityEngine;

namespace Ricave.Core
{
    public class RoomPart_StoreysTransition : RoomPart
    {
        public override void Generate(Room room, WorldGenMemory memory)
        {
            if (room.Role != Room.LayoutRole.StoreysTransition)
            {
                return;
            }
            if (room.RoomAbove == null || room.RoomAbove.Role != Room.LayoutRole.StoreysTransition)
            {
                return;
            }
            World world = Get.World;
            CellCuboid surface = room.Shape.InnerCuboid(1).BottomSurfaceCuboid;
            Vector3Int vector3Int;
            if (surface.EdgeCellsXZNoCorners.Where<Vector3Int>((Vector3Int x) => !world.AnyEntityAt(x) && world.CellsInfo.IsFloorUnderNoActors(x) && !room.IsEntranceCellToAvoidBlocking(x, true) && room.AllCellsToCeilingFree(x) && room.AllCellsToOneBelowCeilingPredicate(x, (Vector3Int y) => !world.CellsInfo.CanPassThroughNoActors(y + surface.GetEdge(x)))).TryGetRandomElement<Vector3Int>(out vector3Int) || surface.EdgeCellsXZ.Where<Vector3Int>((Vector3Int x) => !world.AnyEntityAt(x) && world.CellsInfo.IsFloorUnderNoActors(x) && room.AllCellsToCeilingFree(x)).TryGetRandomElement<Vector3Int>(out vector3Int))
            {
                for (int i = vector3Int.y; i <= room.Shape.yMax; i++)
                {
                    Maker.Make(Get.Entity_Ladder, null, false, false, true).Spawn(new Vector3Int(vector3Int.x, i, vector3Int.z));
                }
            }
        }
    }
}