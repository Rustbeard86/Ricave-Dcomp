using System;
using System.Collections.Generic;
using Ricave.Core;
using Ricave.UI;
using UnityEngine;

namespace Ricave.Rendering
{
    public class PlayerModel
    {
        public GameObject Model
        {
            get
            {
                if (this.modelWithCosmetics == null || this.CosmeticsChanged())
                {
                    this.RebuildModel();
                }
                return this.modelWithCosmetics;
            }
        }

        public Texture2D Icon
        {
            get
            {
                if (this.icon == null || this.CosmeticsChanged())
                {
                    this.RebuildModel();
                }
                return this.icon;
            }
        }

        public RenderTexture Preview
        {
            get
            {
                if (this.modelWithCosmetics == null || this.CosmeticsChanged())
                {
                    this.RebuildModel();
                }
                Vector2 vector = Input.mousePosition / Widgets.UIScale;
                float num = vector.x / Widgets.VirtualWidth;
                float num2 = vector.y / Widgets.VirtualHeight;
                num = (num - 0.5f) * 2f;
                num2 = (num2 - 0.5f) * 2f;
                return GameObjectPreviewRenderer.GetPreview(this.modelWithCosmetics, 180f - num * 80f, num2 * 7f, new Bounds?(new Bounds(Vector3.zero, new Vector3(0.24f, 0.45f, 0.1f) * 2f)));
            }
        }

        private List<CosmeticSpec> CosmeticsToShow
        {
            get
            {
                List<CosmeticSpec> list = Get.CosmeticsManager.Chosen;
                Window_Cosmetics window_Cosmetics = (Window_Cosmetics)Get.WindowManager.GetFirstWindow(Get.Window_Cosmetics);
                if (window_Cosmetics != null && window_Cosmetics.HoverCosmetic != null && !list.Contains(window_Cosmetics.HoverCosmetic) && window_Cosmetics.HoverCosmetic != window_Cosmetics.LastUnchosen)
                {
                    list = list.ToTemporaryList<CosmeticSpec>();
                    foreach (CosmeticSpec cosmeticSpec in list.ToTemporaryList<CosmeticSpec>())
                    {
                        foreach (string text in window_Cosmetics.HoverCosmetic.CollisionTags)
                        {
                            if (cosmeticSpec.CollisionTags.Contains(text))
                            {
                                list.Remove(cosmeticSpec);
                                break;
                            }
                        }
                    }
                    list.Add(window_Cosmetics.HoverCosmetic);
                }
                return list;
            }
        }

        private bool CosmeticsChanged()
        {
            if (this.lastCosmetics == null)
            {
                return true;
            }
            List<CosmeticSpec> cosmeticsToShow = this.CosmeticsToShow;
            if (cosmeticsToShow.Count != this.lastCosmetics.Count)
            {
                return true;
            }
            foreach (CosmeticSpec cosmeticSpec in cosmeticsToShow)
            {
                if (!this.lastCosmetics.Contains(cosmeticSpec))
                {
                    return true;
                }
            }
            return false;
        }

        private void RebuildModel()
        {
            if (this.modelWithCosmetics != null)
            {
                Object.Destroy(this.modelWithCosmetics);
            }
            this.modelWithCosmetics = Object.Instantiate<GameObject>(PlayerModel.BasePlayerModel);
            EntityGOC entityGOC;
            if (this.modelWithCosmetics.TryGetComponent<EntityGOC>(out entityGOC))
            {
                Object.DestroyImmediate(entityGOC);
            }
            List<CosmeticSpec> cosmeticsToShow = this.CosmeticsToShow;
            foreach (CosmeticSpec cosmeticSpec in cosmeticsToShow)
            {
                if (!(cosmeticSpec.Prefab == null))
                {
                    GameObject gameObject = Object.Instantiate<GameObject>(cosmeticSpec.Prefab);
                    this.tmpChildren.Clear();
                    foreach (object obj in gameObject.transform)
                    {
                        Transform transform = (Transform)obj;
                        this.tmpChildren.Add(transform);
                    }
                    foreach (Transform transform2 in this.tmpChildren)
                    {
                        transform2.SetParent(this.modelWithCosmetics.transform, false);
                    }
                    Object.Destroy(gameObject);
                }
            }
            if (this.lastCosmetics == null)
            {
                this.lastCosmetics = new HashSet<CosmeticSpec>();
            }
            else
            {
                this.lastCosmetics.Clear();
            }
            this.lastCosmetics.AddRange<CosmeticSpec>(cosmeticsToShow);
            if (this.icon != null)
            {
                Object.Destroy(this.icon);
            }
            this.icon = IconGenerator.GenerateIcon(this.modelWithCosmetics, false, true);
            this.modelWithCosmetics.SetActive(false);
        }

        private GameObject modelWithCosmetics;

        private Texture2D icon;

        private HashSet<CosmeticSpec> lastCosmetics;

        private static readonly GameObject BasePlayerModel = Assets.Get<GameObject>("Prefabs/Actors/Hero");

        private List<Transform> tmpChildren = new List<Transform>();
    }
}