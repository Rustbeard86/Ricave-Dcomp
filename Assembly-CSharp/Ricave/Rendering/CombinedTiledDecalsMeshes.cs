using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Ricave.Core;
using UnityEngine;
using UnityEngine.Rendering;

namespace Ricave.Rendering
{
    public class CombinedTiledDecalsMeshes
    {
        public void Update()
        {
            for (int i = 0; i < this.meshes.Count; i++)
            {
                if (this.meshes[i].meshDirty || this.meshes[i].meshDirtyPositionsOnly)
                {
                    Profiler.Begin("Update tiled decals mesh");
                    try
                    {
                        this.UpdateMesh(this.meshes[i], !this.meshes[i].meshDirty);
                        CombinedTiledDecalsMeshes.DecalsMesh decalsMesh = this.meshes[i];
                        decalsMesh.meshDirty = false;
                        decalsMesh.meshDirtyPositionsOnly = false;
                        this.meshes[i] = decalsMesh;
                    }
                    finally
                    {
                        Profiler.End();
                    }
                }
                Graphics.DrawMesh(this.meshes[i].mesh, Matrix4x4.identity, this.meshes[i].spec.Material, 0, null, 0, null, ShadowCastingMode.Off, true);
            }
        }

        private void UpdateMesh(CombinedTiledDecalsMeshes.DecalsMesh decalsMesh, bool positionsOnly)
        {
            if (!positionsOnly)
            {
                decalsMesh.mesh.Clear();
                decalsMesh.mesh.SetVertices(decalsMesh.verts);
                decalsMesh.mesh.SetUVs(0, decalsMesh.uv);
                decalsMesh.mesh.SetNormals(decalsMesh.normals);
                List<int> tris = decalsMesh.tris;
                int num = decalsMesh.verts.Count / 16 * 54;
                if (tris.Count > num)
                {
                    tris.RemoveRange(num, tris.Count - num);
                }
                else
                {
                    for (int i = tris.Count; i < num; i += 54)
                    {
                        int num2 = i / 54 * 16;
                        tris.Add(num2);
                        tris.Add(num2 + 1);
                        tris.Add(num2 + 2);
                        tris.Add(num2 + 2);
                        tris.Add(num2 + 3);
                        tris.Add(num2);
                        tris.Add(num2 + 4);
                        tris.Add(num2 + 5);
                        tris.Add(num2);
                        tris.Add(num2 + 5);
                        tris.Add(num2 + 1);
                        tris.Add(num2);
                        tris.Add(num2 + 5);
                        tris.Add(num2 + 6);
                        tris.Add(num2 + 1);
                        tris.Add(num2 + 6);
                        tris.Add(num2 + 7);
                        tris.Add(num2 + 1);
                        tris.Add(num2 + 1);
                        tris.Add(num2 + 7);
                        tris.Add(num2 + 8);
                        tris.Add(num2 + 1);
                        tris.Add(num2 + 8);
                        tris.Add(num2 + 2);
                        tris.Add(num2 + 2);
                        tris.Add(num2 + 8);
                        tris.Add(num2 + 9);
                        tris.Add(num2 + 2);
                        tris.Add(num2 + 9);
                        tris.Add(num2 + 10);
                        tris.Add(num2 + 2);
                        tris.Add(num2 + 10);
                        tris.Add(num2 + 11);
                        tris.Add(num2 + 3);
                        tris.Add(num2 + 2);
                        tris.Add(num2 + 11);
                        tris.Add(num2 + 3);
                        tris.Add(num2 + 11);
                        tris.Add(num2 + 12);
                        tris.Add(num2 + 3);
                        tris.Add(num2 + 12);
                        tris.Add(num2 + 13);
                        tris.Add(num2 + 3);
                        tris.Add(num2 + 13);
                        tris.Add(num2 + 14);
                        tris.Add(num2);
                        tris.Add(num2 + 3);
                        tris.Add(num2 + 14);
                        tris.Add(num2);
                        tris.Add(num2 + 14);
                        tris.Add(num2 + 15);
                        tris.Add(num2);
                        tris.Add(num2 + 15);
                        tris.Add(num2 + 4);
                    }
                }
                decalsMesh.mesh.SetTriangles(decalsMesh.tris, 0);
                decalsMesh.mesh.SetColors(decalsMesh.colors);
                return;
            }
            decalsMesh.mesh.SetVertices(decalsMesh.verts);
            decalsMesh.mesh.SetColors(decalsMesh.colors);
            decalsMesh.mesh.RecalculateBounds();
        }

