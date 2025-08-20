using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Ricave.Core
{
    public static class AdjacentCellsCalculatorBurst
    {
        public static List<Vector3Int> GetAdjacentCells(List<Vector3Int> cells)
        {
            List<Vector3Int> list = FrameLocalPool<List<Vector3Int>>.Get();
            AdjacentCellsCalculatorBurst.GetAdjacentCells(cells, list);
            return list;
        }

        public static void GetAdjacentCells(List<Vector3Int> cells, List<Vector3Int> outResult)
        {
            outResult.Clear();
            if (cells.Count == 0)
            {
                return;
            }
            if (!AdjacentCellsCalculatorBurst.tmpCells.IsCreated)
            {
                AdjacentCellsCalculatorBurst.tmpCells = new NativeArray<int3>(Math.Max(100, cells.Count), Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
                AdjacentCellsCalculatorBurst.tmpResult = new NativeHashSet<int3>(AdjacentCellsCalculatorBurst.tmpCells.Length * 27, Allocator.Persistent);
                AdjacentCellsCalculatorBurst.tmpResultParallelWriter = AdjacentCellsCalculatorBurst.tmpResult.AsParallelWriter();
                AdjacentCellsCalculatorBurst.job = new CalculateAdjacentCellsJob
                {
                    cells = AdjacentCellsCalculatorBurst.tmpCells,
                    result = AdjacentCellsCalculatorBurst.tmpResultParallelWriter
                };
            }
            else if (AdjacentCellsCalculatorBurst.tmpCells.Length < cells.Count)
            {
                int length = AdjacentCellsCalculatorBurst.tmpCells.Length;
                AdjacentCellsCalculatorBurst.tmpCells.Dispose();
                AdjacentCellsCalculatorBurst.tmpCells = new NativeArray<int3>(Math.Max(length * 2, cells.Count), Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
                AdjacentCellsCalculatorBurst.tmpResult.Dispose();
                AdjacentCellsCalculatorBurst.tmpResult = new NativeHashSet<int3>(AdjacentCellsCalculatorBurst.tmpCells.Length * 27, Allocator.Persistent);
                AdjacentCellsCalculatorBurst.tmpResultParallelWriter = AdjacentCellsCalculatorBurst.tmpResult.AsParallelWriter();
                AdjacentCellsCalculatorBurst.job = new CalculateAdjacentCellsJob
                {
                    cells = AdjacentCellsCalculatorBurst.tmpCells,
                    result = AdjacentCellsCalculatorBurst.tmpResultParallelWriter
                };
            }
            Vector3Int size = Get.World.Size;
            AdjacentCellsCalculatorBurst.job.mapSizeX = size.x;
            AdjacentCellsCalculatorBurst.job.mapSizeY = size.y;
            AdjacentCellsCalculatorBurst.job.mapSizeZ = size.z;
            int count = cells.Count;
            for (int i = 0; i < count; i++)
            {
                Vector3Int vector3Int = cells[i];
                AdjacentCellsCalculatorBurst.tmpCells[i] = new int3(vector3Int.x, vector3Int.y, vector3Int.z);
            }
            AdjacentCellsCalculatorBurst.tmpResult.Clear();
            AdjacentCellsCalculatorBurst.job.Schedule(count, 8, default(JobHandle)).Complete();
            foreach (int3 @int in AdjacentCellsCalculatorBurst.tmpResult)
            {
                outResult.Add(new Vector3Int(@int.x, @int.y, @int.z));
            }
        }

        private static CalculateAdjacentCellsJob job;

        private static NativeArray<int3> tmpCells;

        private static NativeHashSet<int3> tmpResult;

        private static NativeHashSet<int3>.ParallelWriter tmpResultParallelWriter;
    }
}