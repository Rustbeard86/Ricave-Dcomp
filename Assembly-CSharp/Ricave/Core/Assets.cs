using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public static class Assets
    {
        public static HashSet<Material> AllLoadedMaterials
        {
            get
            {
                return Assets.allLoadedMaterials;
            }
        }

        public static T Get<T>(string path) where T : Object
        {
            T t;
            if (!Assets.TryGetAssetFromMods<T>(path, out t))
            {
                t = Resources.Load<T>(path);
            }
            if (t == null)
            {
                Log.Error("Could not load " + typeof(T).Name + " at " + path, false);
                if (typeof(Texture).IsAssignableFrom(typeof(T)))
                {
                    t = (T)((object)Assets.MissingTexture);
                }
            }
            Assets.AddToAllLoadedResources(t);
            return t;
        }

        public static bool IsNullOrMissing(this Texture texture)
        {
            return texture == null || texture == Assets.MissingTexture;
        }

        private static bool TryGetAssetFromMods<T>(string path, out T asset) where T : Object
        {
            List<Mod> activeMods = Ricave.Core.Get.ModManager.ActiveMods;
            for (int i = activeMods.Count - 1; i >= 0; i--)
            {
                if (activeMods[i].TryGetAsset<T>(path, out asset))
                {
                    return true;
                }
            }
            asset = default(T);
            return false;
        }

        private static void AddToAllLoadedResources(object obj)
        {
            if (obj == null)
            {
                return;
            }
            Texture2D texture2D = obj as Texture2D;
            if (texture2D != null)
            {
                if (texture2D.name != null && texture2D.name.Contains("MenuBackground"))
                {
                    return;
                }
                if (Assets.allLoadedSingleTextures.Add(texture2D))
                {
                    if (Assets.standardShader == null)
                    {
                        Assets.standardShader = Shader.Find("Standard");
                    }
                    Material material = new Material(Assets.standardShader);
                    material.mainTexture = texture2D;
                    material.name = "AssetsPreloaderMaterialFromTexture";
                    Assets.allLoadedMaterials.Add(material);
                    return;
                }
            }
            else
            {
                Material material2 = obj as Material;
                if (material2 != null)
                {
                    Assets.allLoadedMaterials.Add(material2);
                    return;
                }
                Shader shader = obj as Shader;
                if (shader != null)
                {
                    if (Assets.allLoadedSingleShaders.Add(shader))
                    {
                        Material material3 = new Material(shader);
                        material3.name = "AssetsPreloaderMaterialFromShader";
                        Assets.allLoadedMaterials.Add(material3);
                        return;
                    }
                }
                else
                {
                    GameObject gameObject = obj as GameObject;
                    if (gameObject != null)
                    {
                        Assets.tmpRenderers.Clear();
                        gameObject.GetComponentsInChildren<Renderer>(true, Assets.tmpRenderers);
                        for (int i = 0; i < Assets.tmpRenderers.Count; i++)
                        {
                            Assets.tmpMaterials.Clear();
                            Assets.tmpRenderers[i].GetSharedMaterials(Assets.tmpMaterials);
                            for (int j = 0; j < Assets.tmpMaterials.Count; j++)
                            {
                                if (Assets.tmpMaterials[j] != null)
                                {
                                    Assets.allLoadedMaterials.Add(Assets.tmpMaterials[j]);
                                }
                            }
                            Assets.tmpMaterials.Clear();
                        }
                        Assets.tmpRenderers.Clear();
                    }
                }
            }
        }

        private static HashSet<Material> allLoadedMaterials = new HashSet<Material>();

        private static HashSet<Texture2D> allLoadedSingleTextures = new HashSet<Texture2D>();

        private static HashSet<Shader> allLoadedSingleShaders = new HashSet<Shader>();

        private static Shader standardShader;

        public static readonly Texture2D MissingTexture = Resources.Load<Texture2D>("Textures/Misc/MissingTexture");

        private static List<Renderer> tmpRenderers = new List<Renderer>(10);

        private static List<Material> tmpMaterials = new List<Material>(10);
    }
}