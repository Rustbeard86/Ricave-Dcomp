using System;
using Ricave.Core;
using UnityEngine;

namespace Ricave.Rendering
{
    public class SoundHandle : IDisposable
    {
        public bool IsValid
        {
            get
            {
                return this.audioSource != null;
            }
        }

        public bool IsPlaying
        {
            get
            {
                return this.IsValid && this.audioSource.isPlaying;
            }
        }

        public float Length
        {
            get
            {
                if (!this.IsValid)
                {
                    return 0f;
                }
                return this.audioSource.clip.length;
            }
        }

        public float Pitch
        {
            get
            {
                if (!this.IsValid)
                {
                    return 1f;
                }
                return this.audioSource.pitch;
            }
            set
            {
                if (!this.IsValid)
                {
                    Log.Warning("Tried to set pitch on an invalid sound handle.", false);
                    return;
                }
                this.audioSource.pitch = value;
            }
        }

        public float Volume
        {
            get
            {
                if (!this.IsValid)
                {
                    return 0f;
                }
                return this.audioSource.volume;
            }
            set
            {
                if (!this.IsValid)
                {
                    Log.Warning("Tried to set volume on an invalid sound handle.", false);
                    return;
                }
                this.audioSource.volume = value;
            }
        }

        public float SpatialBlend
        {
            get
            {
                if (!this.IsValid)
                {
                    return 1f;
                }
                return this.audioSource.spatialBlend;
            }
            set
            {
                if (!this.IsValid)
                {
                    Log.Warning("Tried to set spatial blend on an invalid sound handle.", false);
                    return;
                }
                this.audioSource.spatialBlend = value;
            }
        }

        public float Time
        {
            get
            {
                if (!this.IsValid)
                {
                    return 0f;
                }
                return this.audioSource.time;
            }
            set
            {
                if (!this.IsValid)
                {
                    Log.Warning("Tried to set time on an invalid sound handle.", false);
                    return;
                }
                this.audioSource.time = value;
            }
        }

        public Vector3 Position
        {
            get
            {
                if (!this.IsValid)
                {
                    return Vector3.zero;
                }
                return this.audioSource.transform.position;
            }
            set
            {
                if (!this.IsValid)
                {
                    Log.Warning("Tried to set position on an invalid sound handle.", false);
                    return;
                }
                this.audioSource.transform.position = value;
            }
        }

        public float MaxDistance
        {
            get
            {
                if (!this.IsValid)
                {
                    return 10f;
                }
                return this.audioSource.maxDistance;
            }
            set
            {
                if (!this.IsValid)
                {
                    Log.Warning("Tried to set max distance on an invalid sound handle.", false);
                    return;
                }
                this.audioSource.maxDistance = value;
            }
        }

        public SoundHandle(AudioSource audioSource)
        {
            this.audioSource = audioSource;
        }

        public void Play()
        {
            if (!this.IsValid)
            {
                Log.Warning("Tried to play an invalid sound handle.", false);
                return;
            }
            WorldSoundGOC component = this.audioSource.GetComponent<WorldSoundGOC>();
            if (component.enabled)
            {
                component.OnAboutToPlay();
            }
            this.audioSource.Play();
        }

        public void UnPause()
        {
            if (!this.IsValid)
            {
                Log.Warning("Tried to unpause an invalid sound handle.", false);
                return;
            }
            this.audioSource.UnPause();
        }

        public void Pause()
        {
            if (!this.IsValid)
            {
                return;
            }
            this.audioSource.Pause();
        }

        public void Stop()
        {
            if (!this.IsValid)
            {
                return;
            }
            this.audioSource.Stop();
        }

        public void SetVolumeAndStartOrStop(float volume)
        {
            if (!this.IsValid)
            {
                Log.Warning("Tried to set volume on an invalid sound handle.", false);
                return;
            }
            this.Volume = volume;
            if (volume <= 0f && this.IsPlaying)
            {
                this.Stop();
            }
            if (volume > 0f && !this.IsPlaying)
            {
                this.Play();
            }
        }

        private void Cleanup()
        {
            if (!this.IsValid)
            {
                return;
            }
            Get.SustainingSounds.OnSoundHandleCleanup(this.audioSource);
            this.audioSource = null;
        }

        public void Dispose()
        {
            this.Cleanup();
        }

        public static void DisposeIfFinished(ref SoundHandle handle)
        {
            if (handle != null && !handle.IsPlaying)
            {
                handle.Dispose();
                handle = null;
            }
        }

        public static void DisposeIfNotNull(ref SoundHandle handle)
        {
            if (handle != null)
            {
                handle.Dispose();
                handle = null;
            }
        }

        private AudioSource audioSource;
    }
}