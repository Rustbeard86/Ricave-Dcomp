using System;
using Ricave.UI;
using UnityEngine;

namespace Ricave.Core
{
    public class NativeWeapon : IUsable, ITipSubject
    {
        public Actor Actor
        {
            get
            {
                return this.actor;
            }
        }

        public int PropsIndex
        {
            get
            {
                return this.propsIndex;
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
                return this.missUseEffects;
            }
        }

        public NativeWeaponProps Props
        {
            get
            {
                return this.actor.Spec.Actor.NativeWeapons[this.propsIndex];
            }
        }

        public Texture2D Icon
        {
            get
            {
                if (!this.actor.IsMainActor)
                {
                    return NativeWeapon.NonPlayerNativeWeaponIcon;
                }
                return NativeWeapon.PlayerNativeWeaponIcon;
            }
        }

        public string LabelCap
        {
            get
            {
                return this.Props.LabelCap;
            }
        }

        string IUsable.UseLabel_Self
        {
            get
            {
                return UsableUtility.CheckAppendDotsToUseLabel("NativeWeaponUseLabel_Self".Translate(), this);
            }
        }

        string IUsable.UseLabel_Other
        {
            get
            {
                return "NativeWeaponUseLabel_Other".Translate();
            }
        }

        string IUsable.UseDescriptionFormat_Self
        {
            get
            {
                return "AttackDescription_Self".Translate();
            }
        }

        string IUsable.UseDescriptionFormat_Other
        {
            get
            {
                return "AttackDescription_Other".Translate();
            }
        }

        TargetFilter IUsable.UseFilter
        {
            get
            {
                return this.Props.UseFilter;
            }
        }

        TargetFilter IUsable.UseFilterAoE
        {
            get
            {
                return this.Props.UseFilter;
            }
        }

        public int UseRange
        {
            get
            {
                return this.Props.UseRange;
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
                return this.Props.SequencePerUseMultiplier;
            }
        }

        int IUsable.MyStableHash
        {
            get
            {
                return Calc.CombineHashes<int, int, int>(this.actor.MyStableHash, this.propsIndex, 410900513);
            }
        }

        public int ManaCost
        {
            get
            {
                return this.Props.ManaCost;
            }
        }

        public int StaminaCost
        {
            get
            {
                return this.Props.StaminaCost;
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
                return this.Props.MissChance;
            }
        }

        public float CritChance
        {
            get
            {
                return this.Props.CritChance;
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
                return "NativeWeaponDescription".Translate();
            }
        }

        Color ITipSubject.IconColor
        {
            get
            {
                return Color.white;
            }
        }

        protected NativeWeapon()
        {
        }

        public NativeWeapon(Actor actor, int propsIndex)
        {
            this.actor = actor;
            this.propsIndex = propsIndex;
            this.useEffects = new UseEffects(this);
            if (this.Props.DefaultUseEffects != null)
            {
                this.useEffects.AddClonedFrom(this.Props.DefaultUseEffects);
            }
            this.missUseEffects = new UseEffects(this);
            if (this.Props.MissUseEffects != null)
            {
                this.missUseEffects.AddClonedFrom(this.Props.MissUseEffects);
            }
        }

        public bool CanUse_ExtraInstanceSpecificChecks(Actor user, Vector3Int? assumeUserPos = null, bool assumeAnyUserPos = false, StringSlot outReason = null)
        {
            if (this.actor != user)
            {
                return false;
            }
            if (this.Props.RequiresArms && !user.HasArm)
            {
                if (outReason != null)
                {
                    outReason.Set("NoArms".Translate());
                }
                return false;
            }
            return true;
        }

        [Saved]
        private Actor actor;

        [Saved]
        private int propsIndex;

        [Saved]
        private UseEffects useEffects;

        [Saved]
        private UseEffects missUseEffects;

        [Saved]
        private int? lastUsedToRewindTimeSequence;

        [Saved]
        private int? lastUseSequence;

        private static readonly Texture2D PlayerNativeWeaponIcon = Assets.Get<Texture2D>("Textures/UI/NativeWeapon_Player");

        public static readonly Texture2D NonPlayerNativeWeaponIcon = Assets.Get<Texture2D>("Textures/UI/NativeWeapon_NonPlayer");
    }
}