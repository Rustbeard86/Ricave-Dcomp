using System;
using System.Collections.Generic;
using System.Linq;
using Ricave.Core;
using Ricave.UI;
using UnityEngine;

namespace Ricave.Rendering
{
    public class MusicManager
    {
        public bool PlayingMusicNow
        {
            get
            {
                return this.musicPlaying != null;
            }
        }

        public float MuffledFactor
        {
            get
            {
                return this.muffledFactor;
            }
        }

        public float CurEffectiveVolume
        {
            get
            {
                if (this.musicPlaying != null)
                {
                    return this.musicPlaying.Volume * this.VolumeFactorFromMusicStartEnd * this.fadeInVolumeFactor * this.VolumeFactorFromMuffled * PrefsHelper.MusicVolume * (Get.InMainMenu ? 0.75f : 0.2f) * (DebugUI.TrailerMode ? 0f : 1f);
                }
                return 0f;
            }
        }

        private float VolumeFactorFromMusicStartEnd
        {
            get
            {
                if (this.musicPlaying == null)
                {
                    return 0f;
                }
                float time = MusicManager.audioSource.time;
                float num;
                if (time < 5f)
                {
                    num = time / 5f;
                }
                else
                {
                    num = 1f;
                }
                float num2;
                if (time > MusicManager.audioSource.clip.length - 5f)
                {
                    num2 = Calc.Clamp01((MusicManager.audioSource.clip.length - time) / 5f);
                }
                else
                {
                    num2 = 1f;
                }
                return Math.Min(num, num2);
            }
        }

        public float VolumeFactorFromMuffled
        {
            get
            {
                return 1f - this.muffledFactor * 0.1f;
            }
        }

        private bool WantsSilence
        {
            get
            {
                return Clock.UnscaledTime <= this.forceSilenceUntil || this.fadeOutCurrentMusic;
            }
        }

        public void Init()
        {
            if (MusicManager.audioSource == null)
            {
                GameObject gameObject = new GameObject("Music");
                Object.DontDestroyOnLoad(gameObject);
                MusicManager.audioSource = gameObject.AddComponent<AudioSource>();
                MusicManager.audioSource.bypassReverbZones = true;
                MusicManager.audioSource.bypassEffects = true;
                MusicManager.audioSource.bypassListenerEffects = true;
                MusicManager.audioSource.priority = 0;
                MusicManager.audioSource.playOnAwake = false;
                MusicManager.audioSource.spatialBlend = 0f;
                MusicManager.audioSource.volume = 0f;
                MusicManager.audioLowPassFilter = gameObject.AddComponent<AudioLowPassFilter>();
                MusicManager.audioLowPassFilter.enabled = false;
                MusicManager.audioLowPassFilter.cutoffFrequency = 22000f;
            }
        }

        public void OnSceneChanged()
        {
            if (this.musicPlaying == null)
            {
                MusicManager.audioSource.Stop();
            }
            else if (MusicManager.audioSource.clip == null)
            {
                Log.Error("Music playing after scene change is not null, but audioSource.clip is null.", false);
                this.musicPlaying = null;
            }
            if (this.previouslyInMainMenu != Get.InMainMenu)
            {
                this.lastMusicFinishTime = -99999f;
                this.forceSilenceUntil = -99999f;
                this.previouslyInMainMenu = Get.InMainMenu;
            }
        }

        public void Update()
        {
            if (Application.isBatchMode)
            {
                return;
            }
            if (Get.ScreenFader.AnyActionQueued || (!Get.InMainMenu && (Get.DeathScreenDrawer.ShouldShow || Get.TextSequenceDrawer.Showing || Get.DungeonMapDrawer.Showing)))
            {
                this.muffledFactor = Math.Min(this.muffledFactor + Clock.UnscaledDeltaTime * 3.6f, 1f);
            }
            else
            {
                this.muffledFactor = Math.Max(this.muffledFactor - Clock.UnscaledDeltaTime * 0.8f, 0f);
            }
            if (PrefsHelper.Volume <= 0f || PrefsHelper.MusicVolume <= 0f)
            {
                this.StopPlayingNow();
                return;
            }
            this.UpdateFadeInVolumeFactor();
            MusicManager.audioSource.volume = this.CurEffectiveVolume;
            if (this.muffledFactor > 0f)
            {
                MusicManager.audioLowPassFilter.cutoffFrequency = Calc.Lerp(22000f, 650f, Calc.Pow(this.muffledFactor, 0.15f));
                MusicManager.audioLowPassFilter.enabled = true;
                MusicManager.audioSource.bypassEffects = false;
            }
            else
            {
                MusicManager.audioLowPassFilter.enabled = false;
                MusicManager.audioSource.bypassEffects = true;
            }
            if (this.fadeOutCurrentMusic && this.fadeInVolumeFactor <= 0f)
            {
                this.fadeOutCurrentMusic = false;
                this.StopPlayingNow();
                this.lastMusicFinishTime = -99999f;
            }
            if (this.musicPlaying != null)
            {
                if (MusicManager.audioSource.time > 0f)
                {
                    this.audioSourceTimeWasNonZero = true;
                }
                if (MusicManager.audioSource.time >= MusicManager.audioSource.clip.length - 1E-05f || (this.audioSourceTimeWasNonZero && MusicManager.audioSource.time <= 0f))
                {
                    this.StopPlayingNow();
                }
            }
            this.CheckStartNewMusic();
            if (MusicManager.audioSource.volume <= 0f && MusicManager.audioSource.isPlaying)
            {
                MusicManager.audioSource.Pause();
                return;
            }
            if (!MusicManager.audioSource.isPlaying && this.musicPlaying != null)
            {
                MusicManager.audioSource.UnPause();
            }
        }

