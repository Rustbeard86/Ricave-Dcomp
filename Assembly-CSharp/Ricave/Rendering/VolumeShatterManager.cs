using System;
using System.Collections.Generic;
using Ricave.Core;
using UnityEngine;

namespace Ricave.Rendering
{
    public class VolumeShatterManager
    {
        public VolumeShatterManager()
        {
            List<GameObject> list = new List<GameObject>();
            for (int i = 0; i < 100; i++)
            {
                GameObject newGameObject = this.GetNewGameObject();
                newGameObject.SetActive(false);
                list.Add(newGameObject);
            }
            this.gameObjectsPool.AddRange(list);
        }

        public void Shatter(EntityGOC entityGOC, Vector3Int? impactSource, float impactMultiplier = 1f)
        {
            if (entityGOC == null || !entityGOC.gameObject.activeInHierarchy)
            {
                return;
            }
            Transform transform = entityGOC.transform;
            Vector3 impactDir = this.GetImpactDir(entityGOC, impactSource);
            ActorGOC actorGOC = entityGOC as ActorGOC;
            if (actorGOC != null)
            {
                actorGOC.OnPreShatter();
            }
            bool flag = entityGOC.Entity is Structure && (entityGOC.Entity.Spec.Structure.IsFilled || entityGOC.Entity.Spec == Get.Entity_Door);
            bool flag2 = !flag && !(entityGOC.Entity is Actor);
            Structure structure = entityGOC.Entity as Structure;
            bool flag3 = structure == null || !structure.Spec.Structure.HideAdjacentFacesUnfoggedOrNotFilled;
            this.Shatter(transform.position, entityGOC.MeshRenderers, impactDir, flag, flag2, flag3, impactMultiplier, false, (actorGOC != null) ? new Color?(actorGOC.SplatColor) : null);
        }

        public void Shatter(BodyPart bodyPart, Vector3Int? impactSource, float impactMultiplier = 1f)
        {
            ActorGOC actorGOC = bodyPart.Actor.ActorGOC;
            if (actorGOC == null || !actorGOC.gameObject.activeInHierarchy)
            {
                return;
            }
            Transform transform = actorGOC.transform;
            Vector3 impactDir = this.GetImpactDir(actorGOC, impactSource);
            actorGOC.OnPreShatter();
            this.tmpBodyPartMeshRenderers.Clear();
            List<ValueTuple<MeshRenderer, int, Color>> allBodyRenderers = actorGOC.AllBodyRenderers;
            for (int i = 0; i < allBodyRenderers.Count; i++)
            {
                if (allBodyRenderers[i].Item2 == bodyPart.PlacementIndex + 1)
                {
                    this.tmpBodyPartMeshRenderers.Add(allBodyRenderers[i].Item1);
                }
            }
            this.Shatter(transform.position, this.tmpBodyPartMeshRenderers, impactDir, false, false, true, impactMultiplier, true, new Color?(actorGOC.SplatColor));
            this.tmpBodyPartMeshRenderers.Clear();
        }

        public void Shatter(Vector3 pos, List<MeshRenderer> meshRenderers, Vector3 impactDir, bool applySameForceToAllPieces, bool explodeMoreInPlace, bool allowQuads, float impactMultiplier = 1f, bool ignoreActiveCheck = false, Color? splatColor = null)
        {
            for (int i = 0; i < meshRenderers.Count; i++)
            {
                MeshRenderer meshRenderer = meshRenderers[i];
                MeshFilter meshFilter;
                if ((meshRenderer.gameObject.activeInHierarchy || ignoreActiveCheck) && !(meshRenderer.sharedMaterial == null) && meshRenderer.gameObject.TryGetComponent<MeshFilter>(out meshFilter) && !(meshFilter.sharedMesh == null))
                {
                    Mesh mesh = meshFilter.sharedMesh;
                    if (mesh == PrimitiveMeshes.Get(PrimitiveType.Quad))
                    {
                        if (!allowQuads)
                        {
                            goto IL_00E1;
                        }
                        mesh = VolumeShatterManager.DoubleSidedQuad;
                    }
                    else if (mesh == PrimitiveMeshes.CubeXZ)
                    {
                        mesh = PrimitiveMeshes.Cube;
                    }
                    else if (mesh == PrimitiveMeshes.CubeXZ_WrapXZ)
                    {
                        mesh = PrimitiveMeshes.Cube_WrapXZ;
                    }
                    else if (mesh == PrimitiveMeshes.CubeXZ_WrapXZMirror)
                    {
                        mesh = PrimitiveMeshes.Cube_WrapXZMirror;
                    }
                    else if (PrimitiveMeshes.IsCubeUpDown(mesh))
                    {
                        goto IL_00E1;
                    }
                    this.ShatterPart(pos, mesh, meshRenderer.sharedMaterial, meshRenderer.gameObject.transform, impactDir, applySameForceToAllPieces, explodeMoreInPlace, impactMultiplier, splatColor);
                }
            IL_00E1:;
            }
        }

