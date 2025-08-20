using System;
using UnityEngine;

namespace Ricave.Core
{
    public static class GravityUtility
    {
        public static bool IsAltitudeLower(Vector3Int a, Vector3Int b, Vector3Int gravity)
        {
            return Vector3IntUtility.Dot(a, gravity) > Vector3IntUtility.Dot(b, gravity);
        }

        public static bool IsAltitudeEqual(Vector3Int a, Vector3Int b, Vector3Int gravity)
        {
            return Vector3IntUtility.Dot(a, gravity) == Vector3IntUtility.Dot(b, gravity);
        }

        public static bool IsAltitudeLowerOrEqual(Vector3Int a, Vector3Int b, Vector3Int gravity)
        {
            return Vector3IntUtility.Dot(a, gravity) >= Vector3IntUtility.Dot(b, gravity);
        }

        public static void GetForwardAndRight(Vector3Int gravity, out Vector3Int forward, out Vector3Int right)
        {
            if (gravity.y == -1)
            {
                forward = new Vector3Int(0, 0, 1);
                right = new Vector3Int(1, 0, 0);
                return;
            }
            if (gravity.y == 1)
            {
                forward = new Vector3Int(0, 0, -1);
                right = new Vector3Int(1, 0, 0);
                return;
            }
            if (gravity.x == -1)
            {
                forward = new Vector3Int(0, 0, 1);
                right = new Vector3Int(0, -1, 0);
                return;
            }
            if (gravity.x == 1)
            {
                forward = new Vector3Int(0, 0, 1);
                right = new Vector3Int(0, 1, 0);
                return;
            }
            if (gravity.z == -1)
            {
                forward = new Vector3Int(0, -1, 0);
                right = new Vector3Int(1, 0, 0);
                return;
            }
            if (gravity.z == 1)
            {
                forward = new Vector3Int(0, 1, 0);
                right = new Vector3Int(1, 0, 0);
                return;
            }
            forward = new Vector3Int(0, 0, 1);
            right = new Vector3Int(1, 0, 0);
        }
    }
}