        public void AddOrUpdate(Vector3Int pos, TiledDecalsSpec spec, Vector3Int dir, bool transparentTL, bool transparentT, bool transparentTR, bool transparentR, bool transparentBR, bool transparentB, bool transparentBL, bool transparentL, float alpha, Matrix4x4 extraTransform, Vector3 extraTransformRelativeTo, float texCoordsScale)
        {
            CombinedTiledDecalsMeshes.<> c__DisplayClass8_0 CS$<> 8__locals1;
            CS$<> 8__locals1.extraTransform = extraTransform;
            CS$<> 8__locals1.extraTransformRelativeTo = extraTransformRelativeTo;
            CombinedTiledDecalsMeshes.DecalsMesh decalsMesh = default(CombinedTiledDecalsMeshes.DecalsMesh);
            int num = -1;
            for (int i = 0; i < this.meshes.Count; i++)
            {
                if (this.meshes[i].spec == spec)
                {
                    decalsMesh = this.meshes[i];
                    num = i;
                    break;
                }
            }
            if (num < 0)
            {
                decalsMesh = new CombinedTiledDecalsMeshes.DecalsMesh
                {
                    spec = spec,
                    verts = new List<Vector3>(),
                    uv = new List<Vector2>(),
                    colors = new List<Color>(),
                    normals = new List<Vector3>(),
                    tris = new List<int>(),
                    cells = new List<ValueTuple<Vector3Int, Vector3Int>>(),
                    cellsHashSet = new HashSet<ValueTuple<Vector3Int, Vector3Int>>(),
                    mesh = new Mesh(),
                    meshDirty = true
                };
                this.meshes.Add(decalsMesh);
                num = this.meshes.Count - 1;
            }
            this.GetVerts(pos, dir, CombinedTiledDecalsMeshes.tmpVerts, spec.Order, CombinedTiledDecalsMeshes.tmpTexCoords, texCoordsScale);
            if (!CS$<> 8__locals1.extraTransform.isIdentity)
			{
                for (int j = 0; j < CombinedTiledDecalsMeshes.tmpVerts.Length; j++)
                {
                    CombinedTiledDecalsMeshes.tmpVerts[j] = CombinedTiledDecalsMeshes.< AddOrUpdate > g__TransformPoint | 8_0(CombinedTiledDecalsMeshes.tmpVerts[j], ref CS$<> 8__locals1);
                }
            }
            Color color = new Color(1f, 1f, 1f, alpha);
            if (decalsMesh.cellsHashSet.Contains(new ValueTuple<Vector3Int, Vector3Int>(pos, dir)))
            {
                for (int k = 0; k < decalsMesh.cells.Count; k++)
                {
                    if (!(decalsMesh.cells[k].Item1 != pos) && !(decalsMesh.cells[k].Item2 != dir))
                    {
                        int num2 = k * 16;
                        for (int l = 0; l < CombinedTiledDecalsMeshes.tmpVerts.Length; l++)
                        {
                            decalsMesh.verts[num2 + l] = CombinedTiledDecalsMeshes.tmpVerts[l];
                        }
                        decalsMesh.colors[num2] = color;
                        decalsMesh.colors[num2 + 1] = color;
                        decalsMesh.colors[num2 + 2] = color;
                        decalsMesh.colors[num2 + 3] = color;
                        decalsMesh.colors[num2 + 4] = (transparentTL ? new Color(1f, 1f, 1f, 0f) : color);
                        decalsMesh.colors[num2 + 5] = (transparentT ? new Color(1f, 1f, 1f, 0f) : color);
                        decalsMesh.colors[num2 + 6] = (transparentT ? new Color(1f, 1f, 1f, 0f) : color);
                        decalsMesh.colors[num2 + 7] = (transparentTR ? new Color(1f, 1f, 1f, 0f) : color);
                        decalsMesh.colors[num2 + 8] = (transparentR ? new Color(1f, 1f, 1f, 0f) : color);
                        decalsMesh.colors[num2 + 9] = (transparentR ? new Color(1f, 1f, 1f, 0f) : color);
                        decalsMesh.colors[num2 + 10] = (transparentBR ? new Color(1f, 1f, 1f, 0f) : color);
                        decalsMesh.colors[num2 + 11] = (transparentB ? new Color(1f, 1f, 1f, 0f) : color);
                        decalsMesh.colors[num2 + 12] = (transparentB ? new Color(1f, 1f, 1f, 0f) : color);
                        decalsMesh.colors[num2 + 13] = (transparentBL ? new Color(1f, 1f, 1f, 0f) : color);
                        decalsMesh.colors[num2 + 14] = (transparentL ? new Color(1f, 1f, 1f, 0f) : color);
                        decalsMesh.colors[num2 + 15] = (transparentL ? new Color(1f, 1f, 1f, 0f) : color);
                        decalsMesh.meshDirtyPositionsOnly = true;
                        this.meshes[num] = decalsMesh;
                        return;
                    }
                }
            }
            decalsMesh.meshDirty = true;
            this.meshes[num] = decalsMesh;
            decalsMesh.verts.AddRange(CombinedTiledDecalsMeshes.tmpVerts);
            decalsMesh.uv.AddRange(CombinedTiledDecalsMeshes.tmpTexCoords);
            for (int m = 0; m < 4; m++)
            {
                decalsMesh.colors.Add(color);
            }
            decalsMesh.colors.Add(transparentTL ? new Color(1f, 1f, 1f, 0f) : color);
            decalsMesh.colors.Add(transparentT ? new Color(1f, 1f, 1f, 0f) : color);
            decalsMesh.colors.Add(transparentT ? new Color(1f, 1f, 1f, 0f) : color);
            decalsMesh.colors.Add(transparentTR ? new Color(1f, 1f, 1f, 0f) : color);
            decalsMesh.colors.Add(transparentR ? new Color(1f, 1f, 1f, 0f) : color);
            decalsMesh.colors.Add(transparentR ? new Color(1f, 1f, 1f, 0f) : color);
            decalsMesh.colors.Add(transparentBR ? new Color(1f, 1f, 1f, 0f) : color);
            decalsMesh.colors.Add(transparentB ? new Color(1f, 1f, 1f, 0f) : color);
            decalsMesh.colors.Add(transparentB ? new Color(1f, 1f, 1f, 0f) : color);
            decalsMesh.colors.Add(transparentBL ? new Color(1f, 1f, 1f, 0f) : color);
            decalsMesh.colors.Add(transparentL ? new Color(1f, 1f, 1f, 0f) : color);
            decalsMesh.colors.Add(transparentL ? new Color(1f, 1f, 1f, 0f) : color);
            for (int n = 0; n < 16; n++)
            {
                decalsMesh.normals.Add(-dir);
            }
            decalsMesh.cells.Add(new ValueTuple<Vector3Int, Vector3Int>(pos, dir));
            decalsMesh.cellsHashSet.Add(new ValueTuple<Vector3Int, Vector3Int>(pos, dir));
        }

