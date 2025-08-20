using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Ricave.Core
{
    public static class BestRotatedInnerRectFinder
    {
        public static bool FindBestRotatedInnerRect(CellCuboid rect, int width, int depth, Func<CellCuboid, Vector3Int, float> scoreGetter, out CellCuboid innerRect, out Vector3Int dir)
        {
            BestRotatedInnerRectFinder.<> c__DisplayClass0_0 CS$<> 8__locals1;
            CS$<> 8__locals1.scoreGetter = scoreGetter;
            if (rect.height != 1)
            {
                Log.Error("Passed cuboid with non-1 height to FindBestRotatedInnerRect().", false);
                innerRect = default(CellCuboid);
                dir = Vector3IntUtility.Forward;
                return false;
            }
            if (Math.Max(rect.width, rect.depth) < Math.Max(width, depth) || Math.Min(rect.width, rect.depth) < Math.Min(width, depth))
            {
                innerRect = default(CellCuboid);
                dir = Vector3IntUtility.Forward;
                return false;
            }
            CS$<> 8__locals1.best = default(CellCuboid);
            CS$<> 8__locals1.bestDir = Vector3IntUtility.Forward;
            CS$<> 8__locals1.bestScore = 0f;
            for (int i = 0; i < 30; i++)
            {
                if (rect.width >= width && rect.depth >= depth)
                {
                    CellCuboid cellCuboid = rect;
                    cellCuboid.RandomlyClampSizeXZ(width, depth);
                    BestRotatedInnerRectFinder.< FindBestRotatedInnerRect > g__CheckIfBetter | 0_0(cellCuboid, Vector3IntUtility.Forward, ref CS$<> 8__locals1);
                    BestRotatedInnerRectFinder.< FindBestRotatedInnerRect > g__CheckIfBetter | 0_0(cellCuboid, Vector3IntUtility.Back, ref CS$<> 8__locals1);
                }
                if (rect.width >= depth && rect.depth >= width)
                {
                    CellCuboid cellCuboid2 = rect;
                    cellCuboid2.RandomlyClampSizeXZ(depth, width);
                    BestRotatedInnerRectFinder.< FindBestRotatedInnerRect > g__CheckIfBetter | 0_0(cellCuboid2, Vector3IntUtility.Left, ref CS$<> 8__locals1);
                    BestRotatedInnerRectFinder.< FindBestRotatedInnerRect > g__CheckIfBetter | 0_0(cellCuboid2, Vector3IntUtility.Right, ref CS$<> 8__locals1);
                }
            }
            if (!CS$<> 8__locals1.best.Empty)
			{
                innerRect = CS$<> 8__locals1.best;
                dir = CS$<> 8__locals1.bestDir;
                return true;
            }
            innerRect = default(CellCuboid);
            dir = Vector3IntUtility.Forward;
            return false;
        }

        public static bool FindMostSpaciousRotatedInnerRect(CellCuboid rect, int width, int depth, out CellCuboid innerRect, out Vector3Int dir)
        {
            return BestRotatedInnerRectFinder.FindBestRotatedInnerRect(rect, width, depth, delegate (CellCuboid candidate, Vector3Int candidateDir)
            {
                float num = 0f;
                float num2 = 0f;
                int num3 = 0;
                foreach (Vector3Int vector3Int in candidate.GetEdgeCells(candidateDir))
                {
                    num3++;
                    Vector3Int vector3Int2 = vector3Int + candidateDir;
                    while (vector3Int2.InBounds())
                    {
                        if (Get.CellsInfo.AnyDoorAt(vector3Int2))
                        {
                            num += 10f;
                        }
                        if (!Get.CellsInfo.CanPassThroughNoActors(vector3Int2))
                        {
                            break;
                        }
                        num2 += 1f;
                        vector3Int2 += candidateDir;
                    }
                }
                return num2 / (float)num3 + num;
            }, out innerRect, out dir);
        }

        [CompilerGenerated]
        internal static void <FindBestRotatedInnerRect>g__CheckIfBetter|0_0(CellCuboid candidate, Vector3Int candidateDir, ref BestRotatedInnerRectFinder.<>c__DisplayClass0_0 A_2)
		{
            float num = A_2.scoreGetter(candidate, candidateDir);
			if (A_2.best.Empty || num > A_2.bestScore)
			{
				A_2.best = candidate;
				A_2.bestDir = candidateDir;
				A_2.bestScore = num;
			}
}
	}
}