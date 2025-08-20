using System;
using UnityEngine;

namespace Ricave.Core
{
    public class UseEffectSpec : Spec, ISaveableEventsReceiver
    {
        public Type UseEffectClass
        {
            get
            {
                return this.useEffectClass;
            }
        }

        public string LabelFormat
        {
            get
            {
                return this.labelFormat;
            }
        }

        public string DescriptionFormat
        {
            get
            {
                return this.descriptionFormat;
            }
        }

        public bool AlwaysHidden
        {
            get
            {
                return this.alwaysHidden;
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
            }
        }

        public void OnSaved()
        {
        }

        [Saved(typeof(UseEffect), false)]
        private Type useEffectClass = typeof(UseEffect);

        [Saved]
        private string iconPath;

        [Saved]
        [Translatable]
        private string labelFormat;

        [Saved]
        [Translatable]
        private string descriptionFormat;

        [Saved]
        private bool alwaysHidden;

        private Texture2D icon;
    }
}