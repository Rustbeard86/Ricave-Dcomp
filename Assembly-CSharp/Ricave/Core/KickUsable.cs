using System;
using Ricave.UI;
using UnityEngine;

namespace Ricave.Core
{
    public class KickUsable : IUsable, ITipSubject
    {
        public Actor Actor
        {
            get
            {
                return this.actor;
            }
        }

        public UseEffects UseEffects
        {
            get
            {
                return this.useEffects;
            }
        }

        public UseEffects MissUseEffects
        {
            get
            {
                return null;
            }
        }

        public Texture2D Icon
        {
            get
            {
                return KickUsable.KickIcon;
            }
        }

        public TargetFilter UseFilter
        {
            get
            {
                return this.useFilter;
            }
        }

        public string LabelCap
        {
            get
            {
                return "Kick".Translate();
            }
        }

        string IUsable.UseLabel_Self
        {
            get
            {
                return UsableUtility.CheckAppendDotsToUseLabel("KickUseLabel_Self".Translate(), this);
            }
        }

        string IUsable.UseLabel_Other
        {
            get
            {
                return "KickUseLabel_Other".Translate();
            }
        }

        string IUsable.UseDescriptionFormat_Self
        {
            get
            {
                return "KickDescription_Self".Translate();
            }
        }

        string IUsable.UseDescriptionFormat_Other
        {
            get
            {
                return "KickDescription_Other".Translate();
            }
        }

        TargetFilter IUsable.UseFilter
        {
            get
            {
                return this.useFilter;
            }
        }

        TargetFilter IUsable.UseFilterAoE
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
                return 1;
            }
        }

        int? IUsable.LastUsedToRewindTimeSequence
        {
            get
            {
                return this.lastUsedToRewindTimeSequence;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.lastUsedToRewindTimeSequence = value;
            }
        }

        int? IUsable.LastUseSequence
        {
            get
            {
                return this.lastUseSequence;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.lastUseSequence = value;
            }
        }

        int? IUsable.CanRewindTimeEveryTurns
        {
            get
            {
                return null;
            }
        }

        public float SequencePerUseMultiplier
        {
            get
            {
                return 1f;
            }
        }

        int IUsable.MyStableHash
        {
            get
            {
                return Calc.CombineHashes<int, int>(this.actor.MyStableHash, 456317908);
            }
        }

        public int ManaCost
        {
            get
            {
                return 0;
            }
        }

        public int StaminaCost
        {
            get
            {
                return 0;
            }
        }

        int IUsable.CooldownTurns
        {
            get
            {
                return 0;
            }
        }

        public float MissChance
        {
            get
            {
                return 0f;
            }
        }

        public float CritChance
        {
            get
            {
                return 0f;
            }
        }

        UsePrompt IUsable.UsePrompt
        {
            get
            {
                return null;
            }
        }

        string ITipSubject.Description
        {
            get
            {
                return "KickDescription".Translate();
            }
        }

        Color ITipSubject.IconColor
        {
            get
            {
                return Color.white;
            }
        }

        protected KickUsable()
        {
        }

        public KickUsable(Actor actor)
        {
            this.actor = actor;
            this.useEffects = new UseEffects(this);
            this.useEffects.AddUseEffect(new UseEffect_Kick(Get.UseEffect_Kick), -1);
            this.useFilter = TargetFilter.ForKick();
        }

        public bool CanUse_ExtraInstanceSpecificChecks(Actor user, Vector3Int? assumeUserPos = null, bool assumeAnyUserPos = false, StringSlot outReason = null)
        {
            if (this.actor != user)
            {
                return false;
            }
            if (!user.MovingAllowed)
            {
                if (outReason != null)
                {
                    outReason.Set("NoLegs".Translate());
                }
                return false;
            }
            return true;
        }

        [Saved]
        private Actor actor;

        [Saved]
        private UseEffects useEffects;

        [Saved]
        private TargetFilter useFilter;

        [Saved]
        private int? lastUsedToRewindTimeSequence;

        [Saved]
        private int? lastUseSequence;

        private static readonly Texture2D KickIcon = Assets.Get<Texture2D>("Textures/UI/Interaction/Kick1");
    }
}