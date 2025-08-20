using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

namespace Ricave.Rendering
{
    public static class CubesWithBevels
    {
        static CubesWithBevels()
        {
            CubesWithBevels.DummyMaterial.name = "CubesWithBevelsDummyMaterial";
        }

        public static Mesh Get(IList<bool> edgesToBevel, bool left, bool right, bool up, bool down, bool forward, bool back)
        {
            Mesh mesh;
            return CubesWithBevels.GetInternal(edgesToBevel, left, right, up, down, forward, back, false, out mesh);
        }

        public static Mesh GetSplit(IList<bool> edgesToBevel, bool left, bool right, bool up, bool down, bool forward, bool back, out Mesh upDown)
        {
            return CubesWithBevels.GetInternal(edgesToBevel, left, right, up, down, forward, back, true, out upDown);
        }

        private static Mesh GetInternal(IList<bool> edgesToBevel, bool left, bool right, bool up, bool down, bool forward, bool back, bool splitSidesAndUpDown, out Mesh upDown)
        {
            int num = CubesWithBevels.BevelsListToMask(edgesToBevel);
            int num2 = (left ? 1 : 0) + (right ? 1 : 0) * 2 + (up ? 1 : 0) * 4 + (down ? 1 : 0) * 8 + (forward ? 1 : 0) * 16 + (back ? 1 : 0) * 32;
            if (num == 0 && num2 == 63)
            {
                if (splitSidesAndUpDown)
                {
                    upDown = PrimitiveMeshes.CubeWithMissingFaces(false, false, true, true, false, false);
                    return PrimitiveMeshes.CubeWithMissingFaces(true, true, false, false, true, true);
                }
                upDown = null;
                return PrimitiveMeshes.Cube_WrapXZ;
            }
            else if (num == 0)
            {
                if (splitSidesAndUpDown)
                {
                    upDown = PrimitiveMeshes.CubeWithMissingFaces(false, false, up, down, false, false);
                    return PrimitiveMeshes.CubeWithMissingFaces(left, right, false, false, forward, back);
                }
                upDown = null;
                return PrimitiveMeshes.CubeWithMissingFaces(left, right, up, down, forward, back);
            }
            else
            {
                Mesh mesh;
                if (splitSidesAndUpDown)
                {
                    ValueTuple<Mesh, Mesh> valueTuple;
                    if (CubesWithBevels.meshesSplit.TryGetValue(new ValueTuple<int, int>(num, num2), out valueTuple))
                    {
                        upDown = valueTuple.Item2;
                        return valueTuple.Item1;
                    }
                }
                else if (CubesWithBevels.meshes.TryGetValue(new ValueTuple<int, int>(num, num2), out mesh))
                {
                    upDown = null;
                    return mesh;
                }
                ProBuilderMesh proBuilderMesh = ShapeGenerator.GenerateCube(PivotLocation.Center, Vector3.one);
                if (num2 != 63)
                {
                    CubesWithBevels.tmpFacesToRemove.Clear();
                    if (!left)
                    {
                        CubesWithBevels.tmpFacesToRemove.Add(proBuilderMesh.faces[3]);
                    }
                    if (!right)
                    {
                        CubesWithBevels.tmpFacesToRemove.Add(proBuilderMesh.faces[1]);
                    }
                    if (!up)
                    {
                        CubesWithBevels.tmpFacesToRemove.Add(proBuilderMesh.faces[4]);
                    }
                    if (!down)
                    {
                        CubesWithBevels.tmpFacesToRemove.Add(proBuilderMesh.faces[5]);
                    }
                    if (!forward)
                    {
                        CubesWithBevels.tmpFacesToRemove.Add(proBuilderMesh.faces[0]);
                    }
                    if (!back)
                    {
                        CubesWithBevels.tmpFacesToRemove.Add(proBuilderMesh.faces[2]);
                    }
                    for (int i = 0; i < CubesWithBevels.tmpFacesToRemove.Count; i++)
                    {
                        proBuilderMesh.DeleteFace(CubesWithBevels.tmpFacesToRemove[i]);
                    }
                    CubesWithBevels.tmpFacesToRemove.Clear();
                }
                CubesWithBevels.tmpEdgesSet.Clear();
                CubesWithBevels.tmpEdgesToBevel.Clear();
                int j = 0;
                int count = proBuilderMesh.faces.Count;
                while (j < count)
                {
                    Face face = proBuilderMesh.faces[j];
                    int k = 0;
                    int count2 = face.edges.Count;
                    while (k < count2)
                    {
                        Edge edge = face.edges[k];
                        if (CubesWithBevels.tmpEdgesSet.Add(edge))
                        {
                            Vector3 vector = (proBuilderMesh.positions[edge.a] + proBuilderMesh.positions[edge.b]) / 2f;
                            bool flag = false;
                            int num3 = 0;
                            if (vector.y < -0.25f)
                            {
                                num3 += 8;
                            }
                            else if (vector.y < 0.25f)
                            {
                                num3 += 4;
                                flag = true;
                            }
                            if (flag)
                            {
                                if (vector.x < 0f)
                                {
                                    if (vector.z >= 0f)
                                    {
                                        num3++;
                                    }
                                }
                                else if (vector.z > 0f)
                                {
                                    num3 += 2;
                                }
                                else
                                {
                                    num3 += 3;
                                }
                            }
                            else if (vector.x >= -0.25f)
                            {
                                if (vector.z > 0.25f)
                                {
                                    num3++;
                                }
                                else if (vector.x > 0.25f)
                                {
                                    num3 += 2;
                                }
                                else
                                {
                                    num3 += 3;
                                }
                            }
                            if (edgesToBevel[num3])
                            {
                                CubesWithBevels.tmpEdgesToBevel.Add(edge);
                            }
                        }
                        k++;
                    }
                    j++;
                }
                Bevel.BevelEdges(proBuilderMesh, CubesWithBevels.tmpEdgesToBevel, 0.25f);
                CubesWithBevels.tmpEdgesToBevel.Clear();
                if (!splitSidesAndUpDown)
                {
                    proBuilderMesh.ToMesh(MeshTopology.Triangles);
                    proBuilderMesh.Refresh(RefreshMask.All);
                    Mesh sharedMesh = proBuilderMesh.GetComponent<MeshFilter>().sharedMesh;
                    Object.Destroy(proBuilderMesh.gameObject);
                    sharedMesh.Optimize();
                    sharedMesh.name = "CubeWithBevels";
                    CubesWithBevels.meshes.Add(new ValueTuple<int, int>(num, num2), sharedMesh);
                    upDown = null;
                    return sharedMesh;
                }
                CubesWithBevels.tmpFacesToDetach.Clear();
                IList<Face> faces = proBuilderMesh.faces;
                for (int l = 0; l < faces.Count; l++)
                {
                    ReadOnlyCollection<int> indexes = faces[l].indexes;
                    if (indexes.Count != 0)
                    {
                        Vector3 vector2 = Vector3.zero;
                        for (int m = 0; m < indexes.Count; m++)
                        {
                            vector2 += proBuilderMesh.positions[indexes[m]];
                        }
                        vector2 /= (float)indexes.Count;
                        if (Math.Abs(vector2.x) < 0.35f && Math.Abs(vector2.z) < 0.35f && (vector2.y > 0.495f || vector2.y < -0.495f))
                        {
                            CubesWithBevels.tmpFacesToDetach.Add(faces[l]);
                            if (CubesWithBevels.tmpFacesToDetach.Count >= 2)
                            {
                                break;
                            }
                        }
                    }
                }
                if (CubesWithBevels.tmpFacesToDetach.Count != 0)
                {
                    List<Face> list = proBuilderMesh.DetachFaces(CubesWithBevels.tmpFacesToDetach);
                    proBuilderMesh.SetMaterial(list, CubesWithBevels.DummyMaterial);
                    CubesWithBevels.tmpFacesToDetach.Clear();
                    proBuilderMesh.ToMesh(MeshTopology.Triangles);
                    proBuilderMesh.Refresh(RefreshMask.All);
                    Mesh sharedMesh2 = proBuilderMesh.GetComponent<MeshFilter>().sharedMesh;
                    Object.Destroy(proBuilderMesh.gameObject);
                    Mesh mesh2 = Object.Instantiate<Mesh>(sharedMesh2);
                    mesh2.subMeshCount = 0;
                    mesh2.subMeshCount = 1;
                    mesh2.SetTriangles(sharedMesh2.GetTriangles(0), 0);
                    mesh2.Optimize();
                    mesh2.name = "CubeWithBevels (sides)";
                    upDown = Object.Instantiate<Mesh>(sharedMesh2);
                    upDown.subMeshCount = 0;
                    upDown.subMeshCount = 1;
                    upDown.SetTriangles(sharedMesh2.GetTriangles(1), 0);
                    upDown.Optimize();
                    upDown.name = "CubeWithBevels (up/down)";
                    Object.Destroy(sharedMesh2);
                    CubesWithBevels.meshesSplit.Add(new ValueTuple<int, int>(num, num2), new ValueTuple<Mesh, Mesh>(mesh2, upDown));
                    return mesh2;
                }
                proBuilderMesh.ToMesh(MeshTopology.Triangles);
                proBuilderMesh.Refresh(RefreshMask.All);
                Mesh sharedMesh3 = proBuilderMesh.GetComponent<MeshFilter>().sharedMesh;
                Object.Destroy(proBuilderMesh.gameObject);
                sharedMesh3.Optimize();
                sharedMesh3.name = "CubeWithBevels (sides)";
                upDown = new Mesh();
                upDown.name = "CubeWithBevels (up/down)";
                CubesWithBevels.meshesSplit.Add(new ValueTuple<int, int>(num, num2), new ValueTuple<Mesh, Mesh>(sharedMesh3, upDown));
                return sharedMesh3;
            }
        }

        public static int BevelsListToMask(IList<bool> bevels)
        {
            int num = 0;
            for (int i = 0; i < bevels.Count; i++)
            {
                if (bevels[i])
                {
                    num |= 1 << i;
                }
            }
            return num;
        }

        private static Dictionary<ValueTuple<int, int>, Mesh> meshes = new Dictionary<ValueTuple<int, int>, Mesh>(10);

        private static Dictionary<ValueTuple<int, int>, ValueTuple<Mesh, Mesh>> meshesSplit = new Dictionary<ValueTuple<int, int>, ValueTuple<Mesh, Mesh>>(10);

        private static List<Edge> tmpEdgesToBevel = new List<Edge>(50);

        private static HashSet<Edge> tmpEdgesSet = new HashSet<Edge>(50);

        private static List<Face> tmpFacesToRemove = new List<Face>(6);

        private static List<Face> tmpFacesToDetach = new List<Face>(2);

        private static readonly Material DummyMaterial = new Material(Shader.Find("Standard"));
    }
}