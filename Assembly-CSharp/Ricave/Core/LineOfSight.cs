using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Ricave.Core
{
    public static class LineOfSight
    {
        public static bool IsLineOfSight(Vector3Int from, Vector3Int to)
        {
            CellsInfo cellsInfo = Get.CellsInfo;
            Func<Vector3Int, bool> func;
            LineOfSight.PredicateType predicateType;
            if ((!cellsInfo.CanPassThrough(to) || (cellsInfo.CanSeeThroughToImpassable(to) && !cellsInfo.CanSeeThrough(to))) && !cellsInfo.AnyActorAt(to) && !cellsInfo.AnyItemAt(to))
            {
                func = LineOfSight.LineOfSightPredicateToImpassable;
                predicateType = LineOfSight.PredicateType.LineOfSightToImpassable;
            }
            else
            {
                func = LineOfSight.LineOfSightPredicate;
                predicateType = LineOfSight.PredicateType.LineOfSight;
            }
            Profiler.Begin("IsLineOfSight()");
            bool flag;
            try
            {
                flag = LineOfSight.CheckCellsInLineSmart(from, to, func, predicateType);
            }
            finally
            {
                Profiler.End();
            }
            return flag;
        }

        public static bool IsLineOfFire(Vector3Int from, Vector3Int to)
        {
            Profiler.Begin("IsLineOfFire()");
            bool flag;
            try
            {
                flag = LineOfSight.CheckCellsInLineSmart(from, to, LineOfSight.LineOfFirePredicate, LineOfSight.PredicateType.LineOfFire);
            }
            finally
            {
                Profiler.End();
            }
            return flag;
        }

        private static bool CheckCellsInLineSmart(Vector3Int from, Vector3Int to, Func<Vector3Int, bool> predicate, LineOfSight.PredicateType predicateType)
        {
            LineOfSight.<> c__DisplayClass6_0 CS$<> 8__locals1;
            CS$<> 8__locals1.to = to;
            CS$<> 8__locals1.predicate = predicate;
            CS$<> 8__locals1.from = from;
            CS$<> 8__locals1.predicateType = predicateType;
            if (LineOfSight.CheckCellsInLineRaw(CS$<> 8__locals1.from, CS$<> 8__locals1.to, CS$<> 8__locals1.predicateType))
            {
                return true;
            }
            CS$<> 8__locals1.cellsInfo = Get.CellsInfo;
            CS$<> 8__locals1.aboveFrom = CS$<> 8__locals1.from.Above();
            CS$<> 8__locals1.fromIsOnStairs = CS$<> 8__locals1.cellsInfo.AnyStairsAt(CS$<> 8__locals1.from) && CS$<> 8__locals1.aboveFrom.InBounds() && CS$<> 8__locals1.predicate(CS$<> 8__locals1.aboveFrom);
            if (CS$<> 8__locals1.fromIsOnStairs && LineOfSight.CheckCellsInLineRaw(CS$<> 8__locals1.aboveFrom, CS$<> 8__locals1.to, CS$<> 8__locals1.predicateType))
			{
                return true;
            }
            if (!CS$<> 8__locals1.cellsInfo.CanPassThroughNoActors(CS$<> 8__locals1.to) && !CS$<> 8__locals1.cellsInfo.AnyActorAt(CS$<> 8__locals1.to))
			{
                if (CS$<> 8__locals1.from.x < CS$<> 8__locals1.to.x && LineOfSight.< CheckCellsInLineSmart > g__CheckAdjLOS | 6_0(Vector3Int.left, ref CS$<> 8__locals1))
				{
                    return true;
                }
                if (CS$<> 8__locals1.from.x > CS$<> 8__locals1.to.x && LineOfSight.< CheckCellsInLineSmart > g__CheckAdjLOS | 6_0(Vector3Int.right, ref CS$<> 8__locals1))
				{
                    return true;
                }
                if (CS$<> 8__locals1.from.y < CS$<> 8__locals1.to.y && LineOfSight.< CheckCellsInLineSmart > g__CheckAdjLOS | 6_0(Vector3Int.down, ref CS$<> 8__locals1))
				{
                    return true;
                }
                if ((CS$<> 8__locals1.from.y > CS$<> 8__locals1.to.y || (CS$<> 8__locals1.fromIsOnStairs && CS$<> 8__locals1.from.y + 1 > CS$<> 8__locals1.to.y)) && LineOfSight.< CheckCellsInLineSmart > g__CheckAdjLOS | 6_0(Vector3Int.up, ref CS$<> 8__locals1))
				{
                    return true;
                }
                if (CS$<> 8__locals1.from.z < CS$<> 8__locals1.to.z && LineOfSight.< CheckCellsInLineSmart > g__CheckAdjLOS | 6_0(new Vector3Int(0, 0, -1), ref CS$<> 8__locals1))
				{
                    return true;
                }
                if (CS$<> 8__locals1.from.z > CS$<> 8__locals1.to.z && LineOfSight.< CheckCellsInLineSmart > g__CheckAdjLOS | 6_0(new Vector3Int(0, 0, 1), ref CS$<> 8__locals1))
				{
                    return true;
                }
            }

            else
            {
                Vector3Int vector3Int = CS$<> 8__locals1.to.Above();
                if (CS$<> 8__locals1.cellsInfo.AnyStairsAt(CS$<> 8__locals1.to) && vector3Int.InBounds() && CS$<> 8__locals1.predicate(vector3Int))
				{
                    if (LineOfSight.CheckCellsInLineRaw(CS$<> 8__locals1.from, vector3Int, CS$<> 8__locals1.predicateType))
                    {
                        return true;
                    }
                    if (CS$<> 8__locals1.fromIsOnStairs && LineOfSight.CheckCellsInLineRaw(CS$<> 8__locals1.aboveFrom, vector3Int, CS$<> 8__locals1.predicateType))
					{
                        return true;
                    }
                }
            }
            return false;
        }

        private static bool CheckCellsInLineRaw(Vector3Int from, Vector3Int to, LineOfSight.PredicateType predicateType)
        {
            switch (predicateType)
            {
                case LineOfSight.PredicateType.LineOfSight:
                    return LineOfSight.CheckCellsInLineRaw_LineOfSight(from, to);
                case LineOfSight.PredicateType.LineOfSightToImpassable:
                    return LineOfSight.CheckCellsInLineRaw_LineOfSightToImpassable(from, to);
                case LineOfSight.PredicateType.LineOfFire:
                    return LineOfSight.CheckCellsInLineRaw_LineOfFire(from, to);
                default:
                    return false;
            }
        }

        private static bool CheckCellsInLineRaw(Vector3Int from, Vector3Int to, Func<Vector3Int, bool> predicate)
        {
            if (from == to)
            {
                return true;
            }
            int num = Math.Abs(to.x - from.x);
            int num2 = Math.Abs(to.y - from.y);
            int num3 = Math.Abs(to.z - from.z);
            int num4 = ((to.x > from.x) ? 1 : ((to.x < from.x) ? (-1) : 0));
            int num5 = ((to.y > from.y) ? 1 : ((to.y < from.y) ? (-1) : 0));
            int num6 = ((to.z > from.z) ? 1 : ((to.z < from.z) ? (-1) : 0));
            float num7 = (float)from.x + 0.5f;
            float num8 = (float)from.y + 0.5f;
            float num9 = (float)from.z + 0.5f;
            float num10 = (float)to.x + 0.5f;
            float num11 = (float)to.y + 0.5f;
            float num12 = (float)to.z + 0.5f;
            int num13 = from.x;
            int num14 = from.y;
            int num15 = from.z;
            int num16 = from.x + ((to.x > from.x) ? 1 : 0);
            int num17 = from.y + ((to.y > from.y) ? 1 : 0);
            float num18 = (float)(from.z + ((to.z > from.z) ? 1 : 0));
            object obj = ((num10 == num7) ? 1f : (num10 - num7));
            float num19 = ((num11 == num8) ? 1f : (num11 - num8));
            float num20 = ((num12 == num9) ? 1f : (num12 - num9));
            object obj2 = obj;
            float num21 = obj2 * num19;
            float num22 = obj2 * num20;
            float num23 = num19 * num20;
            float num24 = ((float)num16 - num7) * num23;
            float num25 = ((float)num17 - num8) * num22;
            float num26 = (num18 - num9) * num21;
            float num27 = (float)num4 * num23;
            float num28 = (float)num5 * num22;
            float num29 = (float)num6 * num21;
            int num30 = num + num2 + num3 + 1;
            for (int i = 0; i < num30; i++)
            {
                if (num13 == to.x && num14 == to.y && num15 == to.z)
                {
                    return true;
                }
                if ((num13 != from.x || num14 != from.y || num15 != from.z) && !predicate(new Vector3Int(num13, num14, num15)))
                {
                    return false;
                }
                float num31 = ((num24 < 0f) ? (-num24) : num24);
                float num32 = ((num25 < 0f) ? (-num25) : num25);
                float num33 = ((num26 < 0f) ? (-num26) : num26);
                bool flag = num4 != 0 && (num5 == 0 || num31 <= num32 + 1E-07f) && (num6 == 0 || num31 <= num33 + 1E-07f);
                bool flag2 = num5 != 0 && (num4 == 0 || num32 <= num31 + 1E-07f) && (num6 == 0 || num32 <= num33 + 1E-07f);
                bool flag3 = num6 != 0 && (num4 == 0 || num33 <= num31 + 1E-07f) && (num5 == 0 || num33 <= num32 + 1E-07f);
                if (flag && flag2 && flag3)
                {
                    if ((!predicate(new Vector3Int(num13 + num4, num14, num15)) || (!predicate(new Vector3Int(num13 + num4, num14, num15 + num6)) && !predicate(new Vector3Int(num13 + num4, num14 + num5, num15)))) && (!predicate(new Vector3Int(num13, num14, num15 + num6)) || (!predicate(new Vector3Int(num13 + num4, num14, num15 + num6)) && !predicate(new Vector3Int(num13, num14 + num5, num15 + num6)))) && (!predicate(new Vector3Int(num13 + num4, num14 + num5, num15)) || (!predicate(new Vector3Int(num13 + num4, num14, num15)) && !predicate(new Vector3Int(num13, num14 + num5, num15)))) && (!predicate(new Vector3Int(num13, num14 + num5, num15 + num6)) || (!predicate(new Vector3Int(num13, num14, num15 + num6)) && !predicate(new Vector3Int(num13, num14 + num5, num15)))))
                    {
                        return false;
                    }
                }
                else if (flag && flag3)
                {
                    if (!predicate(new Vector3Int(num13 + num4, num14, num15)) && !predicate(new Vector3Int(num13, num14, num15 + num6)))
                    {
                        return false;
                    }
                }
                else if (flag && flag2)
                {
                    if (!predicate(new Vector3Int(num13 + num4, num14, num15)) && !predicate(new Vector3Int(num13, num14 + num5, num15)))
                    {
                        return false;
                    }
                }
                else if (flag3 && flag2 && !predicate(new Vector3Int(num13, num14, num15 + num6)) && !predicate(new Vector3Int(num13, num14 + num5, num15)))
                {
                    return false;
                }
                if (flag)
                {
                    num13 += num4;
                    num24 += num27;
                }
                if (flag2)
                {
                    num14 += num5;
                    num25 += num28;
                }
                if (flag3)
                {
                    num15 += num6;
                    num26 += num29;
                }
            }
            Log.Error("Too many iterations in CheckCellsInLineRaw.", false);
            return false;
        }

        private static bool CheckCellsInLineRaw_LineOfSight(Vector3Int from, Vector3Int to)
        {
            if (from == to)
            {
                return true;
            }
            int num = Math.Abs(to.x - from.x);
            int num2 = Math.Abs(to.y - from.y);
            int num3 = Math.Abs(to.z - from.z);
            int num4 = ((to.x > from.x) ? 1 : ((to.x < from.x) ? (-1) : 0));
            int num5 = ((to.y > from.y) ? 1 : ((to.y < from.y) ? (-1) : 0));
            int num6 = ((to.z > from.z) ? 1 : ((to.z < from.z) ? (-1) : 0));
            float num7 = (float)from.x + 0.5f;
            float num8 = (float)from.y + 0.5f;
            float num9 = (float)from.z + 0.5f;
            float num10 = (float)to.x + 0.5f;
            float num11 = (float)to.y + 0.5f;
            float num12 = (float)to.z + 0.5f;
            int num13 = from.x;
            int num14 = from.y;
            int num15 = from.z;
            int num16 = from.x + ((to.x > from.x) ? 1 : 0);
            int num17 = from.y + ((to.y > from.y) ? 1 : 0);
            float num18 = (float)(from.z + ((to.z > from.z) ? 1 : 0));
            object obj = ((num10 == num7) ? 1f : (num10 - num7));
            float num19 = ((num11 == num8) ? 1f : (num11 - num8));
            float num20 = ((num12 == num9) ? 1f : (num12 - num9));
            object obj2 = obj;
            float num21 = obj2 * num19;
            float num22 = obj2 * num20;
            float num23 = num19 * num20;
            float num24 = ((float)num16 - num7) * num23;
            float num25 = ((float)num17 - num8) * num22;
            float num26 = (num18 - num9) * num21;
            float num27 = (float)num4 * num23;
            float num28 = (float)num5 * num22;
            float num29 = (float)num6 * num21;
            int num30 = num + num2 + num3 + 1;
            CellsInfo.CellInfo[,,] cells = Get.CellsInfo.Cells;
            for (int i = 0; i < num30; i++)
            {
                if (num13 == to.x && num14 == to.y && num15 == to.z)
                {
                    return true;
                }
                if ((num13 != from.x || num14 != from.y || num15 != from.z) && !cells[num13, num14, num15].canSeeThrough)
                {
                    return false;
                }
                float num31 = ((num24 < 0f) ? (-num24) : num24);
                float num32 = ((num25 < 0f) ? (-num25) : num25);
                float num33 = ((num26 < 0f) ? (-num26) : num26);
                bool flag = num4 != 0 && (num5 == 0 || num31 <= num32 + 1E-07f) && (num6 == 0 || num31 <= num33 + 1E-07f);
                bool flag2 = num5 != 0 && (num4 == 0 || num32 <= num31 + 1E-07f) && (num6 == 0 || num32 <= num33 + 1E-07f);
                bool flag3 = num6 != 0 && (num4 == 0 || num33 <= num31 + 1E-07f) && (num5 == 0 || num33 <= num32 + 1E-07f);
                if (flag && flag2 && flag3)
                {
                    if ((!cells[num13 + num4, num14, num15].canSeeThrough || (!cells[num13 + num4, num14, num15 + num6].canSeeThrough && !cells[num13 + num4, num14 + num5, num15].canSeeThrough)) && (!cells[num13, num14, num15 + num6].canSeeThrough || (!cells[num13 + num4, num14, num15 + num6].canSeeThrough && !cells[num13, num14 + num5, num15 + num6].canSeeThrough)) && (!cells[num13 + num4, num14 + num5, num15].canSeeThrough || (!cells[num13 + num4, num14, num15].canSeeThrough && !cells[num13, num14 + num5, num15].canSeeThrough)) && (!cells[num13, num14 + num5, num15 + num6].canSeeThrough || (!cells[num13, num14, num15 + num6].canSeeThrough && !cells[num13, num14 + num5, num15].canSeeThrough)))
                    {
                        return false;
                    }
                }
                else if (flag && flag3)
                {
                    if (!cells[num13 + num4, num14, num15].canSeeThrough && !cells[num13, num14, num15 + num6].canSeeThrough)
                    {
                        return false;
                    }
                }
                else if (flag && flag2)
                {
                    if (!cells[num13 + num4, num14, num15].canSeeThrough && !cells[num13, num14 + num5, num15].canSeeThrough)
                    {
                        return false;
                    }
                }
                else if (flag3 && flag2 && !cells[num13, num14, num15 + num6].canSeeThrough && !cells[num13, num14 + num5, num15].canSeeThrough)
                {
                    return false;
                }
                if (flag)
                {
                    num13 += num4;
                    num24 += num27;
                }
                if (flag2)
                {
                    num14 += num5;
                    num25 += num28;
                }
                if (flag3)
                {
                    num15 += num6;
                    num26 += num29;
                }
            }
            Log.Error("Too many iterations in CheckCellsInLineRaw.", false);
            return false;
        }

        private static bool CheckCellsInLineRaw_LineOfSightToImpassable(Vector3Int from, Vector3Int to)
        {
            if (from == to)
            {
                return true;
            }
            int num = Math.Abs(to.x - from.x);
            int num2 = Math.Abs(to.y - from.y);
            int num3 = Math.Abs(to.z - from.z);
            int num4 = ((to.x > from.x) ? 1 : ((to.x < from.x) ? (-1) : 0));
            int num5 = ((to.y > from.y) ? 1 : ((to.y < from.y) ? (-1) : 0));
            int num6 = ((to.z > from.z) ? 1 : ((to.z < from.z) ? (-1) : 0));
            float num7 = (float)from.x + 0.5f;
            float num8 = (float)from.y + 0.5f;
            float num9 = (float)from.z + 0.5f;
            float num10 = (float)to.x + 0.5f;
            float num11 = (float)to.y + 0.5f;
            float num12 = (float)to.z + 0.5f;
            int num13 = from.x;
            int num14 = from.y;
            int num15 = from.z;
            int num16 = from.x + ((to.x > from.x) ? 1 : 0);
            int num17 = from.y + ((to.y > from.y) ? 1 : 0);
            float num18 = (float)(from.z + ((to.z > from.z) ? 1 : 0));
            object obj = ((num10 == num7) ? 1f : (num10 - num7));
            float num19 = ((num11 == num8) ? 1f : (num11 - num8));
            float num20 = ((num12 == num9) ? 1f : (num12 - num9));
            object obj2 = obj;
            float num21 = obj2 * num19;
            float num22 = obj2 * num20;
            float num23 = num19 * num20;
            float num24 = ((float)num16 - num7) * num23;
            float num25 = ((float)num17 - num8) * num22;
            float num26 = (num18 - num9) * num21;
            float num27 = (float)num4 * num23;
            float num28 = (float)num5 * num22;
            float num29 = (float)num6 * num21;
            int num30 = num + num2 + num3 + 1;
            CellsInfo.CellInfo[,,] cells = Get.CellsInfo.Cells;
            for (int i = 0; i < num30; i++)
            {
                if (num13 == to.x && num14 == to.y && num15 == to.z)
                {
                    return true;
                }
                if ((num13 != from.x || num14 != from.y || num15 != from.z) && !cells[num13, num14, num15].canSeeThroughToImpassable)
                {
                    return false;
                }
                float num31 = ((num24 < 0f) ? (-num24) : num24);
                float num32 = ((num25 < 0f) ? (-num25) : num25);
                float num33 = ((num26 < 0f) ? (-num26) : num26);
                bool flag = num4 != 0 && (num5 == 0 || num31 <= num32 + 1E-07f) && (num6 == 0 || num31 <= num33 + 1E-07f);
                bool flag2 = num5 != 0 && (num4 == 0 || num32 <= num31 + 1E-07f) && (num6 == 0 || num32 <= num33 + 1E-07f);
                bool flag3 = num6 != 0 && (num4 == 0 || num33 <= num31 + 1E-07f) && (num5 == 0 || num33 <= num32 + 1E-07f);
                if (flag && flag2 && flag3)
                {
                    if ((!cells[num13 + num4, num14, num15].canSeeThroughToImpassable || (!cells[num13 + num4, num14, num15 + num6].canSeeThroughToImpassable && !cells[num13 + num4, num14 + num5, num15].canSeeThroughToImpassable)) && (!cells[num13, num14, num15 + num6].canSeeThroughToImpassable || (!cells[num13 + num4, num14, num15 + num6].canSeeThroughToImpassable && !cells[num13, num14 + num5, num15 + num6].canSeeThroughToImpassable)) && (!cells[num13 + num4, num14 + num5, num15].canSeeThroughToImpassable || (!cells[num13 + num4, num14, num15].canSeeThroughToImpassable && !cells[num13, num14 + num5, num15].canSeeThroughToImpassable)) && (!cells[num13, num14 + num5, num15 + num6].canSeeThroughToImpassable || (!cells[num13, num14, num15 + num6].canSeeThroughToImpassable && !cells[num13, num14 + num5, num15].canSeeThroughToImpassable)))
                    {
                        return false;
                    }
                }
                else if (flag && flag3)
                {
                    if (!cells[num13 + num4, num14, num15].canSeeThroughToImpassable && !cells[num13, num14, num15 + num6].canSeeThroughToImpassable)
                    {
                        return false;
                    }
                }
                else if (flag && flag2)
                {
                    if (!cells[num13 + num4, num14, num15].canSeeThroughToImpassable && !cells[num13, num14 + num5, num15].canSeeThroughToImpassable)
                    {
                        return false;
                    }
                }
                else if (flag3 && flag2 && !cells[num13, num14, num15 + num6].canSeeThroughToImpassable && !cells[num13, num14 + num5, num15].canSeeThroughToImpassable)
                {
                    return false;
                }
                if (flag)
                {
                    num13 += num4;
                    num24 += num27;
                }
                if (flag2)
                {
                    num14 += num5;
                    num25 += num28;
                }
                if (flag3)
                {
                    num15 += num6;
                    num26 += num29;
                }
            }
            Log.Error("Too many iterations in CheckCellsInLineRaw.", false);
            return false;
        }

        private static bool CheckCellsInLineRaw_LineOfFire(Vector3Int from, Vector3Int to)
        {
            if (from == to)
            {
                return true;
            }
            int num = Math.Abs(to.x - from.x);
            int num2 = Math.Abs(to.y - from.y);
            int num3 = Math.Abs(to.z - from.z);
            int num4 = ((to.x > from.x) ? 1 : ((to.x < from.x) ? (-1) : 0));
            int num5 = ((to.y > from.y) ? 1 : ((to.y < from.y) ? (-1) : 0));
            int num6 = ((to.z > from.z) ? 1 : ((to.z < from.z) ? (-1) : 0));
            float num7 = (float)from.x + 0.5f;
            float num8 = (float)from.y + 0.5f;
            float num9 = (float)from.z + 0.5f;
            float num10 = (float)to.x + 0.5f;
            float num11 = (float)to.y + 0.5f;
            float num12 = (float)to.z + 0.5f;
            int num13 = from.x;
            int num14 = from.y;
            int num15 = from.z;
            int num16 = from.x + ((to.x > from.x) ? 1 : 0);
            int num17 = from.y + ((to.y > from.y) ? 1 : 0);
            float num18 = (float)(from.z + ((to.z > from.z) ? 1 : 0));
            object obj = ((num10 == num7) ? 1f : (num10 - num7));
            float num19 = ((num11 == num8) ? 1f : (num11 - num8));
            float num20 = ((num12 == num9) ? 1f : (num12 - num9));
            object obj2 = obj;
            float num21 = obj2 * num19;
            float num22 = obj2 * num20;
            float num23 = num19 * num20;
            float num24 = ((float)num16 - num7) * num23;
            float num25 = ((float)num17 - num8) * num22;
            float num26 = (num18 - num9) * num21;
            float num27 = (float)num4 * num23;
            float num28 = (float)num5 * num22;
            float num29 = (float)num6 * num21;
            int num30 = num + num2 + num3 + 1;
            CellsInfo.CellInfo[,,] cells = Get.CellsInfo.Cells;
            for (int i = 0; i < num30; i++)
            {
                if (num13 == to.x && num14 == to.y && num15 == to.z)
                {
                    return true;
                }
                if ((num13 != from.x || num14 != from.y || num15 != from.z) && !cells[num13, num14, num15].canProjectilesPassThrough)
                {
                    return false;
                }
                float num31 = ((num24 < 0f) ? (-num24) : num24);
                float num32 = ((num25 < 0f) ? (-num25) : num25);
                float num33 = ((num26 < 0f) ? (-num26) : num26);
                bool flag = num4 != 0 && (num5 == 0 || num31 <= num32 + 1E-07f) && (num6 == 0 || num31 <= num33 + 1E-07f);
                bool flag2 = num5 != 0 && (num4 == 0 || num32 <= num31 + 1E-07f) && (num6 == 0 || num32 <= num33 + 1E-07f);
                bool flag3 = num6 != 0 && (num4 == 0 || num33 <= num31 + 1E-07f) && (num5 == 0 || num33 <= num32 + 1E-07f);
                if (flag && flag2 && flag3)
                {
                    if ((!cells[num13 + num4, num14, num15].canProjectilesPassThrough || (!cells[num13 + num4, num14, num15 + num6].canProjectilesPassThrough && !cells[num13 + num4, num14 + num5, num15].canProjectilesPassThrough)) && (!cells[num13, num14, num15 + num6].canProjectilesPassThrough || (!cells[num13 + num4, num14, num15 + num6].canProjectilesPassThrough && !cells[num13, num14 + num5, num15 + num6].canProjectilesPassThrough)) && (!cells[num13 + num4, num14 + num5, num15].canProjectilesPassThrough || (!cells[num13 + num4, num14, num15].canProjectilesPassThrough && !cells[num13, num14 + num5, num15].canProjectilesPassThrough)) && (!cells[num13, num14 + num5, num15 + num6].canProjectilesPassThrough || (!cells[num13, num14, num15 + num6].canProjectilesPassThrough && !cells[num13, num14 + num5, num15].canProjectilesPassThrough)))
                    {
                        return false;
                    }
                }
                else if (flag && flag3)
                {
                    if (!cells[num13 + num4, num14, num15].canProjectilesPassThrough && !cells[num13, num14, num15 + num6].canProjectilesPassThrough)
                    {
                        return false;
                    }
                }
                else if (flag && flag2)
                {
                    if (!cells[num13 + num4, num14, num15].canProjectilesPassThrough && !cells[num13, num14 + num5, num15].canProjectilesPassThrough)
                    {
                        return false;
                    }
                }
                else if (flag3 && flag2 && !cells[num13, num14, num15 + num6].canProjectilesPassThrough && !cells[num13, num14 + num5, num15].canProjectilesPassThrough)
                {
                    return false;
                }
                if (flag)
                {
                    num13 += num4;
                    num24 += num27;
                }
                if (flag2)
                {
                    num14 += num5;
                    num25 += num28;
                }
                if (flag3)
                {
                    num15 += num6;
                    num26 += num29;
                }
            }
            Log.Error("Too many iterations in CheckCellsInLineRaw.", false);
            return false;
        }

        public static IEnumerable<Vector3Int> GetCellsInLine(Vector3Int from, Vector3Int to)
        {
            if (from == to)
            {
                yield return from;
                yield break;
            }
            int num = Math.Abs(to.x - from.x);
            int num2 = Math.Abs(to.y - from.y);
            int num3 = Math.Abs(to.z - from.z);
            int xStep = ((to.x > from.x) ? 1 : ((to.x < from.x) ? (-1) : 0));
            int yStep = ((to.y > from.y) ? 1 : ((to.y < from.y) ? (-1) : 0));
            int zStep = ((to.z > from.z) ? 1 : ((to.z < from.z) ? (-1) : 0));
            float num4 = (float)from.x + 0.5f;
            float num5 = (float)from.y + 0.5f;
            float num6 = (float)from.z + 0.5f;
            float num7 = (float)to.x + 0.5f;
            float num8 = (float)to.y + 0.5f;
            float num9 = (float)to.z + 0.5f;
            int curX = from.x;
            int curY = from.y;
            int curZ = from.z;
            int num10 = from.x + ((to.x > from.x) ? 1 : 0);
            int num11 = from.y + ((to.y > from.y) ? 1 : 0);
            int num12 = from.z + ((to.z > from.z) ? 1 : 0);
            object obj = ((num7 == num4) ? 1f : (num7 - num4));
            float num13 = ((num8 == num5) ? 1f : (num8 - num5));
            float num14 = ((num9 == num6) ? 1f : (num9 - num6));
            object obj2 = obj;
            float num15 = obj2 * num13;
            float num16 = obj2 * num14;
            float num17 = num13 * num14;
            float errX = ((float)num10 - num4) * num17;
            float errY = ((float)num11 - num5) * num16;
            float errZ = ((float)num12 - num6) * num15;
            float errXStep = (float)xStep * num17;
            float errYStep = (float)yStep * num16;
            float errZStep = (float)zStep * num15;
            int maxIterations = num + num2 + num3 + 1;
            int num21;
            for (int i = 0; i < maxIterations; i = num21 + 1)
            {
                yield return new Vector3Int(curX, curY, curZ);
                if (curX == to.x && curY == to.y && curZ == to.z)
                {
                    yield break;
                }
                float num18 = Math.Abs(errX);
                float num19 = Math.Abs(errY);
                float num20 = Math.Abs(errZ);
                bool flag = xStep != 0 && (yStep == 0 || num18 <= num19 + 1E-07f) && (zStep == 0 || num18 <= num20 + 1E-07f);
                bool flag2 = yStep != 0 && (xStep == 0 || num19 <= num18 + 1E-07f) && (zStep == 0 || num19 <= num20 + 1E-07f);
                bool flag3 = zStep != 0 && (xStep == 0 || num20 <= num18 + 1E-07f) && (yStep == 0 || num20 <= num19 + 1E-07f);
                if (flag)
                {
                    curX += xStep;
                    errX += errXStep;
                }
                if (flag2)
                {
                    curY += yStep;
                    errY += errYStep;
                }
                if (flag3)
                {
                    curZ += zStep;
                    errZ += errZStep;
                }
                num21 = i;
            }
            Log.Error("Too many iterations in GetCellsInLine.", false);
            yield break;
        }

        [CompilerGenerated]
        internal static bool <CheckCellsInLineSmart>g__CheckAdjLOS|6_0(Vector3Int dir, ref LineOfSight.<>c__DisplayClass6_0 A_1)
		{
            Vector3Int vector3Int = A_1.to + dir;
			return vector3Int.InBounds() && (A_1.predicate(vector3Int) || (A_1.cellsInfo.AnyDoorAt(vector3Int) && A_1.to.GetGridDistance(A_1.from) <= vector3Int.GetGridDistance(A_1.from))) && (LineOfSight.CheckCellsInLineRaw(A_1.from, vector3Int, A_1.predicateType) || (A_1.fromIsOnStairs && LineOfSight.CheckCellsInLineRaw(A_1.aboveFrom, vector3Int, A_1.predicateType)));
		}

		private static Func<Vector3Int, bool> LineOfSightPredicate = (Vector3Int x) => Get.CellsInfo.CanSeeThrough(x);

        private static Func<Vector3Int, bool> LineOfSightPredicateToImpassable = (Vector3Int x) => Get.CellsInfo.CanSeeThroughToImpassable(x);

        private static Func<Vector3Int, bool> LineOfFirePredicate = (Vector3Int x) => Get.CellsInfo.CanProjectilesPassThrough(x);

        private enum PredicateType
    {
        LineOfSight,

        LineOfSightToImpassable,

        LineOfFire
    }
}
}