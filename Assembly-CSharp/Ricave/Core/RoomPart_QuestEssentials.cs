using System;
using System.Linq;
using UnityEngine;

namespace Ricave.Core
{
    public class RoomPart_QuestEssentials : RoomPart
    {
        public override void Generate(Room room, WorldGenMemory memory)
        {
            if (Get.RunConfig.ProgressDisabled)
            {
                return;
            }
            if (Get.Quest_MemoryPiece1.IsActive() && memory.config.Floor == 3 && !Get.World.AnyEntityOfSpec(Get.Entity_MemoryPiece1))
            {
                Vector3Int vector3Int;
                if (room.FreeOnFloorNonBlocking.Where<Vector3Int>((Vector3Int x) => x.Below().InBounds() && Get.CellsInfo.AnyPermanentFilledImpassableAt(x.Below())).TryGetRandomElement<Vector3Int>(out vector3Int))
                {
                    Maker.Make(Get.Entity_Vines, null, false, false, true).Spawn(vector3Int);
                    Maker.Make(Get.Entity_MemoryPiece1, null, false, false, true).Spawn(vector3Int);
                    return;
                }
            }
            if (memory.config.Floor == 2 && Get.Quest_RescueLobbyShopkeeper.IsActive() && !Get.World.AnyEntityOfSpec(Get.Entity_LobbyShopkeeperCage))
            {
                Vector3Int vector3Int2;
                if (room.FreeOnFloorNonBlocking.Where<Vector3Int>((Vector3Int x) => x.Below().InBounds() && Get.CellsInfo.AnyPermanentFilledImpassableAt(x.Below())).TryGetRandomElement<Vector3Int>(out vector3Int2))
                {
                    Maker.Make(Get.Entity_LobbyShopkeeperCage, null, false, false, true).Spawn(vector3Int2);
                    return;
                }
            }
            if (memory.config.Floor == 6 && room.Height >= 4 && Get.Quest_MemoryPiece3.IsActive() && !Get.World.AnyEntityOfSpec(Get.Entity_MemoryPiece3))
            {
                Vector3Int vector3Int3;
                if (room.FreeOnFloorNonBlocking.Where<Vector3Int>(delegate (Vector3Int x)
                {
                    if (!Get.World.AnyEntityAt(x.Above()) && x.Below().InBounds() && Get.CellsInfo.AnyPermanentFilledImpassableAt(x.Below()))
                    {
                        return x.AdjacentCellsXZ().All<Vector3Int>((Vector3Int y) => !Get.World.AnyEntityAt(y));
                    }
                    return false;
                }).TryGetRandomElement<Vector3Int>(out vector3Int3))
                {
                    Maker.Make(Get.Entity_Wall, null, false, false, true).Spawn(vector3Int3);
                    Maker.Make(Get.Entity_MemoryPiece3, null, false, false, true).Spawn(vector3Int3.Above());
                    return;
                }
                Vector3Int vector3Int4;
                if (room.FreeOnFloorNonBlocking.Where<Vector3Int>((Vector3Int x) => !Get.World.AnyEntityAt(x.Above()) && x.Below().InBounds() && Get.CellsInfo.AnyPermanentFilledImpassableAt(x.Below())).TryGetRandomElement<Vector3Int>(out vector3Int4))
                {
                    Maker.Make(Get.Entity_Wall, null, false, false, true).Spawn(vector3Int4);
                    Maker.Make(Get.Entity_MemoryPiece3, null, false, false, true).Spawn(vector3Int4.Above());
                    return;
                }
            }
            if (Get.Quest_MemoryPiece4.IsActive() && memory.config.Floor == 8 && !Get.World.AnyEntityOfSpec(Get.Entity_MemoryPiece4))
            {
                Vector3Int vector3Int5;
                if (room.FreeOnFloorNonBlocking.Where<Vector3Int>((Vector3Int x) => x.Below().InBounds() && Get.CellsInfo.AnyPermanentFilledImpassableAt(x.Below())).TryGetRandomElement<Vector3Int>(out vector3Int5))
                {
                    Maker.Make(Get.Entity_BigPoisonTrap, null, false, false, true).Spawn(vector3Int5);
                    Maker.Make(Get.Entity_MemoryPiece4, null, false, false, true).Spawn(vector3Int5);
                    return;
                }
            }
        }
    }
}