using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public static class AdjacentCellsCalculator
    {
        public static List<Vector3Int> GetAdjacentCells(List<Vector3Int> cells)
        {
            List<Vector3Int> list = FrameLocalPool<List<Vector3Int>>.Get();
            AdjacentCellsCalculator.GetAdjacentCells(cells, list);
            return list;
        }

        public static void GetAdjacentCells(List<Vector3Int> cells, List<Vector3Int> outResult)
        {
            outResult.Clear();
            if (cells.Count == 0)
            {
                return;
            }
            Vector3Int size = Get.World.Size;
            if (AdjacentCellsCalculator.tmpCells == null || AdjacentCellsCalculator.tmpCells.GetLength(0) + 2 != size.x || AdjacentCellsCalculator.tmpCells.GetLength(1) + 2 != size.y || AdjacentCellsCalculator.tmpCells.GetLength(2) + 2 != size.z)
            {
                AdjacentCellsCalculator.tmpCells = new bool[size.x + 2, size.y + 2, size.z + 2];
            }
            int count = cells.Count;
            Vector3Int vector3Int = cells[0];
            int num = vector3Int.x;
            int num2 = vector3Int.y;
            int num3 = vector3Int.z;
            int num4 = vector3Int.x;
            int num5 = vector3Int.y;
            int num6 = vector3Int.z;
            for (int i = 1; i < count; i++)
            {
                Vector3Int vector3Int2 = cells[i];
                num = Math.Min(num, vector3Int2.x);
                num2 = Math.Min(num2, vector3Int2.y);
                num3 = Math.Min(num3, vector3Int2.z);
                num4 = Math.Max(num4, vector3Int2.x);
                num5 = Math.Max(num5, vector3Int2.y);
                num6 = Math.Max(num6, vector3Int2.z);
            }
            if (num > 0)
            {
                num--;
            }
            if (num2 > 0)
            {
                num2--;
            }
            if (num3 > 0)
            {
                num3--;
            }
            if (num4 < size.x - 1)
            {
                num4++;
            }
            if (num5 < size.y - 1)
            {
                num5++;
            }
            if (num6 < size.z - 1)
            {
                num6++;
            }
            for (int j = num; j <= num4; j++)
            {
                for (int k = num2; k <= num5; k++)
                {
                    for (int l = num3; l <= num6; l++)
                    {
                        AdjacentCellsCalculator.tmpCells[j + 1, k + 1, l + 1] = false;
                    }
                }
            }
            for (int m = 0; m < count; m++)
            {
                Vector3Int vector3Int3 = cells[m];
                AdjacentCellsCalculator.tmpCells[vector3Int3.x + 1, vector3Int3.y + 1, vector3Int3.z + 1] = true;
                AdjacentCellsCalculator.tmpCells[vector3Int3.x, vector3Int3.y + 1, vector3Int3.z + 1] = true;
                AdjacentCellsCalculator.tmpCells[vector3Int3.x + 2, vector3Int3.y + 1, vector3Int3.z + 1] = true;
            }
            for (int n = num; n <= num4; n++)
            {
                for (int num7 = num2; num7 <= num5; num7++)
                {
                    for (int num8 = num3; num8 <= num6; num8++)
                    {
                        if (AdjacentCellsCalculator.tmpCells[n + 1, num7 + 1, num8 + 1])
                        {
                            AdjacentCellsCalculator.tmpCells[n + 1, num7, num8 + 1] = true;
                        }
                    }
                }
            }
            for (int num9 = num4; num9 >= num; num9--)
            {
                for (int num10 = num5; num10 >= num2; num10--)
                {
                    for (int num11 = num6; num11 >= num3; num11--)
                    {
                        if (AdjacentCellsCalculator.tmpCells[num9 + 1, num10 + 1, num11 + 1])
                        {
                            AdjacentCellsCalculator.tmpCells[num9 + 1, num10 + 2, num11 + 1] = true;
                        }
                    }
                }
            }
            for (int num12 = num; num12 <= num4; num12++)
            {
                for (int num13 = num2; num13 <= num5; num13++)
                {
                    for (int num14 = num3; num14 <= num6; num14++)
                    {
                        if (AdjacentCellsCalculator.tmpCells[num12 + 1, num13 + 1, num14 + 1])
                        {
                            AdjacentCellsCalculator.tmpCells[num12 + 1, num13 + 1, num14] = true;
                        }
                    }
                }
            }
            for (int num15 = num4; num15 >= num; num15--)
            {
                for (int num16 = num5; num16 >= num2; num16--)
                {
                    for (int num17 = num6; num17 >= num3; num17--)
                    {
                        if (AdjacentCellsCalculator.tmpCells[num15 + 1, num16 + 1, num17 + 1])
                        {
                            AdjacentCellsCalculator.tmpCells[num15 + 1, num16 + 1, num17 + 2] = true;
                        }
                    }
                }
            }
            for (int num18 = num; num18 <= num4; num18++)
            {
                for (int num19 = num2; num19 <= num5; num19++)
                {
                    for (int num20 = num3; num20 <= num6; num20++)
                    {
                        if (AdjacentCellsCalculator.tmpCells[num18 + 1, num19 + 1, num20 + 1])
                        {
                            outResult.Add(new Vector3Int(num18, num19, num20));
                        }
                    }
                }
            }
        }

        private static bool[,,] tmpCells;
    }
}