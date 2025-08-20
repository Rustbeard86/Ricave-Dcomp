using System;
using System.Collections.Generic;
using Ricave.Core;
using UnityEngine;

namespace Ricave.Rendering
{
    public class OneShotSounds
    {
        public void PlayOneShotAt(AudioClip clip, Vector3 pos, float volume = 1f, float pitch = 1f)
        {
            AudioSource pooledAudioSource = Get.Audio.GetPooledAudioSource();
            pooledAudioSource.ConfigureAsWorldSound();
            pooledAudioSource.transform.SetParent(Get.AudioSourceContainer.transform, false);
            pooledAudioSource.transform.position = pos;
            pooledAudioSource.clip = clip;
            pooledAudioSource.volume = volume;
            pooledAudioSource.pitch = pitch;
            pooledAudioSource.GetComponent<WorldSoundGOC>().OnAboutToPlay();
            pooledAudioSource.Play();
            this.oneShotsPlaying.Add(pooledAudioSource);
        }

        public void PlayOneShotOnCamera(AudioClip clip, float volume = 1f, float pitch = 1f)
        {
            AudioSource pooledAudioSource = Get.Audio.GetPooledAudioSource();
            pooledAudioSource.ConfigureAsUISound();
            pooledAudioSource.transform.SetParent(Get.CameraTransform, false);
            pooledAudioSource.transform.localPosition = default(Vector3);
            pooledAudioSource.clip = clip;
            pooledAudioSource.volume = volume;
            pooledAudioSource.pitch = pitch;
            pooledAudioSource.Play();
            this.oneShotsPlaying.Add(pooledAudioSource);
        }

        public void Update()
        {
            for (int i = this.oneShotsPlaying.Count - 1; i >= 0; i--)
            {
                AudioSource audioSource = this.oneShotsPlaying[i];
                if (!audioSource.isPlaying)
                {
                    Get.Audio.ReturnToPool(audioSource);
                    this.oneShotsPlaying.RemoveAt(i);
                }
            }
        }

        public void OnSceneChanged()
        {
            this.oneShotsPlaying.Clear();
        }

        private List<AudioSource> oneShotsPlaying = new List<AudioSource>(10);
    }
}