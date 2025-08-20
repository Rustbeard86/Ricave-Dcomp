using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class UseEffect_Sound : UseEffect
    {
        public SoundSpec SoundSpec
        {
            get
            {
                return this.soundSpec;
            }
        }

        public bool AtTarget
        {
            get
            {
                return this.atTarget;
            }
        }

        public bool AtSource
        {
            get
            {
                return this.atSource;
            }
        }

        public bool OnCameraIfPlayer
        {
            get
            {
                return this.onCameraIfPlayer;
            }
        }

        protected UseEffect_Sound()
        {
        }

        public UseEffect_Sound(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
            UseEffect_Sound useEffect_Sound = (UseEffect_Sound)clone;
            useEffect_Sound.soundSpec = this.soundSpec;
            useEffect_Sound.atTarget = this.atTarget;
            useEffect_Sound.atSource = this.atSource;
            useEffect_Sound.onCameraIfPlayer = this.onCameraIfPlayer;
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            if (this.onCameraIfPlayer && ((user != null && user.IsNowControlledActor) || target.IsNowControlledActor))
            {
                yield return new Instruction_Sound(this.soundSpec, null, 1f, 1f);
            }
            else if (this.atSource)
            {
                yield return new Instruction_Sound(this.soundSpec, new Vector3?(UseEffect_Sound.GetSourcePos(user, usable, target)), 1f, 1f);
            }
            else
            {
                yield return new Instruction_Sound(this.soundSpec, new Vector3?(target.Position), 1f, 1f);
            }
            yield break;
        }

        public static Vector3Int GetSourcePos(Actor user, IUsable usable, Target target)
        {
            Entity entity = usable as Entity;
            if (entity != null && entity.Spawned)
            {
                return entity.Position;
            }
            Item item = entity as Item;
            if (item != null && item.ParentInventory != null)
            {
                return item.ParentInventory.Owner.Position;
            }
            if (usable.UseEffects != null)
            {
                Actor wieldingActor = usable.UseEffects.GetWieldingActor();
                if (wieldingActor != null)
                {
                    return wieldingActor.Position;
                }
            }
            if (entity != null)
            {
                return entity.Position;
            }
            if (user != null)
            {
                return user.Position;
            }
            return target.Position;
        }

        [Saved]
        private SoundSpec soundSpec;

        [Saved]
        private bool atTarget;

        [Saved]
        private bool atSource;

        [Saved]
        private bool onCameraIfPlayer;
    }
}