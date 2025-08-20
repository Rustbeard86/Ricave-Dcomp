using System;
using Ricave.Core;
using UnityEngine;

namespace Ricave.Rendering
{
    public class SustainingSounds
    {
        public SoundHandle CreateHandleAt(AudioClip clip, Vector3 pos, bool loop = false)
        {
            AudioSource pooledAudioSource = Get.Audio.GetPooledAudioSource();
            pooledAudioSource.ConfigureAsWorldSound();
            pooledAudioSource.transform.SetParent(Get.AudioSourceContainer.transform);
            pooledAudioSource.transform.position = pos;
            pooledAudioSource.clip = clip;
            pooledAudioSource.loop = loop;
            return new SoundHandle(pooledAudioSource);
        }

        public SoundHandle PlayWithHandleAt(AudioClip clip, Vector3 pos, bool loop = false)
        {
            SoundHandle soundHandle = this.CreateHandleAt(clip, pos, loop);
            soundHandle.Play();
            return soundHandle;
        }

        public SoundHandle CreateHandleOnCamera(AudioClip clip, bool loop = false)
        {
            AudioSource pooledAudioSource = Get.Audio.GetPooledAudioSource();
            pooledAudioSource.ConfigureAsUISound();
            pooledAudioSource.transform.SetParent(Get.CameraTransform);
            pooledAudioSource.transform.position = default(Vector3);
            pooledAudioSource.clip = clip;
            pooledAudioSource.loop = loop;
            return new SoundHandle(pooledAudioSource);
        }

        public SoundHandle PlayWithHandleOnCamera(AudioClip clip, bool loop = false)
        {
            SoundHandle soundHandle = this.CreateHandleOnCamera(clip, loop);
            soundHandle.Play();
            return soundHandle;
        }

        public void OnSoundHandleCleanup(AudioSource audioSource)
        {
            Get.Audio.ReturnToPool(audioSource);
        }
    }
}