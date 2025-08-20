using System;
using System.Collections.Generic;
using System.Text;
using Ricave.UI;
using UnityEngine;

namespace Ricave.Core
{
    public class TraitSpec : Spec, ITipSubject, ISaveableEventsReceiver
    {
        public string IconPath
        {
            get
            {
                return this.iconPath;
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
                return Color.white;
            }
        }

        public TraitRarity Rarity
        {
            get
            {
                return this.rarity;
            }
        }

        public float UIOrder
        {
            get
            {
                return this.uiOrder;
            }
        }

        public List<string> GoodEffects
        {
            get
            {
                return this.goodEffects;
            }
        }

        public List<string> BadEffects
        {
            get
            {
                return this.badEffects;
            }
        }

        public string EffectsString
        {
            get
            {
                if (this.effectsString == null)
                {
                    TraitSpec.tmpSb.Clear();
                    TraitSpec.tmpSb.Append("TraitEffects".Translate());
                    TraitSpec.tmpSb.Append(":");
                    foreach (string text in this.goodEffects)
                    {
                        TraitSpec.tmpSb.AppendLine();
                        TraitSpec.tmpSb.Append("- ");
                        TraitSpec.tmpSb.Append(RichText.CreateColorTag(text, GoodBadNeutralUtility.GoodEffectColor));
                    }
                    if (this == Get.Trait_LastStand)
                    {
                        TraitSpec.tmpSb.AppendLine();
                        TraitSpec.tmpSb.Append("- ");
                        TraitSpec.tmpSb.Append(RichText.CreateColorTag("TraitLastStand".Translate(0.25f.ToStringPercent(false), 0.35f.ToStringPercent(false)), GoodBadNeutralUtility.GoodEffectColor));
                    }
                    for (int i = 0; i < this.notHostileTo.Count; i++)
                    {
                        TraitSpec.tmpSb.AppendLine();
                        TraitSpec.tmpSb.Append("- ");
                        TraitSpec.tmpSb.Append(RichText.CreateColorTag("TraitNotHostileTo".Translate(RichText.Bold(this.notHostileTo[i].LabelCap)), GoodBadNeutralUtility.GoodEffectColor));
                    }
                    if (this.canFly)
                    {
                        TraitSpec.tmpSb.AppendLine();
                        TraitSpec.tmpSb.Append("- ");
                        TraitSpec.tmpSb.Append(RichText.CreateColorTag("TraitCanFly".Translate(), GoodBadNeutralUtility.GoodEffectColor));
                    }
                    if (this.gainedExpFactor > 1f)
                    {
                        TraitSpec.tmpSb.AppendLine();
                        TraitSpec.tmpSb.Append("- ");
                        TraitSpec.tmpSb.Append(RichText.CreateColorTag("TraitGainedExpFactor_More".Translate((this.gainedExpFactor - 1f).ToStringPercent(false)), GoodBadNeutralUtility.GoodEffectColor));
                    }
                    if (this.hpPerWorthyKill > 0)
                    {
                        TraitSpec.tmpSb.AppendLine();
                        TraitSpec.tmpSb.Append("- ");
                        TraitSpec.tmpSb.Append(RichText.CreateColorTag("TraitHPPerWorthyKill_Gain".Translate(this.hpPerWorthyKill), GoodBadNeutralUtility.GoodEffectColor));
                    }
                    if (this.manaPerWorthyKill > 0)
                    {
                        TraitSpec.tmpSb.AppendLine();
                        TraitSpec.tmpSb.Append("- ");
                        TraitSpec.tmpSb.Append(RichText.CreateColorTag("TraitManaPerWorthyKill".Translate(this.manaPerWorthyKill), GoodBadNeutralUtility.GoodEffectColor));
                    }
                    if (this.walkingNoiseOffset < 0)
                    {
                        TraitSpec.tmpSb.AppendLine();
                        TraitSpec.tmpSb.Append("- ");
                        TraitSpec.tmpSb.Append(RichText.CreateColorTag("TraitWalkingNoiseOffset".Translate(this.walkingNoiseOffset.ToStringOffset(true)), GoodBadNeutralUtility.GoodEffectColor));
                    }
                    if (this.backpackSlotsOffset > 0)
                    {
                        TraitSpec.tmpSb.AppendLine();
                        TraitSpec.tmpSb.Append("- ");
                        TraitSpec.tmpSb.Append(RichText.CreateColorTag("TraitBackpackSlotsOffset".Translate(this.backpackSlotsOffset.ToStringOffset(true)), GoodBadNeutralUtility.GoodEffectColor));
                    }
                    for (int j = 0; j < this.extraStartingItems.Count; j++)
                    {
                        bool flag = false;
                        for (int k = 0; k < j; k++)
                        {
                            if (this.extraStartingItems[k] == this.extraStartingItems[j])
                            {
                                flag = true;
                                break;
                            }
                        }
                        if (!flag)
                        {
                            string text2 = this.extraStartingItems[j].LabelCap;
                            int num = this.extraStartingItems.Count(this.extraStartingItems[j]);
                            if (num != 1)
                            {
                                text2 = text2 + " x" + num.ToString();
                            }
                            TraitSpec.tmpSb.AppendLine();
                            TraitSpec.tmpSb.Append("- ");
                            TraitSpec.tmpSb.Append(RichText.CreateColorTag("TraitExtraStartingItem".Translate(RichText.Bold(text2)), GoodBadNeutralUtility.GoodEffectColor));
                        }
                    }
                    if (this.maxHPFactor > 1f)
                    {
                        TraitSpec.tmpSb.AppendLine();
                        TraitSpec.tmpSb.Append("- ");
                        TraitSpec.tmpSb.Append(RichText.CreateColorTag("TraitMaxHPFactor".Translate(this.maxHPFactor.ToStringPercentOrWhole()), GoodBadNeutralUtility.GoodEffectColor));
                    }
                    if (this.maxHPOffset > 0)
                    {
                        TraitSpec.tmpSb.AppendLine();
                        TraitSpec.tmpSb.Append("- ");
                        TraitSpec.tmpSb.Append(RichText.CreateColorTag("{0} {1}".Formatted("TraitMaxHPOffset".Translate(), this.maxHPOffset.ToStringOffset(true)), GoodBadNeutralUtility.GoodEffectColor));
                    }
                    if (this.maxManaOffset > 0)
                    {
                        TraitSpec.tmpSb.AppendLine();
                        TraitSpec.tmpSb.Append("- ");
                        TraitSpec.tmpSb.Append(RichText.CreateColorTag("{0} {1}".Formatted("TraitMaxManaOffset".Translate(), this.maxManaOffset.ToStringOffset(true)), GoodBadNeutralUtility.GoodEffectColor));
                    }
                    if (this.maxStaminaOffset > 0)
                    {
                        TraitSpec.tmpSb.AppendLine();
                        TraitSpec.tmpSb.Append("- ");
                        TraitSpec.tmpSb.Append(RichText.CreateColorTag("{0} {1}".Formatted("TraitMaxStaminaOffset".Translate(), this.maxStaminaOffset.ToStringOffset(true)), GoodBadNeutralUtility.GoodEffectColor));
                    }
                    if (this.maxHpOffsetPerLevel > 0)
                    {
                        TraitSpec.tmpSb.AppendLine();
                        TraitSpec.tmpSb.Append("- ");
                        TraitSpec.tmpSb.Append(RichText.CreateColorTag("TraitMaxHPOffsetPerLevel_More".Translate(this.maxHpOffsetPerLevel), GoodBadNeutralUtility.GoodEffectColor));
                    }
                    if (this.nativeHealingRate > 1f)
                    {
                        TraitSpec.tmpSb.AppendLine();
                        TraitSpec.tmpSb.Append("- ");
                        TraitSpec.tmpSb.Append(RichText.CreateColorTag("TraitNativeHealingRate".Translate(this.nativeHealingRate.ToStringFactor()), GoodBadNeutralUtility.GoodEffectColor));
                    }
                    if (this.goldPerFloorFactor > 1f)
                    {
                        TraitSpec.tmpSb.AppendLine();
                        TraitSpec.tmpSb.Append("- ");
                        TraitSpec.tmpSb.Append(RichText.CreateColorTag("TraitGoldPerFloorFactor_More".Translate((this.goldPerFloorFactor - 1f).ToStringPercent(false)), GoodBadNeutralUtility.GoodEffectColor));
                    }
                    if (this.damageFactorAgainstBosses > 1f)
                    {
                        TraitSpec.tmpSb.AppendLine();
                        TraitSpec.tmpSb.Append("- ");
                        TraitSpec.tmpSb.Append(RichText.CreateColorTag("TraitDamageFactorAgainstBosses".Translate(this.damageFactorAgainstBosses.ToStringFactor()), GoodBadNeutralUtility.GoodEffectColor));
                    }
                    if (this.immuneToPushing)
                    {
                        TraitSpec.tmpSb.AppendLine();
                        TraitSpec.tmpSb.Append("- ");
                        TraitSpec.tmpSb.Append(RichText.CreateColorTag("TraitImmuneToPushing".Translate(), GoodBadNeutralUtility.GoodEffectColor));
                    }
                    for (int l = 0; l < this.immuneToConditionsFromDamage.Count; l++)
                    {
                        TraitSpec.tmpSb.AppendLine();
                        TraitSpec.tmpSb.Append("- ");
                        TraitSpec.tmpSb.Append(RichText.CreateColorTag("TraitImmuneToConditionsFromDamage".Translate(RichText.Bold(this.immuneToConditionsFromDamage[l].LabelCap)), GoodBadNeutralUtility.GoodEffectColor));
                    }
                    if (this.poisonIncomingDamageFactor == 0f)
                    {
                        TraitSpec.tmpSb.AppendLine();
                        TraitSpec.tmpSb.Append("- ");
                        TraitSpec.tmpSb.Append(RichText.CreateColorTag("TraitPoisonIncomingDamageFactor_Zero".Translate(), GoodBadNeutralUtility.GoodEffectColor));
                    }
                    else if (this.poisonIncomingDamageFactor < 1f)
                    {
                        TraitSpec.tmpSb.AppendLine();
                        TraitSpec.tmpSb.Append("- ");
                        TraitSpec.tmpSb.Append(RichText.CreateColorTag("TraitPoisonIncomingDamageFactor_Less".Translate((1f - this.poisonIncomingDamageFactor).ToStringPercent(false)), GoodBadNeutralUtility.GoodEffectColor));
                    }
                    if (this.fireIncomingDamageFactor < 1f)
                    {
                        TraitSpec.tmpSb.AppendLine();
                        TraitSpec.tmpSb.Append("- ");
                        TraitSpec.tmpSb.Append(RichText.CreateColorTag("TraitFireIncomingDamageFactor_Less".Translate((1f - this.fireIncomingDamageFactor).ToStringPercent(false)), GoodBadNeutralUtility.GoodEffectColor));
                    }
                    if (this.magicIncomingDamageFactor < 1f)
                    {
                        TraitSpec.tmpSb.AppendLine();
                        TraitSpec.tmpSb.Append("- ");
                        TraitSpec.tmpSb.Append(RichText.CreateColorTag("TraitMagicIncomingDamageFactor_Less".Translate((1f - this.magicIncomingDamageFactor).ToStringPercent(false)), GoodBadNeutralUtility.GoodEffectColor));
                    }
                    if (this.fallIncomingDamageFactor == 0f)
                    {
                        TraitSpec.tmpSb.AppendLine();
                        TraitSpec.tmpSb.Append("- ");
                        TraitSpec.tmpSb.Append(RichText.CreateColorTag("TraitFallIncomingDamageFactor_Zero".Translate(), GoodBadNeutralUtility.GoodEffectColor));
                    }
                    else if (this.fallIncomingDamageFactor < 1f)
                    {
                        TraitSpec.tmpSb.AppendLine();
                        TraitSpec.tmpSb.Append("- ");
                        TraitSpec.tmpSb.Append(RichText.CreateColorTag("TraitFallIncomingDamageFactor".Translate(this.fallIncomingDamageFactor.ToStringFactor()), GoodBadNeutralUtility.GoodEffectColor));
                    }
                    if (this.damageFactorAgainstActorFactor > 1f)
                    {
                        TraitSpec.tmpSb.AppendLine();
                        TraitSpec.tmpSb.Append("- ");
                        TraitSpec.tmpSb.Append(RichText.CreateColorTag("TraitDamageFactorAgainstActor".Translate(this.damageFactorAgainstActorFactor.ToStringFactor(), RichText.Bold(this.damageFactorAgainstActor.LabelCap)), GoodBadNeutralUtility.GoodEffectColor));
                    }
                    if (this.damageFactorAgainstActorFactor2 > 1f)
                    {
                        TraitSpec.tmpSb.AppendLine();
                        TraitSpec.tmpSb.Append("- ");
                        TraitSpec.tmpSb.Append(RichText.CreateColorTag("TraitDamageFactorAgainstActor".Translate(this.damageFactorAgainstActorFactor2.ToStringFactor(), RichText.Bold(this.damageFactorAgainstActor2.LabelCap)), GoodBadNeutralUtility.GoodEffectColor));
                    }
                    if (this.hungerRateFactor < 1f)
                    {
                        TraitSpec.tmpSb.AppendLine();
                        TraitSpec.tmpSb.Append("- ");
                        TraitSpec.tmpSb.Append(RichText.CreateColorTag("TraitHungerRateFactor".Translate(this.hungerRateFactor.ToStringFactor()), GoodBadNeutralUtility.GoodEffectColor));
                    }
                    if (this.wanderersAndShopkeepersPricesFactor < 1f)
                    {
                        TraitSpec.tmpSb.AppendLine();
                        TraitSpec.tmpSb.Append("- ");
                        TraitSpec.tmpSb.Append(RichText.CreateColorTag("TraitWanderersAndShopkeepersPricesFactor".Translate(this.wanderersAndShopkeepersPricesFactor.ToStringFactor()), GoodBadNeutralUtility.GoodEffectColor));
                    }
                    foreach (string text3 in this.badEffects)
                    {
                        TraitSpec.tmpSb.AppendLine();
                        TraitSpec.tmpSb.Append("- ");
                        TraitSpec.tmpSb.Append(RichText.CreateColorTag(text3, GoodBadNeutralUtility.BadEffectColor));
                    }
                    for (int m = 0; m < this.hostileTo.Count; m++)
                    {
                        TraitSpec.tmpSb.AppendLine();
                        TraitSpec.tmpSb.Append("- ");
                        TraitSpec.tmpSb.Append(RichText.CreateColorTag("TraitHostileTo".Translate(RichText.Bold(this.hostileTo[m].LabelCap)), GoodBadNeutralUtility.BadEffectColor));
                    }
                    if (this.gainedExpFactor < 1f)
                    {
                        TraitSpec.tmpSb.AppendLine();
                        TraitSpec.tmpSb.Append("- ");
                        TraitSpec.tmpSb.Append(RichText.CreateColorTag("TraitGainedExpFactor_Less".Translate((1f - this.gainedExpFactor).ToStringPercent(false)), GoodBadNeutralUtility.BadEffectColor));
                    }
                    if (this.hpPerWorthyKill < 0)
                    {
                        TraitSpec.tmpSb.AppendLine();
                        TraitSpec.tmpSb.Append("- ");
                        TraitSpec.tmpSb.Append(RichText.CreateColorTag("TraitHPPerWorthyKill_Lose".Translate(-this.hpPerWorthyKill), GoodBadNeutralUtility.BadEffectColor));
                    }
                    if (this.walkingNoiseOffset > 0)
                    {
                        TraitSpec.tmpSb.AppendLine();
                        TraitSpec.tmpSb.Append("- ");
                        TraitSpec.tmpSb.Append(RichText.CreateColorTag("TraitWalkingNoiseOffset".Translate(this.walkingNoiseOffset.ToStringOffset(true)), GoodBadNeutralUtility.BadEffectColor));
                    }
                    if (this.backpackSlotsOffset < 0)
                    {
                        TraitSpec.tmpSb.AppendLine();
                        TraitSpec.tmpSb.Append("- ");
                        TraitSpec.tmpSb.Append(RichText.CreateColorTag("TraitBackpackSlotsOffset".Translate(this.backpackSlotsOffset.ToStringOffset(true)), GoodBadNeutralUtility.BadEffectColor));
                    }
                    if (this.maxHPFactor < 1f)
                    {
                        TraitSpec.tmpSb.AppendLine();
                        TraitSpec.tmpSb.Append("- ");
                        TraitSpec.tmpSb.Append(RichText.CreateColorTag("TraitMaxHPFactor".Translate(this.maxHPFactor.ToStringPercentOrWhole()), GoodBadNeutralUtility.BadEffectColor));
                    }
                    if (this.maxHPOffset < 0)
                    {
                        TraitSpec.tmpSb.AppendLine();
                        TraitSpec.tmpSb.Append("- ");
                        TraitSpec.tmpSb.Append(RichText.CreateColorTag("{0} {1}".Formatted("TraitMaxHPOffset".Translate(), this.maxHPOffset.ToStringOffset(true)), GoodBadNeutralUtility.BadEffectColor));
                    }
                    if (this.maxManaOffset < 0)
                    {
                        TraitSpec.tmpSb.AppendLine();
                        TraitSpec.tmpSb.Append("- ");
                        TraitSpec.tmpSb.Append(RichText.CreateColorTag("{0} {1}".Formatted("TraitMaxManaOffset".Translate(), this.maxManaOffset.ToStringOffset(true)), GoodBadNeutralUtility.BadEffectColor));
                    }
                    if (this.maxStaminaOffset < 0)
                    {
                        TraitSpec.tmpSb.AppendLine();
                        TraitSpec.tmpSb.Append("- ");
                        TraitSpec.tmpSb.Append(RichText.CreateColorTag("{0} {1}".Formatted("TraitMaxStaminaOffset".Translate(), this.maxStaminaOffset.ToStringOffset(true)), GoodBadNeutralUtility.BadEffectColor));
                    }
                    if (this.maxHpOffsetPerLevel < 0)
                    {
                        TraitSpec.tmpSb.AppendLine();
                        TraitSpec.tmpSb.Append("- ");
                        TraitSpec.tmpSb.Append(RichText.CreateColorTag("TraitMaxHPOffsetPerLevel_Less".Translate(-this.maxHpOffsetPerLevel), GoodBadNeutralUtility.BadEffectColor));
                    }
                    if (this.nativeHealingRate < 1f)
                    {
                        TraitSpec.tmpSb.AppendLine();
                        TraitSpec.tmpSb.Append("- ");
                        TraitSpec.tmpSb.Append(RichText.CreateColorTag("TraitNativeHealingRate".Translate(this.nativeHealingRate.ToStringFactor()), GoodBadNeutralUtility.BadEffectColor));
                    }
                    if (this.nativeStaminaRegenIntervalFactor > 1)
                    {
                        TraitSpec.tmpSb.AppendLine();
                        TraitSpec.tmpSb.Append("- ");
                        if (this.nativeStaminaRegenIntervalFactor == 2)
                        {
                            TraitSpec.tmpSb.Append(RichText.CreateColorTag("TraitNativeStaminaRegenIntervalFactor_TwiceAsSlow".Translate(), GoodBadNeutralUtility.BadEffectColor));
                        }
                        else
                        {
                            TraitSpec.tmpSb.Append(RichText.CreateColorTag("TraitNativeStaminaRegenIntervalFactor".Translate(this.nativeStaminaRegenIntervalFactor), GoodBadNeutralUtility.BadEffectColor));
                        }
                    }
                    if (this.goldPerFloorFactor < 1f)
                    {
                        TraitSpec.tmpSb.AppendLine();
                        TraitSpec.tmpSb.Append("- ");
                        TraitSpec.tmpSb.Append(RichText.CreateColorTag("TraitGoldPerFloorFactor_Less".Translate((1f - this.goldPerFloorFactor).ToStringPercent(false)), GoodBadNeutralUtility.BadEffectColor));
                    }
                    if (this.damageFactorAgainstBosses < 1f)
                    {
                        TraitSpec.tmpSb.AppendLine();
                        TraitSpec.tmpSb.Append("- ");
                        TraitSpec.tmpSb.Append(RichText.CreateColorTag("TraitDamageFactorAgainstBosses".Translate(this.damageFactorAgainstBosses.ToStringFactor()), GoodBadNeutralUtility.BadEffectColor));
                    }
                    if (this.poisonIncomingDamageFactor > 1f)
                    {
                        TraitSpec.tmpSb.AppendLine();
                        TraitSpec.tmpSb.Append("- ");
                        TraitSpec.tmpSb.Append(RichText.CreateColorTag("TraitPoisonIncomingDamageFactor_More".Translate((this.poisonIncomingDamageFactor - 1f).ToStringPercent(false)), GoodBadNeutralUtility.BadEffectColor));
                    }
                    if (this.fireIncomingDamageFactor > 1f)
                    {
                        TraitSpec.tmpSb.AppendLine();
                        TraitSpec.tmpSb.Append("- ");
                        TraitSpec.tmpSb.Append(RichText.CreateColorTag("TraitFireIncomingDamageFactor_More".Translate((this.fireIncomingDamageFactor - 1f).ToStringPercent(false)), GoodBadNeutralUtility.BadEffectColor));
                    }
                    if (this.magicIncomingDamageFactor > 1f)
                    {
                        TraitSpec.tmpSb.AppendLine();
                        TraitSpec.tmpSb.Append("- ");
                        TraitSpec.tmpSb.Append(RichText.CreateColorTag("TraitMagicIncomingDamageFactor_More".Translate((this.magicIncomingDamageFactor - 1f).ToStringPercent(false)), GoodBadNeutralUtility.BadEffectColor));
                    }
                    if (this.fallIncomingDamageFactor > 1f)
                    {
                        TraitSpec.tmpSb.AppendLine();
                        TraitSpec.tmpSb.Append("- ");
                        TraitSpec.tmpSb.Append(RichText.CreateColorTag("TraitFallIncomingDamageFactor".Translate(this.fallIncomingDamageFactor.ToStringFactor()), GoodBadNeutralUtility.BadEffectColor));
                    }
                    if (this.damageFactorAgainstActorFactor < 1f)
                    {
                        TraitSpec.tmpSb.AppendLine();
                        TraitSpec.tmpSb.Append("- ");
                        TraitSpec.tmpSb.Append(RichText.CreateColorTag("TraitDamageFactorAgainstActor".Translate(this.damageFactorAgainstActorFactor.ToStringFactor(), RichText.Bold(this.damageFactorAgainstActor.LabelCap)), GoodBadNeutralUtility.BadEffectColor));
                    }
                    if (this.damageFactorAgainstActorFactor2 < 1f)
                    {
                        TraitSpec.tmpSb.AppendLine();
                        TraitSpec.tmpSb.Append("- ");
                        TraitSpec.tmpSb.Append(RichText.CreateColorTag("TraitDamageFactorAgainstActor".Translate(this.damageFactorAgainstActorFactor2.ToStringFactor(), RichText.Bold(this.damageFactorAgainstActor2.LabelCap)), GoodBadNeutralUtility.BadEffectColor));
                    }
                    if (this.hungerRateFactor > 1f)
                    {
                        TraitSpec.tmpSb.AppendLine();
                        TraitSpec.tmpSb.Append("- ");
                        TraitSpec.tmpSb.Append(RichText.CreateColorTag("TraitHungerRateFactor".Translate(this.hungerRateFactor.ToStringFactor()), GoodBadNeutralUtility.BadEffectColor));
                    }
                    if (this.wanderersAndShopkeepersPricesFactor > 1f)
                    {
                        TraitSpec.tmpSb.AppendLine();
                        TraitSpec.tmpSb.Append("- ");
                        TraitSpec.tmpSb.Append(RichText.CreateColorTag("TraitWanderersAndShopkeepersPricesFactor".Translate(this.wanderersAndShopkeepersPricesFactor.ToStringFactor()), GoodBadNeutralUtility.BadEffectColor));
                    }
                    this.effectsString = TraitSpec.tmpSb.ToString();
                }
                return this.effectsString;
            }
        }

