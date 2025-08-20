using System;
using System.Collections.Generic;
using Ricave.UI;
using UnityEngine;

namespace Ricave.Core
{
    public class UseEffect_Pay : UseEffect
    {
        public override GoodBadNeutral GoodBadNeutral
        {
            get
            {
                return GoodBadNeutral.Bad;
            }
        }

        public PriceTag Price
        {
            get
            {
                return this.price;
            }
        }

        public bool PaidItemsDisappear
        {
            get
            {
                return this.paidItemsDisappear;
            }
        }

        public override string LabelBase
        {
            get
            {
                return base.Spec.LabelFormat.Formatted(this.price).AppendedWithSpace(RichText.Grayed("({0})".Formatted("YouHave".Translate(this.price.HasCount(Get.NowControlledActor)))));
            }
        }

        public override Texture2D Icon
        {
            get
            {
                return this.price.ItemSpec.IconAdjusted;
            }
        }

        public override Color IconColor
        {
            get
            {
                return this.price.ItemSpec.IconColorAdjusted;
            }
        }

        public override bool Hidden
        {
            get
            {
                return this.CanBeLockpicked(Get.NowControlledActor) || base.Hidden;
            }
        }

        protected UseEffect_Pay()
        {
        }

        public UseEffect_Pay(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
            UseEffect_Pay useEffect_Pay = (UseEffect_Pay)clone;
            useEffect_Pay.price = this.price;
            useEffect_Pay.paidItemsDisappear = this.paidItemsDisappear;
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            if (this.CanBeLockpicked(user))
            {
                yield break;
            }
            if (user == null)
            {
                yield break;
            }
            if (!this.price.CanAfford(user))
            {
                yield break;
            }
            GenericUsable genericUsable = usable as GenericUsable;
            Actor actor2;
            if (genericUsable != null)
            {
                Actor actor = genericUsable.ParentEntity as Actor;
                if (actor != null && !this.paidItemsDisappear)
                {
                    actor2 = actor;
                    goto IL_0096;
                }
            }
            actor2 = null;
        IL_0096:
            foreach (Instruction instruction in this.price.MakePayInstructions(user, actor2))
            {
                yield return instruction;
            }
            IEnumerator<Instruction> enumerator = null;
            yield break;
            yield break;
        }

        public override bool PreventEntireUse(Actor user, IUsable usable, Target target, StringSlot outReason = null)
        {
            if (this.CanBeLockpicked(user))
            {
                return false;
            }
            if (user == null || !this.price.CanAfford(user))
            {
                if (outReason != null)
                {
                    outReason.Set("CantAfford".Translate(this.price));
                }
                return true;
            }
            return false;
        }

        private bool CanBeLockpicked(Actor user)
        {
            if (Get.Skill_Lockpicking.IsUnlocked() && user != null && user.IsMainActor)
            {
                UseEffects parent = base.Parent;
                Structure structure = ((parent != null) ? parent.Parent : null) as Structure;
                if (structure != null)
                {
                    return SkillUtility.IsLockpickable(structure);
                }
            }
            return false;
        }

        [Saved]
        private PriceTag price;

        [Saved]
        private bool paidItemsDisappear;
    }
}