using System;
using UnityEngine;

namespace Ricave.Core
{
    public class Instruction_Sound : Instruction
    {
        public SoundSpec Sound
        {
            get
            {
                return this.sound;
            }
        }

        public Vector3? Position
        {
            get
            {
                return this.position;
            }
        }

        public float Pitch
        {
            get
            {
                return this.pitch;
            }
        }

        public float Volume
        {
            get
            {
                return this.volume;
            }
        }

        protected Instruction_Sound()
        {
        }

        public Instruction_Sound(SoundSpec sound, Vector3? position = null, float pitch = 1f, float volume = 1f)
        {
            this.sound = sound;
            this.position = position;
            this.pitch = pitch;
            this.volume = volume;
        }

        protected override void DoImpl()
        {
            if (this.position == null || (this.position.Value - Get.CameraPosition).sqrMagnitude < 144f)
            {
                this.sound.PlayOneShot(this.position, this.volume, this.pitch);
            }
        }

        protected override void UndoImpl()
        {
            if (this.position == null || (this.position.Value - Get.CameraPosition).sqrMagnitude < 144f)
            {
                this.sound.PlayOneShot(this.position, this.volume, this.pitch);
            }
        }

        [Saved]
        private SoundSpec sound;

        [Saved]
        private Vector3? position;

        [Saved(1f, false)]
        private float pitch = 1f;

        [Saved(1f, false)]
        private float volume = 1f;
    }
}