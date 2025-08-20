using System;
using System.Collections.Generic;
using Ricave.Core;
using UnityEngine;

namespace Ricave.Rendering
{
    public class ShatterManager
    {
        public ShatterManager()
        {
            List<GameObject> list = new List<GameObject>();
            for (int i = 0; i < 200; i++)
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
            List<MeshRenderer> meshRenderers = entityGOC.MeshRenderers;
            if (meshRenderers.Count == 0 || meshRenderers[0].sharedMaterial == null)
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
            this.Shatter(transform.position, transform.rotation, transform.lossyScale, meshRenderers[0].sharedMaterial, impactDir, impactMultiplier, null);
        }

        public void Shatter(BodyPart bodyPart, Vector3Int? impactSource, float impactMultiplier = 1f)
        {
            EntityGOC entityGOC = bodyPart.Actor.EntityGOC;
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
            Material material = Object.Instantiate<Material>(entityGOC.MeshRenderers[0].material);
            material.shader = ShatterManager.BodyPartFlyingShader;
            material.SetColor(Get.ShaderPropertyIDs.BodyPartBodyMapColorID, BodyPartUtility.GetBodyPartBodyMapColor(bodyPart));
            this.materialsToDestroyWhenFinished.Add(material);
            this.Shatter(transform.position, transform.rotation, transform.lossyScale, material, impactDir, impactMultiplier, ShatterMeshes.GetVisibilityMaskFor(bodyPart.Actor.Spec.Actor.BodyMap, BodyPartUtility.GetBodyPartBodyMapColor(bodyPart), new Rect?(bodyPart.Placement.OccupiedTextureRect)));
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

        public void Shatter(Vector3 pos, Quaternion rot, Vector3 scale, Material material, Vector3 impactDir, float impactMultiplier = 1f, bool[] pieceVisibilityMask = null)
        {
            Mesh[] shatterMeshes = ShatterMeshes.GetShatterMeshes();
            Vector3 vector = pos - impactDir * 0.35f;
            if (pieceVisibilityMask == null)
            {
                Texture2D texture2D = material.mainTexture as Texture2D;
                if (texture2D != null)
                {
                    pieceVisibilityMask = ShatterMeshes.GetVisibilityMaskFor(texture2D, null);
                }
            }
            for (int i = 0; i < shatterMeshes.Length; i++)
            {
                if (pieceVisibilityMask == null || pieceVisibilityMask[i])
                {
                    GameObject newGameObject = this.GetNewGameObject();
                    Rigidbody component = newGameObject.GetComponent<Rigidbody>();
                    newGameObject.transform.position = pos;
                    newGameObject.transform.localScale = scale;
                    newGameObject.transform.rotation = rot;
                    newGameObject.GetComponent<MeshFilter>().sharedMesh = shatterMeshes[i];
                    newGameObject.GetComponent<MeshRenderer>().sharedMaterial = material;
                    component.interpolation = this.GetDesiredInterpolation();
                    BoxCollider component2 = newGameObject.GetComponent<BoxCollider>();
                    component2.center = shatterMeshes[i].bounds.center;
                    component2.size = new Vector3(shatterMeshes[i].bounds.size.x, shatterMeshes[i].bounds.size.y, component2.size.z);
                    if (impactDir != default(Vector3))
                    {
                        Vector3 normalized = (newGameObject.transform.TransformPoint(shatterMeshes[i].bounds.center) - vector).normalized;
                        component.AddForce(normalized * 10f * impactMultiplier, ForceMode.Impulse);
                        component.AddTorque(Rand.Range(-0.1f, 0.1f), Rand.Range(-0.1f, 0.1f), Rand.Range(-0.1f, 0.1f), ForceMode.Impulse);
                    }
                    this.pieces.Add(new ShatterManager.Piece
                    {
                        gameObject = newGameObject,
                        rendererComponent = newGameObject.GetComponent<Renderer>(),
                        rigidbodyComponent = component,
                        startFadeOutAt = Clock.Time + ShatterManager.StartFadeOutAfter.RandomInRange,
                        propertyBlock = MaterialPropertyBlockPool.Get()
                    });
                }
            }
        }

        public void Update()
        {
            for (int i = this.pieces.Count - 1; i >= 0; i--)
            {
                if (Clock.Time >= this.pieces[i].startFadeOutAt + 0.5f)
                {
                    this.RemovePiece(i);
                }
                else
                {
                    float num = Math.Min(1f - (Clock.Time - this.pieces[i].startFadeOutAt) / 0.5f, 1f);
                    if (num != 1f)
                    {
                        Color color = new Color(1f, 1f, 1f, num);
                        if (this.pieces[i].rendererComponent.sharedMaterial != null)
                        {
                            color *= this.pieces[i].rendererComponent.sharedMaterial.color;
                        }
                        this.pieces[i].propertyBlock.SetColor(Get.ShaderPropertyIDs.ColorID, color);
                        this.pieces[i].rendererComponent.SetPropertyBlock(this.pieces[i].propertyBlock);
                    }
                }
            }
            if (this.pieces.Count == 0)
            {
                for (int j = 0; j < this.materialsToDestroyWhenFinished.Count; j++)
                {
                    Object.Destroy(this.materialsToDestroyWhenFinished[j]);
                }
                this.materialsToDestroyWhenFinished.Clear();
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
                for (int i = 0; i < this.pieces.Count; i++)
                {
                    this.pieces[i].rigidbodyComponent.interpolation = desiredInterpolation;
                }
            }
        }

        public void RemoveAll()
        {
            for (int i = this.pieces.Count - 1; i >= 0; i--)
            {
                this.RemovePiece(i);
            }
        }

        private void RemovePiece(int index)
        {
            this.ReturnGameObject(this.pieces[index].gameObject);
            MaterialPropertyBlockPool.Return(this.pieces[index].propertyBlock);
            this.pieces.RemoveAt(index);
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
                gameObject = Object.Instantiate<GameObject>(ShatterManager.ShatterPiecePrefab, Get.RuntimeSpecialContainer.transform);
            }
            return gameObject;
        }

