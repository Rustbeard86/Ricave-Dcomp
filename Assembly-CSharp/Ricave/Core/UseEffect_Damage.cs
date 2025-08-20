using System;
using System.Collections.Generic;
using System.Linq;
using Ricave.UI;
using UnityEngine;

namespace Ricave.Core
{
    public class UseEffect_Damage : UseEffect
    {
        public override GoodBadNeutral GoodBadNeutral
        {
            get
            {
                return GoodBadNeutral.Bad;
            }
        }

        public int BaseFrom
        {
            get
            {
                return this.from;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.from = value;
            }
        }

        public int BaseTo
        {
            get
            {
                return this.to;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.to = value;
            }
        }

        public int From
        {
            get
            {
                return this.from + RampUpUtility.GetOffsetFromRampUp(this.from, base.InheritedRampUp, this.DamageRampUpFactor);
            }
        }

        public int To
        {
            get
            {
                return this.to + RampUpUtility.GetOffsetFromRampUp(this.to, base.InheritedRampUp, this.DamageRampUpFactor);
            }
        }

        public DamageTypeSpec DamageType
        {
            get
            {
                return this.damageType;
            }
        }

        public float DamageRampUpFactor
        {
            get
            {
                if (!this.damageUsesRampUp)
                {
                    return 1f;
                }
                return 1.2500001f;
            }
        }

        public EntitySpec ExcludedEntitySpec
        {
            get
            {
                return this.excludedEntitySpec;
            }
        }

        public override string LabelBase
        {
            get
            {
                string text = base.Spec.LabelFormat.Formatted(StringUtility.RangeToString(this.From, this.To), this.damageType.Adjective);
                Actor wieldingActor = base.WieldingActor;
                string text2;
                if (wieldingActor == null || wieldingActor.IsNowControlledActor)
                {
                    UseEffects parent = base.Parent;
                    if (!(((parent != null) ? parent.Parent : null) is Structure))
                    {
                        if (wieldingActor == null || !wieldingActor.IsNowControlledActor)
                        {
                            UseEffects parent2 = base.Parent;
                            Item item = ((parent2 != null) ? parent2.Parent : null) as Item;
                            if (item == null || !Get.NowControlledActor.Inventory.Contains(item))
                            {
                                return text;
                            }
                        }
                        int num;
                        int num2;
                        this.GetFinalDamageAmount(Get.NowControlledActor, out text2, out num, out num2, false, false, false, false, false, 1f);
                        if (num != this.From || num2 != this.To)
                        {
                            string text3 = "PostProcessedDamageRange".Translate(StringUtility.RangeToString(num, num2));
                            return text.AppendedWithSpace(RichText.Grayed("({0})".Formatted(text3)));
                        }
                        return text;
                    }
                }
                int num3;
                int num4;
                this.GetFinalDamageAmount(wieldingActor, Get.NowControlledActor, null, false, out text2, out num3, out num4, true);
                string text4 = "DamageRangeDealtToPlayer".Translate(StringUtility.RangeToString(num3, num4));
                text = text.AppendedWithSpace(RichText.Grayed("({0})".Formatted(text4)));
                return text;
            }
        }

        protected UseEffect_Damage()
        {
        }

        public UseEffect_Damage(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
            UseEffect_Damage useEffect_Damage = (UseEffect_Damage)clone;
            useEffect_Damage.from = this.from;
            useEffect_Damage.to = this.to;
            useEffect_Damage.damageType = this.damageType;
            useEffect_Damage.damageUsesRampUp = this.damageUsesRampUp;
            useEffect_Damage.excludedEntitySpec = this.excludedEntitySpec;
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            if (!target.IsEntity || target.Entity.MaxHP <= 0 || !target.Spawned)
            {
                yield break;
            }
            if (this.excludedEntitySpec != null)
            {
                Entity entity = target.Entity;
                if (((entity != null) ? entity.Spec : null) == this.excludedEntitySpec)
                {
                    yield break;
                }
            }
            bool flag = target.Entity is Actor && Rand.Chance(usable.CritChance * ((user != null) ? user.ConditionsAccumulated.CritChanceFactor : 1f));
            string text;
            int num;
            int num2;
            int finalDamageAmount = this.GetFinalDamageAmount(user, target.Entity, targetBodyPart, flag, out text, out num, out num2, false);
            int num3 = finalDamageAmount;
            DamageUtility.ApplyDamageProtectionAndClamp(target.Entity, targetBodyPart, this.damageType, ref num3, ref num, ref num2);
            foreach (Instruction instruction in this.DoAttackInstructions(user, target, originalTarget, targetBodyPart, num3, text, finalDamageAmount, flag, user))
            {
                yield return instruction;
            }
            IEnumerator<Instruction> enumerator = null;
            Structure structure = usable as Structure;
            if (structure != null && structure.Spec == Get.Entity_CeilingSpikes)
            {
                Actor actor = target.Entity as Actor;
                if (actor != null && actor.HP <= 0 && !actor.IsPlayerParty && !Get.Achievement_KillEnemyWithCeilingSpikes.IsCompleted)
                {
                    yield return new Instruction_CompleteAchievement(Get.Achievement_KillEnemyWithCeilingSpikes);
                }
            }
            Structure structure2 = target.Entity as Structure;
            if (structure2 != null && structure2.Spec == Get.Entity_Planks && !structure2.Spawned)
            {
                Item item = usable as Item;
                if (item != null && item.Spec == Get.Entity_Axe)
                {
                    Item planksItem = Maker.Make<Item>(Get.Entity_PlanksItem, null, false, false, true);
                    foreach (Instruction instruction2 in InstructionSets_Entity.Spawn(planksItem, SpawnPositionFinder.Near(structure2.Position, planksItem, false, false, null), null))
                    {
                        yield return instruction2;
                    }
                    enumerator = null;
                    yield return new Instruction_StartItemJumpAnimation(planksItem);
                    planksItem = null;
                }
            }
            yield break;
            yield break;
        }

