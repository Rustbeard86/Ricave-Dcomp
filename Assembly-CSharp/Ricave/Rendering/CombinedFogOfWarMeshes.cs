using System;
using System.Collections.Generic;
using Ricave.Core;
using UnityEngine;
using UnityEngine.Rendering;

namespace Ricave.Rendering
{
    public class CombinedFogOfWarMeshes
    {
        public void Init()
        {
            this.mesh = new Mesh();
            this.dirty = true;
            if (CombinedFogOfWarMeshes.material == null)
            {
                CombinedFogOfWarMeshes.material = Assets.Get<Material>("Materials/FogOfWar/FogOfWarBorder");
            }
        }

        public void Update()
        {
            if (this.dirty)
            {
                Profiler.Begin("Update fog mesh");
                try
                {
                    this.dirty = false;
                    this.UpdateMesh();
                }
                finally
                {
                    Profiler.End();
                }
            }
            if (this.optimizeMeshOnFrame != -1 && Clock.Frame >= this.optimizeMeshOnFrame)
            {
                Profiler.Begin("Optimize fog mesh");
                try
                {
                    this.optimizeMeshOnFrame = -1;
                    this.mesh.Optimize();
                }
                finally
                {
                    Profiler.End();
                }
            }
            Graphics.DrawMesh(this.mesh, Matrix4x4.identity, CombinedFogOfWarMeshes.material, 0, null, 0, null, ShadowCastingMode.Off, false);
        }

        private void UpdateMesh()
        {
            this.mesh.Clear();
            this.mesh.SetVertices(this.verts);
            int num = this.verts.Count / 4 * 6;
            if (this.tris.Count > num)
            {
                this.tris.RemoveRange(num, this.tris.Count - num);
            }
            else
            {
                for (int i = this.tris.Count; i < num; i += 6)
                {
                    int num2 = i / 6 * 4;
                    this.tris.Add(num2);
                    this.tris.Add(num2 + 1);
                    this.tris.Add(num2 + 2);
                    this.tris.Add(num2 + 2);
                    this.tris.Add(num2 + 3);
                    this.tris.Add(num2);
                }
            }
            this.mesh.SetTriangles(this.tris, 0);
            this.optimizeMeshOnFrame = Clock.Frame + 1;
        }

        public void Add(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
        {
            this.verts.Add(v1);
            this.verts.Add(v2);
            this.verts.Add(v3);
            this.verts.Add(v4);
            this.dirty = true;
        }

        public void Remove(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
        {
            int i = 0;
            int count = this.verts.Count;
            while (i < count)
            {
                if (this.verts[i] == v1 && this.verts[i + 1] == v2 && this.verts[i + 2] == v3 && this.verts[i + 3] == v4)
                {
                    this.verts.RemoveRange(i, 4);
                    this.dirty = true;
                    return;
                }
                i += 4;
            }
            string[] array = new string[9];
            array[0] = "Could not remove fog of war border mesh at ";
            int num = 1;
            Vector3 vector = v1;
            array[num] = vector.ToString();
            array[2] = " ";
            int num2 = 3;
            vector = v2;
            array[num2] = vector.ToString();
            array[4] = " ";
            int num3 = 5;
            vector = v3;
            array[num3] = vector.ToString();
            array[6] = " ";
            int num4 = 7;
            vector = v4;
            array[num4] = vector.ToString();
            array[8] = " because it doesn't exist.";
            Log.Error(string.Concat(array), false);
        }

        private Mesh mesh;

        private List<Vector3> verts = new List<Vector3>();

        private List<int> tris = new List<int>();

        private bool dirty;

        private int optimizeMeshOnFrame = -1;

        private const string MaterialPath = "Materials/FogOfWar/FogOfWarBorder";

        private static Material material;
    }
}