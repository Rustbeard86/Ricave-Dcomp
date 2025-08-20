using System;
using Ricave.Rendering;
using Ricave.UI;
using UnityEngine;

namespace Ricave.Core
{
    public class UnlockableSpec : Spec, ITipSubject, ISaveableEventsReceiver
    {
        public int StardustPrice
        {
            get
            {
                return this.stardustPrice;
            }
        }

        public string IconPath
        {
            get
            {
                return this.iconPath;
            }
        }

        public Texture2D Icon
        {
            get
            {
                return this.icon;
            }
        }

        public string ItemPrefabName
        {
            get
            {
                return this.itemPrefabName;
            }
        }

        public GameObject ItemPrefab
        {
            get
            {
                return this.itemPrefab;
            }
        }

        public string AutoCreateItemPrefabFromTexture
        {
            get
            {
                return this.autoCreateItemPrefabFromTexture;
            }
        }

        public Texture2D AutoCreateItemPrefabFromTextureTexture
        {
            get
            {
                return this.autoCreateItemPrefabFromTextureTexture;
            }
        }

        public int? AutoUnlockAtScoreLevel
        {
            get
            {
                return this.autoUnlockAtScoreLevel;
            }
        }

        Color ITipSubject.IconColor
        {
            get
            {
                return Color.white;
            }
        }

        public string UnlockedText
        {
            get
            {
                return this.unlockedText;
            }
        }

        public bool ResetAfterRun
        {
            get
            {
                return this.resetAfterRun;
            }
        }

        public EntitySpec PrivateRoomStructureReward
        {
            get
            {
                return this.privateRoomStructureReward;
            }
        }

        public void OnLoaded()
        {
            if (!this.autoCreateItemPrefabFromTexture.NullOrEmpty())
            {
                this.autoCreateItemPrefabFromTextureTexture = Assets.Get<Texture2D>(this.autoCreateItemPrefabFromTexture);
            }
            if (!this.itemPrefabName.NullOrEmpty())
            {
                this.itemPrefab = Assets.Get<GameObject>(this.itemPrefabName);
            }
            else if (this.autoCreateItemPrefabFromTextureTexture != null)
            {
                this.itemPrefab = ItemPrefabCreator.CreatePrefab(this.autoCreateItemPrefabFromTextureTexture, base.SpecID, null);
            }
            if (!this.iconPath.NullOrEmpty())
            {
                this.icon = Assets.Get<Texture2D>(this.iconPath);
            }
        }

        public void OnSaved()
        {
        }

        [Saved]
        private int stardustPrice;

        [Saved]
        private string iconPath;

        [Saved]
        private string itemPrefabName;

        [Saved]
        private string autoCreateItemPrefabFromTexture;

        [Saved]
        private int? autoUnlockAtScoreLevel;

        [Saved]
        [Translatable]
        private string unlockedText;

        [Saved]
        private bool resetAfterRun;

        [Saved]
        private EntitySpec privateRoomStructureReward;

        private GameObject itemPrefab;

        private Texture2D icon;

        private Texture2D autoCreateItemPrefabFromTextureTexture;
    }
}