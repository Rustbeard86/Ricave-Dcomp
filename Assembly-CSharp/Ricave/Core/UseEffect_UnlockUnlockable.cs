using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class UseEffect_UnlockUnlockable : UseEffect
    {
        public override GoodBadNeutral GoodBadNeutral
        {
            get
            {
                return GoodBadNeutral.Good;
            }
        }

        public UnlockableSpec UnlockableSpec
        {
            get
            {
                return this.unlockableSpec;
            }
        }

        public override string LabelBase
        {
            get
            {
                return base.Spec.LabelFormat.Formatted(this.unlockableSpec);
            }
        }

        public override Texture2D Icon
        {
            get
            {
                return this.unlockableSpec.Icon ?? base.Icon;
            }
        }

        public override Color IconColor
        {
            get
            {
                if (!(this.unlockableSpec.Icon != null))
                {
                    return base.IconColor;
                }
                return Color.white;
            }
        }

        protected UseEffect_UnlockUnlockable()
        {
        }

        public UseEffect_UnlockUnlockable(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public UseEffect_UnlockUnlockable(UseEffectSpec spec, UnlockableSpec unlockableSpec, bool hidden = false)
            : base(spec, 1f, 0, null, hidden)
        {
            this.unlockableSpec = unlockableSpec;
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
            ((UseEffect_UnlockUnlockable)clone).unlockableSpec = this.unlockableSpec;
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            if (!this.unlockableSpec.IsUnlocked())
            {
                yield return new Instruction_UnlockUnlockable(this.unlockableSpec);
            }
            yield break;
        }

        [Saved]
        private UnlockableSpec unlockableSpec;
    }
}