using System;
using System.Linq;
using Ricave.UI;
using UnityEngine;

namespace Ricave.Core
{
    public class GenericUsable : IUsable, ITipSubject
    {
        public GenericUsableSpec Spec
        {
            get
            {
                return this.spec;
            }
        }

        public Entity ParentEntity
        {
            get
            {
                return this.parentEntity;
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

        public int StableID
        {
            get
            {
                return this.stableID;
            }
        }

        public int MyStableHash
        {
            get
            {
                return this.stableID;
            }
        }

        public string Label
        {
            get
            {
                return this.spec.Label;
            }
        }

        public string LabelCap
        {
            get
            {
                return this.Label.CapitalizeFirst();
            }
        }

        public string Description
        {
            get
            {
                return this.spec.Description;
            }
        }

        Texture2D ITipSubject.Icon
        {
            get
            {
                return this.Spec.Icon ?? NativeWeapon.NonPlayerNativeWeaponIcon;
            }
        }

        Color ITipSubject.IconColor
        {
            get
            {
                return Color.white;
            }
        }

        public string UseLabel_Self
        {
            get
            {
                return UsableUtility.CheckAppendDotsToUseLabel(this.spec.UseLabelKey_Self.Translate(), this);
            }
        }

        public string UseLabel_Other
        {
            get
            {
                return this.spec.UseLabelKey_Other.Translate();
            }
        }

        public string UseDescriptionFormat_Self
        {
            get
            {
                return this.spec.UseDescriptionFormatKey_Self.Translate();
            }
        }

        public string UseDescriptionFormat_Other
        {
            get
            {
                return this.spec.UseDescriptionFormatKey_Other.Translate();
            }
        }

        public TargetFilter UseFilter
        {
            get
            {
                return this.spec.UseFilter;
            }
        }

        public TargetFilter UseFilterAoE
        {
            get
            {
                return this.spec.UseFilterAoE ?? this.spec.UseFilter;
            }
        }

        public int UseRange
        {
            get
            {
                return this.spec.UseRange;
            }
        }

        public int? LastUsedToRewindTimeSequence
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

        public int? LastUseSequence
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

        public int? CanRewindTimeEveryTurns
        {
            get
            {
                return this.spec.CanRewindTimeEveryTurns;
            }
        }

        public float SequencePerUseMultiplier
        {
            get
            {
                return this.spec.SequencePerUseMultiplier;
            }
        }

        public int ManaCost
        {
            get
            {
                return this.spec.ManaCost;
            }
        }

        public int StaminaCost
        {
            get
            {
                return this.spec.StaminaCost;
            }
        }

        public int CooldownTurns
        {
            get
            {
                return this.spec.CooldownTurns;
            }
        }

        public float MissChance
        {
            get
            {
                return this.spec.MissChance;
            }
        }

        public float CritChance
        {
            get
            {
                return this.spec.CritChance;
            }
        }

        public UsePrompt UsePrompt
        {
            get
            {
                return this.spec.UsePrompt;
            }
        }

        protected GenericUsable()
        {
        }

        public GenericUsable(GenericUsableSpec spec, Entity parentEntity = null)
        {
            this.spec = spec;
            this.parentEntity = parentEntity;
            if (Get.World == null)
            {
                this.stableID = Rand.UniqueID(Enumerable.Empty<int>());
            }
            else
            {
                this.stableID = Rand.UniqueID(from x in Get.World.Actors.SelectMany<Actor, GenericUsable>((Actor x) => x.Abilities)
                                              select x.StableID);
            }
            this.useEffects = new UseEffects(this);
            this.missUseEffects = new UseEffects(this);
            UseEffects defaultUseEffects = spec.DefaultUseEffects;
            if (defaultUseEffects != null)
            {
                this.useEffects.AddClonedFrom(defaultUseEffects);
            }
            UseEffects useEffects = spec.MissUseEffects;
            if (useEffects != null)
            {
                this.missUseEffects.AddClonedFrom(useEffects);
            }
        }

        public bool CanUse_ExtraInstanceSpecificChecks(Actor user, Vector3Int? assumeUserPos = null, bool assumeAnyUserPos = false, StringSlot outReason = null)
        {
            if (this.spec.DisallowIfHostile && user != null)
            {
                Actor actor = this.parentEntity as Actor;
                if (actor != null && user.IsHostile(actor))
                {
                    if (outReason != null)
                    {
                        outReason.Set("Hostile".Translate());
                    }
                    return false;
                }
            }
            return true;
        }

        [Saved]
        private GenericUsableSpec spec;

        [Saved]
        private Entity parentEntity;

        [Saved]
        private UseEffects useEffects;

        [Saved]
        private UseEffects missUseEffects;

        [Saved]
        private int? lastUsedToRewindTimeSequence;

        [Saved]
        private int? lastUseSequence;

        [Saved(-1, false)]
        private int stableID = -1;
    }
}