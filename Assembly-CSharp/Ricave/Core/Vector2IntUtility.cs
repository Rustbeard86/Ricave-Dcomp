using System;
using UnityEngine;

namespace Ricave.Core
{
    public static class Vector2IntUtility
    {
        public static float Atan2(this Vector2Int from, Vector2Int to)
        {
            return Calc.Atan2((float)(to.y - from.y), (float)(to.x - from.x));
        }

        public static float Atan2Deg(this Vector2Int from, Vector2Int to)
        {
            return Calc.Atan2((float)(to.y - from.y), (float)(to.x - from.x)) * 57.29578f;
        }

        public static readonly Vector2Int[] DirectionsCardinal = new Vector2Int[]
        {
            new Vector2Int(1, 0),
            new Vector2Int(-1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(0, -1)
        };
    }
}