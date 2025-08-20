using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Ricave.Rendering;
using UnityEngine;

namespace Ricave.Core
{
    public class Actor : Entity, ISequenceable
    {
        public ActorGOC ActorGOC
        {
            get
            {
                return (ActorGOC)base.EntityGOC;
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

        public Inventory Inventory
        {
            get
            {
                return this.inventory;
            }
        }

        public Conditions Conditions
        {
            get
            {
                return this.conditions;
            }
        }

        public List<NativeWeapon> NativeWeapons
        {
            get
            {
                return this.nativeWeapons;
            }
        }

        public AIMemory AIMemory
        {
            get
            {
                return this.aiMemory;
            }
        }

        public AISpec AI
        {
            get
            {
                if (base.IsNowControlledActor)
                {
                    return null;
                }
                if (base.IsPlayerParty)
                {
                    if (this.partyMemberAIMode != PartyMemberAIMode.Stay)
                    {
                        return Get.AI_PartyMember;
                    }
                    return Get.AI_PartyMember_Stay;
                }
                else
                {
                    if (base.Spec.Actor.AI == Get.AI_StandNearCultistCircleIfUnprovoked && this.faction != null)
                    {
                        Faction firstOfSpec = Get.FactionManager.GetFirstOfSpec(Get.Faction_Monsters);
                        if (firstOfSpec != null && this.faction != firstOfSpec && this.faction.IsHostile(firstOfSpec))
                        {
                            return Get.AI_Standard;
                        }
                    }
                    if (base.Spec == Get.Entity_Phantom && Get.Player.Class == Get.Class_Necromancer && this.ConditionsAccumulated.InvertHostile && Get.Player.MainActor != null && !this.IsHostile(Get.Player.MainActor))
                    {
                        return Get.AI_PlayerFollower;
                    }
                    return base.Spec.Actor.AI;
                }
            }
        }

        public int Velocity
        {
            get
            {
                return this.velocity;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.velocity = value;
            }
        }

        public int Mana
        {
            get
            {
                return this.mana;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.mana = value;
            }
        }

        public int Stamina
        {
            get
            {
                return this.stamina;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.stamina = value;
            }
        }

        public Spells Spells
        {
            get
            {
                return this.spells;
            }
        }

        public bool CanFly
        {
            get
            {
                return this.ConditionsAccumulated.CanFly || (base.IsMainActor && Get.TraitManager.CanFly);
            }
        }

        public bool CanJumpOffLedge
        {
            get
            {
                return (base.IsNowControlledActor || this.ConditionsAccumulated.CanJumpOffLedge) && !this.ConditionsAccumulated.DisableCanJumpOffLedge;
            }
        }

        public bool CanUseLadders
        {
            get
            {
                return this.HasArm;
            }
        }

        public bool AllowDanceAnimation
        {
            get
            {
                return !this.ConditionsAccumulated.StopDanceAnimation && !base.Spec.Actor.Immobile;
            }
        }

        public bool MovingAllowed
        {
            get
            {
                return !this.ConditionsAccumulated.MovingDisallowed && !base.Spec.Actor.Immobile;
            }
        }

        public bool AllowPathingIntoAIAvoids
        {
            get
            {
                return base.IsNowControlledActor || this.ConditionsAccumulated.AllowPathingIntoAIAvoids || (base.Spawned && Get.CellsInfo.AnyAIAvoidsAt(base.Position));
            }
        }

        public int SeeRange
        {
            get
            {
                int num = 5 + this.ConditionsAccumulated.SeeRangeOffset;
                if (base.IsMainActor)
                {
                    if (Get.Trait_NightOwl.IsChosen())
                    {
                        if (Get.DayNightCycleManager.IsNightForNightOwl)
                        {
                            num++;
                        }
                        else
                        {
                            num--;
                        }
                    }
                    else if (Get.Trait_Vampire.IsChosen() && !Get.DayNightCycleManager.IsNightForNightOwl)
                    {
                        num--;
                    }
                }
                return Calc.Clamp(Math.Min(num, this.ConditionsAccumulated.SeeRangeCap), 1, 10);
            }
        }

        public bool ImmuneToPushing
        {
            get
            {
                if (base.IsMainActor && Get.TraitManager.ImmuneToPushing)
                {
                    return true;
                }
                if (base.IsMainActor)
                {
                    ClassSpec @class = Get.Player.Class;
                    if (@class != null && @class.ImmuneToPushing)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public bool ImmuneToPoison
        {
            get
            {
                if (this.ConditionsAccumulated.ImmuneToPoison)
                {
                    return true;
                }
                if (base.IsMainActor && Get.TraitManager.PoisonIncomingDamageFactor == 0f)
                {
                    return true;
                }
                if (base.IsMainActor)
                {
                    ClassSpec @class = Get.Player.Class;
                    if (@class != null && @class.ImmuneToPoison)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public bool ImmuneToFire
        {
            get
            {
                if (this.ConditionsAccumulated.ImmuneToFire)
                {
                    return true;
                }
                if (base.IsMainActor && Get.TraitManager.FireIncomingDamageFactor == 0f)
                {
                    return true;
                }
                if (base.IsMainActor)
                {
                    ClassSpec @class = Get.Player.Class;
                    if (@class != null && @class.FireIncomingDamageFactor == 0f)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public bool ImmuneToDisease
        {
            get
            {
                return this.ConditionsAccumulated.ImmuneToDisease;
            }
        }

        public ConditionsAccumulated ConditionsAccumulated
        {
            get
            {
                return this.conditionsAccumulated;
            }
        }

        public GenericUsable UsableOnDeath
        {
            get
            {
                return this.usableOnDeath;
            }
        }

        public GenericUsable UsableOnTalk
        {
            get
            {
                if (this.usableOnTalk2 != null && this.usableOnTalk2.UseEffects != null && !this.IsHostile(Get.NowControlledActor))
                {
                    UseEffect_ShowDialogue useEffect_ShowDialogue = (UseEffect_ShowDialogue)this.usableOnTalk2.UseEffects.GetFirstOfSpec(Get.UseEffect_ShowDialogue);
                    if (useEffect_ShowDialogue != null && !useEffect_ShowDialogue.DialogueSpec.Ended)
                    {
                        return this.usableOnTalk2;
                    }
                }
                return this.usableOnTalk;
            }
        }

        public bool IsBaby
        {
            get
            {
                return this.isBaby;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.isBaby = value;
            }
        }

        public bool IsBoss
        {
            get
            {
                return this.isBoss;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.isBoss = value;
            }
        }

        public bool IsPenaltyRat
        {
            get
            {
                return this.isPenaltyRat;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.isPenaltyRat = value;
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.name = value;
            }
        }

        public Faction Faction
        {
            get
            {
                return this.faction;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.faction = value;
            }
        }

        public bool IsOfPlayerFaction
        {
            get
            {
                return this.Faction != null && this.Faction == Get.Player.Faction;
            }
        }

        public List<GenericUsable> Abilities
        {
            get
            {
                return this.abilities;
            }
        }

        public int ChargedAttack
        {
            get
            {
                return this.chargedAttack;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.chargedAttack = value;
            }
        }

        public override string Label
        {
            get
            {
                string text;
                if (!this.name.NullOrEmpty())
                {
                    text = this.name;
                }
                else if (base.IsMainActor)
                {
                    text = Get.Progress.PlayerName;
                }
                else
                {
                    text = base.Label;
                }
                if (this.isBoss && this.name.NullOrEmpty() && !base.Spec.Actor.AlwaysBoss)
                {
                    text = "BossLabel".Translate(text);
                }
                else if (this.isBaby)
                {
                    text = "BabyLabel".Translate(text);
                }
                if (this.rampUp != 0)
                {
                    text = "{0} {1}".Formatted(text, this.rampUp.ToStringOffset(true));
                }
                return text;
            }
        }

        public int RampUp
        {
            get
            {
                return this.rampUp;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.rampUp = value;
            }
        }

        public bool AffectedByRampUp
        {
            get
            {
                return RampUpUtility.AffectedByRampUp(this);
            }
        }

        public int KilledExperience
        {
            get
            {
                if (base.IsPlayerParty)
                {
                    return 0;
                }
                if (this.Faction != null && this.Faction == Get.Player.Faction && !this.IsHostile(Get.MainActor))
                {
                    return 0;
                }
                if (this.IsBoss)
                {
                    if (base.Spec.Actor.AlwaysBoss)
                    {
                        return Actor.< get_KilledExperience > g__ApplyFactors | 117_0(base.Spec.Actor.KilledExperience);
                    }
                    return Actor.< get_KilledExperience > g__ApplyFactors | 117_0(4);
                }
                else
                {
                    if (this.IsBaby)
                    {
                        return 0;
                    }
                    return Actor.< get_KilledExperience > g__ApplyFactors | 117_0(base.Spec.Actor.KilledExperience);
                }
            }
        }

        public bool IfDestroyedMoveDrawPosToDestInstantly
        {
            get
            {
                return this.ifDestroyedMoveDrawPosToDestInstantly;
            }
            set
            {
                this.ifDestroyedMoveDrawPosToDestInstantly = value;
            }
        }

        public override int MaxHP
        {
            get
            {
                int num = base.Spec.MaxHP;
                num += RampUpUtility.GetOffsetFromRampUp(base.Spec.MaxHP, this.RampUp, base.Spec.Actor.MaxHPRampUpFactor);
                if (base.IsMainActor)
                {
                    num += (Get.Player.Level - 1) * (5 + Get.TraitManager.MaxHPOffsetPerLevel);
                    if (Get.RunSpec == Get.Run_Tutorial)
                    {
                        num += 20;
                    }
                    if (Get.Skill_IncreasedMaxHP.IsUnlocked())
                    {
                        num++;
                    }
                    num += Get.TraitManager.MaxHPOffset;
                    int num2 = num;
                    ClassSpec @class = Get.Player.Class;
                    num = num2 + ((@class != null) ? @class.MaxHPOffset : 0);
                }
                else if (base.IsPlayerParty && base.Spec.Actor.PlayerLevelAffectsMaxHP)
                {
                    num += (Get.Player.Level - 1) * 3;
                }
                num += this.ConditionsAccumulated.MaxHPOffset;
                float num3 = this.ConditionsAccumulated.MaxHPFactor;
                if (base.IsMainActor)
                {
                    num3 *= Get.TraitManager.MaxHPFactor;
                }
                num = Calc.RoundToIntHalfUp((float)num * num3);
                return Math.Max(num, 1);
            }
        }

        public int MaxMana
        {
            get
            {
                int num = base.Spec.Actor.MaxMana + this.ConditionsAccumulated.MaxManaOffset;
                if (base.IsMainActor)
                {
                    if (Get.Skill_IncreasedMaxMana.IsUnlocked())
                    {
                        num++;
                    }
                    num += Get.TraitManager.MaxManaOffset;
                    int num2 = num;
                    ClassSpec @class = Get.Player.Class;
                    num = num2 + ((@class != null) ? @class.MaxManaOffset : 0);
                }
                return Math.Max(num, 0);
            }
        }

        public int MaxStamina
        {
            get
            {
                int num = 10 + this.ConditionsAccumulated.MaxStaminaOffset;
                if (base.IsMainActor)
                {
                    if (Get.Skill_IncreasedMaxStamina.IsUnlocked())
                    {
                        num += 2;
                    }
                    num += Get.TraitManager.MaxStaminaOffset;
                    int num2 = num;
                    ClassSpec @class = Get.Player.Class;
                    num = num2 + ((@class != null) ? @class.MaxStaminaOffset : 0);
                }
                return Math.Max(num, 0);
            }
        }

        public int SequencePerTurn
        {
            get
            {
                return this.ConditionsAccumulated.SequencePerTurn;
            }
        }

        public int SequencePerMove
        {
            get
            {
                return Math.Max(Calc.RoundToIntHalfUp((float)this.SequencePerTurn * this.ConditionsAccumulated.SequencePerMoveMultiplier), 1);
            }
        }

        public int MoveNoiseRadius
        {
            get
            {
                int num = base.Spec.Actor.MoveNoiseRadius;
                if (base.IsMainActor)
                {
                    num += Get.TraitManager.WalkingNoiseOffset;
                    int num2 = num;
                    ClassSpec @class = Get.Player.Class;
                    num = num2 + ((@class != null) ? @class.WalkingNoiseOffset : 0);
                }
                return Math.Max(num, 0);
            }
        }

        public List<BodyPart> BodyParts
        {
            get
            {
                return this.bodyParts;
            }
        }

        public bool HasArm
        {
            get
            {
                for (int i = 0; i < this.bodyParts.Count; i++)
                {
                    if (this.bodyParts[i].Spec.IsArm && !this.bodyParts[i].IsMissing)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public bool HasEyes
        {
            get
            {
                for (int i = 0; i < this.bodyParts.Count; i++)
                {
                    if (this.bodyParts[i].Spec.AreEyes && !this.bodyParts[i].IsMissing)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public bool Limping
        {
            get
            {
                for (int i = 0; i < this.bodyParts.Count; i++)
                {
                    if (this.bodyParts[i].IsMissing && this.bodyParts[i].Spec.CausesActorToLimp)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public Vector3Int Gravity
        {
            get
            {
                return this.gravity;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.gravity = value;
            }
        }

        public float HungerRateMultiplier
        {
            get
            {
                float num = this.ConditionsAccumulated.HungerRateMultiplier;
                if (base.IsMainActor && Get.Skill_SlowerHunger.IsUnlocked())
                {
                    num *= 0.5f;
                }
                if (base.IsMainActor)
                {
                    num *= Get.TraitManager.HungerRateFactor;
                }
                if (base.IsMainActor)
                {
                    float num2 = num;
                    ClassSpec @class = Get.Player.Class;
                    num = num2 * ((@class != null) ? @class.HungerRateFactor : 1f);
                }
                return num;
            }
        }

        public Color HostilityColor
        {
            get
            {
                if (this.IsHostile(Get.NowControlledActor))
                {
                    Faction faction = this.Faction;
                    if (faction == null)
                    {
                        return Faction.DefaultHostileColor;
                    }
                    return faction.Color_Hostile;
                }
                else if (base.IsNowControlledActor || (this.Faction != null && this.Faction == Get.NowControlledActor.Faction))
                {
                    Faction faction2 = this.Faction;
                    if (faction2 == null)
                    {
                        return Faction.DefaultAllyColor;
                    }
                    return faction2.Color_Ally;
                }
                else
                {
                    Faction faction3 = this.Faction;
                    if (faction3 == null)
                    {
                        return Faction.DefaultNeutralColor;
                    }
                    return faction3.Color_Neutral;
                }
            }
        }

        public ChainPost AttachedToChainPostDirect
        {
            get
            {
                return this.attachedToChainPost;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.attachedToChainPost = value;
            }
        }

        public ChainPost AttachedToChainPost
        {
            get
            {
                if (this.attachedToChainPost == null || !this.attachedToChainPost.Spawned || this.attachedToChainPost.AttachedActorDirect != this || !base.Spawned)
                {
                    return null;
                }
                return this.attachedToChainPost;
            }
        }

        public float LastTimeCausedPlayerToLoseHP
        {
            get
            {
                return this.lastTimeCausedPlayerToLoseHP;
            }
        }

        public KickUsable KickUsable
        {
            get
            {
                return this.kickUsable;
            }
        }

        public PartyMemberAIMode PartyMemberAIMode
        {
            get
            {
                return this.partyMemberAIMode;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.partyMemberAIMode = value;
            }
        }

        protected Actor()
        {
            this.conditionsAccumulated = new ConditionsAccumulated(this);
        }

        public Actor(EntitySpec spec)
            : base(spec)
        {
            this.conditionsAccumulated = new ConditionsAccumulated(this);
            this.inventory = new Inventory(this);
            this.conditions = new Conditions(this);
            this.spells = new Spells(this);
            FactionSpec factionSpec;
            if (base.Spec == Get.Entity_Dog)
            {
                if (!Get.Trait_Vampire.IsChosen())
                {
                    ClassSpec @class = Get.Player.Class;
                    if (@class == null || !@class.DogsAreHostile)
                    {
                        goto IL_00BD;
                    }
                }
                factionSpec = Get.Faction_Monsters;
                goto IL_00EE;
            }
        IL_00BD:
            if (base.Spec.Actor.DefaultFaction != null)
            {
                factionSpec = base.Spec.Actor.DefaultFaction;
            }
            else
            {
                factionSpec = null;
            }
        IL_00EE:
            if (factionSpec != null)
            {
                Faction faction;
                if (Get.FactionManager.GetFactionCountOfSpec(factionSpec) == 1)
                {
                    this.faction = Get.FactionManager.GetFirstOfSpec(factionSpec);
                }
                else if (Get.FactionManager.Factions.Where<Faction>((Faction x) => x.Spec == factionSpec && !x.Hidden).TryGetRandomElement<Faction>(out faction))
                {
                    this.faction = faction;
                }
                else if (Get.FactionManager.Factions.Where<Faction>((Faction x) => x.Spec == factionSpec).TryGetRandomElement<Faction>(out faction))
                {
                    this.faction = faction;
                }
            }
            List<NativeWeaponProps> list = spec.Actor.NativeWeapons;
            for (int i = 0; i < list.Count; i++)
            {
                this.nativeWeapons.Add(new NativeWeapon(this, i));
            }
            Conditions defaultConditions = spec.Actor.DefaultConditions;
            if (defaultConditions != null)
            {
                this.conditions.AddClonedFrom(defaultConditions);
            }
            foreach (SpellSpec spellSpec in spec.Actor.DefaultSpells)
            {
                this.spells.AddSpell(Maker.Make(spellSpec), -1);
            }
            SpellSpec spellSpec2;
            if (spec.Actor.RandomSpell.TryGetRandomElement<SpellSpec>(out spellSpec2))
            {
                this.spells.AddSpell(Maker.Make(spellSpec2), -1);
            }
            List<BodyPartPlacement> list2 = spec.Actor.BodyParts;
            for (int j = 0; j < list2.Count; j++)
            {
                this.bodyParts.Add(new BodyPart(this, j));
            }
            if (spec.Actor.UsableOnDeath != null)
            {
                this.usableOnDeath = new GenericUsable(spec.Actor.UsableOnDeath, this);
            }
            if (spec.Actor.UsableOnTalk != null)
            {
                this.usableOnTalk = new GenericUsable(spec.Actor.UsableOnTalk, this);
            }
            if (spec.Actor.UsableOnTalk2 != null)
            {
                this.usableOnTalk2 = new GenericUsable(spec.Actor.UsableOnTalk2, this);
            }
            if (base.Spec == Get.Entity_Player)
            {
                this.kickUsable = new KickUsable(this);
            }
            this.CalculateInitialHPManaAndStamina();
        }

        public void CalculateInitialHPManaAndStamina()
        {
            Instruction.ThrowIfNotExecuting();
            base.HP = this.MaxHP;
            this.mana = this.MaxMana;
            this.stamina = this.MaxStamina;
            foreach (BodyPart bodyPart in this.bodyParts)
            {
                bodyPart.CalculateInitialMaxHP();
            }
        }

        void ISequenceable.TakeTurn()
        {
            ActorTurnResolver.TakeTurn(this);
            Get.NextTurnsUI.OnActorTookTurn(this);
            Get.SeenEntitiesDrawer.OnActorTookTurn(this);
        }

        public bool Sees(Target target, Vector3Int? assumeMyPos = null)
        {
            return (target.Entity == null || target.Entity.Spawned) && this.Sees(target.Position, assumeMyPos);
        }

        public bool Sees(Vector3Int pos, Vector3Int? assumeMyPos = null)
        {
            if (assumeMyPos == null && !base.Spawned)
            {
                return false;
            }
            Vector3Int vector3Int = assumeMyPos ?? base.Position;
            return vector3Int.GetGridDistance(pos) <= this.SeeRange && LineOfSight.IsLineOfSight(vector3Int, pos);
        }

        public bool CanUseOn(IUsable usable, Target on, Vector3Int? assumeUserPos = null, bool assumeAnyUserPos = false, StringSlot outReason = null)
        {
            if (!this.CanUse(usable, assumeUserPos, assumeAnyUserPos, outReason))
            {
                return false;
            }
            if (on.Entity != null && !on.Entity.Spawned)
            {
                return false;
            }
            if (!usable.UseFilter.Allows(on, this))
            {
                if (outReason != null)
                {
                    outReason.Set("InvalidTarget".Translate());
                }
                return false;
            }
            if (!assumeAnyUserPos && !this.Sees(on, assumeUserPos))
            {
                if (outReason != null)
                {
                    outReason.Set("NoLineOfSight".Translate());
                }
                return false;
            }
            if (!assumeAnyUserPos && (assumeUserPos ?? base.Position).GetGridDistance(on.Position) > usable.UseRange)
            {
                if (outReason != null)
                {
                    outReason.Set("OutOfRange".Translate());
                }
                return false;
            }
            if (!assumeAnyUserPos && !LineOfSight.IsLineOfFire(assumeUserPos ?? base.Position, on.Position))
            {
                if (outReason != null)
                {
                    outReason.Set("NoLineOfSight".Translate());
                }
                return false;
            }
            if (usable.UseEffects.AnyPreventsEntireUse(this, on, outReason))
            {
                return false;
            }
            ClassSpec @class = Get.Player.Class;
            if (@class != null && @class.DisablesPressurePlates)
            {
                Structure structure = usable as Structure;
                if (structure != null && base.IsMainActor && (structure.Spec == Get.Entity_PressurePlate || structure.Spec == Get.Entity_SpawnerPressurePlate))
                {
                    return false;
                }
            }
            return true;
        }

        public bool CanUse(IUsable usable, Vector3Int? assumeUserPos = null, bool assumeAnyUserPos = false, StringSlot outReason = null)
        {
            if (assumeUserPos == null && !assumeAnyUserPos && !base.Spawned)
            {
                return false;
            }
            if (!usable.UseEffects.Any)
            {
                return false;
            }
            if (usable.LastUseSequence != null && Get.TurnManager.CurrentSequence - usable.LastUseSequence.Value < usable.CooldownTurns * 12)
            {
                if (outReason != null)
                {
                    outReason.Set("OnCooldown".Translate(StringUtility.TurnsString((usable.CooldownTurns * 12 - (Get.TurnManager.CurrentSequence - usable.LastUseSequence.Value)) / 12)));
                }
                return false;
            }
            if (this.Mana < usable.ManaCost)
            {
                if (outReason != null)
                {
                    outReason.Set("NotEnoughMana".Translate(usable.ManaCost));
                }
                return false;
            }
            if (this.Stamina < usable.StaminaCost)
            {
                if (outReason != null)
                {
                    outReason.Set("NotEnoughStamina".Translate(usable.StaminaCost));
                }
                return false;
            }
            return usable.CanUse_ExtraInstanceSpecificChecks(this, assumeUserPos, assumeAnyUserPos, outReason);
        }

        public bool CanSurpriseAttack(Actor other)
        {
            return other != this && !other.IsNowControlledActor && !other.IsPlayerParty && !other.AIMemory.AwareOf.Contains(this) && Get.TurnManager.HasSequencePriorityOver(this.Sequence, Get.TurnManager.GetSequenceableOrder(this), other.Sequence, Get.TurnManager.GetSequenceableOrder(other));
        }

        public bool IsHostile(Actor other)
        {
            if (this == other)
            {
                return false;
            }
            bool flag = this.Faction != null && other.Faction != null && (base.Spec != Get.Entity_Spirit || this.AttachedToChainPost == null) && (other.Spec != Get.Entity_Spirit || other.AttachedToChainPost == null) && this.Faction.IsHostile(other.Faction);
            if (this.ConditionsAccumulated.InvertHostile || other.ConditionsAccumulated.InvertHostile)
            {
                flag = !flag;
            }
            return flag;
        }

        public void OnCausedPlayerToLoseHP()
        {
            this.lastTimeCausedPlayerToLoseHP = Clock.UnscaledTime;
        }

        [CompilerGenerated]
        internal static int <get_KilledExperience>g__ApplyFactors|117_0(int baseExp)
		{
			if (Get.TraitManager.GainedExpFactor != 1f)
			{
				baseExp = Calc.RoundToIntHalfUp((float) baseExp * Get.TraitManager.GainedExpFactor);
    }
			return baseExp;
		}

[Saved]
private int sequence;

[Saved]
private Inventory inventory;

[Saved]
private Conditions conditions;

[Saved(Default.New, true)]
private List<NativeWeapon> nativeWeapons = new List<NativeWeapon>();

[Saved(Default.New, false)]
private AIMemory aiMemory = new AIMemory();

[Saved(Default.Vector3Int_Down, false)]
private Vector3Int gravity = Vector3IntUtility.Down;

[Saved]
private int velocity;

[Saved]
private int mana;

[Saved]
private int stamina;

[Saved]
private Spells spells;

[Saved]
private bool isBaby;

[Saved]
private bool isBoss;

[Saved]
private string name;

[Saved(Default.New, true)]
private List<BodyPart> bodyParts = new List<BodyPart>();

[Saved]
private int rampUp;

[Saved]
private GenericUsable usableOnDeath;

[Saved]
private GenericUsable usableOnTalk;

[Saved]
private GenericUsable usableOnTalk2;

[Saved]
private bool isPenaltyRat;

[Saved]
private Faction faction;

[Saved(Default.New, true)]
private List<GenericUsable> abilities = new List<GenericUsable>();

[Saved]
private int chargedAttack;

[Saved]
private ChainPost attachedToChainPost;

[Saved]
private KickUsable kickUsable;

[Saved(PartyMemberAIMode.Follow, false)]
private PartyMemberAIMode partyMemberAIMode;

private ConditionsAccumulated conditionsAccumulated;

private bool ifDestroyedMoveDrawPosToDestInstantly;

private float lastTimeCausedPlayerToLoseHP = -99999f;

public const int DefaultSeeRange = 5;

public const int MaxSeeRange = 10;

public const int MaxSpells = 7;

public const int DefaultMaxStamina = 10;

public const int BossKilledExperience = 4;
	}
}