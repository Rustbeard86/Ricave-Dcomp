using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class PlaceSpec : Spec, ISaveableEventsReceiver
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

        public List<string> PossibleNames
        {
            get
            {
                return this.possibleNames;
            }
        }

        public WorldSpec WorldSpec
        {
            get
            {
                return this.worldSpec;
            }
        }

        public bool GiveNewFloorReachedScore
        {
            get
            {
                return this.giveNewFloorReachedScore;
            }
        }

        public void OnLoaded()
        {
            if (!this.iconPath.NullOrEmpty())
            {
                this.icon = Assets.Get<Texture2D>(this.iconPath);
                return;
            }
            Log.Error("PlaceSpec " + base.SpecID + " has no icon.", false);
        }

        public void OnSaved()
        {
        }

        [Saved]
        private string iconPath;

        [Saved]
        [Translatable]
        private List<string> possibleNames;

        [Saved]
        private WorldSpec worldSpec;

        [Saved]
        private bool giveNewFloorReachedScore;

        private Texture2D icon;
    }
}