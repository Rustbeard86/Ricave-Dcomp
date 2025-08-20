using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Rendering
{
    public static class MeshSlicer
    {
        private static void ClearWorkingLists()
        {
            MeshSlicer.first_tris.Clear();
            MeshSlicer.first_verts.Clear();
            MeshSlicer.second_tris.Clear();
            MeshSlicer.second_verts.Clear();
            MeshSlicer.first_uvs.Clear();
            MeshSlicer.second_uvs.Clear();
            MeshSlicer.first_normals.Clear();
            MeshSlicer.second_normals.Clear();
            MeshSlicer.planePoints.Clear();
        }

        public static void Slice(Mesh mesh, Plane plane, out Mesh slice1, out Mesh slice2)
        {
            MeshSlicer.ClearWorkingLists();
            MeshSlicer.CalculateSlices(mesh, plane);
            slice1 = MeshSlicer.GetSliceMesh(false);
            slice2 = MeshSlicer.GetSliceMesh(true);
        }

        private static void CalculateSlices(Mesh mesh, Plane plane)
        {
            Vector3[] vertices = mesh.vertices;
            Vector2[] uv = mesh.uv;
            Vector3[] normals = mesh.normals;
            int[] triangles = mesh.triangles;
            for (int i = 0; i < triangles.Length; i += 3)
            {
                Vector3 vector = vertices[triangles[i]];
                int num = triangles[i];
                Vector2 vector2 = uv[num];
                Vector3 vector3 = normals[num];
                bool side = plane.GetSide(vector);
                Vector3 vector4 = vertices[triangles[i + 1]];
                int num2 = triangles[i + 1];
                Vector2 vector5 = uv[num2];
                Vector3 vector6 = normals[num2];
                bool side2 = plane.GetSide(vector4);
                Vector3 vector7 = vertices[triangles[i + 2]];
                int num3 = triangles[i + 2];
                Vector2 vector8 = uv[num3];
                Vector3 vector9 = normals[num3];
                bool side3 = plane.GetSide(vector7);
                if (side == side2 && side2 == side3)
                {
                    MeshSlicer.AddTriangle(!side, vector, new Vector3?(vector3), vector2, vector4, new Vector3?(vector6), vector5, vector7, new Vector3?(vector9), vector8, false);
                }
                else
                {
                    bool flag = !side;
                    bool flag2 = side;
                    Vector3 vector10;
                    Vector3 vector12;
                    if (side == side2)
                    {
                        Vector2 vector11;
                        vector10 = MeshSlicer.GetRayPlaneIntersection(plane, vector4, vector5, vector7, vector8, out vector11);
                        Vector2 vector13;
                        vector12 = MeshSlicer.GetRayPlaneIntersection(plane, vector7, vector8, vector, vector2, out vector13);
                        MeshSlicer.AddTriangle(flag, vector, null, vector2, vector4, null, vector5, vector10, null, vector11, false);
                        MeshSlicer.AddTriangle(flag, vector, null, vector2, vector10, null, vector11, vector12, null, vector13, false);
                        MeshSlicer.AddTriangle(flag2, vector10, null, vector11, vector7, null, vector8, vector12, null, vector13, false);
                    }
                    else if (side == side3)
                    {
                        Vector2 vector14;
                        vector10 = MeshSlicer.GetRayPlaneIntersection(plane, vector, vector2, vector4, vector5, out vector14);
                        Vector2 vector15;
                        vector12 = MeshSlicer.GetRayPlaneIntersection(plane, vector4, vector5, vector7, vector8, out vector15);
                        MeshSlicer.AddTriangle(flag, vector, null, vector2, vector10, null, vector14, vector7, null, vector8, false);
                        MeshSlicer.AddTriangle(flag, vector10, null, vector14, vector12, null, vector15, vector7, null, vector8, false);
                        MeshSlicer.AddTriangle(flag2, vector10, null, vector14, vector4, null, vector5, vector12, null, vector15, false);
                    }
                    else
                    {
                        Vector2 vector16;
                        vector10 = MeshSlicer.GetRayPlaneIntersection(plane, vector, vector2, vector4, vector5, out vector16);
                        Vector2 vector17;
                        vector12 = MeshSlicer.GetRayPlaneIntersection(plane, vector7, vector8, vector, vector2, out vector17);
                        MeshSlicer.AddTriangle(flag, vector, null, vector2, vector10, null, vector16, vector12, null, vector17, false);
                        MeshSlicer.AddTriangle(flag2, vector10, null, vector16, vector4, null, vector5, vector7, null, vector8, false);
                        MeshSlicer.AddTriangle(flag2, vector10, null, vector16, vector7, null, vector8, vector12, null, vector17, false);
                    }
                    MeshSlicer.planePoints.Add(vector10);
                    MeshSlicer.planePoints.Add(vector12);
                }
            }
            MeshSlicer.CloseShape(plane);
        }

        private static void AddTriangle(bool negative, Vector3 vertex1, Vector3? normal1, Vector2 uv1, Vector3 vertex2, Vector3? normal2, Vector2 uv2, Vector3 vertex3, Vector3? normal3, Vector2 uv3, bool addFirst)
        {
            if (!negative)
            {
                MeshSlicer.AddTriangle(MeshSlicer.first_verts, MeshSlicer.first_tris, MeshSlicer.first_normals, MeshSlicer.first_uvs, vertex1, normal1, uv1, vertex2, normal2, uv2, vertex3, normal3, uv3, addFirst);
                return;
            }
            MeshSlicer.AddTriangle(MeshSlicer.second_verts, MeshSlicer.second_tris, MeshSlicer.second_normals, MeshSlicer.second_uvs, vertex1, normal1, uv1, vertex2, normal2, uv2, vertex3, normal3, uv3, addFirst);
        }

        private static void AddTriangle(List<Vector3> vertices, List<int> triangles, List<Vector3> normals, List<Vector2> uvs, Vector3 vertex1, Vector3? normal1, Vector2 uv1, Vector3 vertex2, Vector3? normal2, Vector2 uv2, Vector3 vertex3, Vector3? normal3, Vector2 uv3, bool insertAsFirst)
        {
            if (insertAsFirst)
            {
                MeshSlicer.ShiftIndicesOneRight(triangles);
            }
            if (normal1 == null)
            {
                normal1 = new Vector3?(MeshSlicer.CalculateNormal(vertex1, vertex2, vertex3));
            }
            int? num = (insertAsFirst ? new int?(0) : null);
            MeshSlicer.InsertVertex(vertices, normals, uvs, triangles, vertex1, normal1.Value, uv1, num);
            if (normal2 == null)
            {
                normal2 = new Vector3?(MeshSlicer.CalculateNormal(vertex2, vertex3, vertex1));
            }
            int? num2 = (insertAsFirst ? new int?(1) : null);
            MeshSlicer.InsertVertex(vertices, normals, uvs, triangles, vertex2, normal2.Value, uv2, num2);
            if (normal3 == null)
            {
                normal3 = new Vector3?(MeshSlicer.CalculateNormal(vertex3, vertex1, vertex2));
            }
            int? num3 = (insertAsFirst ? new int?(2) : null);
            MeshSlicer.InsertVertex(vertices, normals, uvs, triangles, vertex3, normal3.Value, uv3, num3);
        }

        private static void InsertVertex(List<Vector3> vertices, List<Vector3> normals, List<Vector2> uvs, List<int> triangles, Vector3 vertex, Vector3 normal, Vector2 uv, int? index)
        {
            if (index != null)
            {
                int value = index.Value;
                vertices.Insert(value, vertex);
                uvs.Insert(value, uv);
                normals.Insert(value, normal);
                triangles.Insert(value, value);
                return;
            }
            vertices.Add(vertex);
            normals.Add(normal);
            uvs.Add(uv);
            triangles.Add(vertices.Count - 1);
        }

        private static void ShiftIndicesOneRight(List<int> tris)
        {
            for (int i = 0; i < tris.Count; i += 3)
            {
                int num = i;
                tris[num] += 3;
                num = i + 1;
                tris[num] += 3;
                num = i + 2;
                tris[num] += 3;
            }
        }

        private static void CloseShape(Plane plane)
        {
            float num;
            Vector3 midPoint = MeshSlicer.GetMidPoint(out num);
            for (int i = 0; i < MeshSlicer.planePoints.Count; i += 2)
            {
                Vector3 vector = MeshSlicer.planePoints[i];
                Vector3 vector2 = MeshSlicer.planePoints[i + 1];
                Vector3 vector3 = MeshSlicer.CalculateNormal(midPoint, vector2, vector);
                if (Vector3.Dot(vector3, plane.normal) > 0f)
                {
                    MeshSlicer.AddTriangle(false, midPoint, new Vector3?(-vector3), Vector2.zero, vector, new Vector3?(-vector3), Vector2.zero, vector2, new Vector3?(-vector3), Vector2.zero, true);
                    MeshSlicer.AddTriangle(true, midPoint, new Vector3?(vector3), Vector2.zero, vector2, new Vector3?(vector3), Vector2.zero, vector, new Vector3?(vector3), Vector2.zero, true);
                }
                else
                {
                    MeshSlicer.AddTriangle(false, midPoint, new Vector3?(vector3), Vector2.zero, vector2, new Vector3?(vector3), Vector2.zero, vector, new Vector3?(vector3), Vector2.zero, true);
                    MeshSlicer.AddTriangle(true, midPoint, new Vector3?(-vector3), Vector2.zero, vector, new Vector3?(-vector3), Vector2.zero, vector2, new Vector3?(-vector3), Vector2.zero, true);
                }
            }
        }

        private static Vector3 GetMidPoint(out float distance)
        {
            if (MeshSlicer.planePoints.Count == 0)
            {
                distance = 0f;
                return Vector3.zero;
            }
            Vector3 vector = MeshSlicer.planePoints[0];
            Vector3 vector2 = Vector3.zero;
            distance = 0f;
            for (int i = 0; i < MeshSlicer.planePoints.Count; i++)
            {
                Vector3 vector3 = MeshSlicer.planePoints[i];
                float num = Vector3.Distance(vector, vector3);
                if (num > distance)
                {
                    distance = num;
                    vector2 = vector3;
                }
            }
            return (vector + vector2) / 2f;
        }

        private static Mesh GetSliceMesh(bool negative)
        {
            Mesh mesh = new Mesh();
            if (!negative)
            {
                mesh.SetVertices(MeshSlicer.first_verts);
                mesh.SetTriangles(MeshSlicer.first_tris, 0);
                mesh.SetNormals(MeshSlicer.first_normals);
                mesh.SetUVs(0, MeshSlicer.first_uvs);
            }
            else
            {
                mesh.SetVertices(MeshSlicer.second_verts);
                mesh.SetTriangles(MeshSlicer.second_tris, 0);
                mesh.SetNormals(MeshSlicer.second_normals);
                mesh.SetUVs(0, MeshSlicer.second_uvs);
            }
            mesh.Optimize();
            return mesh;
        }

        private static Vector3 CalculateNormal(Vector3 vertex1, Vector3 vertex2, Vector3 vertex3)
        {
            return Vector3.Cross(vertex2 - vertex1, vertex3 - vertex1).normalized;
        }

        private static Vector3 GetRayPlaneIntersection(Plane plane, Vector3 vertex1, Vector2 vertex1Uv, Vector3 vertex2, Vector2 vertex2Uv, out Vector2 uv)
        {
            Ray ray = new Ray(vertex1, (vertex2 - vertex1).normalized);
            float num;
            plane.Raycast(ray, out num);
            Vector3 point = ray.GetPoint(num);
            uv = Vector2.Lerp(vertex1Uv, vertex2Uv, num / (vertex1 - vertex2).magnitude);
            return point;
        }

        private static List<Vector3> first_verts = new List<Vector3>();

        private static List<Vector2> first_uvs = new List<Vector2>();

        private static List<Vector3> first_normals = new List<Vector3>();

        private static List<int> first_tris = new List<int>();

        private static List<Vector3> second_verts = new List<Vector3>();

        private static List<Vector2> second_uvs = new List<Vector2>();

        private static List<Vector3> second_normals = new List<Vector3>();

        private static List<int> second_tris = new List<int>();

        private static List<Vector3> planePoints = new List<Vector3>();
    }
}