using System;
using UnityEngine;

namespace Ricave.Core
{
    public class GenPass_EdgeWalls : GenPass
    {
        public override int SeedPart
        {
            get
            {
                return 375091114;
            }
        }

        public override void DoPass(WorldGenMemory memory)
        {
            Vector3Int size = Get.World.Size;
            for (int i = 0; i < size.x; i++)
            {
                for (int j = 0; j < size.y; j++)
                {
                    for (int k = 0; k < size.z; k++)
                    {
                        if (i == 0 || j == 0 || k == 0 || i == size.x - 1 || j == size.y - 1 || k == size.z - 1)
                        {
                            Maker.Make((j == 0 || j == size.y - 1) ? Get.Entity_Floor : Get.Entity_Wall, null, false, false, true).Spawn(new Vector3Int(i, j, k));
                        }
                    }
                }
            }
        }
    }
}