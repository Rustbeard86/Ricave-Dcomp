using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ricave.Core
{
    public static class MapFromString
    {
        public static void Generate(string map, Action<int, int, char> symbolHandler, WorldGenMemory memory, int height = 4)
        {
            List<string> list = (from x in map.Trim().Split('\n', StringSplitOptions.None)
                                 select x.Trim()).Reverse<string>().ToList<string>();
            int length = list[0].Length;
            int count = list.Count;
            Get.World.InitAsNew(new Vector3Int(length, height, count), new WorldInfo(memory.config));
            for (int i = 0; i < count; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    symbolHandler(j, i, list[i][j]);
                }
            }
        }

        public static void GetCoordinates(int x, int z, out Vector3Int floor, out Vector3Int main, out Vector3Int aboveMain, out Vector3Int ceiling, out IEnumerable<Vector3Int> wall, int height = 4)
        {
            floor = new Vector3Int(x, 0, z);
            main = new Vector3Int(x, 1, z);
            aboveMain = new Vector3Int(x, 2, z);
            ceiling = new Vector3Int(x, height - 1, z);
            wall = from y in Enumerable.Range(0, height)
                   select new Vector3Int(x, y, z);
        }
    }
}