        public void TryRemove(Vector3Int pos, TiledDecalsSpec spec, Vector3Int dir)
        {
            for (int i = 0; i < this.meshes.Count; i++)
            {
                CombinedTiledDecalsMeshes.DecalsMesh decalsMesh = this.meshes[i];
                if (decalsMesh.spec == spec)
                {
                    if (decalsMesh.cellsHashSet.Contains(new ValueTuple<Vector3Int, Vector3Int>(pos, dir)))
                    {
                        for (int j = 0; j < decalsMesh.cells.Count; j++)
                        {
                            if (decalsMesh.cells[j].Item1 == pos && decalsMesh.cells[j].Item2 == dir)
                            {
                                int num = j * 16;
                                decalsMesh.verts.RemoveRange(num, 16);
                                decalsMesh.uv.RemoveRange(num, 16);
                                decalsMesh.colors.RemoveRange(num, 16);
                                decalsMesh.normals.RemoveRange(num, 16);
                                decalsMesh.cellsHashSet.Remove(decalsMesh.cells[j]);
                                decalsMesh.cells.RemoveAt(j);
                                CombinedTiledDecalsMeshes.DecalsMesh decalsMesh2 = decalsMesh;
                                decalsMesh2.meshDirty = true;
                                this.meshes[i] = decalsMesh2;
                                return;
                            }
                        }
                    }
                    return;
                }
            }
        }

