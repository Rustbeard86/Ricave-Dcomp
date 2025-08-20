using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public static class PushUtility
    {
        public static IEnumerable<Vector3Int> Push(Func<Vector3Int> curPosGetter, Vector3Int directionNonNormalized, int distance)
        {
            Vector3Int from = curPosGetter();
            yield return from;
            if (distance <= 0 || directionNonNormalized == Vector3Int.zero)
            {
                yield break;
            }
            List<Vector3Int> list = LineOfSight.GetCellsInLine(from, from + directionNonNormalized).ToTemporaryList<Vector3Int>();
            List<Vector3Int> force = FrameLocalPool<List<Vector3Int>>.Get();
            for (int j = 1; j < list.Count; j++)
            {
                force.Add(list[j] - list[j - 1]);
            }
            Vector3Int vector3Int = new Vector3Int(Math.Sign(directionNonNormalized.x), 0, 0);
            Vector3Int vector3Int2 = new Vector3Int(0, Math.Sign(directionNonNormalized.y), 0);
            Vector3Int vector3Int3 = new Vector3Int(0, 0, Math.Sign(directionNonNormalized.z));
            PushUtility.slidingToTry[0] = vector3Int + vector3Int2 + vector3Int3;
            PushUtility.slidingToTry[1] = vector3Int + vector3Int2;
            PushUtility.slidingToTry[2] = vector3Int2 + vector3Int3;
            PushUtility.slidingToTry[3] = vector3Int + vector3Int3;
            PushUtility.slidingToTry[4] = vector3Int;
            PushUtility.slidingToTry[5] = vector3Int2;
            PushUtility.slidingToTry[6] = vector3Int3;
            int num;
            for (int i = 0; i < distance; i = num + 1)
            {
                Vector3Int vector3Int4 = curPosGetter();
                Vector3Int vector3Int5 = vector3Int4 + force[i % force.Count];
                if (PushUtility.CanPushFromTo(vector3Int4, vector3Int5))
                {
                    yield return vector3Int5;
                }
                else if (Get.CellsInfo.AnyStairsAt(vector3Int4) && PushUtility.CanPushFromTo(vector3Int4, vector3Int5.Above()))
                {
                    yield return vector3Int5.Above();
                }
                else
                {
                    bool flag = false;
                    for (int k = 0; k < PushUtility.slidingToTry.Length; k++)
                    {
                        Vector3Int vector3Int6 = vector3Int4 + PushUtility.slidingToTry[k];
                        if (PushUtility.CanPushFromTo(vector3Int4, vector3Int6))
                        {
                            yield return vector3Int6;
                            flag = true;
                            break;
                        }
                        if (Get.CellsInfo.AnyStairsAt(vector3Int4) && PushUtility.CanPushFromTo(vector3Int4, vector3Int6.Above()))
                        {
                            yield return vector3Int6.Above();
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)
                    {
                        yield break;
                    }
                }
                num = i;
            }
            yield break;
        }

        private static bool CanPushFromTo(Vector3Int from, Vector3Int to)
        {
            if (from == to)
            {
                return false;
            }
            if (!Get.World.InBounds(to))
            {
                return false;
            }
            if (!from.IsAdjacent(to))
            {
                return false;
            }
            if (!Get.CellsInfo.CanPassThrough(to))
            {
                return false;
            }
            if (!Get.World.CanUseStairsFromTo(from, to, Vector3IntUtility.Down, true))
            {
                if (from.x != to.x && from.z != to.z && (Get.CellsInfo.BlocksDiagonalMovement(new Vector3Int(from.x, from.y, to.z)) || Get.CellsInfo.BlocksDiagonalMovement(new Vector3Int(to.x, from.y, from.z))))
                {
                    return false;
                }
                if (from.x != to.x && from.y != to.y && (Get.CellsInfo.BlocksDiagonalMovement(new Vector3Int(from.x, to.y, from.z)) || Get.CellsInfo.BlocksDiagonalMovement(new Vector3Int(to.x, from.y, from.z))))
                {
                    return false;
                }
                if (from.y != to.y && from.z != to.z && (Get.CellsInfo.BlocksDiagonalMovement(new Vector3Int(from.x, from.y, to.z)) || Get.CellsInfo.BlocksDiagonalMovement(new Vector3Int(from.x, to.y, from.z))))
                {
                    return false;
                }
            }
            return true;
        }

        public static int GetExpectedPushDistance(Vector3Int startPos, Vector3Int directionNonNormalized, int distance)
        {
            int num = 0;
            Vector3Int cur = startPos;
            Func<Vector3Int> <> 9__0;
            Func<Vector3Int> func;
            if ((func = <> 9__0) == null)
            {
                func = (<> 9__0 = () => cur);
            }
            foreach (Vector3Int vector3Int in PushUtility.Push(func, directionNonNormalized, distance))
            {
                if (!(vector3Int == cur))
                {
                    cur = vector3Int;
                    num++;
                }
            }
            return num;
        }

        private static readonly Vector3Int[] slidingToTry = new Vector3Int[7];
    }
}