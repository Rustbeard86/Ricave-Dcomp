using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class CosmeticSpec : Spec, ISaveableEventsReceiver
    {
        public string PrefabPath
        {
            get
            {
                return this.prefabPath;
            }
        }

        public string IconPath
        {
            get
            {
                return this.iconPath;
            }
        }

        public List<string> CollisionTags
        {
            get
            {
                return this.collisionTags;
            }
        }

        public PriceTag Price
        {
            get
            {
                return this.price;
            }
        }

        public GameObject Prefab
        {
            get
            {
                return this.prefab;
            }
        }

        public Texture2D Icon
        {
            get
            {
                return this.icon;
            }
        }

        public float UIOrder
        {
            get
            {
                return this.uiOrder;
            }
        }

        public void OnLoaded()
        {
            if (!this.prefabPath.NullOrEmpty())
            {
                this.prefab = Assets.Get<GameObject>(this.prefabPath);
            }
            else
            {
                Log.Error("CosmeticSpec " + base.SpecID + " has no prefab.", false);
            }
            if (!this.iconPath.NullOrEmpty())
            {
                this.icon = Assets.Get<Texture2D>(this.iconPath);
                return;
            }
            Log.Error("CosmeticSpec " + base.SpecID + " has no icon.", false);
        }

        public void OnSaved()
        {
        }

        [Saved]
        private string prefabPath;

        [Saved]
        private string iconPath;

        [Saved(Default.New, true)]
        private List<string> collisionTags = new List<string>();

        [Saved]
        private PriceTag price;

        [Saved]
        private float uiOrder;

        private GameObject prefab;

        private Texture2D icon;
    }
}