        private IEnumerable<Instruction> DoAttackInstructions(Actor user, Target target, Target originalTarget, BodyPart targetBodyPart, int damage, string extraReason, int rawDamage, bool criticalHit, Actor originalUser)
        {
            if (user != null)
            {
                foreach (Instruction instruction in InstructionSets_Entity.PreViolentlyAttackedBy(target.Entity, user))
                {
                    yield return instruction;
                }
                IEnumerator<Instruction> enumerator = null;
            }
            if (user != null && target.Entity.IsPlayerParty && user.ConditionsAccumulated.DisableDamageDealtToPlayer)
            {
                Actor deflectOnto;
                if (Get.Skill_ShieldDeflect.IsUnlocked() && target.Entity.IsMainActor)
                {
                    deflectOnto = SkillUtility.GetActorToDeflectBlockedDamageOnto(user);
                    if (deflectOnto != null)
                    {
                        yield return new Instruction_PlayLog("DamageBlockedAndDeflected".Translate(target, RichText.HP(damage.ToStringCached()), this.damageType.Adjective, deflectOnto));
                        yield return new Instruction_Sound(Get.Sound_DamageBlocked, new Vector3?(target.Position), 1f, 1f);
                        yield return UseEffect_Visual_UseBow.LineBetween(target.Position, deflectOnto, false);
                        int num = rawDamage;
                        int num2 = rawDamage;
                        int num3 = rawDamage;
                        DamageUtility.ApplyDamageProtectionAndClamp(deflectOnto, null, this.damageType, ref num, ref num2, ref num3);
                        foreach (Instruction instruction2 in this.DoAttackInstructions((Actor)target.Entity, deflectOnto, deflectOnto, null, num, "FromDeflectedDamage".Translate(), rawDamage, criticalHit, originalUser))
                        {
                            yield return instruction2;
                        }
                        IEnumerator<Instruction> enumerator = null;
                        goto IL_038C;
                    }
                }
                yield return new Instruction_PlayLog("DamageBlocked".Translate(target, RichText.HP(damage.ToStringCached()), this.damageType.Adjective));
                yield return new Instruction_Sound(Get.Sound_DamageBlocked, new Vector3?(target.Position), 1f, 1f);
            IL_038C:
                deflectOnto = null;
            }
            else
            {
                if (criticalHit)
                {
                    yield return new Instruction_Sound(Get.Sound_CriticalHit, new Vector3?(target.Position), 1f, 1f);
                    if (!target.IsNowControlledActor)
                    {
                        yield return new Instruction_AddFloatingText(target, "CriticalHitFloatingText".Translate(), new Color(0.8f, 0.2f, 0.2f), 0.4f, 0f, -0.1f, null);
                    }
                }
                Vector3Int? impactSource = DamageUtility.DeduceImpactSource(user, target, originalTarget);
                Vector3? exactImpactPos = ((user != null && user.IsNowControlledActor && Get.InteractionManager.PointedAtEntity == target.Entity) ? Get.InteractionManager.PointedAtExactPos : null);
                if (targetBodyPart != null)
                {
                    foreach (Instruction instruction3 in InstructionSets_Actor.DamageBodyPart(targetBodyPart, damage, this.damageType, extraReason, impactSource, criticalHit, user != null && user.IsPlayerParty, exactImpactPos, false, true, true))
                    {
                        yield return instruction3;
                    }
                    IEnumerator<Instruction> enumerator = null;
                    if (target.Entity.Spawned && target.Entity is Actor && damage > 0)
                    {
                        int num4 = Rand.ProbabilisticRound((float)damage * 0.2f);
                        if (num4 > 0)
                        {
                            foreach (Instruction instruction4 in InstructionSets_Entity.Damage(target.Entity, num4, this.damageType, "DamageSpreadOntoSomethingElse".Translate(), impactSource, criticalHit, user != null && user.IsPlayerParty, exactImpactPos, user, true, false))
                            {
                                yield return instruction4;
                            }
                            enumerator = null;
                        }
                    }
                }
                else
                {
                    foreach (Instruction instruction5 in InstructionSets_Entity.Damage(target.Entity, damage, this.damageType, extraReason, impactSource, criticalHit, user != null && user.IsPlayerParty, exactImpactPos, user, false, true))
                    {
                        yield return instruction5;
                    }
                    IEnumerator<Instruction> enumerator = null;
                    if (target.Entity.Spawned)
                    {
                        Actor actor = target.Entity as Actor;
                        if (actor != null && !actor.IsPlayerParty && damage > 0 && Rand.Chance(this.damageType.SpreadOntoBodyPartChance))
                        {
                            BodyPart bodyPart;
                            if (actor.BodyParts.Where<BodyPart>((BodyPart x) => !x.IsMissing).TryGetRandomElement<BodyPart>(out bodyPart))
                            {
                                int num5 = Math.Max(Rand.RangeInclusive((int)((float)damage * 0.4f), damage), 1);
                                foreach (Instruction instruction6 in InstructionSets_Actor.DamageBodyPart(bodyPart, num5, this.damageType, "DamageSpreadOntoSomethingElse".Translate(), impactSource, criticalHit, user != null && user.IsPlayerParty, exactImpactPos, true, false, false))
                                {
                                    yield return instruction6;
                                }
                                enumerator = null;
                            }
                        }
                    }
                }
                if (user != null)
                {
                    foreach (Item item in originalUser.Inventory.EquippedItems.ToTemporaryList<Item>())
                    {
                        if (item.ChargesLeft > 0 && item.ConditionsEquipped.All.Any<Condition>(delegate (Condition x)
                        {
                            Condition_DealtDamageFactor condition_DealtDamageFactor = x as Condition_DealtDamageFactor;
                            if (condition_DealtDamageFactor == null || condition_DealtDamageFactor.DamageType != this.damageType)
                            {
                                Condition_DealtDamageOffset condition_DealtDamageOffset = x as Condition_DealtDamageOffset;
                                return condition_DealtDamageOffset != null && condition_DealtDamageOffset.DamageType == this.damageType;
                            }
                            return true;
                        }))
                        {
                            foreach (Instruction instruction7 in InstructionSets_Usable.TryDecrementChargesLeft(item))
                            {
                                yield return instruction7;
                            }
                            IEnumerator<Instruction> enumerator = null;
                        }
                    }
                    List<Item>.Enumerator enumerator2 = default(List<Item>.Enumerator);
                }
                impactSource = null;
                exactImpactPos = null;
            }
            if (user != null)
            {
                foreach (Instruction instruction8 in InstructionSets_Entity.PostViolentlyAttackedBy(target.Entity, user))
                {
                    yield return instruction8;
                }
                IEnumerator<Instruction> enumerator = null;
            }
            yield break;
            yield break;
        }

