using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ricave.Core
{
    public class RoomPart_CeilingPlants : RoomPart
    {
        public override void Generate(Room room, WorldGenMemory memory)
        {
            if (memory.config.Floor < 3 || memory.config.Floor % 2 != 1)
            {
                return;
            }
            if (!room.Storey.IsUpperStorey)
            {
                return;
            }
            if (room.Height <= 3)
            {
                return;
            }
            World world = Get.World;
            if (world.AnyEntityOfSpec(Get.Entity_Healplant) || world.AnyEntityOfSpec(Get.Entity_Manaplant))
            {
                return;
            }
            Func<Vector3Int, bool> <> 9__1;
            Vector3Int vector3Int;
            if (room.FreeBelowCeilingNoEntranceBlocking.Where<Vector3Int>(delegate (Vector3Int x)
            {
                if (room.AllCellsToOneBelowCeilingFree(x.WithY(room.StartY + 1)))
                {
                    IEnumerable<Vector3Int> enumerable = x.WithY(room.StartY + 1).AdjacentCellsXZAndInside();
                    Func<Vector3Int, bool> func;
                    if ((func = <> 9__1) == null)
                    {
                        func = (<> 9__1 = (Vector3Int y) => world.CellsInfo.IsFloorUnderNoActors(y));
                    }
                    return enumerable.Any<Vector3Int>(func);
                }
                return false;
            }).TryGetRandomElement<Vector3Int>(out vector3Int))
            {
                Maker.Make(Rand.Bool ? Get.Entity_Healplant : Get.Entity_Manaplant, null, false, false, true).Spawn(vector3Int);
            }
        }
    }
}