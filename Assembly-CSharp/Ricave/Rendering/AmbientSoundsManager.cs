using System;
using Ricave.Core;
using UnityEngine;

namespace Ricave.Rendering
{
    public class AmbientSoundsManager
    {
        private bool WantsSilence
        {
            get
            {
                return Clock.UnscaledTime <= this.forceSilenceUntil || Get.MusicManager.PlayingMusicNow || Get.ScreenFader.AnyActionQueued || Root.ChangingScene || (!Get.InMainMenu && Get.DungeonMapDrawer.Showing) || this.CurrentWantedAmbience == null || this.audioSource.clip != this.CurrentWantedAmbience.Value.Item1;
            }
        }

        private ValueTuple<AudioClip, float>? CurrentWantedAmbience
        {
            get
            {
                if (Get.InMainMenu)
                {
                    return null;
                }
                ValueTuple<AudioClip, float>? ambience = Get.WorldSituationsManager.Ambience;
                if (ambience != null)
                {
                    return ambience;
                }
                if (Get.WorldSpec.Ambience != null)
                {
                    return new ValueTuple<AudioClip, float>?(new ValueTuple<AudioClip, float>(Get.WorldSpec.Ambience, Get.WorldSpec.AmbienceVolume));
                }
                return null;
            }
        }

        public float CurEffectiveVolume
        {
            get
            {
                if (!(this.audioSource.clip == null))
                {
                    return this.fadeInVolumeFactor * Get.MusicManager.VolumeFactorFromMuffled * this.currentAmbienceVolume;
                }
                return 0f;
            }
        }

        public void Update()
        {
            if (Application.isBatchMode)
            {
                return;
            }
            if (PrefsHelper.Volume <= 0f)
            {
                this.audioSource.Pause();
                return;
            }
            this.UpdateFadeInVolumeFactor();
            this.audioSource.volume = this.CurEffectiveVolume;
            if (this.fadeInVolumeFactor <= 0f)
            {
                this.CheckSwitchAmbienceNow();
            }
            if (Get.MusicManager.MuffledFactor > 0f)
            {
                this.audioLowPassFilter.cutoffFrequency = Calc.Lerp(22000f, 650f, Calc.Pow(Get.MusicManager.MuffledFactor, 0.15f));
                this.audioLowPassFilter.enabled = true;
                this.audioSource.bypassEffects = false;
            }
            else
            {
                this.audioLowPassFilter.enabled = false;
                this.audioSource.bypassEffects = true;
            }
            if (this.audioSource.volume <= 0f && this.audioSource.isPlaying)
            {
                this.audioSource.Pause();
                return;
            }
            if (!this.audioSource.isPlaying && this.audioSource.clip != null)
            {
                this.audioSource.UnPause();
            }
        }

        public void OnSceneChanged()
        {
            this.audioSource = null;
            this.audioLowPassFilter = null;
            this.CreateAudioSource();
        }

        private void UpdateFadeInVolumeFactor()
        {
            if (this.WantsSilence)
            {
                this.fadeInVolumeFactor -= Clock.UnscaledDeltaTime * 4f;
            }
            else
            {
                this.fadeInVolumeFactor += Clock.UnscaledDeltaTime * 0.3f;
            }
            this.fadeInVolumeFactor = Calc.Clamp01(this.fadeInVolumeFactor);
        }

        public void ForceSilenceFor(float duration)
        {
            this.forceSilenceUntil = Math.Max(this.forceSilenceUntil, Clock.UnscaledTime + duration);
        }

        private void CreateAudioSource()
        {
            if (this.audioSource != null)
            {
                return;
            }
            GameObject gameObject = new GameObject("Ambience");
            this.audioSource = gameObject.AddComponent<AudioSource>();
            this.audioSource.bypassReverbZones = true;
            this.audioSource.bypassEffects = true;
            this.audioSource.bypassListenerEffects = true;
            this.audioSource.priority = 0;
            this.audioSource.playOnAwake = false;
            this.audioSource.spatialBlend = 0f;
            this.audioSource.volume = 0f;
            this.audioSource.loop = true;
            this.audioLowPassFilter = gameObject.AddComponent<AudioLowPassFilter>();
            this.audioLowPassFilter.enabled = false;
            this.audioLowPassFilter.cutoffFrequency = 22000f;
        }

        private void CheckSwitchAmbienceNow()
        {
            ValueTuple<AudioClip, float>? currentWantedAmbience = this.CurrentWantedAmbience;
            if (currentWantedAmbience == null)
            {
                if (this.audioSource.clip == null)
                {
                    return;
                }
                this.audioSource.Stop();
                this.audioSource.clip = null;
                return;
            }
            else
            {
                if (this.audioSource.clip == currentWantedAmbience.Value.Item1)
                {
                    this.currentAmbienceVolume = currentWantedAmbience.Value.Item2;
                    return;
                }
                this.audioSource.Stop();
                this.audioSource.clip = currentWantedAmbience.Value.Item1;
                this.audioSource.time = Rand.Range(0f, this.audioSource.clip.length);
                this.currentAmbienceVolume = currentWantedAmbience.Value.Item2;
                this.audioSource.Play();
                return;
            }
        }

        private AudioSource audioSource;

        private AudioLowPassFilter audioLowPassFilter;

        private float forceSilenceUntil = -99999f;

        private float fadeInVolumeFactor;

        private float currentAmbienceVolume = 1f;

        private const float FadeOutSpeed = 4f;

        private const float FadeInSpeed = 0.3f;
    }
}