        public int GetFinalDamageAmount(Actor user, out string extraReason, out int minPossible, out int maxPossible, bool targetIsBelow, bool surpriseAttack, bool criticalHit, bool surroundedBonus, bool targetIsBodyPart, float specificTargetFactor = 1f)
        {
            int num = Rand.RangeInclusive(this.From, this.To);
            extraReason = "";
            minPossible = this.From;
            maxPossible = this.To;
            if (targetIsBelow)
            {
                num++;
                extraReason = extraReason.AppendedWithComma("AltitudeDamageBonus".Translate());
                minPossible++;
                maxPossible++;
            }
            if (user != null)
            {
                foreach (Condition condition in user.ConditionsAccumulated.AllConditions)
                {
                    ValueTuple<IntRange, DamageTypeSpec> dealtDamageOffset = condition.DealtDamageOffset;
                    IntRange item = dealtDamageOffset.Item1;
                    if (dealtDamageOffset.Item2 == this.damageType)
                    {
                        num += item.RandomInRange;
                        minPossible += item.from;
                        maxPossible += item.to;
                    }
                }
            }
            float num2 = 1f;
            if (surpriseAttack)
            {
                num2 *= 1.4f;
                extraReason = extraReason.AppendedWithComma("Ambushed".Translate());
            }
            if (criticalHit)
            {
                num2 *= 2f;
                extraReason = extraReason.AppendedWithComma("CriticalHit".Translate());
            }
            if (user != null && user.ChargedAttack != 0)
            {
                float num3 = ((user.ChargedAttack == 1) ? 1.2f : 1.4f);
                num2 *= num3;
                extraReason = extraReason.AppendedWithComma("ChargedAttack".Translate());
            }
            if (surroundedBonus)
            {
                num2 *= 1.25f;
                extraReason = extraReason.AppendedWithComma(Get.Skill_SurroundedBonus.Label);
            }
            if (user != null)
            {
                if (targetIsBodyPart && user.IsMainActor && Get.Skill_BodyPartsDamage.IsUnlocked())
                {
                    num2 *= 1.3f;
                }
                foreach (Condition condition2 in user.ConditionsAccumulated.AllConditions)
                {
                    ValueTuple<float, DamageTypeSpec> dealtDamageFactor = condition2.DealtDamageFactor;
                    float item2 = dealtDamageFactor.Item1;
                    if (dealtDamageFactor.Item2 == this.damageType)
                    {
                        num2 *= item2;
                    }
                }
                if (user.IsMainActor && Get.Trait_LastStand.IsChosen() && (float)user.HP <= (float)user.MaxHP * 0.35f)
                {
                    num2 *= 1.25f;
                }
            }
            num2 *= specificTargetFactor;
            num = Rand.ProbabilisticRound((float)num * num2);
            minPossible = Calc.FloorToInt((float)minPossible * num2);
            maxPossible = Calc.CeilToInt((float)maxPossible * num2);
            return num;
        }

