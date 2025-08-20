using System;
using UnityEngine;

namespace Ricave.Core
{
    public class RoomPart_Ceiling : RoomPart
    {
        public override void Generate(Room room, WorldGenMemory memory)
        {
            if (room.RoomAbove != null)
            {
                return;
            }
            World world = Get.World;
            int startY = room.StartY;
            int height = room.Height;
            foreach (Vector2Int vector2Int in room.Surface)
            {
                Vector3Int vector3Int = new Vector3Int(vector2Int.x, startY + height - 1, vector2Int.y);
                Room adjacentRoomAtNoCorners = room.GetAdjacentRoomAtNoCorners(vector2Int);
                if ((adjacentRoomAtNoCorners == null || adjacentRoomAtNoCorners.RoomAbove == null || !adjacentRoomAtNoCorners.RoomAbove.Shape.Contains(vector3Int)) && !world.AnyEntityAt(vector3Int))
                {
                    Maker.Make(Get.Entity_Floor, null, false, false, true).Spawn(vector3Int);
                }
            }
        }
    }
}