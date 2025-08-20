using System;
using UnityEngine;

namespace Ricave.Core
{
    public class LightCompProps : EntityCompProps, ISaveableEventsReceiver
    {
        public LightType LightType
        {
            get
            {
                return this.lightType;
            }
        }

        public float Range
        {
            get
            {
                return this.range;
            }
        }

        public Color Color
        {
            get
            {
                return this.color;
            }
        }

        public float Intensity
        {
            get
            {
                return this.intensity;
            }
        }

        public float BounceIntensity
        {
            get
            {
                return this.bounceIntensity;
            }
        }

        public LightShadows ShadowType
        {
            get
            {
                return this.shadowType;
            }
        }

        public bool Flicker
        {
            get
            {
                return this.flicker;
            }
        }

        public bool RandomColor
        {
            get
            {
                return this.randomColor;
            }
        }

        public Vector3 PositionOffset
        {
            get
            {
                return this.positionOffset;
            }
        }

        public string Cookie
        {
            get
            {
                return this.cookie;
            }
        }

        public Texture CookieTexture
        {
            get
            {
                return this.cookieTexture;
            }
        }

        public float RotationSpeed
        {
            get
            {
                return this.rotationSpeed;
            }
        }

        public bool CastShadows
        {
            get
            {
                return this.castShadows;
            }
        }

        public bool OnlyIfEntityVisible
        {
            get
            {
                return this.onlyIfEntityVisible;
            }
        }

        public bool FadeOutOnEntityDespawned
        {
            get
            {
                return this.fadeOutOnEntityDespawned;
            }
        }

        public void OnLoaded()
        {
            if (!this.cookie.NullOrEmpty())
            {
                this.cookieTexture = Assets.Get<Texture>(this.cookie);
            }
        }

        public void OnSaved()
        {
        }

        [Saved(LightType.Point, false)]
        private LightType lightType = LightType.Point;

        [Saved(10f, false)]
        private float range = 10f;

        [Saved(Default.Color_White, false)]
        private Color color = Color.white;

        [Saved(1f, false)]
        private float intensity = 1f;

        [Saved(1f, false)]
        private float bounceIntensity = 1f;

        [Saved(LightShadows.Soft, false)]
        private LightShadows shadowType = LightShadows.Soft;

        [Saved]
        private bool flicker;

        [Saved]
        private bool randomColor;

        [Saved]
        private Vector3 positionOffset;

        [Saved]
        private string cookie;

        [Saved]
        private float rotationSpeed;

        [Saved(true, false)]
        private bool castShadows = true;

        [Saved]
        private bool onlyIfEntityVisible;

        [Saved]
        private bool fadeOutOnEntityDespawned;

        private Texture cookieTexture;
    }
}