        private void GetVerts(Vector3Int pos, Vector3Int dir, Vector3[] outVerts, int order, List<Vector2> outTexCoords, float texCoordsScale)
        {
            this.GetVerts(pos, outVerts, order);
            outTexCoords.Clear();
            if (dir == new Vector3Int(0, -1, 0))
            {
                for (int i = 0; i < outVerts.Length; i++)
                {
                    outTexCoords.Add(new Vector2(outVerts[i].x, outVerts[i].z) * texCoordsScale - new Vector2(0.5f, 0.5f));
                }
                return;
            }
            if (dir == new Vector3Int(0, 1, 0))
            {
                for (int j = 0; j < outVerts.Length; j++)
                {
                    Vector3 vector = outVerts[j];
                    Vector3 vector2 = vector - pos;
                    vector = pos;
                    vector.x += vector2.x;
                    vector.y -= vector2.y;
                    vector.z -= vector2.z;
                    outVerts[j] = vector;
                    outTexCoords.Add(new Vector2(vector.x, vector.z) * texCoordsScale - new Vector2(0.5f, 0.5f));
                }
                return;
            }
            if (dir == new Vector3Int(1, 0, 0))
            {
                for (int k = 0; k < outVerts.Length; k++)
                {
                    Vector3 vector3 = outVerts[k];
                    Vector3 vector4 = vector3 - pos;
                    vector3 = pos;
                    vector3.x -= vector4.y;
                    vector3.y += vector4.z;
                    vector3.z -= vector4.x;
                    outVerts[k] = vector3;
                    outTexCoords.Add(new Vector2(vector3.z, vector3.y) * texCoordsScale - new Vector2(0.5f, 0.5f));
                }
                return;
            }
            if (dir == new Vector3Int(-1, 0, 0))
            {
                for (int l = 0; l < outVerts.Length; l++)
                {
                    Vector3 vector5 = outVerts[l];
                    Vector3 vector6 = vector5 - pos;
                    vector5 = pos;
                    vector5.x += vector6.y;
                    vector5.y += vector6.z;
                    vector5.z += vector6.x;
                    outVerts[l] = vector5;
                    outTexCoords.Add(new Vector2(vector5.z, vector5.y) * texCoordsScale - new Vector2(0.5f, 0.5f));
                }
                return;
            }
            if (dir == new Vector3Int(0, 0, 1))
            {
                for (int m = 0; m < outVerts.Length; m++)
                {
                    Vector3 vector7 = outVerts[m];
                    Vector3 vector8 = vector7 - pos;
                    vector7 = pos;
                    vector7.x += vector8.x;
                    vector7.y += vector8.z;
                    vector7.z -= vector8.y;
                    outVerts[m] = vector7;
                    outTexCoords.Add(new Vector2(vector7.x, vector7.y) * texCoordsScale - new Vector2(0.5f, 0.5f));
                }
                return;
            }
            if (dir == new Vector3Int(0, 0, -1))
            {
                for (int n = 0; n < outVerts.Length; n++)
                {
                    Vector3 vector9 = outVerts[n];
                    Vector3 vector10 = vector9 - pos;
                    vector9 = pos;
                    vector9.x -= vector10.x;
                    vector9.y += vector10.z;
                    vector9.z += vector10.y;
                    outVerts[n] = vector9;
                    outTexCoords.Add(new Vector2(vector9.x, vector9.y) * texCoordsScale - new Vector2(0.5f, 0.5f));
                }
            }
        }

