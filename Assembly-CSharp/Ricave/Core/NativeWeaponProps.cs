using System;

namespace Ricave.Core
{
    public class NativeWeaponProps
    {
        public string Label
        {
            get
            {
                return this.label;
            }
        }

        public UseEffects DefaultUseEffects
        {
            get
            {
                return this.defaultUseEffects;
            }
        }

        public UseEffects MissUseEffects
        {
            get
            {
                return this.missUseEffects;
            }
        }

        public TargetFilter UseFilter
        {
            get
            {
                return this.useFilter;
            }
        }

        public int UseRange
        {
            get
            {
                return this.useRange;
            }
        }

        public bool RequiresArms
        {
            get
            {
                return this.requiresArms;
            }
        }

        public float SequencePerUseMultiplier
        {
            get
            {
                return this.sequencePerUseMultiplier;
            }
        }

        public int ManaCost
        {
            get
            {
                return this.manaCost;
            }
        }

        public int StaminaCost
        {
            get
            {
                return this.staminaCost;
            }
        }

        public float MissChance
        {
            get
            {
                return this.missChance;
            }
        }

        public float CritChance
        {
            get
            {
                return this.critChance;
            }
        }

        public string LabelCap
        {
            get
            {
                if (this.labelCapCached == null && this.label != null)
                {
                    this.labelCapCached = this.label.CapitalizeFirst();
                }
                return this.labelCapCached;
            }
        }

        [Saved]
        [Translatable]
        private string label;

        [Saved]
        private UseEffects defaultUseEffects;

        [Saved]
        private UseEffects missUseEffects;

        [Saved(Default.New, false)]
        private TargetFilter useFilter = new TargetFilter();

        [Saved(1, false)]
        private int useRange = 1;

        [Saved]
        private bool requiresArms;

        [Saved(1f, false)]
        private float sequencePerUseMultiplier = 1f;

        [Saved]
        private int manaCost;

        [Saved]
        private int staminaCost;

        [Saved]
        private float missChance;

        [Saved]
        private float critChance;

        private string labelCapCached;
    }
}