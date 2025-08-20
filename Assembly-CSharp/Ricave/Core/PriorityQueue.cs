using System;
using UnityEngine;

namespace Ricave.Core
{
    public class PriorityQueue
    {
        public int Count
        {
            get
            {
                return this.count;
            }
        }

        public PriorityQueue(Vector3Int worldSize)
        {
            this.worldSize = worldSize;
            this.heap = new AStarNode[worldSize.x * worldSize.y * worldSize.z];
            this.posToHeapIndex = new int[worldSize.x * worldSize.y * worldSize.z];
            Array.Fill<int>(this.posToHeapIndex, -1);
        }

        public void Clear()
        {
            for (int i = 0; i < this.count; i++)
            {
                this.posToHeapIndex[this.heap[i].pos] = -1;
            }
            this.count = 0;
        }

        public void PushOrDecreaseKey(ref AStarNode node)
        {
            int num = this.posToHeapIndex[node.pos];
            if (num < 0)
            {
                int num2 = this.count;
                this.count++;
                this.BubbleUp(num2, ref node);
                return;
            }
            this.BubbleUp(num, ref node);
        }

        public AStarNode Pop()
        {
            AStarNode astarNode = this.heap[0];
            this.posToHeapIndex[astarNode.pos] = -1;
            this.count--;
            if (this.count > 0)
            {
                AStarNode astarNode2 = this.heap[this.count];
                int num = 0;
                int num2;
                while ((num2 = num * 2 + 1) < this.count)
                {
                    AStarNode astarNode3 = this.heap[num2];
                    int num3 = num2;
                    int num4 = Math.Min(num2 + 2, this.count);
                    while (++num2 < num4)
                    {
                        AStarNode astarNode4 = this.heap[num2];
                        if (astarNode4.costPlusHeuristicCostToDest < astarNode3.costPlusHeuristicCostToDest)
                        {
                            astarNode3 = astarNode4;
                            num3 = num2;
                        }
                    }
                    if (astarNode2.costPlusHeuristicCostToDest <= astarNode3.costPlusHeuristicCostToDest)
                    {
                        break;
                    }
                    this.heap[num] = astarNode3;
                    this.posToHeapIndex[astarNode3.pos] = num;
                    num = num3;
                }
                this.heap[num] = astarNode2;
                this.posToHeapIndex[astarNode2.pos] = num;
            }
            return astarNode;
        }

        public AStarNode Peek()
        {
            return this.heap[0];
        }

        private void BubbleUp(int index, ref AStarNode node)
        {
            while (index > 0)
            {
                int num = (index - 1) / 2;
                AStarNode astarNode = this.heap[num];
                if (node.costPlusHeuristicCostToDest >= astarNode.costPlusHeuristicCostToDest)
                {
                    break;
                }
                this.heap[index] = astarNode;
                this.posToHeapIndex[astarNode.pos] = index;
                index = num;
            }
            this.heap[index] = node;
            this.posToHeapIndex[node.pos] = index;
        }

        private AStarNode[] heap;

        private int count;

        private int[] posToHeapIndex;

        private Vector3Int worldSize;
    }
}