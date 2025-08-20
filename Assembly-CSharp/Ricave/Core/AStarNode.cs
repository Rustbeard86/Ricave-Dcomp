using System;

namespace Ricave.Core
{
    public struct AStarNode
    {
        public int pos;

        public float costPlusHeuristicCostToDest;
    }
}