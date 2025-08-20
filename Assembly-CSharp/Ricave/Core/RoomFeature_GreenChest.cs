using System;
using System.Linq;
using UnityEngine;

namespace Ricave.Core
{
    public class RoomFeature_GreenChest : RoomFeature
    {
        public override bool TryGenerate(Room room, WorldGenMemory memory)
        {
            if (WorldGenUtility.MiniChallengeForCurrentWorld == MiniChallenge.GreenKeyFragments)
            {
                Vector3Int vector3Int;
                if (room.FreeOnFloorNonBlocking.Where<Vector3Int>((Vector3Int x) => x.Below().InBounds() && Get.CellsInfo.AnyPermanentFilledImpassableAt(x.Below())).TryGetRandomElement<Vector3Int>(out vector3Int))
                {
                    Item item = WorldGenUtility.GenerateMiniChallengeReward();
                    Structure structure = Maker.Make<Structure>(Get.Entity_GreenChest, null, false, false, true);
                    structure.InnerEntities.Add(item);
                    structure.Spawn(vector3Int);
                    return true;
                }
            }
            return false;
        }
    }
}