        public List<EntitySpec> ExtraStartingItems
        {
            get
            {
                return this.extraStartingItems;
            }
        }

        public float GoldPerFloorFactor
        {
            get
            {
                return this.goldPerFloorFactor;
            }
        }

        public float MaxHPFactor
        {
            get
            {
                return this.maxHPFactor;
            }
        }

        public int MaxHPOffset
        {
            get
            {
                return this.maxHPOffset;
            }
        }

        public int MaxManaOffset
        {
            get
            {
                return this.maxManaOffset;
            }
        }

        public int MaxStaminaOffset
        {
            get
            {
                return this.maxStaminaOffset;
            }
        }

        public List<FactionSpec> NotHostileTo
        {
            get
            {
                return this.notHostileTo;
            }
        }

        public List<FactionSpec> HostileTo
        {
            get
            {
                return this.hostileTo;
            }
        }

        public float PoisonIncomingDamageFactor
        {
            get
            {
                return this.poisonIncomingDamageFactor;
            }
        }

        public float FallIncomingDamageFactor
        {
            get
            {
                return this.fallIncomingDamageFactor;
            }
        }

        public float FireIncomingDamageFactor
        {
            get
            {
                return this.fireIncomingDamageFactor;
            }
        }

        public float MagicIncomingDamageFactor
        {
            get
            {
                return this.magicIncomingDamageFactor;
            }
        }