        private void GetVerts(Vector3Int pos, Vector3[] outVerts, int order)
        {
            outVerts[0] = (outVerts[1] = (outVerts[2] = (outVerts[3] = pos)));
            int num = 0;
            outVerts[num].x = outVerts[num].x - 0.3f;
            int num2 = 0;
            outVerts[num2].y = outVerts[num2].y - 0.5f;
            int num3 = 0;
            outVerts[num3].y = outVerts[num3].y + (float)(order + 1) * 0.0001f;
            int num4 = 0;
            outVerts[num4].z = outVerts[num4].z + 0.3f;
            int num5 = 1;
            outVerts[num5].x = outVerts[num5].x + 0.3f;
            int num6 = 1;
            outVerts[num6].y = outVerts[num6].y - 0.5f;
            int num7 = 1;
            outVerts[num7].y = outVerts[num7].y + (float)(order + 1) * 0.0001f;
            int num8 = 1;
            outVerts[num8].z = outVerts[num8].z + 0.3f;
            int num9 = 2;
            outVerts[num9].x = outVerts[num9].x + 0.3f;
            int num10 = 2;
            outVerts[num10].y = outVerts[num10].y - 0.5f;
            int num11 = 2;
            outVerts[num11].y = outVerts[num11].y + (float)(order + 1) * 0.0001f;
            int num12 = 2;
            outVerts[num12].z = outVerts[num12].z - 0.3f;
            int num13 = 3;
            outVerts[num13].x = outVerts[num13].x - 0.3f;
            int num14 = 3;
            outVerts[num14].y = outVerts[num14].y - 0.5f;
            int num15 = 3;
            outVerts[num15].y = outVerts[num15].y + (float)(order + 1) * 0.0001f;
            int num16 = 3;
            outVerts[num16].z = outVerts[num16].z - 0.3f;
            outVerts[4] = (outVerts[5] = (outVerts[6] = (outVerts[7] = (outVerts[8] = (outVerts[9] = (outVerts[10] = (outVerts[11] = (outVerts[12] = (outVerts[13] = (outVerts[14] = (outVerts[15] = pos)))))))))));
            int num17 = 4;
            outVerts[num17].x = outVerts[num17].x - 0.5f;
            int num18 = 4;
            outVerts[num18].y = outVerts[num18].y - 0.5f;
            int num19 = 4;
            outVerts[num19].y = outVerts[num19].y + (float)(order + 1) * 0.0001f;
            int num20 = 4;
            outVerts[num20].z = outVerts[num20].z + 0.5f;
            int num21 = 5;
            outVerts[num21].x = outVerts[num21].x - 0.3f;
            int num22 = 5;
            outVerts[num22].y = outVerts[num22].y - 0.5f;
            int num23 = 5;
            outVerts[num23].y = outVerts[num23].y + (float)(order + 1) * 0.0001f;
            int num24 = 5;
            outVerts[num24].z = outVerts[num24].z + 0.5f;
            int num25 = 6;
            outVerts[num25].x = outVerts[num25].x + 0.3f;
            int num26 = 6;
            outVerts[num26].y = outVerts[num26].y - 0.5f;
            int num27 = 6;
            outVerts[num27].y = outVerts[num27].y + (float)(order + 1) * 0.0001f;
            int num28 = 6;
            outVerts[num28].z = outVerts[num28].z + 0.5f;
            int num29 = 7;
            outVerts[num29].x = outVerts[num29].x + 0.5f;
            int num30 = 7;
            outVerts[num30].y = outVerts[num30].y - 0.5f;
            int num31 = 7;
            outVerts[num31].y = outVerts[num31].y + (float)(order + 1) * 0.0001f;
            int num32 = 7;
            outVerts[num32].z = outVerts[num32].z + 0.5f;
            int num33 = 8;
            outVerts[num33].x = outVerts[num33].x + 0.5f;
            int num34 = 8;
            outVerts[num34].y = outVerts[num34].y - 0.5f;
            int num35 = 8;
            outVerts[num35].y = outVerts[num35].y + (float)(order + 1) * 0.0001f;
            int num36 = 8;
            outVerts[num36].z = outVerts[num36].z + 0.3f;
            int num37 = 9;
            outVerts[num37].x = outVerts[num37].x + 0.5f;
            int num38 = 9;
            outVerts[num38].y = outVerts[num38].y - 0.5f;
            int num39 = 9;
            outVerts[num39].y = outVerts[num39].y + (float)(order + 1) * 0.0001f;
            int num40 = 9;
            outVerts[num40].z = outVerts[num40].z - 0.3f;
            int num41 = 10;
            outVerts[num41].x = outVerts[num41].x + 0.5f;
            int num42 = 10;
            outVerts[num42].y = outVerts[num42].y - 0.5f;
            int num43 = 10;
            outVerts[num43].y = outVerts[num43].y + (float)(order + 1) * 0.0001f;
            int num44 = 10;
            outVerts[num44].z = outVerts[num44].z - 0.5f;
            int num45 = 11;
            outVerts[num45].x = outVerts[num45].x + 0.3f;
            int num46 = 11;
            outVerts[num46].y = outVerts[num46].y - 0.5f;
            int num47 = 11;
            outVerts[num47].y = outVerts[num47].y + (float)(order + 1) * 0.0001f;
            int num48 = 11;
            outVerts[num48].z = outVerts[num48].z - 0.5f;
            int num49 = 12;
            outVerts[num49].x = outVerts[num49].x - 0.3f;
            int num50 = 12;
            outVerts[num50].y = outVerts[num50].y - 0.5f;
            int num51 = 12;
            outVerts[num51].y = outVerts[num51].y + (float)(order + 1) * 0.0001f;
            int num52 = 12;
            outVerts[num52].z = outVerts[num52].z - 0.5f;
            int num53 = 13;
            outVerts[num53].x = outVerts[num53].x - 0.5f;
            int num54 = 13;
            outVerts[num54].y = outVerts[num54].y - 0.5f;
            int num55 = 13;
            outVerts[num55].y = outVerts[num55].y + (float)(order + 1) * 0.0001f;
            int num56 = 13;
            outVerts[num56].z = outVerts[num56].z - 0.5f;
            int num57 = 14;
            outVerts[num57].x = outVerts[num57].x - 0.5f;
            int num58 = 14;
            outVerts[num58].y = outVerts[num58].y - 0.5f;
            int num59 = 14;
            outVerts[num59].y = outVerts[num59].y + (float)(order + 1) * 0.0001f;
            int num60 = 14;
            outVerts[num60].z = outVerts[num60].z - 0.3f;
            int num61 = 15;
            outVerts[num61].x = outVerts[num61].x - 0.5f;
            int num62 = 15;
            outVerts[num62].y = outVerts[num62].y - 0.5f;
            int num63 = 15;
            outVerts[num63].y = outVerts[num63].y + (float)(order + 1) * 0.0001f;
            int num64 = 15;
            outVerts[num64].z = outVerts[num64].z + 0.3f;
        }

