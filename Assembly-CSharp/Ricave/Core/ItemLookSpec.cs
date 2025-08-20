using System;
using Ricave.Rendering;
using UnityEngine;

namespace Ricave.Core
{
    public class ItemLookSpec : Spec, ISaveableEventsReceiver
    {
        public GameObject Prefab
        {
            get
            {
                return this.prefab;
            }
        }

        public string AutoCreatePrefabFromTexture
        {
            get
            {
                return this.autoCreatePrefabFromTexture;
            }
        }

        public Texture2D AutoCreatePrefabFromTextureTexture
        {
            get
            {
                return this.autoCreatePrefabFromTextureTexture;
            }
        }

        public Color AutoCreatePrefabFromTexture_Color
        {
            get
            {
                return this.autoCreatePrefabFromTexture_color;
            }
        }

        public Texture2D Icon
        {
            get
            {
                return this.icon;
            }
        }

        public Color IconColor
        {
            get
            {
                return this.autoCreatePrefabFromTexture_color;
            }
        }

        public void OnLoaded()
        {
            if (!this.autoCreatePrefabFromTexture.NullOrEmpty())
            {
                this.autoCreatePrefabFromTextureTexture = Assets.Get<Texture2D>(this.autoCreatePrefabFromTexture);
            }
            if (!this.prefabName.NullOrEmpty())
            {
                this.prefab = Assets.Get<GameObject>(this.prefabName);
            }
            else if (this.autoCreatePrefabFromTextureTexture != null)
            {
                this.prefab = ItemPrefabCreator.CreatePrefab(this.autoCreatePrefabFromTextureTexture, base.SpecID, new Color?(this.autoCreatePrefabFromTexture_color));
            }
            else
            {
                Log.Error("ItemLookSpec " + base.SpecID + " has no prefab.", false);
            }
            if (!this.iconPath.NullOrEmpty())
            {
                this.icon = Assets.Get<Texture2D>(this.iconPath);
                return;
            }
            Log.Error("ItemLookSpec " + base.SpecID + " has no icon.", false);
        }

        public void OnSaved()
        {
        }

        [Saved]
        private string iconPath;

        [Saved]
        private string prefabName;

        [Saved]
        private string autoCreatePrefabFromTexture;

        [Saved(Default.Color_White, false)]
        private Color autoCreatePrefabFromTexture_color = Color.white;

        private GameObject prefab;

        private Texture2D icon;

        private Texture2D autoCreatePrefabFromTextureTexture;
    }
}