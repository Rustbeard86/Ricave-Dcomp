using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class GenPass_DebugWorld : GenPass
    {
        public override int SeedPart
        {
            get
            {
                return 751285723;
            }
        }

        public override void DoPass(WorldGenMemory memory)
        {
            World world = Get.World;
            world.InitAsNew(new Vector3Int(50, 20, 50), new WorldInfo(memory.config));
            memory.playerStartPos = new Vector3Int(1, 1, 1);
            int num = 1;
            int num2 = 2;
            List<EntitySpec> all = Get.Specs.GetAll<EntitySpec>();
            for (int i = 0; i < all.Count; i++)
            {
                if (all[i].IsItem && all[i] != Get.Entity_UnlockableAsItem && all[i] != Get.Entity_PrivateRoomStructureAsItem)
                {
                    this.Advance(world, ref num, ref num2, 2);
                    Maker.Make(all[i], null, false, false, true).Spawn(new Vector3Int(num, 1, num2));
                }
            }
            num += 2;
            num2 = 2;
            for (int j = 0; j < all.Count; j++)
            {
                if (all[j].IsStructure && all[j] != Get.Entity_BossTrophy)
                {
                    this.Advance(world, ref num, ref num2, 2);
                    Maker.Make(all[j], null, false, false, true).Spawn(new Vector3Int(num, 1, num2));
                }
            }
            this.DoSeparator(world, ref num, ref num2);
            num++;
            for (int k = 0; k < all.Count; k++)
            {
                if (all[k].IsActor && all[k] != Get.Entity_Player && all[k] != Get.Entity_LobbyPlayer)
                {
                    this.AdvanceMultiple(world, ref num, ref num2, 4, 3);
                    CellRect cellRect = new CellRect(num, num2 - 2, 3, 3);
                    foreach (Vector2Int vector2Int in cellRect.EdgeCells)
                    {
                        if (vector2Int.x == cellRect.x && vector2Int.y == cellRect.Center.y)
                        {
                            Maker.Make(Get.Entity_Door, null, false, false, true).Spawn(new Vector3Int(vector2Int.x, 1, vector2Int.y));
                        }
                        else
                        {
                            Maker.Make(Get.Entity_Wall, null, false, false, true).Spawn(new Vector3Int(vector2Int.x, 1, vector2Int.y));
                        }
                    }
                    foreach (Vector2Int vector2Int2 in cellRect)
                    {
                        if (vector2Int2 == cellRect.Center)
                        {
                            Actor actor = Maker.Make<Actor>(all[k], null, false, false, true);
                            actor.Conditions.AddCondition(new Condition_Sleeping(Get.Condition_Sleeping), -1);
                            actor.Spawn(new Vector3Int(vector2Int2.x, 1, vector2Int2.y));
                        }
                        else
                        {
                            Maker.Make(Get.Entity_Wall, null, false, false, true).Spawn(new Vector3Int(vector2Int2.x, 2, vector2Int2.y));
                        }
                    }
                }
            }
            num += 2;
            this.DoSeparator(world, ref num, ref num2);
        }

        private void DoSeparator(World world, ref int curX, ref int curZ)
        {
            curX += 2;
            for (int i = 3; i < world.Size.z - 1; i++)
            {
                Maker.Make(Get.Entity_Wall, null, false, false, true).Spawn(new Vector3Int(curX, 1, i));
                Maker.Make(Get.Entity_Wall, null, false, false, true).Spawn(new Vector3Int(curX, 2, i));
            }
            curX++;
            curZ = 2;
        }

        private void Advance(World world, ref int curX, ref int curZ, int advanceXAtEnd)
        {
            curZ++;
            if (curZ == world.Size.z - 1)
            {
                curX += advanceXAtEnd;
                curZ = 3;
            }
        }

        private void AdvanceMultiple(World world, ref int curX, ref int curZ, int advanceXAtEnd, int times)
        {
            int num = 0;
            for (int i = 0; i < times * 2; i++)
            {
                int num2 = curX;
                this.Advance(world, ref curX, ref curZ, advanceXAtEnd);
                if (num2 != curX)
                {
                    num = 1;
                }
                else
                {
                    num++;
                }
                if (num >= times)
                {
                    return;
                }
            }
        }
    }
}