        public EntitySpec DamageFactorAgainstActor
        {
            get
            {
                return this.damageFactorAgainstActor;
            }
        }

        public float DamageFactorAgainstActorFactor
        {
            get
            {
                return this.damageFactorAgainstActorFactor;
            }
        }

        public EntitySpec DamageFactorAgainstActor2
        {
            get
            {
                return this.damageFactorAgainstActor2;
            }
        }

        public float DamageFactorAgainstActorFactor2
        {
            get
            {
                return this.damageFactorAgainstActorFactor2;
            }
        }

        public int NativeStaminaRegenIntervalFactor
        {
            get
            {
                return this.nativeStaminaRegenIntervalFactor;
            }
        }

        public List<GenericUsableSpec> Abilities
        {
            get
            {
                return this.abilities;
            }
        }

        public int MaxHPOffsetPerLevel
        {
            get
            {
                return this.maxHpOffsetPerLevel;
            }
        }

        public int WalkingNoiseOffset
        {
            get
            {
                return this.walkingNoiseOffset;
            }
        }

        public int ManaPerWorthyKill
        {
            get
            {
                return this.manaPerWorthyKill;
            }
        }

        public int HPPerWorthyKill
        {
            get
            {
                return this.hpPerWorthyKill;
            }
        }

        public int BackpackSlotsOffset
        {
            get
            {
                return this.backpackSlotsOffset;
            }
        }

