using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class FactionSpec : Spec, ISaveableEventsReceiver
    {
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

        public List<FactionSpec> DefaultHostileTo
        {
            get
            {
                return this.defaultHostileTo;
            }
        }

        public float UIOrder
        {
            get
            {
                return this.uiOrder;
            }
        }

        public bool GenerateOneAtStart
        {
            get
            {
                return this.generateOneAtStart;
            }
        }

        public bool DefaultHidden
        {
            get
            {
                return this.defaultHidden;
            }
        }

        public int GenerateOrder
        {
            get
            {
                return this.generateOrder;
            }
        }

        public bool HideIfNoMemberSpawned
        {
            get
            {
                return this.hideIfNoMemberSpawned;
            }
        }

        public List<string> PossibleNames
        {
            get
            {
                return this.possibleNames;
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

        [Saved]
        private string iconPath;

        [Saved(Default.New, true)]
        private List<FactionSpec> defaultHostileTo = new List<FactionSpec>();

        [Saved]
        private float uiOrder;

        [Saved]
        private bool generateOneAtStart;

        [Saved]
        private bool defaultHidden;

        [Saved]
        private int generateOrder;

        [Saved]
        private bool hideIfNoMemberSpawned;

        [Saved]
        [Translatable]
        private List<string> possibleNames;

        private Texture2D icon;
    }
}