using System;
using Ricave.Core;
using UnityEngine;

namespace Ricave.Rendering
{
    public static class AudioUtility
    {
        public static void PlayOneShotAt(this AudioClip clip, Vector3 pos)
        {
            Get.OneShotSounds.PlayOneShotAt(clip, pos, 1f, 1f);
        }

        public static void PlayOneShotOnCamera(this AudioClip clip)
        {
            Get.OneShotSounds.PlayOneShotOnCamera(clip, 1f, 1f);
        }

        public static bool ReachedEnd(this AudioSource audioSource)
        {
            return audioSource.clip != null && audioSource.time >= audioSource.clip.length - 1E-05f;
        }

        public static void ConfigureAsUISound(this AudioSource audioSource)
        {
            audioSource.spatialBlend = 0f;
            audioSource.bypassReverbZones = true;
            audioSource.bypassEffects = true;
            audioSource.bypassListenerEffects = true;
        }

        public static void ConfigureAsWorldSound(this AudioSource audioSource)
        {
            audioSource.spatialBlend = 1f;
            audioSource.GetComponent<WorldSoundGOC>().enabled = true;
        }
    }
}