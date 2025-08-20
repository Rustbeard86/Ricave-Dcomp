using System;
using UnityEngine;

namespace Ricave.Core
{
    public class VisualEffectSpec : Spec, ISaveableEventsReceiver
    {
        public string ParticleSystemPrefabName
        {
            get
            {
                return this.particleSystemPrefabName;
            }
        }

        public float CameraWobbleAmount
        {
            get
            {
                return this.cameraWobbleAmount;
            }
        }

        public FloatRange CameraWobbleAmountRange
        {
            get
            {
                return this.cameraWobbleAmountRange;
            }
        }

        public float VignetteAmount
        {
            get
            {
                return this.vignetteAmount;
            }
        }

        public bool StrikeOverlay
        {
            get
            {
                return this.strikeOverlay;
            }
        }

        public bool CameraFallEffect
        {
            get
            {
                return this.cameraFallEffect;
            }
        }

        public float ShakeAmount
        {
            get
            {
                return this.shakeAmount;
            }
        }

        public SoundSpec Sound
        {
            get
            {
                return this.sound;
            }
        }

        public bool DisablePooling
        {
            get
            {
                return this.disablePooling;
            }
        }

        public bool MoveTowardsPlayer
        {
            get
            {
                return this.moveTowardsPlayer;
            }
        }

        public GameObject ParticleSystemPrefab
        {
            get
            {
                return this.particleSystemPrefab;
            }
        }

        public void OnLoaded()
        {
            if (!this.particleSystemPrefabName.NullOrEmpty())
            {
                this.particleSystemPrefab = Assets.Get<GameObject>(this.particleSystemPrefabName);
            }
        }

        public void OnSaved()
        {
        }

        [Saved]
        private string particleSystemPrefabName;

        [Saved]
        private float cameraWobbleAmount;

        [Saved]
        private FloatRange cameraWobbleAmountRange;

        [Saved]
        private float vignetteAmount;

        [Saved]
        private bool strikeOverlay;

        [Saved]
        private bool cameraFallEffect;

        [Saved]
        private float shakeAmount;

        [Saved]
        private SoundSpec sound;

        [Saved]
        private bool disablePooling;

        [Saved]
        private bool moveTowardsPlayer;

        private GameObject particleSystemPrefab;
    }
}