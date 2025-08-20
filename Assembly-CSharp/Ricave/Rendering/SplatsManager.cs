using System;
using System.Collections.Generic;
using Ricave.Core;
using UnityEngine;
using UnityEngine.Rendering;

namespace Ricave.Rendering
{
    public class SplatsManager
    {
        public void Update()
        {
            if (SplatsManager.SplatMaterial == null)
            {
                SplatsManager.SplatMaterial = Assets.Get<Material>("Materials/Actors/Splat");
            }
            for (int i = this.splats.Count - 1; i >= 0; i--)
            {
                if ((this.splats[i].createdAtSequence > Get.TurnManager.CurrentSequence - 1 && Get.TurnManager.IsPlayerTurn) || !Get.CellsInfo.IsFilled(this.splats[i].attachedAtPos))
                {
                    this.splats.RemoveAt(i);
                    this.meshDirty = true;
                }
            }
            if (this.meshDirty)
            {
                Profiler.Begin("Rebuild splat mesh");
                try
                {
                    this.RebuildMesh();
                    this.meshDirty = false;
                }
                finally
                {
                    Profiler.End();
                }
            }
            if (this.splats.Count != 0)
            {
                Graphics.DrawMesh(this.combinedMesh, Matrix4x4.identity, SplatsManager.SplatMaterial, 0, null, 0, null, ShadowCastingMode.Off, true);
            }
        }

        public void AddSplat(Vector3 position, Vector3 normal, Color color, float size, Vector3Int attachedAtPos, int createdAtSequence = -1)
        {
            for (int i = 0; i < this.splats.Count; i++)
            {
                if ((this.splats[i].position - position).sqrMagnitude < 0.0225f)
                {
                    return;
                }
            }
            if (this.splats.Count >= 250)
            {
                this.splats.RemoveAt(0);
            }
            this.splats.Add(new SplatsManager.Splat
            {
                position = position,
                normal = normal,
                color = color,
                size = size,
                rotation = Rand.Range(0f, 360f),
                createdAtSequence = ((createdAtSequence != -1) ? createdAtSequence : Get.TurnManager.CurrentSequence),
                attachedAtPos = attachedAtPos
            });
            this.meshDirty = true;
        }

        private void RebuildMesh()
        {
            SplatsManager.tmpVerts.Clear();
            SplatsManager.tmpNormals.Clear();
            SplatsManager.tmpColors.Clear();
            SplatsManager.tmpUVs.Clear();
            SplatsManager.tmpTris.Clear();
            for (int i = 0; i < this.splats.Count; i++)
            {
                this.AddSplatQuadToWorkingLists(this.splats[i]);
            }
            if (this.combinedMesh == null)
            {
                this.combinedMesh = new Mesh();
            }
            else
            {
                this.combinedMesh.Clear();
            }
            if (SplatsManager.tmpVerts.Count != 0)
            {
                this.combinedMesh.SetVertices(SplatsManager.tmpVerts);
                this.combinedMesh.SetNormals(SplatsManager.tmpNormals);
                this.combinedMesh.SetColors(SplatsManager.tmpColors);
                this.combinedMesh.SetUVs(0, SplatsManager.tmpUVs);
                this.combinedMesh.SetTriangles(SplatsManager.tmpTris, 0);
                this.combinedMesh.RecalculateBounds();
            }
        }

        private void AddSplatQuadToWorkingLists(SplatsManager.Splat splat)
        {
            Vector3 vector;
            if (Mathf.Abs(splat.normal.y) > 0.9f)
            {
                vector = Vector3.right;
            }
            else
            {
                vector = Vector3.Cross(Vector3.up, splat.normal).normalized;
            }
            Vector3 normalized = Vector3.Cross(splat.normal, vector).normalized;
            float num = splat.rotation * 0.017453292f;
            float num2 = Mathf.Cos(num);
            float num3 = Mathf.Sin(num);
            Vector3 vector2 = vector * num2 + normalized * num3;
            Vector3 vector3 = -vector * num3 + normalized * num2;
            int count = SplatsManager.tmpVerts.Count;
            SplatsManager.tmpVerts.Add(splat.position + (-vector2 - vector3) * splat.size);
            SplatsManager.tmpVerts.Add(splat.position + (vector2 - vector3) * splat.size);
            SplatsManager.tmpVerts.Add(splat.position + (vector2 + vector3) * splat.size);
            SplatsManager.tmpVerts.Add(splat.position + (-vector2 + vector3) * splat.size);
            for (int i = 0; i < 4; i++)
            {
                SplatsManager.tmpNormals.Add(splat.normal);
                SplatsManager.tmpColors.Add(splat.color);
            }
            SplatsManager.tmpUVs.Add(new Vector2(0f, 0f));
            SplatsManager.tmpUVs.Add(new Vector2(1f, 0f));
            SplatsManager.tmpUVs.Add(new Vector2(1f, 1f));
            SplatsManager.tmpUVs.Add(new Vector2(0f, 1f));
            SplatsManager.tmpTris.Add(count);
            SplatsManager.tmpTris.Add(count + 1);
            SplatsManager.tmpTris.Add(count + 2);
            SplatsManager.tmpTris.Add(count);
            SplatsManager.tmpTris.Add(count + 2);
            SplatsManager.tmpTris.Add(count + 3);
        }

        public void Clear()
        {
            this.splats.Clear();
            this.meshDirty = true;
        }

        private List<SplatsManager.Splat> splats = new List<SplatsManager.Splat>();

        private Mesh combinedMesh;

        private bool meshDirty;

        private static List<Vector3> tmpVerts = new List<Vector3>();

        private static List<Vector3> tmpNormals = new List<Vector3>();

        private static List<Color> tmpColors = new List<Color>();

        private static List<Vector2> tmpUVs = new List<Vector2>();

        private static List<int> tmpTris = new List<int>();

        private const int MaxSplats = 250;

        private const float MinDistBetweenSplats = 0.15f;

        private static Material SplatMaterial;

        private struct Splat
        {
            public Vector3 position;

            public Vector3 normal;

            public Color color;

            public float size;

            public float rotation;

            public int createdAtSequence;

            public Vector3Int attachedAtPos;
        }
    }
}