        private void ShatterPart(Vector3 pos, Mesh mesh, Material material, Transform origTransform, Vector3 impactDir, bool applySameForceToAllPieces, bool explodeMoreInPlace, float impactMultiplier = 1f, Color? splatColor = null)
        {
            Vector3 vector;
            if (explodeMoreInPlace)
            {
                vector = pos - impactDir * 0.2f;
                vector = vector.WithAddedY(-0.35f);
            }
            else if (applySameForceToAllPieces)
            {
                vector = pos - impactDir * 0.6f;
            }
            else
            {
                vector = pos - impactDir * 0.35f;
            }
            List<Mesh> slicedMesh = this.GetSlicedMesh(mesh);
            for (int i = 0; i < slicedMesh.Count; i++)
            {
                GameObject newGameObject = this.GetNewGameObject();
                newGameObject.transform.position = origTransform.position;
                newGameObject.transform.localScale = origTransform.lossyScale * 0.9f;
                newGameObject.transform.rotation = origTransform.rotation;
                newGameObject.GetComponent<MeshFilter>().sharedMesh = slicedMesh[i];
                newGameObject.GetComponent<MeshRenderer>().sharedMaterial = material;
                Rigidbody component = newGameObject.GetComponent<Rigidbody>();
                component.interpolation = this.GetDesiredInterpolation();
                BoxCollider component2 = newGameObject.GetComponent<BoxCollider>();
                component2.center = slicedMesh[i].bounds.center;
                component2.size = slicedMesh[i].bounds.size;
                if (impactDir != default(Vector3))
                {
                    Vector3 vector2;
                    if (applySameForceToAllPieces)
                    {
                        vector2 = (pos - vector).normalized;
                    }
                    else
                    {
                        vector2 = (newGameObject.transform.TransformPoint(slicedMesh[i].bounds.center) - vector).normalized;
                    }
                    component.AddForce(vector2 * 7f * impactMultiplier, ForceMode.Impulse);
                    if (!applySameForceToAllPieces)
                    {
                        component.AddTorque(Rand.Range(-0.1f, 0.1f), Rand.Range(-0.1f, 0.1f), Rand.Range(-0.1f, 0.1f), ForceMode.Impulse);
                    }
                }
                if (splatColor != null)
                {
                    ShatterPieceGOC component3 = newGameObject.GetComponent<ShatterPieceGOC>();
                    if (component3 != null)
                    {
                        component3.InitSplatColor(splatColor.Value);
                    }
                }
                this.blocks.Add(new VolumeShatterManager.Block
                {
                    gameObject = newGameObject,
                    rigidbodyComponent = component,
                    originalScale = newGameObject.transform.localScale,
                    startFadeOutAt = Clock.Time + VolumeShatterManager.StartFadeOutAfter.RandomInRange
                });
            }
        }

        public void Update()
        {
            for (int i = this.blocks.Count - 1; i >= 0; i--)
            {
                if (Clock.Time >= this.blocks[i].startFadeOutAt + 0.5f)
                {
                    this.RemovePiece(i);
                }
                else
                {
                    float num = Math.Min(1f - (Clock.Time - this.blocks[i].startFadeOutAt) / 0.5f, 1f);
                    if (num != 1f)
                    {
                        this.blocks[i].gameObject.transform.localScale = this.blocks[i].originalScale * num;
                    }
                }
            }
            this.CheckSetNewInterpolation();
        }

        public void OnTimeScaleChangedDrastically()
        {
            this.CheckSetNewInterpolation();
        }

        private void CheckSetNewInterpolation()
        {
            RigidbodyInterpolation desiredInterpolation = this.GetDesiredInterpolation();
            if (this.setInterpolation != desiredInterpolation)
            {
                this.setInterpolation = desiredInterpolation;
                for (int i = 0; i < this.blocks.Count; i++)
                {
                    this.blocks[i].rigidbodyComponent.interpolation = desiredInterpolation;
                }
            }
        }