        public float DamageFactorAgainstBosses
        {
            get
            {
                return this.damageFactorAgainstBosses;
            }
        }

        public float NativeHealingRate
        {
            get
            {
                return this.nativeHealingRate;
            }
        }

        public List<ConditionSpec> ImmuneToConditionsFromDamage
        {
            get
            {
                return this.immuneToConditionsFromDamage;
            }
        }

        public bool ImmuneToPushing
        {
            get
            {
                return this.immuneToPushing;
            }
        }

        public float HungerRateFactor
        {
            get
            {
                return this.hungerRateFactor;
            }
        }

        public float WanderersAndShopkeepersPricesFactor
        {
            get
            {
                return this.wanderersAndShopkeepersPricesFactor;
            }
        }

        public bool CanFly
        {
            get
            {
                return this.canFly;
            }
        }

        public float GainedExpFactor
        {
            get
            {
                return this.gainedExpFactor;
            }
        }

        public void OnLoaded()
        {
            if (!this.iconPath.NullOrEmpty())
            {
                this.icon = Assets.Get<Texture2D>(this.iconPath);
                return;
            }
            Log.Error("TraitSpec " + base.SpecID + " has no icon.", false);
        }

        public void OnSaved()
        {
        }

        [Saved]
        private string iconPath;

        [Saved]
        private TraitRarity rarity;

