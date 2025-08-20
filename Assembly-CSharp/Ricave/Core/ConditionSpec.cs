using System;
using UnityEngine;

namespace Ricave.Core
{
    public class ConditionSpec : Spec, ISaveableEventsReceiver
    {
        public Type ConditionClass
        {
            get
            {
                return this.conditionClass;
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

        public Color? ActorOverlayColor
        {
            get
            {
                return this.actorOverlayColor;
            }
        }

        public bool ActorOverlayColorScreenOnly
        {
            get
            {
                return this.actorOverlayColorScreenOnly;
            }
        }

        public bool EndAtTheBeginningOfATurn
        {
            get
            {
                return this.endAtTheBeginningOfATurn;
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

        public GameObject ParticleSystem
        {
            get
            {
                return this.particleSystem;
            }
        }

        public bool ShowParticleSystemEvenOnPlayer
        {
            get
            {
                return this.showParticleSystemEvenOnPlayer;
            }
        }

        public Vector3 ParticleSystemPlayerOffset
        {
            get
            {
                return this.particleSystemPlayerOffset;
            }
        }

        public SoundSpec SoundLoop
        {
            get
            {
                return this.soundLoop;
            }
        }

        public float SoundLoopMaxDistance
        {
            get
            {
                return this.soundLoopMaxDistance;
            }
        }

        public float ActorScaleFactor
        {
            get
            {
                return this.actorScaleFactor;
            }
        }

        public void OnLoaded()
        {
            if (!this.iconPath.NullOrEmpty())
            {
                this.icon = Assets.Get<Texture2D>(this.iconPath);
            }
            if (!this.particleSystemPath.NullOrEmpty())
            {
                this.particleSystem = Assets.Get<GameObject>(this.particleSystemPath);
            }
        }

        public void OnSaved()
        {
        }

        [Saved(typeof(Condition), false)]
        private Type conditionClass = typeof(Condition);

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

        [Saved]
        private Color? actorOverlayColor;

        [Saved]
        private bool actorOverlayColorScreenOnly;

        [Saved]
        private bool endAtTheBeginningOfATurn;

        [Saved]
        private string particleSystemPath;

        [Saved]
        private bool showParticleSystemEvenOnPlayer;

        [Saved]
        private Vector3 particleSystemPlayerOffset;

        [Saved]
        private SoundSpec soundLoop;

        [Saved(10f, false)]
        private float soundLoopMaxDistance = 10f;

        [Saved(1f, false)]
        private float actorScaleFactor = 1f;

        private Texture2D icon;

        private GameObject particleSystem;
    }
}