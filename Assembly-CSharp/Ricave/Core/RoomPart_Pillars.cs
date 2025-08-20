using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ricave.Core
{
    public class RoomPart_Pillars : RoomPart
    {
        public override void Generate(Room room, WorldGenMemory memory)
        {
            if (memory.roomsWithPillars >= 2)
            {
                return;
            }
            if (room.Shape.width < 8 && room.Shape.depth < 8 && !Rand.Chance(0.35f))
            {
                return;
            }
            if (room.Height <= 3)
            {
                return;
            }
            World world = Get.World;
            Func<Vector3Int, bool> <> 9__1;
            Vector3Int vector3Int;
            if (room.FreeOnFloorNonBlocking.Where<Vector3Int>(delegate (Vector3Int x)
            {
                if (world.CellsInfo.IsFilled(x.Below()))
                {
                    IEnumerable<Vector3Int> enumerable = x.AdjacentCellsXZ();
                    Func<Vector3Int, bool> func;
                    if ((func = <> 9__1) == null)
                    {
                        func = (<> 9__1 = (Vector3Int y) => world.CellsInfo.CanPassThroughNoActors(y));
                    }
                    if (enumerable.All<Vector3Int>(func))
                    {
                        return room.AllCellsToOneBelowCeilingFree(x);
                    }
                }
                return false;
            }).TryGetRandomElement<Vector3Int>(out vector3Int))
            {
                for (int i = 0; i < room.Height - 2; i++)
                {
                    Maker.Make(Get.Entity_Pillar, null, false, false, true).Spawn(vector3Int.WithAddedY(i));
                }
                memory.roomsWithPillars++;
            }
        }
    }
}