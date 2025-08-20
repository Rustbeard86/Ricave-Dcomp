using System;
using Ricave.UI;
using UnityEngine;

namespace Ricave.Core
{
    public class WorldSituationSpec : Spec, ITipSubject, ISaveableEventsReceiver
    {
        public Type WorldSituationClass
        {
            get
            {
                return this.worldSituationClass;
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

        public Color IconColor
        {
            get
            {
                return Color.white;
            }
        }

        public Conditions EveryoneConditions
        {
            get
            {
                return this.everyoneConditions;
            }
        }

        public string AmbiencePath
        {
            get
            {
                return this.ambiencePath;
            }
        }

        public AudioClip Ambience
        {
            get
            {
                return this.ambience;
            }
        }

        public float AmbienceVolume
        {
            get
            {
                return this.ambienceVolume;
            }
        }

        public void OnLoaded()
        {
            if (!this.iconPath.NullOrEmpty())
            {
                this.icon = Assets.Get<Texture2D>(this.iconPath);
            }
            else
            {
                Log.Error("WorldSituationSpec " + base.SpecID + " has no icon.", false);
            }
            if (!this.ambiencePath.NullOrEmpty())
            {
                this.ambience = Assets.Get<AudioClip>(this.ambiencePath);
            }
        }

        public void OnSaved()
        {
        }

        [Saved(typeof(WorldSituation), false)]
        private Type worldSituationClass = typeof(WorldSituation);

        [Saved]
        private string iconPath;

        [Saved]
        private Conditions everyoneConditions;

        [Saved]
        private string ambiencePath;

        [Saved(1f, false)]
        private float ambienceVolume = 1f;

        private Texture2D icon;

        private AudioClip ambience;
    }
}