using System;
using System.Linq;
using UnityEngine;

namespace Ricave.Core
{
    public class RoomFeature_BigMagnet : RoomFeature
    {
        public override bool TryGenerate(Room room, WorldGenMemory memory)
        {
            Vector3Int vector3Int;
            if (!room.FreeOnFloorNonBlocking.Where<Vector3Int>((Vector3Int x) => x.AdjacentCellsXZ().All<Vector3Int>((Vector3Int y) => !Get.World.AnyEntityAt(y)) && !this.AnyForbiddenEntityInRadius(x)).TryGetRandomElement<Vector3Int>(out vector3Int))
            {
                return false;
            }
            Maker.Make<Structure>(Get.Entity_BigMagnet, null, false, false, true).Spawn(vector3Int);
            return true;
        }

        private bool AnyForbiddenEntityInRadius(Vector3Int pos)
        {
            foreach (Vector3Int vector3Int in pos.GetCellsWithin(3))
            {
                if (vector3Int.InBounds() && Get.CellsInfo.AnyLadderAt(vector3Int) && LineOfSight.IsLineOfFire(pos, vector3Int))
                {
                    return true;
                }
            }
            return false;
        }
    }
}