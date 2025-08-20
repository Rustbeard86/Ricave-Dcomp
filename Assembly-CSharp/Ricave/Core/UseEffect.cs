using System;
using System.Collections.Generic;
using Ricave.UI;
using UnityEngine;

namespace Ricave.Core
{
    public abstract class UseEffect
    {
        public UseEffects Parent
        {
            get
            {
                return this.parent;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.parent = value;
            }
        }

        public UseEffectSpec Spec
        {
            get
            {
                return this.spec;
            }
        }

        public Actor WieldingActor
        {
            get
            {
                UseEffects useEffects = this.parent;
                if (useEffects == null)
                {
                    return null;
                }
                return useEffects.GetWieldingActor();
            }
        }

        public float Chance
        {
            get
            {
                return this.chance;
            }
        }

        public int UsesLeft
        {
            get
            {
                return this.usesLeft;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.usesLeft = value;
            }
        }

        public int? AoERadius
        {
            get
            {
                return this.aoeRadius;
            }
        }

        public bool AoEXZOnly
        {
            get
            {
                return this.aoeXZOnly;
            }
        }

        public bool AoELineOnly
        {
            get
            {
                return this.aoeLineOnly;
            }
        }

        public bool AoELineStopOnImpassable
        {
            get
            {
                return this.aoeLineStopOnImpassable;
            }
        }

        public bool AoEExcludeCenter
        {
            get
            {
                return this.aoeExcludeCenter;
            }
        }

        public virtual bool Hidden
        {
            get
            {
                return this.hidden || this.spec.AlwaysHidden;
            }
        }

        public virtual Texture2D Icon
        {
            get
            {
                return this.Spec.Icon;
            }
        }

        public virtual Color IconColor
        {
            get
            {
                return this.GoodBadNeutral.GetColor();
            }
        }

        public virtual GoodBadNeutral GoodBadNeutral
        {
            get
            {
                return GoodBadNeutral.Neutral;
            }
        }

        protected virtual bool CanWriteAoERadius
        {
            get
            {
                return true;
            }
        }

        public virtual string LabelBase
        {
            get
            {
                return this.Spec.Label;
            }
        }

        public virtual bool AoEHandledManually
        {
            get
            {
                return false;
            }
        }

        public virtual bool HasCustomAoEArea
        {
            get
            {
                return false;
            }
        }

        public int InheritedRampUp
        {
            get
            {
                if (this.parent == null)
                {
                    return 0;
                }
                Item item = this.parent.Parent as Item;
                if (item != null)
                {
                    return item.RampUp;
                }
                NativeWeapon nativeWeapon = this.parent.Parent as NativeWeapon;
                if (nativeWeapon != null)
                {
                    Actor actor = nativeWeapon.Actor;
                    if (actor == null)
                    {
                        return 0;
                    }
                    return actor.RampUp;
                }
                else
                {
                    Spell spell = this.parent.Parent as Spell;
                    if (spell != null)
                    {
                        Spells spells = spell.Parent;
                        int? num;
                        if (spells == null)
                        {
                            num = null;
                        }
                        else
                        {
                            Actor owner = spells.Owner;
                            num = ((owner != null) ? new int?(owner.RampUp) : null);
                        }
                        int? num2 = num;
                        return num2.GetValueOrDefault();
                    }
                    GenericUsable genericUsable = this.parent.Parent as GenericUsable;
                    if (genericUsable == null)
                    {
                        return 0;
                    }
                    Actor actor2 = genericUsable.ParentEntity as Actor;
                    if (actor2 != null)
                    {
                        return actor2.RampUp;
                    }
                    Item item2 = genericUsable.ParentEntity as Item;
                    if (item2 != null)
                    {
                        return item2.RampUp;
                    }
                    return 0;
                }
            }
        }

