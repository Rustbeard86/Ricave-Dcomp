using System;
using Ricave.Core;
using UnityEngine;

namespace Ricave.Rendering
{
    public class LightFadeInOutGOC : MonoBehaviour
    {
        private void Start()
        {
            this.InitIfNeeded();
        }

        private void OnEnable()
        {
            this.InitIfNeeded();
            this.startTime = Clock.Time;
            this.lightComp.intensity = 0f;
        }

        private void InitIfNeeded()
        {
            if (this.initialized)
            {
                return;
            }
            this.initialized = true;
            this.lightComp = base.GetComponent<Light>();
            this.initialIntensity = this.lightComp.intensity;
        }

        private void Update()
        {
            this.lightComp.intensity = this.initialIntensity * Calc.ResolveFadeInStayOut(Clock.Time - this.startTime, this.fadeIn, this.stay, this.fadeOut);
        }

        [SerializeField]
        private float fadeIn = 1f;

        [SerializeField]
        private float stay = 1f;

        [SerializeField]
        private float fadeOut = 1f;

        private float initialIntensity;

        private Light lightComp;

        private float startTime;

        private bool initialized;
    }
}