        [CompilerGenerated]
        internal static Vector3<AddOrUpdate> g__TransformPoint|8_0(Vector3 p, ref CombinedTiledDecalsMeshes.<>c__DisplayClass8_0 A_1)
		{
			return A_1.extraTransform.MultiplyPoint3x4(p - A_1.extraTransformRelativeTo) + A_1.extraTransformRelativeTo;
		}

		private List<CombinedTiledDecalsMeshes.DecalsMesh> meshes = new List<CombinedTiledDecalsMeshes.DecalsMesh>();

        private const float DecalEps = 0.0001f;

        private const float TiledDecalInnerRadius = 0.3f;

        private static List<Vector2> tmpTexCoords = new List<Vector2>(16);

        private static Vector3[] tmpVerts = new Vector3[16];

        private struct DecalsMesh
    {
        public TiledDecalsSpec spec;

        public List<Vector3> verts;

        public List<Vector2> uv;

        public List<Vector3> normals;

        public List<Color> colors;

        public List<int> tris;

        public List<ValueTuple<Vector3Int, Vector3Int>> cells;

        public HashSet<ValueTuple<Vector3Int, Vector3Int>> cellsHashSet;

        public Mesh mesh;

        public bool meshDirty;

        public bool meshDirtyPositionsOnly;
    }
}
}