        public int GetFinalDamageAmount(Actor user, Entity target, BodyPart targetBodyPart, bool criticalHit, out string extraReason, out int minPossible, out int maxPossible, bool applyTargetProtection = true)
        {
            Actor actor = target as Actor;
            bool flag = user != null && actor != null && GravityUtility.IsAltitudeLower(actor.Position, user.Position, actor.Gravity);
            bool flag2 = user != null && actor != null && user.CanSurpriseAttack(actor);
            bool flag3;
            if (user != null && user.IsMainActor)
            {
                if (Get.Player.SurroundedBonusJustBeforeUsing != null)
                {
                    flag3 = Get.Player.SurroundedBonusJustBeforeUsing.Value;
                }
                else
                {
                    flag3 = Get.Skill_SurroundedBonus.IsUnlocked() && SkillUtility.TwoAdjacentEnemiesToPlayer;
                }
            }
            else
            {
                flag3 = false;
            }
            bool flag4 = targetBodyPart != null;
            float num = 1f;
            if (user != null && user.IsMainActor)
            {
                foreach (TraitSpec traitSpec in Get.TraitManager.Chosen)
                {
                    if (traitSpec.DamageFactorAgainstActor == target.Spec)
                    {
                        num *= traitSpec.DamageFactorAgainstActorFactor;
                    }
                    else if (traitSpec.DamageFactorAgainstActor2 == target.Spec)
                    {
                        num *= traitSpec.DamageFactorAgainstActorFactor2;
                    }
                }
                if (actor != null && actor.IsBoss)
                {
                    num *= Get.TraitManager.DamageFactorAgainstBosses;
                }
            }
            if (user != null && user.IsMainActor && actor != null && UseEffect_Damage.IsUndead(actor.Spec))
            {
                float num2 = num;
                ClassSpec @class = Get.Player.Class;
                num = num2 * ((@class != null) ? @class.DamageFactorAgainstUndead : 1f);
            }
            int finalDamageAmount = this.GetFinalDamageAmount(user, out extraReason, out minPossible, out maxPossible, flag, flag2, criticalHit, flag3, flag4, num);
            if (applyTargetProtection)
            {
                DamageUtility.ApplyDamageProtectionAndClamp(target, targetBodyPart, this.damageType, ref finalDamageAmount, ref minPossible, ref maxPossible);
            }
            return finalDamageAmount;
        }

        private static bool IsUndead(EntitySpec spec)
        {
            return spec == Get.Entity_Skeleton || spec == Get.Entity_Mummy || spec == Get.Entity_Ghost || spec == Get.Entity_Phantom || spec == Get.Entity_Spirit;
        }

        [Saved]
        private int from;

        [Saved]
        private int to;

        [Saved]
        private DamageTypeSpec damageType;

        [Saved]
        private bool damageUsesRampUp;

        [Saved]
        private EntitySpec excludedEntitySpec;

        public const float SurpriseAttackDamageFactor = 1.4f;

        public const float CriticalHitDamageFactor = 2f;

        public const int AltitudeExtraDamage = 1;

        public const float SurroundedBonusDamageFactor = 1.25f;

        public const float ChargedAttackDamageFactor1 = 1.2f;

        public const float ChargedAttackDamageFactor2 = 1.4f;

        private const float BodyPartDamagePctToSpreadOntoMainHP = 0.2f;
    }
}