        public void ForceSilenceFor(float duration)
        {
            this.forceSilenceUntil = Math.Max(this.forceSilenceUntil, Clock.UnscaledTime + duration);
        }

        public void ForciblyFadeOutCurrentMusic()
        {
            if (this.musicPlaying == null)
            {
                return;
            }
            this.fadeOutCurrentMusic = true;
        }

        private void UpdateFadeInVolumeFactor()
        {
            if (this.WantsSilence)
            {
                this.fadeInVolumeFactor -= Clock.UnscaledDeltaTime * 4f;
            }
            else
            {
                this.fadeInVolumeFactor += Clock.UnscaledDeltaTime * 0.26f;
            }
            this.fadeInVolumeFactor = Calc.Clamp01(this.fadeInVolumeFactor);
        }

        private void StopPlayingNow()
        {
            if (this.musicPlaying != null)
            {
                this.lastMusicFinishTime = Clock.UnscaledTime;
            }
            MusicManager.audioSource.Stop();
            this.musicPlaying = null;
            this.audioSourceTimeWasNonZero = false;
        }

        private void StartPlayingNow(MusicSpec spec)
        {
            if (spec == null)
            {
                Log.Error("Tried to start playing null MusicSpec.", false);
                return;
            }
            this.StopPlayingNow();
            this.musicPlaying = spec;
            MusicManager.audioSource.clip = spec.AudioClip;
            MusicManager.audioSource.volume = 0f;
            MusicManager.audioSource.Play();
            MusicManager.audioSource.Pause();
            this.lastMusicPlayed.Add(spec);
            while (this.lastMusicPlayed.Count > 10)
            {
                this.lastMusicPlayed.RemoveAt(0);
            }
            this.lastMusicFinishTime = Clock.UnscaledTime;
            this.audioSourceTimeWasNonZero = false;
        }

        private void CheckStartNewMusic()
        {
            if (this.musicPlaying != null)
            {
                return;
            }
            if (Clock.UnscaledTime - this.lastMusicFinishTime < 70f)
            {
                return;
            }
            if (Clock.TimeSinceSceneLoad < 5f && !Get.InMainMenu)
            {
                return;
            }
            if (!Get.InMainMenu && Get.TextSequenceDrawer.Showing)
            {
                return;
            }
            if (this.WantsSilence)
            {
                return;
            }
            if (Get.ScreenFader.AnyActionQueued || Root.ChangingScene)
            {
                return;
            }
            List<MusicSpec> all = Get.Specs.GetAll<MusicSpec>();
            bool flag = false;
            for (int i = 0; i < all.Count; i++)
            {
                if (all[i].MainMenu == Get.InMainMenu)
                {
                    flag = true;
                    break;
                }
            }
            if (!flag)
            {
                return;
            }
            IEnumerable<MusicSpec> enumerable = all.Where<MusicSpec>((MusicSpec x) => x.MainMenu == Get.InMainMenu);
            MusicSpec musicSpec;
            if (enumerable.Where<MusicSpec>((MusicSpec x) => !this.PlayedRecently(x, 2)).TryGetRandomElement<MusicSpec>(out musicSpec) || enumerable.Where<MusicSpec>((MusicSpec x) => !this.PlayedRecently(x, 1)).TryGetRandomElement<MusicSpec>(out musicSpec) || enumerable.TryGetRandomElement<MusicSpec>(out musicSpec))
            {
                this.StartPlayingNow(musicSpec);
            }
        }

        private bool PlayedRecently(MusicSpec spec, int distance)
        {
            if (distance <= 0 || this.lastMusicPlayed.Count == 0)
            {
                return false;
            }
            int num = this.lastMusicPlayed.Count - 1;
            for (int i = 0; i < distance; i++)
            {
                if (this.lastMusicPlayed[num] == spec)
                {
                    return true;
                }
                num--;
                if (num < 0)
                {
                    break;
                }
            }
            return false;
        }

        private float fadeInVolumeFactor;

        private MusicSpec musicPlaying;

        private List<MusicSpec> lastMusicPlayed = new List<MusicSpec>();

        private float lastMusicFinishTime = -99999f;

        private bool audioSourceTimeWasNonZero;

        private float forceSilenceUntil = -99999f;

        private bool previouslyInMainMenu;

        private bool fadeOutCurrentMusic;

        private float muffledFactor;

        private static AudioSource audioSource;

        private static AudioLowPassFilter audioLowPassFilter;

        private const float FadeOutSpeed = 4f;

        private const float FadeInSpeed = 0.26f;

        private const float FadeInOutStartEndDuration = 5f;

        private const float SceneChangeInitialSilencePeriod = 5f;

        private const float SilenceBetweenSongs = 70f;

        private const float MusicVolumeFactor = 0.2f;
    }
}