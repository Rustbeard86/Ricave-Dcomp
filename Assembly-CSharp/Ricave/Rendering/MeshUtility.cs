using System;
using UnityEngine;

namespace Ricave.Rendering
{
    public static class MeshUtility
    {
        public static Mesh MirrorXAxis(Mesh mesh)
        {
            CombineInstance combineInstance = default(CombineInstance);
            combineInstance.mesh = mesh;
            combineInstance.transform = Matrix4x4.Scale(new Vector3(-1f, 1f, 1f));
            MeshUtility.tmpCombineInstanceArray[0] = combineInstance;
            Mesh mesh2 = new Mesh();
            mesh2.CombineMeshes(MeshUtility.tmpCombineInstanceArray, true);
            return mesh2;
        }

        private static readonly CombineInstance[] tmpCombineInstanceArray = new CombineInstance[1];
    }
}