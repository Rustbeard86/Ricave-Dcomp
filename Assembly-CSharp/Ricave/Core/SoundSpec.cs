using System;
using Ricave.Rendering;
using UnityEngine;

namespace Ricave.Core
{
    public class SoundSpec : Spec, ISaveableEventsReceiver
    {
        public string Path
        {
            get
            {
                return this.path;
            }
        }

        public AudioClip AudioClip
        {
            get
            {
                return this.audioClip;
            }
        }

        public FloatRange VolumeRange
        {
            get
            {
                return this.volumeRange;
            }
        }

        public FloatRange PitchRange
        {
            get
            {
                return this.pitchRange;
            }
        }

        public void PlayOneShot(Vector3? pos = null, float volumeFactor = 1f, float pitchFactor = 1f)
        {
            if (pos != null)
            {
                Get.OneShotSounds.PlayOneShotAt(this.AudioClip, pos.Value, this.volumeRange.RandomInRange * volumeFactor, this.pitchRange.RandomInRange * pitchFactor);
                return;
            }
            Get.OneShotSounds.PlayOneShotOnCamera(this.AudioClip, this.volumeRange.RandomInRange * volumeFactor, this.pitchRange.RandomInRange * pitchFactor);
        }

        public SoundHandle PlayWithHandle(Vector3? pos = null, bool loop = false)
        {
            SoundHandle soundHandle;
            if (pos != null)
            {
                soundHandle = Get.SustainingSounds.PlayWithHandleAt(this.AudioClip, pos.Value, loop);
            }
            else
            {
                soundHandle = Get.SustainingSounds.PlayWithHandleOnCamera(this.AudioClip, loop);
            }
            soundHandle.Volume = this.volumeRange.RandomInRange;
            soundHandle.Pitch = this.pitchRange.RandomInRange;
            return soundHandle;
        }

        public void OnLoaded()
        {
            if (!this.path.NullOrEmpty())
            {
                this.audioClip = Assets.Get<AudioClip>(this.path);
                return;
            }
            Log.Error("SoundSpec " + base.SpecID + " has no sound path.", false);
        }

        public void OnSaved()
        {
        }

        [Saved]
        private string path;

        [Saved(Default.FloatRange_One, false)]
        private FloatRange volumeRange = new FloatRange(1f, 1f);

        [Saved(Default.FloatRange_One, false)]
        private FloatRange pitchRange = new FloatRange(1f, 1f);

        private AudioClip audioClip;
    }
}