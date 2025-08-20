using System;
using UnityEngine;

namespace Ricave.Core
{
    public class Instruction_VisualEffect : Instruction
    {
        public VisualEffectSpec Effect
        {
            get
            {
                return this.effect;
            }
        }

        public Vector3 Position
        {
            get
            {
                return this.position;
            }
        }

        public Quaternion Rotation
        {
            get
            {
                return this.rotation;
            }
        }

        public Vector3 Scale
        {
            get
            {
                return this.scale;
            }
        }

        protected Instruction_VisualEffect()
        {
        }

        public Instruction_VisualEffect(VisualEffectSpec effect, Vector3 position)
        {
            this.effect = effect;
            this.position = position;
            this.rotation = Quaternion.identity;
            this.scale = Vector3.one;
        }

        public Instruction_VisualEffect(VisualEffectSpec effect, Vector3 position, Quaternion rotation)
        {
            this.effect = effect;
            this.position = position;
            this.rotation = rotation;
            this.scale = Vector3.one;
        }

        public Instruction_VisualEffect(VisualEffectSpec effect, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            this.effect = effect;
            this.position = position;
            this.rotation = rotation;
            this.scale = scale;
        }

        protected override void DoImpl()
        {
            Get.VisualEffectsManager.DoOneShot(this.effect, this.position, this.rotation, this.scale, null, null, null);
        }

        protected override void UndoImpl()
        {
            if (this.effect.StrikeOverlay)
            {
                Get.StrikeOverlays.ToggleNextMirror();
            }
            Get.VisualEffectsManager.DoOneShot(this.effect, this.position, this.rotation, this.scale, null, null, null);
        }

        [Saved]
        private VisualEffectSpec effect;

        [Saved]
        private Vector3 position;

        [Saved(Default.Quaternion_Identity, false)]
        private Quaternion rotation = Quaternion.identity;

        [Saved(Default.Vector3_One, false)]
        private Vector3 scale = Vector3.one;
    }
}