using System;
using System.Linq;
using Ricave.UI;
using UnityEngine;

namespace Ricave.Core
{
    public class Spell : IUsable, ITipSubject
    {
        public Spells Parent
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

        public SpellSpec Spec
        {
            get
            {
                return this.spec;
            }
        }

        public UseEffects UseEffects
        {
            get
            {
                return this.useEffects;
            }
        }

        UseEffects IUsable.MissUseEffects
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
                return this.Spec.Icon;
            }
        }

        public Color IconColor
        {
            get
            {
                return this.Spec.IconColor;
            }
        }

        public string Description
        {
            get
            {
                return this.Spec.Description;
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

        public float TimeGained
        {
            get
            {
                return this.timeGained;
            }
        }

        public string LabelCap
        {
            get
            {
                return this.spec.LabelCap;
            }
        }

        public string UseLabel_Self
        {
            get
            {
                return UsableUtility.CheckAppendDotsToUseLabel("SpellUseLabel_Self".Translate(), this);
            }
        }

        public string UseLabel_Other
        {
            get
            {
                return "SpellUseLabel_Other".Translate();
            }
        }

        string IUsable.UseDescriptionFormat_Self
        {
            get
            {
                return "SpellUseDescription_Self".Translate();
            }
        }

        string IUsable.UseDescriptionFormat_Other
        {
            get
            {
                return "SpellUseDescription_Other".Translate();
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
                return this.spec.UseFilterAoE ?? this.UseFilter;
            }
        }

        public int UseRange
        {
            get
            {
                return this.spec.UseRange;
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
                return this.spec.UsePrompt;
            }
        }

        protected Spell()
        {
        }

        public Spell(SpellSpec spec)
        {
            this.spec = spec;
            if (Get.World == null)
            {
                this.stableID = Rand.UniqueID(Enumerable.Empty<int>());
            }
            else
            {
                this.stableID = Rand.UniqueID(from x in Get.World.Actors.Where<Actor>((Actor x) => x.Spells != null).SelectMany<Actor, Spell>((Actor x) => x.Spells.All)
                                              select x.StableID);
            }
            this.useEffects = new UseEffects(this);
            if (spec.DefaultUseEffects != null)
            {
                this.useEffects.AddClonedFrom(spec.DefaultUseEffects);
            }
        }

        public bool CanUse_ExtraInstanceSpecificChecks(Actor user, Vector3Int? assumeUserPos = null, bool assumeAnyUserPos = false, StringSlot outReason = null)
        {
            if (this.parent.Owner != user)
            {
                return false;
            }
            if (user.Spec.Actor.UsingSpellsRequiresArms && !user.HasArm)
            {
                if (outReason != null)
                {
                    outReason.Set("NoArms".Translate());
                }
                return false;
            }
            if (user.Spec.Actor.UsingSpellsRequiresEyes && !user.HasEyes)
            {
                if (outReason != null)
                {
                    outReason.Set("MissingBodyParts".Translate());
                }
                return false;
            }
            return true;
        }

        public void OnGained()
        {
            if (!Maker.MakingEntity)
            {
                this.timeGained = Clock.UnscaledTime;
            }
        }

        [Saved]
        private Spells parent;

        [Saved]
        private SpellSpec spec;

        [Saved]
        private UseEffects useEffects;

        [Saved(-1, false)]
        private int stableID = -1;

        [Saved]
        private int? lastUsedToRewindTimeSequence;

        [Saved]
        private int? lastUseSequence;

        private float timeGained = -99999f;
    }
}