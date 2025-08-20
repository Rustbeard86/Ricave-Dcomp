using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Ricave.Core
{
    public class BFSCache
    {
        public List<Vector3Int> AllVisited
        {
            get
            {
                return this.visited;
            }
        }

        public BFSCache(World world)
        {
            this.world = world;
            this.worldSize = world.Size;
            this.worldSizeXTimesY = this.worldSize.x * this.worldSize.y;
            this.distances = new int[this.worldSize.x * this.worldSize.y * this.worldSize.z];
            this.firstReachedByActorIndex = new int[this.worldSize.x * this.worldSize.y * this.worldSize.z];
            this.cameFrom = new int[this.worldSize.x * this.worldSize.y * this.worldSize.z];
            this.visited = new List<Vector3Int>(this.worldSize.x * this.worldSize.y * this.worldSize.z / 2);
            for (int i = 0; i < this.distances.Length; i++)
            {
                this.distances[i] = -1;
                this.firstReachedByActorIndex[i] = -1;
            }
        }

        public void InitFor(Vector3Int from, Actor actor, int maxDistance)
        {
            this.InitFor(from, new World.CanMoveFromToParams(actor), maxDistance);
        }

        public void InitFor(Vector3Int from, World.CanMoveFromToParams pathParams, int maxDistance)
        {
            this.from = from;
            this.from_pathParams = pathParams;
            this.fromMultiple.Clear();
            this.Clear();
            this.queue.Clear();
            int num = this.PosToIndex(from.x, from.y, from.z);
            this.queue.Enqueue(num);
            this.distances[num] = 0;
            this.cameFrom[num] = num;
            this.visited.Add(from);
            CellsInfo cellsInfo = this.world.CellsInfo;
            Profiler.Begin("BFSCache.InitFor()");
            try
            {
                while (this.queue.Count != 0)
                {
                    int num2 = this.queue.Dequeue();
                    int num3 = this.distances[num2];
                    Vector3Int vector3Int = this.IndexToPos(num2);
                    int num4;
                    Vector3Int[] adjacentCanPassThrough = cellsInfo.GetAdjacentCanPassThrough(num2, out num4);
                    for (int i = 0; i < num4; i++)
                    {
                        Vector3Int vector3Int2 = adjacentCanPassThrough[i];
                        int num5 = this.PosToIndex(vector3Int2.x, vector3Int2.y, vector3Int2.z);
                        if (this.distances[num5] < 0 && this.world.CanMoveFromTo_AssumeAdjacentAndCanPassThrough(vector3Int, vector3Int2, ref pathParams))
                        {
                            int num6 = num3 + 1;
                            this.distances[num5] = num6;
                            this.cameFrom[num5] = num2;
                            this.visited.Add(vector3Int2);
                            if (num6 < maxDistance)
                            {
                                this.queue.Enqueue(num5);
                            }
                        }
                    }
                }
            }
            finally
            {
                Profiler.End();
            }
        }

        public void InitFor(List<Actor> actors, int maxDistance, bool assumeEveryoneCanFly = false)
        {
            this.from = default(Vector3Int);
            this.from_pathParams = default(World.CanMoveFromToParams);
            this.fromMultiple.Clear();
            for (int i = 0; i < actors.Count; i++)
            {
                Actor actor = actors[i];
                Vector3Int gravity = actor.Gravity;
                bool flag = assumeEveryoneCanFly || actor.CanFly;
                bool canJumpOffLedge = actor.CanJumpOffLedge;
                bool canUseLadders = actor.CanUseLadders;
                bool movingAllowed = actor.MovingAllowed;
                bool allowPathingIntoAIAvoids = actor.AllowPathingIntoAIAvoids;
                ChainPost attachedToChainPost = actor.AttachedToChainPost;
                World.CanMoveFromToParams canMoveFromToParams = new World.CanMoveFromToParams(gravity, flag, canJumpOffLedge, canUseLadders, movingAllowed, allowPathingIntoAIAvoids, (attachedToChainPost != null) ? new Vector3Int?(attachedToChainPost.Position) : null);
                this.fromMultiple.Add(new ValueTuple<Vector3Int, World.CanMoveFromToParams>(actor.Position, canMoveFromToParams));
            }
            this.Clear();
            this.queue.Clear();
            for (int j = 0; j < actors.Count; j++)
            {
                Vector3Int position = actors[j].Position;
                int num = this.PosToIndex(position.x, position.y, position.z);
                this.queue.Enqueue(num);
                this.distances[num] = 0;
                this.cameFrom[num] = num;
                this.firstReachedByActorIndex[num] = j;
                this.visited.Add(position);
            }
            CellsInfo cellsInfo = this.world.CellsInfo;
            Profiler.Begin("BFSCache.InitFor()");
            try
            {
                while (this.queue.Count != 0)
                {
                    int num2 = this.queue.Dequeue();
                    int num3 = this.distances[num2];
                    int num4 = this.firstReachedByActorIndex[num2];
                    World.CanMoveFromToParams item = this.fromMultiple[num4].Item2;
                    Vector3Int vector3Int = this.IndexToPos(num2);
                    int num5;
                    Vector3Int[] adjacentCanPassThrough = cellsInfo.GetAdjacentCanPassThrough(num2, out num5);
                    for (int k = 0; k < num5; k++)
                    {
                        Vector3Int vector3Int2 = adjacentCanPassThrough[k];
                        int num6 = this.PosToIndex(vector3Int2.x, vector3Int2.y, vector3Int2.z);
                        if (this.distances[num6] < 0 && this.world.CanMoveFromTo_AssumeAdjacentAndCanPassThrough(vector3Int, vector3Int2, ref item))
                        {
                            int num7 = num3 + 1;
                            this.distances[num6] = num7;
                            this.cameFrom[num6] = num2;
                            this.firstReachedByActorIndex[num6] = num4;
                            this.visited.Add(vector3Int2);
                            if (num7 < maxDistance)
                            {
                                this.queue.Enqueue(num6);
                            }
                        }
                    }
                }
            }
            finally
            {
                Profiler.End();
            }
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
                    return this.distances[this.PosToIndex(to.x, to.y, to.z)] >= 0;
                case PathFinder.Request.Mode.Touch:
                    {
                        for (int i = 0; i < Vector3IntUtility.DirectionsAndInside.Length; i++)
                        {
                            Vector3Int vector3Int = to + Vector3IntUtility.DirectionsAndInside[i];
                            if (this.world.InBounds(vector3Int) && this.distances[this.PosToIndex(vector3Int.x, vector3Int.y, vector3Int.z)] >= 0 && this.CanTouch(vector3Int, to))
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

        public int GetDistTo(Vector3Int to, PathFinder.Request.Mode mode)
        {
            if (!this.world.InBounds(to))
            {
                return -1;
            }
            switch (mode)
            {
                case PathFinder.Request.Mode.None:
                    return -1;
                case PathFinder.Request.Mode.ToCell:
                    return this.distances[this.PosToIndex(to.x, to.y, to.z)];
                case PathFinder.Request.Mode.Touch:
                    {
                        int num = -1;
                        for (int i = 0; i < Vector3IntUtility.DirectionsAndInside.Length; i++)
                        {
                            Vector3Int vector3Int = to + Vector3IntUtility.DirectionsAndInside[i];
                            if (this.world.InBounds(vector3Int))
                            {
                                int num2 = this.distances[this.PosToIndex(vector3Int.x, vector3Int.y, vector3Int.z)];
                                if (num2 >= 0 && (num < 0 || num2 < num) && this.CanTouch(vector3Int, to))
                                {
                                    num = num2;
                                }
                            }
                        }
                        return num;
                    }
                default:
                    return -1;
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
                    {
                        int num = this.PosToIndex(to.x, to.y, to.z);
                        if (this.distances[num] >= 0)
                        {
                            Vector3Int vector3Int = ((this.firstReachedByActorIndex[num] >= 0) ? this.fromMultiple[this.firstReachedByActorIndex[num]].Item1 : this.from);
                            return Get.PathFinder.ReconstructPath(vector3Int, to, this.cameFrom, null);
                        }
                        return null;
                    }
                case PathFinder.Request.Mode.Touch:
                    {
                        int num2 = -1;
                        Vector3Int vector3Int2 = default(Vector3Int);
                        for (int i = 0; i < Vector3IntUtility.DirectionsAndInside.Length; i++)
                        {
                            Vector3Int vector3Int3 = to + Vector3IntUtility.DirectionsAndInside[i];
                            if (this.world.InBounds(vector3Int3))
                            {
                                int num3 = this.distances[this.PosToIndex(vector3Int3.x, vector3Int3.y, vector3Int3.z)];
                                if (num3 >= 0 && (num2 < 0 || num3 < num2) && this.CanTouch(vector3Int3, to))
                                {
                                    num2 = num3;
                                    vector3Int2 = vector3Int3;
                                }
                            }
                        }
                        if (num2 >= 0)
                        {
                            int num4 = this.PosToIndex(vector3Int2.x, vector3Int2.y, vector3Int2.z);
                            Vector3Int vector3Int4 = ((this.firstReachedByActorIndex[num4] >= 0) ? this.fromMultiple[this.firstReachedByActorIndex[num4]].Item1 : this.from);
                            return Get.PathFinder.ReconstructPath(vector3Int4, vector3Int2, this.cameFrom, null);
                        }
                        return null;
                    }
                default:
                    return null;
            }
        }

        public int GetFirstReachedByActorIndex(Vector3Int to, PathFinder.Request.Mode mode)
        {
            if (!this.world.InBounds(to))
            {
                return -1;
            }
            switch (mode)
            {
                case PathFinder.Request.Mode.None:
                    return -1;
                case PathFinder.Request.Mode.ToCell:
                    return this.firstReachedByActorIndex[this.PosToIndex(to.x, to.y, to.z)];
                case PathFinder.Request.Mode.Touch:
                    {
                        int num = -1;
                        Vector3Int vector3Int = default(Vector3Int);
                        for (int i = 0; i < Vector3IntUtility.DirectionsAndInside.Length; i++)
                        {
                            Vector3Int vector3Int2 = to + Vector3IntUtility.DirectionsAndInside[i];
                            if (this.world.InBounds(vector3Int2))
                            {
                                int num2 = this.distances[this.PosToIndex(vector3Int2.x, vector3Int2.y, vector3Int2.z)];
                                if (num2 >= 0 && (num < 0 || num2 < num) && this.CanTouch(vector3Int2, to))
                                {
                                    num = num2;
                                    vector3Int = vector3Int2;
                                }
                            }
                        }
                        if (num >= 0)
                        {
                            return this.firstReachedByActorIndex[this.PosToIndex(vector3Int.x, vector3Int.y, vector3Int.z)];
                        }
                        return -1;
                    }
                default:
                    return -1;
            }
        }

        private bool CanTouch(Vector3Int from, Vector3Int target)
        {
            if (!this.world.InBounds(from))
            {
                return false;
            }
            int num = this.firstReachedByActorIndex[this.PosToIndex(from.x, from.y, from.z)];
            if (num >= 0)
            {
                return this.world.CanTouch(from, target, this.fromMultiple[num].Item2.Gravity, this.fromMultiple[num].Item2.CanFly, this.fromMultiple[num].Item2.CanUseLadders);
            }
            return this.world.CanTouch(from, target, this.from_pathParams.Gravity, this.from_pathParams.CanFly, this.from_pathParams.CanUseLadders);
        }

        private void Clear()
        {
            for (int i = 0; i < this.visited.Count; i++)
            {
                int num = this.PosToIndex(this.visited[i].x, this.visited[i].y, this.visited[i].z);
                this.distances[num] = -1;
                this.firstReachedByActorIndex[num] = -1;
                this.cameFrom[num] = 0;
            }
            this.visited.Clear();
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

        [TupleElementNames(new string[] { "from", null })]
        private List<ValueTuple<Vector3Int, World.CanMoveFromToParams>> fromMultiple = new List<ValueTuple<Vector3Int, World.CanMoveFromToParams>>();

        private int[] distances;

        private int[] firstReachedByActorIndex;

        private int[] cameFrom;

        private List<Vector3Int> visited;

        private Vector3Int worldSize;

        private int worldSizeXTimesY;

        private Queue<int> queue = new Queue<int>(400);
    }
}