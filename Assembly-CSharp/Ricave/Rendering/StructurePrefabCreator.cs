using System;
using System.Collections.Generic;
using Ricave.Core;
using UnityEngine;

namespace Ricave.Rendering
{
    public class StructurePrefabCreator
    {
        public GameObject CreatePrefab(string debugNameSuffix)
        {
            bool activeSelf = StructurePrefabCreator.StructureBasePrefab.activeSelf;
            StructurePrefabCreator.StructureBasePrefab.SetActive(false);
            GameObject gameObject2;
            try
            {
                GameObject gameObject = Object.Instantiate<GameObject>(StructurePrefabCreator.StructureBasePrefab);
                gameObject.name = "Prefab_" + debugNameSuffix;
                this.AddParts(gameObject);
                Object.DontDestroyOnLoad(gameObject);
                gameObject2 = gameObject;
            }
            finally
            {
                StructurePrefabCreator.StructureBasePrefab.SetActive(activeSelf);
            }
            return gameObject2;
        }

        private void AddParts(GameObject prefab)
        {
            foreach (StructurePrefabCreator.Part part in this.parts)
            {
                part.Instantiate().transform.SetParent(prefab.transform, false);
            }
        }

        [Saved(Default.New, false)]
        private List<StructurePrefabCreator.Part> parts = new List<StructurePrefabCreator.Part>();

        private static readonly GameObject StructureBasePrefab = Assets.Get<GameObject>("Prefabs/Structures/StructureBase");

        private class Part
        {
            public GameObject Instantiate()
            {
                GameObject gameObject = new GameObject();
                this.SetName(gameObject);
                gameObject.transform.localPosition = this.offset;
                gameObject.transform.localRotation = Quaternion.Euler(this.rotation.x, this.rotation.y, this.rotation.z);
                gameObject.transform.localScale = this.scale;
                this.AddMesh(gameObject);
                this.AddMeshRenderer(gameObject);
                this.AddCollider(gameObject);
                return gameObject;
            }

            private void SetName(GameObject obj)
            {
                if (this.mesh == null)
                {
                    if (this.meshPrimitive != null)
                    {
                        obj.name = this.meshPrimitive.Value.ToString();
                    }
                    return;
                }
                int num = this.mesh.LastIndexOf('/');
                if (num >= 0 && num != this.mesh.Length - 1)
                {
                    obj.name = this.mesh.Substring(num + 1);
                    return;
                }
                obj.name = this.mesh;
            }

            private void AddMesh(GameObject obj)
            {
                if (!this.mesh.NullOrEmpty())
                {
                    Mesh mesh = Assets.Get<Mesh>(this.mesh);
                    if (mesh != null)
                    {
                        obj.AddComponent<MeshFilter>().sharedMesh = mesh;
                        return;
                    }
                }
                else if (this.meshPrimitive != null)
                {
                    obj.AddComponent<MeshFilter>().sharedMesh = PrimitiveMeshes.Get(this.meshPrimitive.Value);
                }
            }

            private void AddMeshRenderer(GameObject obj)
            {
                MeshFilter meshFilter;
                if (!obj.TryGetComponent<MeshFilter>(out meshFilter) || meshFilter.sharedMesh == null)
                {
                    return;
                }
                MeshRenderer meshRenderer = obj.AddComponent<MeshRenderer>();
                if (!this.material.NullOrEmpty())
                {
                    Material material = Assets.Get<Material>(this.material);
                    if (material != null)
                    {
                        meshRenderer.sharedMaterial = material;
                    }
                }
                else if (this.materialFromColor != null)
                {
                    meshRenderer.sharedMaterial = ColoredUniversalTextureMaterials.Get(this.materialFromColor.Value);
                }
                else if (this.materialFromTexture != null)
                {
                    Texture2D texture2D = Assets.Get<Texture2D>(this.materialFromTexture);
                    if (texture2D != null)
                    {
                        meshRenderer.sharedMaterial = MaterialsFromTexture.Get(texture2D);
                    }
                }
                if (meshRenderer.sharedMaterial == null)
                {
                    meshRenderer.sharedMaterial = ColoredUniversalTextureMaterials.Get(Color.black);
                }
            }

            private void AddCollider(GameObject obj)
            {
                if (this.collider == StructurePrefabCreator.Part.ColliderType.None)
                {
                    return;
                }
                MeshFilter component = obj.GetComponent<MeshFilter>();
                Mesh mesh = ((component != null) ? component.sharedMesh : null);
                Collider collider = null;
                if (this.collider == StructurePrefabCreator.Part.ColliderType.BoundingBox)
                {
                    if (mesh != null)
                    {
                        BoxCollider boxCollider = obj.AddComponent<BoxCollider>();
                        boxCollider.center = mesh.bounds.center;
                        boxCollider.size = mesh.bounds.size;
                        collider = boxCollider;
                    }
                }
                else if (this.collider == StructurePrefabCreator.Part.ColliderType.Box)
                {
                    BoxCollider boxCollider2 = obj.AddComponent<BoxCollider>();
                    boxCollider2.center = Vector3.zero;
                    boxCollider2.size = Vector3.one;
                    collider = boxCollider2;
                }
                else if (this.collider == StructurePrefabCreator.Part.ColliderType.BoundingSphere)
                {
                    if (mesh != null)
                    {
                        SphereCollider sphereCollider = obj.AddComponent<SphereCollider>();
                        Vector3 vector;
                        float num;
                        BoundingSphereCalculator.CalculateBoundingSphere(mesh, out vector, out num);
                        sphereCollider.center = vector;
                        sphereCollider.radius = num;
                        collider = sphereCollider;
                    }
                }
                else if (this.collider == StructurePrefabCreator.Part.ColliderType.Sphere)
                {
                    SphereCollider sphereCollider2 = obj.AddComponent<SphereCollider>();
                    sphereCollider2.center = Vector3.zero;
                    sphereCollider2.radius = 0.5f;
                    collider = sphereCollider2;
                }
                else if (this.collider == StructurePrefabCreator.Part.ColliderType.Mesh && mesh != null)
                {
                    MeshCollider meshCollider = obj.AddComponent<MeshCollider>();
                    meshCollider.sharedMesh = mesh;
                    if (this.colliderIsForRaycastsOnly)
                    {
                        meshCollider.convex = true;
                    }
                    collider = meshCollider;
                }
                if (collider != null && this.colliderIsForRaycastsOnly)
                {
                    collider.isTrigger = true;
                }
                if (this.colliderIsInspectModeOnly)
                {
                    obj.layer = Get.InspectModeOnlyLayer;
                }
            }

            [Saved]
            private Vector3 offset;

            [Saved]
            private Vector3 rotation;

            [Saved(Default.Vector3_One, false)]
            private Vector3 scale = Vector3.one;

            [Saved]
            private string mesh;

            [Saved]
            private PrimitiveType? meshPrimitive;

            [Saved]
            private string material;

            [Saved]
            private Color? materialFromColor;

            [Saved]
            private string materialFromTexture;

            [Saved(StructurePrefabCreator.Part.ColliderType.BoundingBox, false)]
            private StructurePrefabCreator.Part.ColliderType collider = StructurePrefabCreator.Part.ColliderType.BoundingBox;

            [Saved]
            private bool colliderIsInspectModeOnly;

            [Saved]
            private bool colliderIsForRaycastsOnly;

            private enum ColliderType
            {
                None,

                BoundingBox,

                BoundingSphere,

                Box,

                Sphere,

                Mesh
            }
        }
    }
}