        private Vector3 GetImpactDir(EntityGOC entityGOC, Vector3Int? impactSource)
        {
            Transform transform = entityGOC.transform;
            if (impactSource == null)
            {
                return default(Vector3);
            }
            Vector3Int? vector3Int = impactSource;
            Vector3? vector = ((vector3Int != null) ? new Vector3?(vector3Int.GetValueOrDefault()) : null);
            Vector3 position = transform.position;
            if (vector != null && (vector == null || vector.GetValueOrDefault() == position))
            {
                return Vector3.forward;
            }
            return (transform.position - impactSource.Value).normalized;
        }

        public void RemoveAll()
        {
            for (int i = this.blocks.Count - 1; i >= 0; i--)
            {
                this.RemovePiece(i);
            }
        }

        private void RemovePiece(int index)
        {
            this.ReturnGameObject(this.blocks[index].gameObject);
            this.blocks.RemoveAt(index);
        }

        private GameObject GetNewGameObject()
        {
            GameObject gameObject;
            if (this.gameObjectsPool.Count != 0)
            {
                gameObject = this.gameObjectsPool[this.gameObjectsPool.Count - 1];
                this.gameObjectsPool.RemoveAt(this.gameObjectsPool.Count - 1);
                gameObject.SetActive(true);
            }
            else
            {
                gameObject = Object.Instantiate<GameObject>(VolumeShatterManager.ShatterBlockPrefab, Get.RuntimeSpecialContainer.transform);
            }
            return gameObject;
        }

        private void ReturnGameObject(GameObject gameObject)
        {
            gameObject.SetActive(false);
            gameObject.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.None;
            ShatterPieceGOC component = gameObject.GetComponent<ShatterPieceGOC>();
            if (component != null)
            {
                component.OnReturnedToPool();
            }
            this.gameObjectsPool.Add(gameObject);
        }

        private List<Mesh> GetSlicedMesh(Mesh mesh)
        {
            List<Mesh> list;
            if (this.cachedSlicedMeshes.TryGetValue(mesh, out list))
            {
                return list;
            }
            Mesh mesh2;
            Mesh mesh3;
            MeshSlicer.Slice(mesh, new Plane(Vector3.up, mesh.bounds.center), out mesh2, out mesh3);
            Mesh mesh4;
            Mesh mesh5;
            MeshSlicer.Slice(mesh2, new Plane(Vector3.right, mesh.bounds.center), out mesh4, out mesh5);
            Mesh mesh6;
            Mesh mesh7;
            MeshSlicer.Slice(mesh3, new Plane(Vector3.right, mesh.bounds.center), out mesh6, out mesh7);
            Object.Destroy(mesh2);
            Object.Destroy(mesh3);
            list = new List<Mesh> { mesh4, mesh5, mesh6, mesh7 };
            this.cachedSlicedMeshes.Add(mesh, list);
            return list;
        }

        private RigidbodyInterpolation GetDesiredInterpolation()
        {
            if (Clock.TimeScale < 0.95f)
            {
                return RigidbodyInterpolation.Interpolate;
            }
            return RigidbodyInterpolation.None;
        }

        private List<VolumeShatterManager.Block> blocks = new List<VolumeShatterManager.Block>();

        private List<GameObject> gameObjectsPool = new List<GameObject>();

        private Dictionary<Mesh, List<Mesh>> cachedSlicedMeshes = new Dictionary<Mesh, List<Mesh>>();

        private RigidbodyInterpolation setInterpolation;

        private List<MeshRenderer> tmpBodyPartMeshRenderers = new List<MeshRenderer>();

        private static readonly FloatRange StartFadeOutAfter = new FloatRange(0.3f, 0.6f);

        private const float FadeOutDuration = 0.5f;

        private const int InitialPoolSize = 100;

        private static readonly GameObject ShatterBlockPrefab = Assets.Get<GameObject>("Prefabs/Misc/ShatterBlock");

        private static readonly Mesh DoubleSidedQuad = Assets.Get<Mesh>("Models/DoubleSidedQuad");

        private struct Block
        {
            public GameObject gameObject;

            public Rigidbody rigidbodyComponent;

            public Vector3 originalScale;

            public float startFadeOutAt;
        }
    }
}