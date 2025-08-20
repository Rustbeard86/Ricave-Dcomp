using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ricave.Core
{
    public class RoomFeature_ItemInBigPoisonTrap : RoomFeature
    {
        public override bool TryGenerate(Room room, WorldGenMemory memory)
        {
            World world = Get.World;
            Func<Vector3Int, bool> <> 9__1;
            Vector3Int vector3Int;
            if (!room.FreeOnFloorNonBlocking.Where<Vector3Int>(delegate (Vector3Int x)
            {
                if (world.CellsInfo.AnyPermanentFilledImpassableAt(x.Below()))
                {
                    IEnumerable<Vector3Int> enumerable = x.AdjacentCells();
                    Func<Vector3Int, bool> func;
                    if ((func = <> 9__1) == null)
                    {
                        func = (<> 9__1 = (Vector3Int y) => !world.CellsInfo.CanPassThroughNoActors(y) || world.CellsInfo.CanSeeThrough(y));
                    }
                    return enumerable.All<Vector3Int>(func);
                }
                return false;
            }).TryGetRandomElement<Vector3Int>(out vector3Int))
            {
                return false;
            }
            Maker.Make(Get.Entity_BigPoisonTrap, null, false, false, true).Spawn(vector3Int);
            ItemGenerator.Reward(true).Spawn(vector3Int);
            return true;
        }
    }
}