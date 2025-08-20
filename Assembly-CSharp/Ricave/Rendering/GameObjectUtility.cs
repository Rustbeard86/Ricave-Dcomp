using System;
using System.Collections.Generic;
using Ricave.Core;
using UnityEngine;

namespace Ricave.Rendering
{
    public static class GameObjectUtility
    {
        public static List<MeshRenderer> GetMeshRenderersInChildren(this GameObject gameObject, bool includeInactive = false)
        {
            if (gameObject == null)
            {
                return EmptyLists<MeshRenderer>.Get();
            }
            EntityGOC entityGOC;
            if (!gameObject.TryGetComponent<EntityGOC>(out entityGOC))
            {
                List<MeshRenderer> list = FrameLocalPool<List<MeshRenderer>>.Get();
                gameObject.GetComponentsInChildren<MeshRenderer>(includeInactive, list);
                return list;
            }
            if (!includeInactive)
            {
                return entityGOC.MeshRenderers;
            }
            return entityGOC.MeshRenderersIncludingInactive;
        }

        public static List<Renderer> GetRenderersInChildren(this GameObject gameObject, bool includeInactive = false)
        {
            if (gameObject == null)
            {
                return EmptyLists<Renderer>.Get();
            }
            EntityGOC entityGOC;
            if (!gameObject.TryGetComponent<EntityGOC>(out entityGOC))
            {
                List<Renderer> list = FrameLocalPool<List<Renderer>>.Get();
                gameObject.GetComponentsInChildren<Renderer>(includeInactive, list);
                return list;
            }
            if (!includeInactive)
            {
                return entityGOC.Renderers;
            }
            return entityGOC.RenderersIncludingInactive;
        }

        public static List<Collider> GetCollidersInChildren(this GameObject gameObject, bool includeInactive = false)
        {
            if (gameObject == null)
            {
                return EmptyLists<Collider>.Get();
            }
            EntityGOC entityGOC;
            if (!gameObject.TryGetComponent<EntityGOC>(out entityGOC))
            {
                List<Collider> list = FrameLocalPool<List<Collider>>.Get();
                gameObject.GetComponentsInChildren<Collider>(includeInactive, list);
                return list;
            }
            if (!includeInactive)
            {
                return entityGOC.Colliders;
            }
            return entityGOC.CollidersIncludingInactive;
        }

        public static MeshFilter GetMeshFilter(this MeshRenderer meshRenderer)
        {
            if (meshRenderer == null)
            {
                return null;
            }
            return meshRenderer.gameObject.GetComponent<MeshFilter>();
        }

        public static MeshRenderer GetMeshRenderer(this MeshFilter meshFilter)
        {
            if (meshFilter == null)
            {
                return null;
            }
            return meshFilter.gameObject.GetComponent<MeshRenderer>();
        }

        public static bool AnyInCameraFrustum(this List<Renderer> renderers)
        {
            if (renderers == null)
            {
                return false;
            }
            for (int i = 0; i < renderers.Count; i++)
            {
                if (renderers[i].enabled && renderers[i].isVisible)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool AnyRendererInCameraFrustum(this GameObject gameObject)
        {
            return gameObject.GetRenderersInChildren(false).AnyInCameraFrustum();
        }

        public static bool AnyRendererInCameraFrustumOrCombined(this GameObject gameObject)
        {
            EntityGOC componentInParent = gameObject.GetComponentInParent<EntityGOC>();
            return (componentInParent != null && CombinedMeshes.ShouldEntityBeCombined(componentInParent.Entity.Spec)) || gameObject.AnyRendererInCameraFrustum();
        }

        public static void FilterActive<T>(T[] all, ref T[] active) where T : Component
        {
            int num = 0;
            for (int i = 0; i < all.Length; i++)
            {
                if (all[i].gameObject.activeSelf)
                {
                    num++;
                }
            }
            if (active == null || active.Length != num)
            {
                active = new T[num];
            }
            int num2 = 0;
            for (int j = 0; j < all.Length; j++)
            {
                if (all[j].gameObject.activeSelf)
                {
                    active[num2] = all[j];
                    num2++;
                }
            }
        }

        public static void FilterActive<T>(List<T> all, List<T> active) where T : Component
        {
            active.Clear();
            for (int i = 0; i < all.Count; i++)
            {
                if (all[i].gameObject.activeSelf)
                {
                    active.Add(all[i]);
                }
            }
        }

        public static void ScaleAround(this Transform transform, Vector3 newScale, Vector3 pivotInWorldSpace)
        {
            Vector3 vector = transform.position - pivotInWorldSpace;
            Vector3 vector2 = new Vector3(newScale.x / transform.localScale.x, newScale.y / transform.localScale.y, newScale.z / transform.localScale.z);
            vector.Scale(vector2);
            transform.position = pivotInWorldSpace + vector;
            transform.localScale = newScale;
        }

        public static void Destroy(GameObject gameObject)
        {
            if (gameObject == null)
            {
                return;
            }
            EntityGOC componentInChildren = gameObject.GetComponentInChildren<EntityGOC>();
            if (componentInChildren != null)
            {
                componentInChildren.OnDestroyEvenIfInactive();
            }
            Object.Destroy(gameObject);
        }

        public static void DestroyImmediate(GameObject gameObject)
        {
            if (gameObject == null)
            {
                return;
            }
            EntityGOC componentInChildren = gameObject.GetComponentInChildren<EntityGOC>();
            if (componentInChildren != null)
            {
                componentInChildren.OnDestroyEvenIfInactive();
            }
            Object.DestroyImmediate(gameObject);
        }
    }
}