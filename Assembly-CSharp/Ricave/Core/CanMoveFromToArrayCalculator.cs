using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Ricave.Core
{
    public static class CanMoveFromToArrayCalculator
    {
        public static List<Vector3Int>[,,] GetArray(Vector3Int pos, int maxDistance, Vector3Int gravity, bool canFly, bool canJumpOffLedge, bool canUseLadders)
        {
            int num = (maxDistance * 2 + 1) * (maxDistance * 2 + 1);
            if (CanMoveFromToArrayCalculator.CalculateAdjacencyWorker_result.GetLength(0) < num)
            {
                CanMoveFromToArrayCalculator.CalculateAdjacencyWorker_result = new List<Vector3Int>[num, num, num];
            }
            while (CanMoveFromToArrayCalculator.listsPool.Count < num * num * num)
            {
                CanMoveFromToArrayCalculator.listsPool.Add(new List<Vector3Int>());
            }
            Array.Clear(CanMoveFromToArrayCalculator.CalculateAdjacencyWorker_result, 0, CanMoveFromToArrayCalculator.CalculateAdjacencyWorker_result.Length);
            CanMoveFromToArrayCalculator.CalculateAdjacencyWorker_perAxis = num;
            CanMoveFromToArrayCalculator.CalculateAdjacencyWorker_pos = pos;
            Profiler.Begin("CanMoveFromToArrayCalculator Parallel.For()");
            try
            {
                Parallel.For(0, num * num * num, new Action<int>(CanMoveFromToArrayCalculator.CalculateAdjacencyWorker));
            }
            finally
            {
                Profiler.End();
            }
            return CanMoveFromToArrayCalculator.CalculateAdjacencyWorker_result;
        }

        private static void CalculateAdjacencyWorker(int index)
        {
            int calculateAdjacencyWorker_perAxis = CanMoveFromToArrayCalculator.CalculateAdjacencyWorker_perAxis;
            Vector3Int calculateAdjacencyWorker_pos = CanMoveFromToArrayCalculator.CalculateAdjacencyWorker_pos;
            int num = calculateAdjacencyWorker_perAxis * calculateAdjacencyWorker_perAxis;
            int num2 = index / num;
            index -= num2 * num;
            int num3 = index / calculateAdjacencyWorker_perAxis;
            int num4 = index - num3 * calculateAdjacencyWorker_perAxis;
            int num5 = -calculateAdjacencyWorker_perAxis / 2;
            int x = calculateAdjacencyWorker_pos.x;
            num3 += num5 + calculateAdjacencyWorker_pos.y;
            num2 += num5 + calculateAdjacencyWorker_pos.z;
        }

        private static int CalculateAdjacencyWorker_perAxis;

        private static Vector3Int CalculateAdjacencyWorker_pos;

        private static List<Vector3Int>[,,] CalculateAdjacencyWorker_result = new List<Vector3Int>[40, 40, 40];

        private static List<List<Vector3Int>> listsPool = new List<List<Vector3Int>>();

        private static readonly Action<int> CalculateAdjacencyWorkerDelegate = new Action<int>(CanMoveFromToArrayCalculator.CalculateAdjacencyWorker);
    }
}