        [Saved]
        private float uiOrder;

        [Saved(Default.New, false)]
        [Translatable]
        private List<string> goodEffects = new List<string>();

        [Saved(Default.New, false)]
        [Translatable]
        private List<string> badEffects = new List<string>();

        [Saved(Default.New, false)]
        private List<EntitySpec> extraStartingItems = new List<EntitySpec>();

        [Saved(1f, false)]
        private float goldPerFloorFactor = 1f;

        [Saved(1f, false)]
        private float maxHPFactor = 1f;

        [Saved]
        private int maxHPOffset;

        [Saved]
        private int maxManaOffset;

        [Saved]
        private int maxStaminaOffset;

        [Saved(Default.New, false)]
        private List<FactionSpec> notHostileTo = new List<FactionSpec>();

        [Saved(Default.New, false)]
        private List<FactionSpec> hostileTo = new List<FactionSpec>();

        [Saved(1f, false)]
        private float poisonIncomingDamageFactor = 1f;

        [Saved(1f, false)]
        private float fallIncomingDamageFactor = 1f;

        [Saved(1f, false)]
        private float fireIncomingDamageFactor = 1f;

        [Saved(1f, false)]
        private float magicIncomingDamageFactor = 1f;

        [Saved]
        private EntitySpec damageFactorAgainstActor;

