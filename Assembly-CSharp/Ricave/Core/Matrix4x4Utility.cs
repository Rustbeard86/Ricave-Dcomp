using System;
using UnityEngine;

namespace Ricave.Core
{
    public static class Matrix4x4Utility
    {
        public static void SetPosition(this Matrix4x4 matrix, Vector3 pos)
        {
            matrix.m03 = pos.x;
            matrix.m13 = pos.y;
            matrix.m23 = pos.z;
        }
    }
}