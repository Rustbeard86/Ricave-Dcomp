using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class WorldSpec : Spec, ISaveableEventsReceiver
    {
        public bool DisableRandomEvents
        {
            get
            {
                return this.disableRandomEvents;
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

        public List<GenPassSpec> GenPassesInOrder
        {
            get
            {
                if (this.genPassesFromAllSourcesCached == null)
                {
                    this.genPassesFromAllSourcesCached = new List<GenPassSpec>();
                    this.genPassesFromAllSourcesCached.AddRange(this.genPasses);
                    foreach (GenPassSpec genPassSpec in Get.Specs.GetAll<GenPassSpec>())
                    {
                        if (genPassSpec.AddToWorldSpecs.Contains(this))
                        {
                            this.genPassesFromAllSourcesCached.Add(genPassSpec);
                        }
                    }
                    this.genPassesFromAllSourcesCached.StableSort<GenPassSpec, float>((GenPassSpec x) => x.Order);
                }
                return this.genPassesFromAllSourcesCached;
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

        public void OnLoaded()
        {
            if (!this.iconPath.NullOrEmpty())
            {
                this.icon = Assets.Get<Texture2D>(this.iconPath);
            }
            else
            {
                Log.Error("WorldSpec " + base.SpecID + " has no icon.", false);
            }
            if (!this.ambiencePath.NullOrEmpty())
            {
                this.ambience = Assets.Get<AudioClip>(this.ambiencePath);
            }
        }

        public void OnSaved()
        {
        }

        [Saved(Default.New, false)]
        private List<GenPassSpec> genPasses = new List<GenPassSpec>();

        [Saved]
        private string iconPath;

        [Saved]
        private bool disableRandomEvents;

        [Saved]
        private string ambiencePath;

        [Saved(1f, false)]
        private float ambienceVolume = 1f;

        private List<GenPassSpec> genPassesFromAllSourcesCached;

        private Texture2D icon;

        private AudioClip ambience;
    }
}