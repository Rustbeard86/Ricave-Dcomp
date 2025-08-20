using System;
using System.Linq;
using UnityEngine;

namespace Ricave.Core
{
    public class RoomFiller_TrapsPit : RoomFiller
    {
        public override float GetSelectionWeight(Room room, WorldGenMemory memory)
        {
            if (room.RoomBelow != null)
            {
                return 0f;
            }
            if (!memory.UnusedBaseGear.Any<Item>())
            {
                return 0f;
            }
            if (!room.IsLeaf)
            {
                return 0f;
            }
            if (room.StartY <= 0)
            {
                return 0f;
            }
            if (room.IsBossRoom)
            {
                return 0f;
            }
            Vector3Int vector3Int;
            int num;
            int num2;
            Func<Vector3Int, Vector3Int> func;
            RoomPart_TrapsPit.GetDirectionalDimensions(room, out vector3Int, out num, out num2, out func);
            if (num < 4 || num2 < 6)
            {
                return 0f;
            }
            return base.GetSelectionWeight(room, memory);
        }

        private const int MinWidth = 4;

        private const int MinLength = 6;
    }
}