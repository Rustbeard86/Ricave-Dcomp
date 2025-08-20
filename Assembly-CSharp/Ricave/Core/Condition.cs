using System;
using System.Collections.Generic;
using System.Linq;
using Ricave.UI;
using UnityEngine;

namespace Ricave.Core
{
    public abstract class Condition : ISequenceable
    {
        public Conditions Parent
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

        public ConditionSpec Spec
        {
            get
            {
                return this.spec;
            }
        }

        public Actor AffectedActor
        {
            get
            {
                Conditions conditions = this.parent;
                if (conditions == null)
                {
                    return null;
                }
                return conditions.GetAffectedActor();
            }
        }

        public virtual bool Hidden
        {
            get
            {
                return this.hidden || this.spec.AlwaysHidden;
            }
        }

        public virtual bool AnyForcedAction
        {
            get
            {
                return false;
            }
        }

        public virtual string LabelBase
        {
            get
            {
                return this.Spec.Label;
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
                Actor actor = this.parent.Parent as Actor;
                if (actor != null)
                {
                    return actor.RampUp;
                }
                return 0;
            }
        }

        public string Label
        {
            get
            {
                return StringUtility.Concat(this.LabelBase, (this.turnsLeft > 0) ? RichText.Turns(" ({0})".Formatted(StringUtility.TurnsString(this.turnsLeft))) : "");
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

        public virtual bool CanFly
        {
            get
            {
                return false;
            }
        }

        public virtual bool DisableFlying
        {
            get
            {
                return false;
            }
        }

        public virtual bool CanJumpOffLedge
        {
            get
            {
                return false;
            }
        }

        public virtual bool DisableCanJumpOffLedge
        {
            get
            {
                return false;
            }
        }

        public virtual bool AllowPathingIntoAIAvoids
        {
            get
            {
                return false;
            }
        }

        public virtual int MaxHPOffset
        {
            get
            {
                return 0;
            }
        }

        public virtual float MaxHPFactor
        {
            get
            {
                return 1f;
            }
        }

        public virtual int MaxManaOffset
        {
            get
            {
                return 0;
            }
        }

        public virtual int MaxStaminaOffset
        {
            get
            {
                return 0;
            }
        }

        public virtual ValueTuple<IntRange, DamageTypeSpec> DealtDamageOffset
        {
            get
            {
                return default(ValueTuple<IntRange, DamageTypeSpec>);
            }
        }

        public virtual ValueTuple<IntRange, DamageTypeSpec> IncomingDamageOffset
        {
            get
            {
                return default(ValueTuple<IntRange, DamageTypeSpec>);
            }
        }

        public virtual ValueTuple<float, DamageTypeSpec> DealtDamageFactor
        {
            get
            {
                return new ValueTuple<float, DamageTypeSpec>(1f, null);
            }
        }

        public virtual ValueTuple<float, DamageTypeSpec> IncomingDamageFactor
        {
            get
            {
                return new ValueTuple<float, DamageTypeSpec>(1f, null);
            }
        }

        public virtual bool ImmuneToPoison
        {
            get
            {
                return false;
            }
        }

        public virtual bool ImmuneToFire
        {
            get
            {
                return false;
            }
        }

        public virtual bool ImmuneToDisease
        {
            get
            {
                return false;
            }
        }

        public virtual int SeeRangeOffset
        {
            get
            {
                return 0;
            }
        }

        public virtual int SeeRangeCap
        {
            get
            {
                return 10;
            }
        }

        public virtual int SequencePerTurnMultiplier
        {
            get
            {
                return 1;
            }
        }

        public virtual bool SequencePerTurnMultiplierInvert
        {
            get
            {
                return false;
            }
        }

        public virtual bool InvertHostile
        {
            get
            {
                return false;
            }
        }

        public virtual bool StopIdentification
        {
            get
            {
                return false;
            }
        }

        public virtual bool DisableNativeHPRegen
        {
            get
            {
                return false;
            }
        }

        public virtual bool DisableNativeStaminaRegen
        {
            get
            {
                return false;
            }
        }

        public virtual bool DisableNativeManaRegen
        {
            get
            {
                return false;
            }
        }

        public virtual int NativeStaminaRegenIntervalFactor
        {
            get
            {
                return 1;
            }
        }

        public virtual bool StopDanceAnimation
        {
            get
            {
                return false;
            }
        }

        public virtual bool Lying
        {
            get
            {
                return false;
            }
        }

        public virtual bool DisableDamageDealtToPlayer
        {
            get
            {
                return false;
            }
        }

        public virtual float SequencePerMoveMultiplier
        {
            get
            {
                return 1f;
            }
        }

        public virtual bool MovingDisallowed
        {
            get
            {
                return false;
            }
        }

        public virtual bool MovingDisallowedIfCantFly
        {
            get
            {
                return false;
            }
        }

        public virtual bool LevitatingDisabledByBodyParts
        {
            get
            {
                return false;
            }
        }

        public virtual float ActorScaleFactor
        {
            get
            {
                return this.Spec.ActorScaleFactor;
            }
        }

        public virtual float MinMissChanceOverride
        {
            get
            {
                return 0f;
            }
        }

        public virtual float HungerRateMultiplier
        {
            get
            {
                return 1f;
            }
        }

        public virtual float IdentificationRateMultiplier
        {
            get
            {
                return 1f;
            }
        }

        public virtual float CritChanceFactor
        {
            get
            {
                return 1f;
            }
        }

        public virtual float MissChanceFactor
        {
            get
            {
                return 1f;
            }
        }

        public int TurnsLeft
        {
            get
            {
                return this.turnsLeft;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.turnsLeft = value;
            }
        }

        public int MyStableHash
        {
            get
            {
                return this.spec.MyStableHash;
            }
        }

        public int Sequence
        {
            get
            {
                return this.sequence;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.sequence = value;
                Get.TurnManager.OnSequenceChanged();
            }
        }

        public virtual string Tip
        {
            get
            {
                string text = this.Description;
                if (this.CanFly && this.AffectedActor == Get.NowControlledActor)
                {
                    text = text.AppendedInDoubleNewLine(RichText.Grayed("ChangeAltitudeTip".Translate().FormattedKeyBindings()));
                }
                if (this.originWorldSituation != null)
                {
                    text = text.AppendedInDoubleNewLine(RichText.Grayed("{0}: {1}".Formatted("ConditionOrigin".Translate(), this.originWorldSituation.LabelCap)));
                }
                if (this.originDifficulty != null)
                {
                    text = text.AppendedInDoubleNewLine(RichText.Grayed("DifficultyOrigin".Translate()));
                }
                return text;
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

        public float TimeStartedAffectingActor
        {
            get
            {
                return this.timeStartedAffectingActor;
            }
        }

        public UseEffect_AddCondition OriginUseEffect
        {
            get
            {
                return this.originUseEffect;
            }
            set
            {
                this.originUseEffect = value;
            }
        }

        public BodyPartSpec OriginBodyPart
        {
            get
            {
                return this.originBodyPart;
            }
            set
            {
                this.originBodyPart = value;
            }
        }

        public WorldSituation OriginWorldSituation
        {
            get
            {
                return this.originWorldSituation;
            }
            set
            {
                this.originWorldSituation = value;
            }
        }

        public DifficultySpec OriginDifficulty
        {
            get
            {
                return this.originDifficulty;
            }
            set
            {
                this.originDifficulty = value;
            }
        }

        protected Condition()
        {
            this.cachedTipGetter = () => this.Tip;
            this.cachedLabelCapGetter = () => this.LabelCap;
        }

        public Condition(ConditionSpec spec)
        {
            this.cachedTipGetter = () => this.Tip;
            this.cachedLabelCapGetter = () => this.LabelCap;
            this.spec = spec;
            if (spec.ConditionClass != base.GetType())
            {
                string[] array = new string[5];
                array[0] = "Created a Condition with spec ";
                array[1] = ((spec != null) ? spec.ToString() : null);
                array[2] = ", but the created Condition type is ";
                int num = 3;
                Type type = base.GetType();
                array[num] = ((type != null) ? type.ToString() : null);
                array[4] = ".";
                Log.Warning(string.Concat(array), false);
            }
        }

        public Condition(ConditionSpec spec, int turnsLeft)
            : this(spec)
        {
            this.turnsLeft = turnsLeft;
        }

        public abstract IEnumerable<Instruction> MakeResolveConditionInstructions();

        public virtual Action GetForcedAction(Actor actor)
        {
            return null;
        }

        public virtual IEnumerable<Instruction> MakePostAddInstructions()
        {
            return Enumerable.Empty<Instruction>();
        }

        public virtual IEnumerable<Instruction> MakePostRemoveInstructions(Conditions removedFrom)
        {
            return Enumerable.Empty<Instruction>();
        }

        public virtual IEnumerable<Instruction> MakeOnNewlyAffectingActorInstructions()
        {
            Actor actor = this.AffectedActor;
            yield return new Instruction_Immediate(delegate
            {
                this.timeStartedAffectingActor = Clock.UnscaledTime;
            });
            if (actor.Spawned)
            {
                yield return new Instruction_AddSequenceable(this, 12, this.Spec.EndAtTheBeginningOfATurn ? Get.TurnManager.OrderBeforeCurrent : Get.TurnManager.OrderAfterCurrent);
            }
            yield break;
        }

        public virtual IEnumerable<Instruction> MakeOnNoLongerAffectingActorInstructions(Actor actor)
        {
            if (actor.Spawned)
            {
                yield return new Instruction_RemoveSequenceable(this);
            }
            yield break;
        }

        public virtual IEnumerable<Instruction> MakeOtherActorDestroyedInstructions(Actor otherActor)
        {
            return Enumerable.Empty<Instruction>();
        }

        public virtual IEnumerable<Instruction> MakeOtherActorBodyPartDestroyedInstructions(BodyPart bodyPart)
        {
            return Enumerable.Empty<Instruction>();
        }

        public virtual IEnumerable<Instruction> MakeAffectedActorLostHPInstructions(int damage, bool onBodyPart)
        {
            return Enumerable.Empty<Instruction>();
        }

        public virtual IEnumerable<Instruction> MakeAffectedActorMovedInstructions(Vector3Int prevPos)
        {
            return Enumerable.Empty<Instruction>();
        }

        void ISequenceable.TakeTurn()
        {
            new Action_ResolveCondition(Get.Action_ResolveCondition, this).Do(false);
        }

        public Condition Clone()
        {
            Condition condition = Maker.Make(this.spec);
            condition.spec = this.spec;
            condition.turnsLeft = this.turnsLeft;
            condition.originUseEffect = this.originUseEffect;
            condition.originBodyPart = this.originBodyPart;
            condition.originWorldSituation = this.originWorldSituation;
            condition.originDifficulty = this.originDifficulty;
            condition.hidden = this.hidden;
            this.CopyFieldsTo(condition);
            return condition;
        }

        public abstract void CopyFieldsTo(Condition clone);

        public override string ToString()
        {
            if (this.Spec == null)
            {
                return base.ToString();
            }
            return this.Spec.SpecID;
        }

        [Saved]
        private Conditions parent;

        [Saved]
        private ConditionSpec spec;

        [Saved]
        private int turnsLeft;

        [Saved]
        private bool hidden;

        [Saved]
        private UseEffect_AddCondition originUseEffect;

        [Saved]
        private BodyPartSpec originBodyPart;

        [Saved]
        private WorldSituation originWorldSituation;

        [Saved]
        private DifficultySpec originDifficulty;

        [Saved]
        private int sequence;

        private Func<string> cachedTipGetter;

        private Func<string> cachedLabelCapGetter;

        private float timeStartedAffectingActor = -99999f;
    }
}