using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ricave.Core
{
    public static class BiggestRectFinder
    {
        public static CellCuboid FindBiggestRect(CellCuboid rect, Predicate<Vector3Int> predicate)
        {
            BiggestRectFinder.<> c__DisplayClass0_0 CS$<> 8__locals1 = new BiggestRectFinder.<> c__DisplayClass0_0();
            CS$<> 8__locals1.predicate = predicate;
            CS$<> 8__locals1.rect = rect;
            if (CS$<> 8__locals1.rect.height != 1)
			{
                Log.Error("Passed cuboid with non-1 height to FindBiggestRect().", false);
                return default(CellCuboid);
            }
            if (CS$<> 8__locals1.rect.Empty)
			{
                return CS$<> 8__locals1.rect;
            }
            CellCuboid cellCuboid = default(CellCuboid);
            for (int i = 0; i < 20; i++)
            {
                BiggestRectFinder.<> c__DisplayClass0_1 CS$<> 8__locals2 = new BiggestRectFinder.<> c__DisplayClass0_1();
                CS$<> 8__locals2.CS$<> 8__locals1 = CS$<> 8__locals1;
                IEnumerable<Vector3Int> enumerable = CS$<> 8__locals2.CS$<> 8__locals1.rect;
                Func<Vector3Int, bool> func;
                if ((func = CS$<> 8__locals2.CS$<> 8__locals1.<> 9__0) == null)
				{
                func = (CS$<> 8__locals2.CS$<> 8__locals1.<> 9__0 = (Vector3Int x) => CS$<> 8__locals2.CS$<> 8__locals1.predicate(x));
            }
            Vector3Int vector3Int;
            if (!enumerable.Where<Vector3Int>(func).TryGetRandomElement<Vector3Int>(out vector3Int))
            {
                break;
            }
            CS$<> 8__locals2.cur = new CellCuboid(vector3Int, 0);
            for (; ; )
            {
                if (CS$<> 8__locals2.cur.width < CS$<> 8__locals2.cur.depth)
					{
                if (!CS$<> 8__locals2.< FindBiggestRect > g__TryExpand | 1(1, 0) && !CS$<> 8__locals2.< FindBiggestRect > g__TryExpand | 1(-1, 0) && !CS$<> 8__locals2.< FindBiggestRect > g__TryExpand | 1(0, 1))
						{
                    if (!CS$<> 8__locals2.< FindBiggestRect > g__TryExpand | 1(0, -1))
							{
                        break;
                    }
                }
            }

                    else if (!CS$<> 8__locals2.< FindBiggestRect > g__TryExpand | 1(0, 1) && !CS$<> 8__locals2.< FindBiggestRect > g__TryExpand | 1(0, -1) && !CS$<> 8__locals2.< FindBiggestRect > g__TryExpand | 1(1, 0) && !CS$<> 8__locals2.< FindBiggestRect > g__TryExpand | 1(-1, 0))
					{
                break;
            }
        }
				if (CS$<>8__locals2.cur.Volume > cellCuboid.Volume)
				{
					cellCuboid = CS$<>8__locals2.cur;
				}
}
return cellCuboid;
		}

		public static bool TryFindRectOfSize(CellCuboid rect, Predicate<Vector3Int> predicate, int size, out CellCuboid foundRect)
{
    return BiggestRectFinder.TryFindRectOfMinMaxSize(rect, predicate, size, size, out foundRect);
}

public static bool TryFindRectOfMinMaxSize(CellCuboid rect, Predicate<Vector3Int> predicate, int minSize, int maxSize, out CellCuboid foundRect)
{
    foundRect = BiggestRectFinder.FindBiggestRect(rect, predicate);
    if (foundRect.width < minSize || foundRect.depth < minSize)
    {
        return false;
    }
    foundRect.RandomlyClampSizeXZ(maxSize, maxSize);
    return true;
}

public static bool TryFindRotatedRectOfSize(CellCuboid rect, Predicate<Vector3Int> predicate, int width, int depth, out CellCuboid foundRect, out Vector3Int foundRectDir, out Func<Vector3Int, Vector3Int> localToRotatedConverter)
{
    CellCuboid cellCuboid = BiggestRectFinder.FindBiggestRect(rect, predicate);
    if (Math.Max(cellCuboid.width, cellCuboid.depth) < Math.Max(width, depth) || Math.Min(cellCuboid.width, cellCuboid.depth) < Math.Min(width, depth))
    {
        foundRect = default(CellCuboid);
        foundRectDir = Vector3IntUtility.Forward;
        localToRotatedConverter = null;
        return false;
    }
    if (!BestRotatedInnerRectFinder.FindMostSpaciousRotatedInnerRect(cellCuboid, width, depth, out foundRect, out foundRectDir))
    {
        localToRotatedConverter = null;
        return false;
    }
    Vector3Int vector3Int;
    Vector3Int vector3Int2;
    int num;
    int num2;
    foundRect.GetRotatedOrigin(foundRectDir, out vector3Int, out vector3Int2, out num, out num2, out localToRotatedConverter);
    return true;
}
	}
}