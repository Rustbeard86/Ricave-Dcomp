using System;
using Ricave.Core;
using UnityEngine;

namespace Ricave.Rendering
{
    public static class ActorPrefabCreator
    {
        public static GameObject CreatePrefab(Texture2D texture, string debugNameSuffix)
        {
            bool activeSelf = ActorPrefabCreator.ActorBasePrefab.activeSelf;
            ActorPrefabCreator.ActorBasePrefab.SetActive(false);
            GameObject gameObject2;
            try
            {
                GameObject gameObject = Object.Instantiate<GameObject>(ActorPrefabCreator.ActorBasePrefab);
                gameObject.name = "Prefab_" + debugNameSuffix;
                MeshRenderer component = gameObject.GetComponent<MeshRenderer>();
                component.material = ActorPrefabCreator.ActorBaseMaterial;
                component.material.mainTexture = texture;
                BoxCollider component2 = gameObject.GetComponent<BoxCollider>();
                Rect rect = TextureBoundsFinder.FindBounds(texture);
                component2.center = new Vector3(rect.center.x - 0.5f, rect.center.y - 0.5f, 0f);
                component2.size = new Vector3(rect.size.x, rect.size.y, 0.01f);
                ShatterMeshes.CacheVisibilityMaskFor(texture, new Rect?(rect));
                Object.DontDestroyOnLoad(gameObject);
                gameObject2 = gameObject;
            }
            finally
            {
                ActorPrefabCreator.ActorBasePrefab.SetActive(activeSelf);
            }
            return gameObject2;
        }

        private static readonly Material ActorBaseMaterial = Assets.Get<Material>("Materials/Actors/ActorBase");

        private static readonly GameObject ActorBasePrefab = Assets.Get<GameObject>("Prefabs/Actors/ActorBase");
    }
}