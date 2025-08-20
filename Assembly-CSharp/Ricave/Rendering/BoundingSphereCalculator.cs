using System;
using Ricave.Core;
using UnityEngine;

namespace Ricave.Rendering
{
    public static class BoundingSphereCalculator
    {
        public static void CalculateBoundingSphere(Mesh mesh, out Vector3 center, out float radius)
        {
            center = mesh.bounds.center;
            radius = 0.001f;
            Vector3[] vertices = mesh.vertices;
            for (int i = 0; i < vertices.Length; i++)
            {
                float sqrMagnitude = (center - vertices[i]).sqrMagnitude;
                if (sqrMagnitude > radius * radius)
                {
                    radius = Calc.Sqrt(sqrMagnitude);
                }
            }
        }
    }
}