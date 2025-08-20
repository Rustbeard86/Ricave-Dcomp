using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class ShortestPathsCache
    {
        public List<Vector3Int> AllReachableWithToCellMode
        {
            get
            {
                if (this.allReachableWithToCellModeDirty)
                {
                    this.allReachableWithToCellModeDirty = false;
                    this.allReachableWithToCellMode.Clear();
                    int i = 0;
                    int count = this.visited.Count;
                    while (i < count)
                    {
                        int num = this.visited[i];
                        if (this.costs[num] >= 0f)
                        {
                            this.allReachableWithToCellMode.Add(this.IndexToPos(num));
                        }
                        i++;
                    }
                }
                return this.allReachableWithToCellMode;
            }
        }

        public List<Vector3Int> AllReachableWithToCellModeOrderedByDistance
        {
            get
            {
                if (this.allReachableWithToCellModeOrderedByDistanceDirty)
                {
                    this.allReachableWithToCellModeOrderedByDistanceDirty = false;
                    this.allReachableWithToCellModeOrderedByDistance.Clear();
                    this.allReachableWithToCellModeOrderedByDistance.AddRange(this.AllReachableWithToCellMode);
                    this.allReachableWithToCellModeOrderedByDistance.Sort(this.ByCost);
                }
                return this.allReachableWithToCellModeOrderedByDistance;
            }
        }

        public ShortestPathsCache(World world)
        {
            this.world = world;
            this.worldSize = world.Size;
            this.worldSizeXTimesY = this.worldSize.x * this.worldSize.y;
            this.costs = new float[this.worldSize.x * this.worldSize.y * this.worldSize.z];
            this.cameFrom = new int[this.worldSize.x * this.worldSize.y * this.worldSize.z];
            this.visited = new List<int>(this.worldSize.x * this.worldSize.y * this.worldSize.z / 2);
            for (int i = 0; i < this.costs.Length; i++)
            {
                this.costs[i] = -1f;
            }
            this.ByCost = delegate (Vector3Int a, Vector3Int b)
            {
                float num = this.costs[this.PosToIndex(a.x, a.y, a.z)];
                float num2 = this.costs[this.PosToIndex(b.x, b.y, b.z)];
                return num.CompareTo(num2);
            };
        }

        public void InitFor(Vector3Int from, Actor actor, int maxDistance)
        {
            this.InitFor(from, new World.CanMoveFromToParams(actor), maxDistance);
        }

        public void InitFor(Vector3Int from, World.CanMoveFromToParams pathParams, int maxDistance)
        {
            this.from = from;
            this.from_pathParams = pathParams;
            Get.PathFinder.ClearVisited(this.visited, null, this.costs, this.cameFrom, true);
            Get.PathFinder.FindAllCosts(from, pathParams, maxDistance, this.visited, this.costs, this.cameFrom);
            this.allReachableWithToCellModeDirty = true;
            this.allReachableWithToCellModeOrderedByDistanceDirty = true;
        }

        public bool CanReach(Vector3Int to, PathFinder.Request.Mode mode)
        {
            if (!this.world.InBounds(to))
            {
                return false;
            }
            switch (mode)
            {
                case PathFinder.Request.Mode.None:
                    return false;
                case PathFinder.Request.Mode.ToCell:
                    return this.costs[this.PosToIndex(to.x, to.y, to.z)] >= 0f;
                case PathFinder.Request.Mode.Touch:
                    {
                        for (int i = 0; i < Vector3IntUtility.DirectionsAndInside.Length; i++)
                        {
                            Vector3Int vector3Int = to + Vector3IntUtility.DirectionsAndInside[i];
                            if (this.world.InBounds(vector3Int) && this.costs[this.PosToIndex(vector3Int.x, vector3Int.y, vector3Int.z)] >= 0f && this.world.CanTouch(vector3Int, to, this.from_pathParams.Gravity, this.from_pathParams.CanFly, this.from_pathParams.CanUseLadders))
                            {
                                return true;
                            }
                        }
                        return false;
                    }
                default:
                    return false;
            }
        }

        public float GetCostTo(Vector3Int to, PathFinder.Request.Mode mode)
        {
            if (!this.world.InBounds(to))
            {
                return -1f;
            }
            switch (mode)
            {
                case PathFinder.Request.Mode.None:
                    return -1f;
                case PathFinder.Request.Mode.ToCell:
                    return this.costs[this.PosToIndex(to.x, to.y, to.z)];
                case PathFinder.Request.Mode.Touch:
                    {
                        float num = -1f;
                        for (int i = 0; i < Vector3IntUtility.DirectionsAndInside.Length; i++)
                        {
                            Vector3Int vector3Int = to + Vector3IntUtility.DirectionsAndInside[i];
                            if (this.world.InBounds(vector3Int))
                            {
                                float num2 = this.costs[this.PosToIndex(vector3Int.x, vector3Int.y, vector3Int.z)];
                                if (num2 >= 0f && (num < 0f || num2 < num) && this.world.CanTouch(vector3Int, to, this.from_pathParams.Gravity, this.from_pathParams.CanFly, this.from_pathParams.CanUseLadders))
                                {
                                    num = num2;
                                }
                            }
                        }
                        return num;
                    }
                default:
                    return -1f;
            }
        }

        public List<Vector3Int> GetPathTo(Vector3Int to, PathFinder.Request.Mode mode)
        {
            if (!this.world.InBounds(to))
            {
                return null;
            }
            switch (mode)
            {
                case PathFinder.Request.Mode.None:
                    return null;
                case PathFinder.Request.Mode.ToCell:
                    if (this.costs[this.PosToIndex(to.x, to.y, to.z)] >= 0f)
                    {
                        return Get.PathFinder.ReconstructPath(this.from, to, this.cameFrom, null);
                    }
                    return null;
                case PathFinder.Request.Mode.Touch:
                    {
                        float num = -1f;
                        Vector3Int vector3Int = default(Vector3Int);
                        for (int i = 0; i < Vector3IntUtility.DirectionsAndInside.Length; i++)
                        {
                            Vector3Int vector3Int2 = to + Vector3IntUtility.DirectionsAndInside[i];
                            if (this.world.InBounds(vector3Int2))
                            {
                                float num2 = this.costs[this.PosToIndex(vector3Int2.x, vector3Int2.y, vector3Int2.z)];
                                if (num2 >= 0f && (num < 0f || num2 < num) && this.world.CanTouch(vector3Int2, to, this.from_pathParams.Gravity, this.from_pathParams.CanFly, this.from_pathParams.CanUseLadders))
                                {
                                    num = num2;
                                    vector3Int = vector3Int2;
                                }
                            }
                        }
                        if (num >= 0f)
                        {
                            return Get.PathFinder.ReconstructPath(this.from, vector3Int, this.cameFrom, null);
                        }
                        return null;
                    }
                default:
                    return null;
            }
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

        private Vector3Int from;

        private World.CanMoveFromToParams from_pathParams;

        private float[] costs;

        private int[] cameFrom;

        private List<int> visited;

        private List<Vector3Int> allReachableWithToCellMode = new List<Vector3Int>(400);

        private bool allReachableWithToCellModeDirty = true;

        private List<Vector3Int> allReachableWithToCellModeOrderedByDistance = new List<Vector3Int>(400);

        private bool allReachableWithToCellModeOrderedByDistanceDirty = true;

        private Vector3Int worldSize;

        private int worldSizeXTimesY;

        private readonly Comparison<Vector3Int> ByCost;
    }
}