        [Saved(1f, false)]
        private float damageFactorAgainstActorFactor = 1f;

        [Saved]
        private EntitySpec damageFactorAgainstActor2;

        [Saved(1f, false)]
        private float damageFactorAgainstActorFactor2 = 1f;

        [Saved(1, false)]
        private int nativeStaminaRegenIntervalFactor = 1;

        [Saved(Default.New, false)]
        private List<GenericUsableSpec> abilities = new List<GenericUsableSpec>();

        [Saved]
        private int maxHpOffsetPerLevel;

        [Saved]
        private int walkingNoiseOffset;

        [Saved]
        private int manaPerWorthyKill;

        [Saved]
        private int hpPerWorthyKill;

        [Saved]
        private int backpackSlotsOffset;

        [Saved(1f, false)]
        private float damageFactorAgainstBosses = 1f;

        [Saved(1f, false)]
        private float nativeHealingRate = 1f;

        [Saved(Default.New, false)]
        private List<ConditionSpec> immuneToConditionsFromDamage = new List<ConditionSpec>();

        [Saved]
        private bool immuneToPushing;

        [Saved(1f, false)]
        private float hungerRateFactor = 1f;

        [Saved(1f, false)]
        private float wanderersAndShopkeepersPricesFactor = 1f;

        [Saved]
        private bool canFly;

        [Saved(1f, false)]
        private float gainedExpFactor = 1f;

        private static StringBuilder tmpSb = new StringBuilder();

        private Texture2D icon;

        private string effectsString;

        public const float LastStandHPThreshold = 0.35f;

        public const float LastStandDamageFactor = 1.25f;
    }
}