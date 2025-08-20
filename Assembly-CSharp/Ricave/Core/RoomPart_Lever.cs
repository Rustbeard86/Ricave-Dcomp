using System;
using System.Linq;
using UnityEngine;

namespace Ricave.Core
{
    public class RoomPart_Lever : RoomPart
    {
        public override void Generate(Room room, WorldGenMemory memory)
        {
            if (room.Role != Room.LayoutRole.LeverRoom)
            {
                return;
            }
            Vector3Int vector3Int;
            if (room.FreeOnFloorNoEntranceBlocking.TryGetRandomElement<Vector3Int>(out vector3Int) || room.FreeOnFloor.TryGetRandomElement<Vector3Int>(out vector3Int))
            {
                Maker.Make(Get.Entity_Lever, null, false, false, true).Spawn(vector3Int);
                return;
            }
            if (room.Shape.InnerCuboid(1).BottomSurfaceCuboid.Where<Vector3Int>((Vector3Int x) => Get.CellsInfo.CanPassThroughNoActors(x) && Get.CellsInfo.IsFloorUnderNoActors(x)).TryGetRandomElement<Vector3Int>(out vector3Int))
            {
                Log.Warning("Could not find unoccupied place for a lever. Used a passable fallback. May collide with something and look bad.", false);
                Maker.Make(Get.Entity_Lever, null, false, false, true).Spawn(vector3Int);
                return;
            }
            Log.Error("Could not place lever in the lever room.", false);
        }
    }
}