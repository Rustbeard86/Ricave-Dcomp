using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class PathFinder
    {
        public PathFinder(World world)
        {
            this.world = world;
            this.worldSize = world.Size;
            this.worldSizeXTimesY = this.worldSize.x * this.worldSize.y;
            PathFinder.openSet = new PriorityQueue(this.worldSize);
            this.costs = new float[this.worldSize.x * this.worldSize.y * this.worldSize.z];
            PathFinder.closed = new bool[this.worldSize.x * this.worldSize.y * this.worldSize.z];
            this.cameFrom = new int[this.worldSize.x * this.worldSize.y * this.worldSize.z];
            this.visited = new List<int>(this.worldSize.x * this.worldSize.y * this.worldSize.z / 2);
            Array.Fill<float>(this.costs, -1f);
        }

        public List<Vector3Int> FindPath(Vector3Int from, Vector3Int to, PathFinder.Request request, int maxDistance, List<Vector3Int> listToUse = null)
        {
            if (listToUse != null)
            {
                listToUse.Clear();
            }
            if (!this.world.InBounds(from) || !this.world.InBounds(to))
            {
                return null;
            }
            switch (request.PathMode)
            {
                case PathFinder.Request.Mode.None:
                    return null;
                case PathFinder.Request.Mode.ToCell:
                    if (from == to)
                    {
                        if (listToUse != null)
                        {
                            listToUse.Add(from);
                            return listToUse;
                        }
                        return new List<Vector3Int> { from };
                    }
                    else
                    {
                        if (!this.world.CellsInfo.CanPassThrough(to))
                        {
                            return null;
                        }
                        if (from.GetGridDistance(to) > maxDistance)
                        {
                            return null;
                        }
                    }
                    break;
                case PathFinder.Request.Mode.Touch:
                    if (this.world.CanTouch(from, to, request.PathParams.Gravity, request.PathParams.CanFly, request.PathParams.CanUseLadders))
                    {
                        if (listToUse != null)
                        {
                            listToUse.Add(from);
                            return listToUse;
                        }
                        return new List<Vector3Int> { from };
                    }
                    else if (from.GetGridDistance(to) - 1 > maxDistance)
                    {
                        return null;
                    }
                    break;
            }
            Profiler.Begin("FindPath()");
            try
            {
                PathFinder.openSet.Clear();
                int num = this.PosToIndex(from.x, from.y, from.z);
                int num2 = this.PosToIndex(to.x, to.y, to.z);
                AStarNode astarNode = new AStarNode
                {
                    pos = num,
                    costPlusHeuristicCostToDest = this.GetHeuristicCost(from, to)
                };
                PathFinder.openSet.PushOrDecreaseKey(ref astarNode);
                this.costs[num] = 0f;
                this.cameFrom[num] = num;
                this.visited.Add(num);
                CellsInfo cellsInfo = this.world.CellsInfo;
                World.CanMoveFromToParams pathParams = request.PathParams;
                while (PathFinder.openSet.Count != 0)
                {
                    int pos = PathFinder.openSet.Pop().pos;
                    Vector3Int vector3Int = this.IndexToPos(pos);
                    PathFinder.Request.Mode pathMode = request.PathMode;
                    if (pathMode != PathFinder.Request.Mode.ToCell)
                    {
                        if (pathMode == PathFinder.Request.Mode.Touch)
                        {
                            if (this.world.CanTouch(vector3Int, to, pathParams.Gravity, pathParams.CanFly, pathParams.CanUseLadders))
                            {
                                List<Vector3Int> list = this.ReconstructPath(from, vector3Int, this.cameFrom, listToUse);
                                this.ClearVisited(this.visited, PathFinder.closed, this.costs, this.cameFrom, true);
                                return list;
                            }
                        }
                    }
                    else if (pos == num2)
                    {
                        List<Vector3Int> list2 = this.ReconstructPath(from, to, this.cameFrom, listToUse);
                        this.ClearVisited(this.visited, PathFinder.closed, this.costs, this.cameFrom, true);
                        return list2;
                    }
                    float num3 = this.costs[pos];
                    PathFinder.closed[pos] = true;
                    this.visited.Add(pos);
                    int num4;
                    Vector3Int[] adjacentCanPassThrough = cellsInfo.GetAdjacentCanPassThrough(pos, out num4);
                    for (int i = 0; i < num4; i++)
                    {
                        Vector3Int vector3Int2 = adjacentCanPassThrough[i];
                        int num5 = this.PosToIndex(vector3Int2.x, vector3Int2.y, vector3Int2.z);
                        if (!PathFinder.closed[num5])
                        {
                            float num6 = num3 + PathFinder.GetCostBetween(vector3Int, vector3Int2, cellsInfo);
                            if (num6 <= (float)maxDistance)
                            {
                                float num7 = this.costs[num5];
                                if ((num7 == -1f || num6 < num7) && this.world.CanMoveFromTo_AssumeAdjacentAndCanPassThrough(vector3Int, vector3Int2, ref pathParams))
                                {
                                    this.costs[num5] = num6;
                                    this.cameFrom[num5] = pos;
                                    this.visited.Add(num5);
                                    AStarNode astarNode2 = new AStarNode
                                    {
                                        pos = num5,
                                        costPlusHeuristicCostToDest = num6 + this.GetHeuristicCost(vector3Int2, to)
                                    };
                                    PathFinder.openSet.PushOrDecreaseKey(ref astarNode2);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error in PathFinder.FindPath().", ex);
            }
            finally
            {
                Profiler.End();
            }
            this.ClearVisited(this.visited, PathFinder.closed, this.costs, this.cameFrom, true);
            return null;
        }

        public void FindAllCosts(Vector3Int from, World.CanMoveFromToParams pathParams, int maxDistance, List<int> visited, float[] costs, int[] cameFrom)
        {
            if (!this.world.InBounds(from))
            {
                return;
            }
            Profiler.Begin("PathFinder.FindAllCosts()");
            try
            {
                PathFinder.openSet.Clear();
                int num = this.PosToIndex(from.x, from.y, from.z);
                AStarNode astarNode = new AStarNode
                {
                    pos = num,
                    costPlusHeuristicCostToDest = 0f
                };
                PathFinder.openSet.PushOrDecreaseKey(ref astarNode);
                costs[num] = 0f;
                cameFrom[num] = num;
                visited.Add(num);
                CellsInfo cellsInfo = this.world.CellsInfo;
                while (PathFinder.openSet.Count != 0)
                {
                    int pos = PathFinder.openSet.Pop().pos;
                    Vector3Int vector3Int = this.IndexToPos(pos);
                    float num2 = costs[pos];
                    PathFinder.closed[pos] = true;
                    visited.Add(pos);
                    int num3;
                    Vector3Int[] adjacentCanPassThrough = cellsInfo.GetAdjacentCanPassThrough(pos, out num3);
                    for (int i = 0; i < num3; i++)
                    {
                        Vector3Int vector3Int2 = adjacentCanPassThrough[i];
                        int num4 = this.PosToIndex(vector3Int2.x, vector3Int2.y, vector3Int2.z);
                        if (!PathFinder.closed[num4])
                        {
                            float num5 = num2 + PathFinder.GetCostBetween(vector3Int, vector3Int2, cellsInfo);
                            if (num5 <= (float)maxDistance)
                            {
                                float num6 = costs[num4];
                                if ((num6 == -1f || num5 < num6) && this.world.CanMoveFromTo_AssumeAdjacentAndCanPassThrough(vector3Int, vector3Int2, ref pathParams))
                                {
                                    costs[num4] = num5;
                                    cameFrom[num4] = pos;
                                    visited.Add(num4);
                                    AStarNode astarNode2 = new AStarNode
                                    {
                                        pos = num4,
                                        costPlusHeuristicCostToDest = num5
                                    };
                                    PathFinder.openSet.PushOrDecreaseKey(ref astarNode2);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error in PathFinder.FindAllCosts().", ex);
            }
            finally
            {
                Profiler.End();
            }
            this.ClearVisited(visited, PathFinder.closed, null, null, false);
        }

        private static float GetCostBetween(Vector3Int from, Vector3Int to, CellsInfo cellsInfo)
        {
            float num = 1f;
            if ((from.x == to.x && from.y == to.y) || (from.x == to.x && from.z == to.z) || (from.y == to.y && from.z == to.z))
            {
                num -= 0.0005f;
            }
            if (cellsInfo.AnyAIAvoidsAt(to))
            {
                num += 5f;
            }
            return num;
        }

        private float GetHeuristicCost(Vector3Int from, Vector3Int dest)
        {
            return (float)from.GetGridDistance(dest) * 0.999f;
        }

        public void ClearVisited(List<int> visited, bool[] closed, float[] costs, int[] cameFrom, bool clearVisitedList)
        {
            int i = 0;
            int count = visited.Count;
            while (i < count)
            {
                int num = visited[i];
                if (closed != null)
                {
                    closed[num] = false;
                }
                if (costs != null)
                {
                    costs[num] = -1f;
                }
                if (cameFrom != null)
                {
                    cameFrom[num] = -1;
                }
                i++;
            }
            if (clearVisitedList)
            {
                visited.Clear();
            }
        }

        public List<Vector3Int> ReconstructPath(Vector3Int from, Vector3Int to, int[] cameFrom, List<Vector3Int> listToUse = null)
        {
            List<Vector3Int> list;
            if (listToUse != null)
            {
                listToUse.Clear();
                list = listToUse;
            }
            else
            {
                list = new List<Vector3Int>();
            }
            Vector3Int vector3Int = to;
            int num = this.PosToIndex(to.x, to.y, to.z);
            int num2 = this.PosToIndex(from.x, from.y, from.z);
            if (num != num2)
            {
                int num3 = 0;
                do
                {
                    list.Add(vector3Int);
                    num = cameFrom[num];
                    vector3Int = this.IndexToPos(num);
                    if (num == num2)
                    {
                        goto IL_0088;
                    }
                    num3++;
                }
                while (num3 <= 10000);
                Log.Error("Too many iterations in ReconstructPath(). Did cameFrom create a loop?", false);
            }
        IL_0088:
            list.Add(from);
            return list;
        }

        private int PosToIndex(int x, int y, int z)
        {
            return z * this.worldSizeXTimesY + y * this.worldSize.x + x;
        }

        private Vector3Int IndexToPos(int index)
        {
            int num = index / this.worldSizeXTimesY;
            index -= num * this.worldSizeXTimesY;
            int num2 = index / this.worldSize.x;
            return new Vector3Int(index - num2 * this.worldSize.x, num2, num);
        }

        private World world;

        private static PriorityQueue openSet;

        private float[] costs;

        private static bool[] closed;

        private int[] cameFrom;

        private List<int> visited;

        private Vector3Int worldSize;

        private int worldSizeXTimesY;

        public struct Request
        {
            public Request(PathFinder.Request.Mode mode, World.CanMoveFromToParams pathParams)
            {
                this.PathMode = mode;
                this.PathParams = pathParams;
            }

            public Request(PathFinder.Request.Mode mode, Actor actor)
            {
                this.PathMode = mode;
                this.PathParams = new World.CanMoveFromToParams(actor);
            }

            public readonly PathFinder.Request.Mode PathMode;

            public readonly World.CanMoveFromToParams PathParams;

            public enum Mode
            {
                None,

                ToCell,

                Touch
            }
        }
    }
}