using System;
using System.Linq;
using UnityEngine;

namespace Ricave.Core
{
    public class GenPass_CreateWorld : GenPass
    {
        public override int SeedPart
        {
            get
            {
                return 278461091;
            }
        }

        public override void DoPass(WorldGenMemory memory)
        {
            int num = memory.AllRooms.First<Room>().Shape.xMax;
            int num2 = memory.AllRooms.First<Room>().Shape.yMax;
            int num3 = memory.AllRooms.First<Room>().Shape.zMax;
            for (int i = 0; i < memory.storeys.Count; i++)
            {
                foreach (Room room in memory.storeys[i].Rooms)
                {
                    num = Math.Max(num, room.Shape.xMax);
                    num2 = Math.Max(num2, room.Shape.yMax);
                    num3 = Math.Max(num3, room.Shape.zMax);
                }
            }
            Get.World.InitAsNew(new Vector3Int(num + 1 + 1, num2 + 1 + 1, num3 + 1 + 1), new WorldInfo(memory.config));
        }

        public const int ExtraPad = 1;

        public const int Height = 10;
    }
}