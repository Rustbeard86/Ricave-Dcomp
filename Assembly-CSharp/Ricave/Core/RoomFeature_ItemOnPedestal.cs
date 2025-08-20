using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ricave.Core
{
    public class RoomFeature_ItemOnPedestal : RoomFeature
    {
        public override bool TryGenerate(Room room, WorldGenMemory memory)
        {
            if (room.Height <= 3)
            {
                return false;
            }
            World world = Get.World;
            Func<Vector3Int, bool> <> 9__1;
            Vector3Int vector3Int;
            if (!room.FreeOnFloorNonBlocking.Where<Vector3Int>(delegate (Vector3Int x)
            {
                if (world.CellsInfo.AnyPermanentFilledImpassableAt(x.Below()) && !world.AnyEntityAt(x.Above()))
                {
                    IEnumerable<Vector3Int> enumerable = x.AdjacentCellsXZ();
                    Func<Vector3Int, bool> func;
                    if ((func = <> 9__1) == null)
                    {
                        func = (<> 9__1 = (Vector3Int y) => world.CellsInfo.CanPassThroughNoActors(y) && !world.CellsInfo.AnyStairsAt(y) && !world.CellsInfo.AnyLadderAt(y));
                    }
                    return enumerable.All<Vector3Int>(func);
                }
                return false;
            }).TryGetRandomElement<Vector3Int>(out vector3Int))
            {
                return false;
            }
            Maker.Make(Get.Entity_Pedestal, null, false, false, true).Spawn(vector3Int);
            ItemGenerator.SmallReward(true).Spawn(vector3Int.Above());
            return true;
        }
    }
}