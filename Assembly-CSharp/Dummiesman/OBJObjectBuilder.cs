using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Dummiesman
{
    public class OBJObjectBuilder
    {
        public int PushedFaceCount { get; private set; }

        public GameObject Build()
        {
            GameObject gameObject = new GameObject(this._name);
            MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
            int num = 0;
            Material[] array = new Material[this._materialIndices.Count];
            foreach (KeyValuePair<string, List<int>> keyValuePair in this._materialIndices)
            {
                Material material = null;
                if (this._loader.Materials == null)
                {
                    material = OBJLoaderHelper.CreateNullMaterial();
                    material.name = keyValuePair.Key;
                }
                else if (!this._loader.Materials.TryGetValue(keyValuePair.Key, out material))
                {
                    material = OBJLoaderHelper.CreateNullMaterial();
                    material.name = keyValuePair.Key;
                    this._loader.Materials[keyValuePair.Key] = material;
                }
                array[num] = material;
                num++;
            }
            meshRenderer.sharedMaterials = array;
            MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
            num = 0;
            Mesh mesh = new Mesh
            {
                name = this._name,
                indexFormat = ((this._vertices.Count > 65535) ? IndexFormat.UInt32 : IndexFormat.UInt16),
                subMeshCount = this._materialIndices.Count
            };
            mesh.SetVertices(this._vertices);
            mesh.SetNormals(this._normals);
            mesh.SetUVs(0, this._uvs);
            foreach (KeyValuePair<string, List<int>> keyValuePair2 in this._materialIndices)
            {
                mesh.SetTriangles(keyValuePair2.Value, num);
                num++;
            }
            if (this.recalculateNormals)
            {
                mesh.RecalculateNormals();
            }
            mesh.RecalculateTangents();
            mesh.RecalculateBounds();
            meshFilter.sharedMesh = mesh;
            return gameObject;
        }

        public void SetMaterial(string name)
        {
            if (!this._materialIndices.TryGetValue(name, out this._currentIndexList))
            {
                this._currentIndexList = new List<int>();
                this._materialIndices[name] = this._currentIndexList;
            }
        }

        public void PushFace(string material, List<int> vertexIndices, List<int> normalIndices, List<int> uvIndices)
        {
            if (vertexIndices.Count < 3)
            {
                return;
            }
            if (material != this._lastMaterial)
            {
                this.SetMaterial(material);
                this._lastMaterial = material;
            }
            int[] array = new int[vertexIndices.Count];
            for (int i = 0; i < vertexIndices.Count; i++)
            {
                int num = vertexIndices[i];
                int num2 = normalIndices[i];
                int num3 = uvIndices[i];
                OBJObjectBuilder.ObjLoopHash objLoopHash = new OBJObjectBuilder.ObjLoopHash
                {
                    vertexIndex = num,
                    normalIndex = num2,
                    uvIndex = num3
                };
                int num4 = -1;
                if (!this._globalIndexRemap.TryGetValue(objLoopHash, out num4))
                {
                    this._globalIndexRemap.Add(objLoopHash, this._vertices.Count);
                    num4 = this._vertices.Count;
                    this._vertices.Add((num >= 0 && num < this._loader.Vertices.Count) ? this._loader.Vertices[num] : Vector3.zero);
                    this._normals.Add((num2 >= 0 && num2 < this._loader.Normals.Count) ? this._loader.Normals[num2] : Vector3.zero);
                    this._uvs.Add((num3 >= 0 && num3 < this._loader.UVs.Count) ? this._loader.UVs[num3] : Vector2.zero);
                    if (num2 < 0)
                    {
                        this.recalculateNormals = true;
                    }
                }
                array[i] = num4;
            }
            if (array.Length == 3)
            {
                this._currentIndexList.AddRange(new int[]
                {
                    array[0],
                    array[1],
                    array[2]
                });
            }
            else if (array.Length == 4)
            {
                this._currentIndexList.AddRange(new int[]
                {
                    array[0],
                    array[1],
                    array[2]
                });
                this._currentIndexList.AddRange(new int[]
                {
                    array[2],
                    array[3],
                    array[0]
                });
            }
            else if (array.Length > 4)
            {
                for (int j = array.Length - 1; j >= 2; j--)
                {
                    this._currentIndexList.AddRange(new int[]
                    {
                        array[0],
                        array[j - 1],
                        array[j]
                    });
                }
            }
            int pushedFaceCount = this.PushedFaceCount;
            this.PushedFaceCount = pushedFaceCount + 1;
        }

        public OBJObjectBuilder(string name, OBJLoader loader)
        {
            this._name = name;
            this._loader = loader;
        }

        private OBJLoader _loader;

        private string _name;

        private Dictionary<OBJObjectBuilder.ObjLoopHash, int> _globalIndexRemap = new Dictionary<OBJObjectBuilder.ObjLoopHash, int>();

        private Dictionary<string, List<int>> _materialIndices = new Dictionary<string, List<int>>();

        private List<int> _currentIndexList;

        private string _lastMaterial;

        private List<Vector3> _vertices = new List<Vector3>();

        private List<Vector3> _normals = new List<Vector3>();

        private List<Vector2> _uvs = new List<Vector2>();

        private bool recalculateNormals;

        private class ObjLoopHash
        {
            public override bool Equals(object obj)
            {
                if (!(obj is OBJObjectBuilder.ObjLoopHash))
                {
                    return false;
                }
                OBJObjectBuilder.ObjLoopHash objLoopHash = obj as OBJObjectBuilder.ObjLoopHash;
                return objLoopHash.vertexIndex == this.vertexIndex && objLoopHash.uvIndex == this.uvIndex && objLoopHash.normalIndex == this.normalIndex;
            }

            public override int GetHashCode()
            {
                return ((3 * 314159 + this.vertexIndex) * 314159 + this.normalIndex) * 314159 + this.uvIndex;
            }

            public int vertexIndex;

            public int normalIndex;

            public int uvIndex;
        }
    }
}