using System;
using System.Collections.Generic;
using Ricave.Core;
using UnityEngine;

namespace Ricave.Rendering
{
    public class Audio
    {
        public OneShotSounds OneShots
        {
            get
            {
                return this.oneShots;
            }
        }

        public SustainingSounds Sustaining
        {
            get
            {
                return this.sustaining;
            }
        }

        public MusicManager MusicManager
        {
            get
            {
                return this.musicManager;
            }
        }

        public AmbientSoundsManager AmbientSoundsManager
        {
            get
            {
                return this.ambientSoundsManager;
            }
        }

        public void Update()
        {
            try
            {
                Profiler.Begin("oneShots.Update()");
                this.oneShots.Update();
            }
            catch (Exception ex)
            {
                Log.Error("Error in oneShots.Update().", ex);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("withSoundLoop.Update()");
                this.withSoundLoop.Update();
            }
            catch (Exception ex2)
            {
                Log.Error("Error in withSoundLoop.Update().", ex2);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("musicManager.Update()");
                this.musicManager.Update();
            }
            catch (Exception ex3)
            {
                Log.Error("Error in musicManager.Update().", ex3);
            }
            finally
            {
                Profiler.End();
            }
            try
            {
                Profiler.Begin("ambientSoundsManager.Update()");
                this.ambientSoundsManager.Update();
            }
            catch (Exception ex4)
            {
                Log.Error("Error in ambientSoundsManager.Update().", ex4);
            }
            finally
            {
                Profiler.End();
            }
        }

        public void OnSceneChanged()
        {
            this.oneShots.OnSceneChanged();
            this.withSoundLoop.OnSceneChanged();
            this.musicManager.OnSceneChanged();
            this.audioSourcePool.Clear();
            this.ambientSoundsManager.OnSceneChanged();
            this.InitAudioSourcePool();
        }

        private void InitAudioSourcePool()
        {
            List<AudioSource> list = new List<AudioSource>(8);
            for (int i = 0; i < 8; i++)
            {
                list.Add(this.GetPooledAudioSource());
            }
            foreach (AudioSource audioSource in list)
            {
                this.ReturnToPool(audioSource);
            }
        }

        public void ReturnToPool(AudioSource audioSource)
        {
            if (audioSource == null)
            {
                Log.Error("Tried to return null AudioSource to pool.", false);
                return;
            }
            if (this.audioSourcePool.Contains(audioSource))
            {
                Log.Error("Tried to return the same AudioSource twice to pool.", false);
                return;
            }
            this.ResetValuesToDefault(audioSource);
            this.audioSourcePool.Add(audioSource);
        }

        private void ResetValuesToDefault(AudioSource audioSource)
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
            audioSource.gameObject.SetActive(false);
            audioSource.transform.localPosition = default(Vector3);
            audioSource.transform.localRotation = default(Quaternion);
            audioSource.clip = null;
            audioSource.spatialBlend = 1f;
            audioSource.loop = false;
            audioSource.volume = 1f;
            audioSource.pitch = 1f;
            audioSource.time = 0f;
            audioSource.maxDistance = 10f;
            audioSource.bypassReverbZones = false;
            audioSource.bypassEffects = false;
            audioSource.bypassListenerEffects = false;
            AudioLowPassFilter component = audioSource.GetComponent<AudioLowPassFilter>();
            component.enabled = false;
            component.cutoffFrequency = 22000f;
            audioSource.GetComponent<WorldSoundGOC>().enabled = false;
        }

        public AudioSource GetPooledAudioSource()
        {
            if (this.audioSourcePool.Count != 0)
            {
                AudioSource audioSource = this.audioSourcePool[this.audioSourcePool.Count - 1];
                this.audioSourcePool.RemoveAt(this.audioSourcePool.Count - 1);
                audioSource.gameObject.SetActive(true);
                return audioSource;
            }
            GameObject gameObject = new GameObject("AudioSource");
            AudioSource audioSource2 = gameObject.AddComponent<AudioSource>();
            audioSource2.maxDistance = 10f;
            audioSource2.rolloffMode = AudioRolloffMode.Linear;
            audioSource2.playOnAwake = false;
            AudioLowPassFilter audioLowPassFilter = gameObject.AddComponent<AudioLowPassFilter>();
            audioLowPassFilter.enabled = false;
            audioLowPassFilter.cutoffFrequency = 22000f;
            gameObject.AddComponent<WorldSoundGOC>().enabled = false;
            this.ResetValuesToDefault(audioSource2);
            gameObject.SetActive(true);
            return audioSource2;
        }

        private OneShotSounds oneShots = new OneShotSounds();

        private SustainingSounds sustaining = new SustainingSounds();

        private EntitiesWithSoundLoop withSoundLoop = new EntitiesWithSoundLoop();

        private MusicManager musicManager = new MusicManager();

        private AmbientSoundsManager ambientSoundsManager = new AmbientSoundsManager();

        private List<AudioSource> audioSourcePool = new List<AudioSource>(8);

        public const float MaxDistance = 10f;

        private const int InitialAudioSourcePoolSize = 8;
    }
}