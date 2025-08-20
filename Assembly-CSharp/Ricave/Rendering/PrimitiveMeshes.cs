using System;
using System.Collections.Generic;
using Ricave.Core;
using UnityEngine;

namespace Ricave.Rendering
{
    public static class PrimitiveMeshes
    {
        public static Mesh Cube
        {
            get
            {
                return PrimitiveMeshes.Get(PrimitiveType.Cube);
            }
        }

        public static void Init()
        {
            PrimitiveMeshes.primitives = new Mesh[EnumUtility.GetMaxValue<PrimitiveType>() + 1];
            foreach (PrimitiveType primitiveType in EnumUtility.GetValues<PrimitiveType>())
            {
                GameObject gameObject = GameObject.CreatePrimitive(primitiveType);
                PrimitiveMeshes.primitives[(int)primitiveType] = gameObject.GetComponent<MeshFilter>().sharedMesh;
                Object.Destroy(gameObject);
            }
        }

        public static Mesh Get(PrimitiveType primitiveType)
        {
            return PrimitiveMeshes.primitives[(int)primitiveType];
        }

        public static Mesh CubeWithMissingFaces(bool left, bool right, bool up, bool down, bool forward, bool back)
        {
            int num = (left ? 1 : 0) + (right ? 1 : 0) * 2 + (up ? 1 : 0) * 4 + (down ? 1 : 0) * 8 + (forward ? 1 : 0) * 16 + (back ? 1 : 0) * 32;
            if (PrimitiveMeshes.cubesWithMissingFaces[num] != null)
            {
                return PrimitiveMeshes.cubesWithMissingFaces[num];
            }
            if (num == 63)
            {
                Mesh cube_WrapXZ = PrimitiveMeshes.Cube_WrapXZ;
                PrimitiveMeshes.cubesWithMissingFaces[num] = cube_WrapXZ;
                return cube_WrapXZ;
            }
            Mesh mesh = Object.Instantiate<Mesh>(PrimitiveMeshes.Cube_WrapXZ);
            mesh.name = "CubeWithMissingFaces";
            PrimitiveMeshes.tmpNewTris.Clear();
            if (num != 0)
            {
                PrimitiveMeshes.tmpNormals.Clear();
                PrimitiveMeshes.tmpTris.Clear();
                mesh.GetTriangles(PrimitiveMeshes.tmpTris, 0);
                mesh.GetNormals(PrimitiveMeshes.tmpNormals);
                for (int i = 0; i < PrimitiveMeshes.tmpTris.Count; i++)
                {
                    Vector3 vector = PrimitiveMeshes.tmpNormals[PrimitiveMeshes.tmpTris[i]];
                    if ((left && Vector3.Dot(vector, Vector3.left) > 0.9999f) || (right && Vector3.Dot(vector, Vector3.right) > 0.9999f) || (up && Vector3.Dot(vector, Vector3.up) > 0.9999f) || (down && Vector3.Dot(vector, Vector3.down) > 0.9999f) || (forward && Vector3.Dot(vector, Vector3.forward) > 0.9999f) || (back && Vector3.Dot(vector, Vector3.back) > 0.9999f))
                    {
                        PrimitiveMeshes.tmpNewTris.Add(PrimitiveMeshes.tmpTris[i]);
                    }
                }
            }
            mesh.SetTriangles(PrimitiveMeshes.tmpNewTris, 0);
            mesh.Optimize();
            PrimitiveMeshes.cubesWithMissingFaces[num] = mesh;
            return mesh;
        }

        public static bool IsCube(Mesh mesh)
        {
            return mesh == PrimitiveMeshes.Cube || mesh == PrimitiveMeshes.Cube_WrapXZ || mesh == PrimitiveMeshes.Cube_WrapXZMirror || mesh == PrimitiveMeshes.Cube_WrapFull;
        }

        public static bool IsCubeXZ(Mesh mesh)
        {
            return mesh == PrimitiveMeshes.CubeXZ || mesh == PrimitiveMeshes.CubeXZ_WrapXZ || mesh == PrimitiveMeshes.CubeXZ_WrapXZMirror;
        }

        public static bool IsCubeUpDown(Mesh mesh)
        {
            return mesh == PrimitiveMeshes.CubeUpDown || mesh == PrimitiveMeshes.CubeUpDown_WrapXZ;
        }

        private static Mesh[] primitives;

        private static Mesh[] cubesWithMissingFaces = new Mesh[64];

        public static readonly Mesh Cube_WrapXZ = Assets.Get<Mesh>("Models/Cube_WrapXZ");

        public static readonly Mesh Cube_WrapXZMirror = Assets.Get<Mesh>("Models/Cube_WrapXZMirror");

        public static readonly Mesh Cube_WrapFull = Assets.Get<Mesh>("Models/Cube_WrapFull");

        public static readonly Mesh CubeXZ = Assets.Get<Mesh>("Models/CubeXZ");

        public static readonly Mesh CubeXZ_WrapXZ = Assets.Get<Mesh>("Models/CubeXZ_WrapXZ");

        public static readonly Mesh CubeXZ_WrapXZMirror = Assets.Get<Mesh>("Models/CubeXZ_WrapXZMirror");

        public static readonly Mesh CubeUpDown = Assets.Get<Mesh>("Models/CubeUpDown");

        public static readonly Mesh CubeUpDown_WrapXZ = Assets.Get<Mesh>("Models/CubeUpDown_WrapXZ");

        private static List<int> tmpTris = new List<int>(100);

        private static List<Vector3> tmpNormals = new List<Vector3>(50);

        private static List<int> tmpNewTris = new List<int>(100);
    }
}