        public string Label
        {
            get
            {
                string text;
                if (this.aoeRadius != null && this.CanWriteAoERadius)
                {
                    int? num = this.aoeRadius;
                    int num2 = 1;
                    if (((num.GetValueOrDefault() == num2) & (num != null)) && this.aoeExcludeCenter)
                    {
                        text = RichText.AoERadius(" ({0})".Formatted("AdjacentLower".Translate()));
                    }
                    else
                    {
                        text = RichText.AoERadius(" ({0})".Formatted("AoERadius".Translate(this.aoeRadius.Value)));
                    }
                }
                else
                {
                    text = "";
                }
                return StringUtility.Concat(this.LabelBase, text, (this.chance != 1f) ? RichText.Chance(" ({0})".Formatted("Chance".Translate(this.chance.ToStringPercent(false)))) : "", (this.usesLeft > 0) ? RichText.Uses(" ({0})".Formatted(StringUtility.UsesString(this.usesLeft))) : "");
            }
        }

        public string LabelCap
        {
            get
            {
                return this.Label.CapitalizeFirst();
            }
        }

        public virtual string Description
        {
            get
            {
                return this.Spec.Description;
            }
        }

        public int MyStableHash
        {
            get
            {
                return this.spec.MyStableHash;
            }
        }

        public virtual string Tip
        {
            get
            {
                return this.Description;
            }
        }

        public Func<string> CachedTipGetter
        {
            get
            {
                return this.cachedTipGetter;
            }
        }

        public Func<string> CachedLabelCapGetter
        {
            get
            {
                return this.cachedLabelCapGetter;
            }
        }

        protected UseEffect()
        {
            this.cachedTipGetter = () => this.Tip;
            this.cachedLabelCapGetter = () => this.LabelCap;
        }

        public UseEffect(UseEffectSpec spec, float chance = 1f, int usesLeft = 0, int? aoeRadius = null, bool hidden = false)
        {
            this.cachedTipGetter = () => this.Tip;
            this.cachedLabelCapGetter = () => this.LabelCap;
            this.spec = spec;
            this.chance = chance;
            this.usesLeft = usesLeft;
            this.aoeRadius = aoeRadius;
            this.hidden = hidden;
            if (spec.UseEffectClass != base.GetType())
            {
                string[] array = new string[5];
                array[0] = "Created a UseEffect with spec ";
                array[1] = ((spec != null) ? spec.ToString() : null);
                array[2] = ", but the created UseEffect type is ";
                int num = 3;
                Type type = base.GetType();
                array[num] = ((type != null) ? type.ToString() : null);
                array[4] = ".";
                Log.Warning(string.Concat(array), false);
            }
        }

        public UseEffect Clone()
        {
            UseEffect useEffect = Maker.Make(this.spec);
            useEffect.spec = this.spec;
            useEffect.chance = this.chance;
            useEffect.usesLeft = this.usesLeft;
            useEffect.hidden = this.hidden;
            useEffect.aoeRadius = this.aoeRadius;
            useEffect.aoeXZOnly = this.aoeXZOnly;
            useEffect.aoeLineOnly = this.aoeLineOnly;
            useEffect.aoeLineStopOnImpassable = this.aoeLineStopOnImpassable;
            useEffect.aoeExcludeCenter = this.aoeExcludeCenter;
            this.CopyFieldsTo(useEffect);
            return useEffect;
        }

        public abstract void CopyFieldsTo(UseEffect clone);

        public abstract IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart);

        public virtual bool PreventEntireUse(Actor user, IUsable usable, Target target, StringSlot outReason = null)
        {
            return false;
        }

        public virtual void GetCustomAoEArea(List<Vector3Int> outCells)
        {
        }

        public override string ToString()
        {
            if (this.Spec == null)
            {
                return base.ToString();
            }
            return this.Spec.SpecID;
        }

        [Saved]
        private UseEffects parent;

        [Saved]
        private UseEffectSpec spec;

        [Saved(1f, false)]
        private float chance = 1f;

        [Saved]
        private int usesLeft;

        [Saved]
        private bool hidden;

        [Saved]
        private int? aoeRadius;

        [Saved]
        private bool aoeXZOnly;

        [Saved]
        private bool aoeLineOnly;

        [Saved]
        private bool aoeLineStopOnImpassable;

        [Saved]
        private bool aoeExcludeCenter;

        private Func<string> cachedTipGetter;

        private Func<string> cachedLabelCapGetter;
    }
}