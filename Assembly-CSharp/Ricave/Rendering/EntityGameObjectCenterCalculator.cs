using System;
using System.Collections.Generic;
using Ricave.Core;
using UnityEngine;

namespace Ricave.Rendering
{
    public static class EntityGameObjectCenterCalculator
    {
        public static Bounds CalculateBoundingBox(EntityGOC entity)
        {
            ActorGOC actorGOC = entity as ActorGOC;
            if ((actorGOC != null && actorGOC.Actor != null && actorGOC.Actor.Spec.Actor.Texture != null) || entity is ItemGOC)
            {
                Bounds? bounds = null;
                List<Collider> list = entity.Colliders;
                if (list.Count == 0 && Get.GameObjectFader.IsFading(entity.gameObject))
                {
                    list = entity.CollidersIncludingInactive;
                    for (int i = 0; i < list.Count; i++)
                    {
                        BoxCollider boxCollider = list[i] as BoxCollider;
                        if (boxCollider != null)
                        {
                            Bounds bounds2 = EntityGameObjectCenterCalculator.CalculateBoxColliderBounds(boxCollider);
                            if (bounds == null)
                            {
                                bounds = new Bounds?(bounds2);
                            }
                            else
                            {
                                Bounds value = bounds.Value;
                                value.Encapsulate(bounds2);
                                bounds = new Bounds?(value);
                            }
                        }
                    }
                }
                else
                {
                    for (int j = 0; j < list.Count; j++)
                    {
                        Collider collider = list[j];
                        BoxCollider boxCollider2 = collider as BoxCollider;
                        Bounds bounds3;
                        if (boxCollider2 != null)
                        {
                            bounds3 = EntityGameObjectCenterCalculator.CalculateBoxColliderBounds(boxCollider2);
                        }
                        else
                        {
                            bounds3 = collider.bounds;
                        }
                        if (bounds == null)
                        {
                            bounds = new Bounds?(bounds3);
                        }
                        else
                        {
                            Bounds value2 = bounds.Value;
                            value2.Encapsulate(bounds3);
                            bounds = new Bounds?(value2);
                        }
                    }
                }
                if (bounds == null)
                {
                    return new Bounds(entity.transform.position, Vector3.zero);
                }
                return bounds.Value;
            }
            else
            {
                Bounds? bounds4 = null;
                List<MeshRenderer> meshRenderersIncludingInactive = entity.MeshRenderersIncludingInactive;
                for (int k = 0; k < meshRenderersIncludingInactive.Count; k++)
                {
                    MeshRenderer meshRenderer = meshRenderersIncludingInactive[k];
                    if (bounds4 == null)
                    {
                        bounds4 = new Bounds?(meshRenderer.bounds);
                    }
                    else
                    {
                        Bounds value3 = bounds4.Value;
                        value3.Encapsulate(meshRenderer.bounds);
                        bounds4 = new Bounds?(value3);
                    }
                }
                if (bounds4 == null)
                {
                    return new Bounds(entity.transform.position, Vector3.zero);
                }
                return bounds4.Value;
            }
        }

        private static Bounds CalculateBoxColliderBounds(BoxCollider boxCollider)
        {
            Transform transform = boxCollider.transform;
            Vector3 vector = boxCollider.size * 0.5f;
            Vector3 center = boxCollider.center;
            Vector3 vector2 = transform.TransformPoint(center + new Vector3(-vector.x, -vector.y, -vector.z));
            Vector3 vector3 = transform.TransformPoint(center + new Vector3(vector.x, -vector.y, -vector.z));
            Vector3 vector4 = transform.TransformPoint(center + new Vector3(-vector.x, -vector.y, vector.z));
            Vector3 vector5 = transform.TransformPoint(center + new Vector3(vector.x, -vector.y, vector.z));
            Vector3 vector6 = transform.TransformPoint(center + new Vector3(-vector.x, vector.y, -vector.z));
            Vector3 vector7 = transform.TransformPoint(center + new Vector3(vector.x, vector.y, -vector.z));
            Vector3 vector8 = transform.TransformPoint(center + new Vector3(-vector.x, vector.y, vector.z));
            Vector3 vector9 = transform.TransformPoint(center + new Vector3(vector.x, vector.y, vector.z));
            Bounds bounds = new Bounds(vector2, Vector3.zero);
            bounds.Encapsulate(vector3);
            bounds.Encapsulate(vector4);
            bounds.Encapsulate(vector5);
            bounds.Encapsulate(vector6);
            bounds.Encapsulate(vector7);
            bounds.Encapsulate(vector8);
            bounds.Encapsulate(vector9);
            return bounds;
        }

        public static Bounds CalculatePrefabBoundingBox(GameObject prefab, Predicate<GameObject> skip)
        {
            Bounds? bounds = null;
            EntityGameObjectCenterCalculator.tmpMeshRenderers.Clear();
            prefab.GetComponentsInChildren<MeshRenderer>(true, EntityGameObjectCenterCalculator.tmpMeshRenderers);
            for (int i = 0; i < EntityGameObjectCenterCalculator.tmpMeshRenderers.Count; i++)
            {
                MeshRenderer meshRenderer = EntityGameObjectCenterCalculator.tmpMeshRenderers[i];
                if (skip == null || !skip(meshRenderer.gameObject))
                {
                    if (bounds == null)
                    {
                        bounds = new Bounds?(meshRenderer.bounds);
                    }
                    else
                    {
                        Bounds value = bounds.Value;
                        value.Encapsulate(meshRenderer.bounds);
                        bounds = new Bounds?(value);
                    }
                }
            }
            EntityGameObjectCenterCalculator.tmpMeshRenderers.Clear();
            if (bounds == null)
            {
                return new Bounds(Vector3.zero, Vector3.zero);
            }
            return bounds.Value;
        }

        private static List<MeshRenderer> tmpMeshRenderers = new List<MeshRenderer>();
    }
}