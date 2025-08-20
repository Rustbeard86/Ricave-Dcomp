using System;
using Ricave.Core;
using UnityEngine;

namespace Ricave.Rendering
{
    public static class ItemPrefabCreator
    {
        public static GameObject CreatePrefab(Texture2D texture, string debugNameSuffix, Color? color = null)
        {
            bool activeSelf = ItemPrefabCreator.ItemBasePrefab.activeSelf;
            ItemPrefabCreator.ItemBasePrefab.SetActive(false);
            GameObject gameObject2;
            try
            {
                GameObject gameObject = Object.Instantiate<GameObject>(ItemPrefabCreator.ItemBasePrefab);
                gameObject.name = "Prefab_" + debugNameSuffix;
                MeshRenderer component = gameObject.GetComponent<MeshRenderer>();
                component.material = ItemPrefabCreator.ItemBaseMaterial;
                component.material.mainTexture = texture;
                if (color != null)
                {
                    component.material.color = color.Value;
                }
                Object.DontDestroyOnLoad(gameObject);
                gameObject2 = gameObject;
            }
            finally
            {
                ItemPrefabCreator.ItemBasePrefab.SetActive(activeSelf);
            }
            return gameObject2;
        }

        private static readonly Material ItemBaseMaterial = Assets.Get<Material>("Materials/Items/ItemBase");

        private static readonly GameObject ItemBasePrefab = Assets.Get<GameObject>("Prefabs/Items/ItemBase");
    }
}