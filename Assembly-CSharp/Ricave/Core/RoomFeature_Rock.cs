using System;
using System.Linq;
using UnityEngine;

namespace Ricave.Core
{
    public class RoomFeature_Rock : RoomFeature
    {
        public override bool TryGenerate(Room room, WorldGenMemory memory)
        {
            Vector3Int vector3Int;
            if (!room.FreeOnFloor.Where<Vector3Int>(delegate (Vector3Int x)
            {
                if (Get.CellsInfo.AnyFilledImpassableAt(x.Below()))
                {
                    return x.AdjacentCardinalCellsXZ().Any<Vector3Int>((Vector3Int y) => Get.CellsInfo.AnyFilledImpassableAt(y));
                }
                return false;
            }).TryGetRandomElement<Vector3Int>(out vector3Int))
            {
                return false;
            }
            Structure structure = Maker.Make<Structure>(Get.Entity_Rock, null, false, false, true);
            structure.InnerEntities.Add(Maker.Make<Item>(Get.Entity_Stone, null, false, false, true));
            structure.Spawn(vector3Int);
            return true;
        }
    }
}