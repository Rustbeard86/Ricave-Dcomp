using System;
using UnityEngine;

namespace Ricave.Core
{
    public class RoomFiller_Pit : RoomFiller
    {
        public override float GetSelectionWeight(Room room, WorldGenMemory memory)
        {
            if (room.RoomBelow != null)
            {
                return 0f;
            }
            if (!room.IsLeaf)
            {
                return 0f;
            }
            if (room.IsBossRoom)
            {
                return 0f;
            }
            int floor = Get.Floor;
            int? floorCount = Get.RunSpec.FloorCount;
            if ((floor == floorCount.GetValueOrDefault()) & (floorCount != null))
            {
                return 0f;
            }
            Vector3Int vector3Int;
            int num;
            int num2;
            Func<Vector3Int, Vector3Int> func;
            RoomPart_TrapsPit.GetDirectionalDimensions(room, out vector3Int, out num, out num2, out func);
            if (num < 3 || num2 < 5)
            {
                return 0f;
            }
            return base.GetSelectionWeight(room, memory);
        }

        private const int MinWidth = 3;

        private const int MinLength = 5;
    }
}