        private void ReturnGameObject(GameObject gameObject)
        {
            gameObject.SetActive(false);
            gameObject.GetComponent<Renderer>().SetPropertyBlock(null);
            gameObject.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.None;
            ShatterPieceGOC component = gameObject.GetComponent<ShatterPieceGOC>();
            if (component != null)
            {
                component.OnReturnedToPool();
            }
            this.gameObjectsPool.Add(gameObject);
        }

        private RigidbodyInterpolation GetDesiredInterpolation()
        {
            if (Clock.TimeScale < 0.95f)
            {
                return RigidbodyInterpolation.Interpolate;
            }
            return RigidbodyInterpolation.None;
        }

        private List<ShatterManager.Piece> pieces = new List<ShatterManager.Piece>();

        private List<GameObject> gameObjectsPool = new List<GameObject>();

        private List<Material> materialsToDestroyWhenFinished = new List<Material>();

        private RigidbodyInterpolation setInterpolation;

        private static readonly FloatRange StartFadeOutAfter = new FloatRange(1f, 2f);

        private const float FadeOutDuration = 0.5f;

        private const int InitialPoolSize = 200;

        private static readonly GameObject ShatterPiecePrefab = Assets.Get<GameObject>("Prefabs/Misc/ShatterPiece");

        private static readonly Shader BodyPartFlyingShader = Assets.Get<Shader>("Shaders/Actors/BodyPartFlying");

        private struct Piece
        {
            public GameObject gameObject;

            public Renderer rendererComponent;

            public Rigidbody rigidbodyComponent;

            public float startFadeOutAt;

            public MaterialPropertyBlock propertyBlock;
        }
    }
}