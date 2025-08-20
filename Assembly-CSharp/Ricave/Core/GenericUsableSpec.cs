using System;
using UnityEngine;

namespace Ricave.Core
{
    public class GenericUsableSpec : Spec, ISaveableEventsReceiver
    {
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

        public TargetFilter UseFilterAoE
        {
            get
            {
                return this.useFilterAoE;
            }
        }

        public int UseRange
        {
            get
            {
                return this.useRange;
            }
        }

        public string UseDescriptionFormatKey_Self
        {
            get
            {
                return this.useDescriptionFormatKey_self;
            }
        }

        public string UseDescriptionFormatKey_Other
        {
            get
            {
                return this.useDescriptionFormatKey_other;
            }
        }

        public string UseLabelKey_Self
        {
            get
            {
                return this.useLabelKey_self;
            }
        }

        public string UseLabelKey_Other
        {
            get
            {
                return this.useLabelKey_other;
            }
        }

        public int? CanRewindTimeEveryTurns
        {
            get
            {
                return this.canRewindTimeEveryTurns;
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

        public UsePrompt UsePrompt
        {
            get
            {
                return this.usePrompt;
            }
        }

        public int CooldownTurns
        {
            get
            {
                return this.cooldownTurns;
            }
        }

        public Texture2D Icon
        {
            get
            {
                return this.icon;
            }
        }

        public bool DisallowIfHostile
        {
            get
            {
                return this.disallowIfHostile;
            }
        }

        public void OnLoaded()
        {
            if (!this.iconPath.NullOrEmpty())
            {
                this.icon = Assets.Get<Texture2D>(this.iconPath);
            }
        }

        public void OnSaved()
        {
        }

        public override void OnActiveLanguageChanged()
        {
            base.OnActiveLanguageChanged();
            TranslateUtility.ValidateTranslationKey(this.useDescriptionFormatKey_self);
            TranslateUtility.ValidateTranslationKey(this.useDescriptionFormatKey_other);
            TranslateUtility.ValidateTranslationKey(this.useLabelKey_self);
            TranslateUtility.ValidateTranslationKey(this.useLabelKey_other);
        }

        [Saved]
        private UseEffects defaultUseEffects;

        [Saved]
        private UseEffects missUseEffects;

        [Saved(Default.New, false)]
        private TargetFilter useFilter = new TargetFilter();

        [Saved]
        private TargetFilter useFilterAoE;

        [Saved(1, false)]
        private int useRange = 1;

        [Saved("UseDescription_Self", false)]
        private string useDescriptionFormatKey_self = "UseDescription_Self";

        [Saved("UseDescription_Other", false)]
        private string useDescriptionFormatKey_other = "UseDescription_Other";

        [Saved("UseLabel_Self", false)]
        private string useLabelKey_self = "UseLabel_Self";

        [Saved("UseLabel_Other", false)]
        private string useLabelKey_other = "UseLabel_Other";

        [Saved]
        private int? canRewindTimeEveryTurns;

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

        [Saved]
        private UsePrompt usePrompt;

        [Saved]
        private int cooldownTurns;

        [Saved]
        private string iconPath;

        [Saved]
        private bool disallowIfHostile;

        private Texture2D icon;
    }
}