using System;
using Ricave.UI;
using UnityEngine;

namespace Ricave.Core
{
    public class SpellSpec : Spec, ITipSubject, ISaveableEventsReceiver
    {
        public UseEffects DefaultUseEffects
        {
            get
            {
                return this.defaultUseEffects;
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

        public int CooldownTurns
        {
            get
            {
                return 0;
            }
        }

        public Texture2D Icon
        {
            get
            {
                return this.icon;
            }
        }

        public Color IconColor
        {
            get
            {
                if (!this.IsAbilityLike)
                {
                    return Color.white;
                }
                return new Color(1f, 1f, 0.5f);
            }
        }

        public bool AllowUseOnSelfViaInterface
        {
            get
            {
                return this.allowUseOnSelfViaInterface;
            }
        }

        public bool CanAppearAsChoiceForPlayer
        {
            get
            {
                return this.canAppearAsChoiceForPlayer;
            }
        }

        public UsePrompt UsePrompt
        {
            get
            {
                return this.usePrompt;
            }
        }

        public bool IsAbilityLike
        {
            get
            {
                return this.staminaCost != 0 && this.manaCost == 0;
            }
        }

        public bool IsSupportSpellForAI
        {
            get
            {
                return this.isSupportSpellForAI;
            }
        }

        public void OnLoaded()
        {
            if (!this.iconPath.NullOrEmpty())
            {
                this.icon = Assets.Get<Texture2D>(this.iconPath);
                return;
            }
            Log.Error("SpellSpec " + base.SpecID + " has no icon.", false);
        }

        public void OnSaved()
        {
        }

        [Saved]
        private UseEffects defaultUseEffects;

        [Saved(Default.New, false)]
        private TargetFilter useFilter = new TargetFilter();

        [Saved]
        private TargetFilter useFilterAoE;

        [Saved(1, false)]
        private int useRange = 1;

        [Saved]
        private string iconPath;

        [Saved(1f, false)]
        private float sequencePerUseMultiplier = 1f;

        [Saved]
        private int manaCost;

        [Saved]
        private int staminaCost;

        [Saved(true, false)]
        private bool allowUseOnSelfViaInterface = true;

        [Saved(true, false)]
        private bool canAppearAsChoiceForPlayer = true;

        [Saved]
        private UsePrompt usePrompt;

        [Saved]
        private bool isSupportSpellForAI;

        private Texture2D icon;
    }
}