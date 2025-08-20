using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ricave.Core
{
    public class RoomPart_SpiderWeb : RoomPart
    {
        public override void Generate(Room room, WorldGenMemory memory)
        {
            World world = Get.World;
            if (world.GetEntitiesOfSpec(Get.Entity_SpiderWeb).Count >= 2)
            {
                return;
            }
            if (room.Index % 2 != 0)
            {
                return;
            }
            Func<Vector3Int, bool> <> 9__1;
            Func<Vector3Int, bool> <> 9__2;
            Vector3Int vector3Int;
            if (room.Shape.InnerCuboid(1).TopSurfaceCuboid.Corners.Where<Vector3Int>(delegate (Vector3Int x)
            {
                if (!world.AnyEntityAt(x) && !room.IsEntranceCellToAvoidBlocking(x, true))
                {
                    IEnumerable<Vector3Int> enumerable = x.AdjacentCardinalCellsXZ();
                    Func<Vector3Int, bool> func;
                    if ((func = <> 9__1) == null)
                    {
                        func = (<> 9__1 = (Vector3Int y) => room.Shape.IsOnEdgeXZ(y));
                    }
                    IEnumerable<Vector3Int> enumerable2 = enumerable.Where<Vector3Int>(func);
                    Func<Vector3Int, bool> func2;
                    if ((func2 = <> 9__2) == null)
                    {
                        func2 = (<> 9__2 = (Vector3Int y) => world.CellsInfo.AnyFilledImpassableAt(y));
                    }
                    return enumerable2.All<Vector3Int>(func2);
                }
                return false;
            }).TryGetRandomElement<Vector3Int>(out vector3Int))
            {
                Maker.Make(Get.Entity_SpiderWeb, null, false, false, true).Spawn(vector3Int);
            }
        }
    }
}