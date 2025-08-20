using System;
using UnityEngine;

namespace Ricave.Core
{
    public class ItemSlotSpec : Spec, ISaveableEventsReceiver
    {
        public Vector2Int Offset
        {
            get
            {
                return this.offset;
            }
        }

        public Texture2D Icon
        {
            get
            {
                return this.icon;
            }
        }

        public void OnLoaded()
        {
            if (!this.iconPath.NullOrEmpty())
            {
                this.icon = Assets.Get<Texture2D>(this.iconPath);
                return;
            }
            Log.Error("ItemSlotSpec " + base.SpecID + " has no icon.", false);
        }

        public void OnSaved()
        {
        }

        [Saved]
        private Vector2Int offset;

        [Saved]
        private string iconPath;

        private Texture2D icon;
    }
}