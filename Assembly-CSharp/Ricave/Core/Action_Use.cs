using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class Action_Use : Action
    {
        public Actor User
        {
            get
            {
                return this.user;
            }
        }

        public IUsable Usable
        {
            get
            {
                return this.usable;
            }
        }

        public Target On
        {
            get
            {
                return this.on;
            }
        }

        public BodyPart OnBodyPart
        {
            get
            {
                return this.onBodyPart;
            }
        }

        public bool ResetChargedAttackToOne
        {
            get
            {
                return this.resetChargedAttackToOne;
            }
            set
            {
                this.resetChargedAttackToOne = value;
            }
        }

        protected override int RandSeedPart
        {
            get
            {
                int myStableHash = this.user.MyStableHash;
                int myStableHash2 = this.on.MyStableHash;
                BodyPart bodyPart = this.onBodyPart;
                return Calc.CombineHashes<int, int, int, int>(myStableHash, myStableHash2, (bodyPart != null) ? bodyPart.MyStableHash : 0, 912351830);
            }
        }

        public override string Description
        {
            get
            {
                if (this.on == this.user)
                {
                    return this.usable.UseDescriptionFormat_Self.Formatted(this.user, this.usable);
                }
                return this.usable.UseDescriptionFormat_Other.Formatted(this.user, this.usable, this.on.ToObject());
            }
        }

        protected Action_Use()
        {
        }

        public Action_Use(ActionSpec spec, Actor user, IUsable usable, Target on, BodyPart onBodyPart = null)
            : base(spec)
        {
            this.user = user;
            this.usable = usable;
            this.on = on;
            this.onBodyPart = onBodyPart;
        }

        public override bool CanDo(bool ignoreActorState = false)
        {
            if (this.on.Entity != null && !this.on.Entity.Spawned)
            {
                return false;
            }
            if (this.onBodyPart != null)
            {
                Actor actor = this.on.Entity as Actor;
                if (actor == null)
                {
                    return false;
                }
                if (!actor.BodyParts.Contains(this.onBodyPart))
                {
                    return false;
                }
                if (this.onBodyPart.IsMissing)
                {
                    return false;
                }
            }
            if (!ignoreActorState)
            {
                if (!this.user.Spawned)
                {
                    return false;
                }
                if (!this.user.CanUseOn(this.usable, this.on, null, false, null))
                {
                    return false;
                }
            }
            return true;
        }

        protected override bool CalculateConcernsPlayer()
        {
            return ActionUtility.TargetConcernsPlayer(this.user) || ActionUtility.UsableConcernsPlayer(this.usable) || ActionUtility.TargetConcernsPlayer(this.on);
        }

        protected override IEnumerable<Instruction> MakeInstructions()
        {
            this.resetChargedAttackToOne = false;
            int sequencePerUse = Action_Use.SequencePerUse(this.user, this.usable);
            yield return new Instruction_SetActorLastUseSequence(this.user, this.user.Sequence);
            if (this.on.IsEntity)
            {
                Actor actor = this.on.Entity as Actor;
                if (actor != null && actor != this.user && actor.IsHostile(this.user))
                {
                    yield return new Instruction_SetActorLastUseOnHostileSequence(this.user, this.user.Sequence);
                }
            }
            yield return new Instruction_Awareness_PreAction(this.user);
            Entity entity = this.usable as Entity;
            Vector3Int? usableSpawnedPos = ((entity != null && entity.Spawned) ? new Vector3Int?(entity.Position) : null);
            foreach (Instruction instruction in InstructionSets_Usable.Use(this.user, this.usable, this.on, this.onBodyPart))
            {
                yield return instruction;
            }
            IEnumerator<Instruction> enumerator = null;
            IUsable usable = this.usable;
            Item usableItem = usable as Item;
            if (usableItem != null)
            {
                if (usableItem.UsesLeft > 0)
                {
                    bool itemDisappears = usableItem.UsesLeft == 1;
                    yield return new Instruction_ChangeItemUsesLeft(usableItem, -1);
                    if (itemDisappears)
                    {
                        foreach (Instruction instruction2 in InstructionSets_Entity.Destroy(usableItem, null, null))
                        {
                            yield return instruction2;
                        }
                        enumerator = null;
                        if (usableItem.Spec.Item.DefaultUsesLeft >= 2)
                        {
                            yield return new Instruction_Sound(Get.Sound_DestroyItemInInventory, null, 1f, 1f);
                        }
                    }
                }
                if (Get.IdentificationGroups.ShouldAddToIdentifiedListOnIdentified(usableItem.Spec))
                {
                    yield return new Instruction_AddToIdentifiedList(usableItem.Spec);
                    if (usableItem.Spawned || usableItem.ParentInventory != null)
                    {
                        yield return new Instruction_PlayLog("ItemIdentifiedOnUse_StillExists".Translate(usableItem));
                    }
                    else
                    {
                        yield return new Instruction_PlayLog("ItemIdentifiedOnUse_Removed".Translate(usableItem));
                    }
                    foreach (Instruction instruction3 in InstructionSets_Misc.ResetTurnsCanRewind())
                    {
                        yield return instruction3;
                    }
                    enumerator = null;
                }
            }
            if (this.UsableCreatesUseNoise(this.usable))
            {
                foreach (Instruction instruction4 in InstructionSets_Noise.MakeNoise(this.on.Position, 1, NoiseType.ActorUsedSomething, this.user, false, true))
                {
                    yield return instruction4;
                }
                enumerator = null;
                if (usableSpawnedPos != null)
                {
                    foreach (Instruction instruction5 in InstructionSets_Noise.MakeNoise(usableSpawnedPos.Value, 1, NoiseType.ActorUsedSomething, this.user, false, true))
                    {
                        yield return instruction5;
                    }
                    enumerator = null;
                }
            }
            if (this.user.IsNowControlledActor && Get.UseOnTargetUI.TargetingUsable == this.usable)
            {
                yield return new Instruction_Immediate(delegate
                {
                    Get.UseOnTargetUI.StopTargeting();
                });
            }
            if (ActionUtility.TargetConcernsPlayer(this.user) || ActionUtility.TargetConcernsPlayer(this.on))
            {
                yield return new Instruction_OptionalDelay(0.25f);
            }
            if (!this.user.IsMainActor && this.on.IsMainActor && Get.Skill_EntangleOnAttacked.IsUnlocked() && Rand.Chance(0.5f))
            {
                Condition_Entangled condition_Entangled = new Condition_Entangled(Get.Condition_Entangled, 1);
                foreach (Instruction instruction6 in InstructionSets_Misc.AddCondition(condition_Entangled, this.user.Conditions, true, true))
                {
                    yield return instruction6;
                }
                enumerator = null;
            }
            if (this.resetChargedAttackToOne)
            {
                yield return new Instruction_SetChargedAttack(this.user, 1);
            }
            else if (this.user.ChargedAttack != 0)
            {
                yield return new Instruction_SetChargedAttack(this.user, 0);
            }
            yield return new Instruction_Awareness_PostAction(this.user);
            if (sequencePerUse != 0)
            {
                yield return new Instruction_AddSequence(this.user, sequencePerUse);
            }
            Item item = this.usable as Item;
            if (item != null && item.Spec.Item.Generator_IsPotion && this.user.IsNowControlledActor)
            {
                yield return new Instruction_Immediate(delegate
                {
                    Get.LessonManager.FinishIfCurrent(Get.Lesson_DrinkingPotion);
                });
            }
            yield break;
            yield break;
        }

        public static int SequencePerUse(Actor user, IUsable usable)
        {
            return Calc.RoundToIntHalfUp((float)user.SequencePerTurn * usable.SequencePerUseMultiplier);
        }

        private bool UsableCreatesUseNoise(IUsable usable)
        {
            Item item = usable as Item;
            return item == null || (item.Spec != Get.Entity_Shield && item.Spec != Get.Entity_GreatShield);
        }

        [Saved]
        private Actor user;

        [Saved]
        private IUsable usable;

        [Saved]
        private Target on;

        [Saved]
        private BodyPart onBodyPart;

        private bool resetChargedAttackToOne;
    }
}