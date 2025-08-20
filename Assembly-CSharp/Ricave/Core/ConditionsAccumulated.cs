using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class ConditionsAccumulated
    {
        public Actor Actor
        {
            get
            {
                return this.actor;
            }
        }

        public List<Condition> AllConditions
        {
            get
            {
                if (this.allConditionsCachedForVersion == ConditionsAccumulated.conditionsVersion)
                {
                    return this.allConditionsCached;
                }
                this.allConditionsCachedForVersion = ConditionsAccumulated.conditionsVersion;
                this.allConditionsCached.Clear();
                this.allConditionsCached.AddRange(this.actor.Conditions.All);
                List<Item> equippedItems = this.actor.Inventory.EquippedItems;
                for (int i = 0; i < equippedItems.Count; i++)
                {
                    this.allConditionsCached.AddRange(equippedItems[i].ConditionsEquipped.All);
                }
                return this.allConditionsCached;
            }
        }

        public List<ConditionDrawRequest> ActorConditionDrawRequestsPlusExtra
        {
            get
            {
                this.tmpActorConditionDrawRequests.Clear();
                ExtraDrawConditionRequests.GetExtraConditionDrawRequests(this.actor, this.tmpActorConditionDrawRequests);
                List<Condition> all = this.actor.Conditions.All;
                for (int i = 0; i < all.Count; i++)
                {
                    ConditionDrawRequest? conditionDrawRequest = all[i].ToDrawRequestOrNull();
                    if (conditionDrawRequest != null)
                    {
                        this.tmpActorConditionDrawRequests.Add(conditionDrawRequest.Value);
                    }
                }
                return this.tmpActorConditionDrawRequests;
            }
        }

        public List<ConditionDrawRequest> AllConditionDrawRequestsPlusExtra
        {
            get
            {
                this.tmpAllConditionDrawRequests.Clear();
                ExtraDrawConditionRequests.GetExtraConditionDrawRequests(this.actor, this.tmpAllConditionDrawRequests);
                List<Condition> allConditions = this.AllConditions;
                for (int i = 0; i < allConditions.Count; i++)
                {
                    ConditionDrawRequest? conditionDrawRequest = allConditions[i].ToDrawRequestOrNull();
                    if (conditionDrawRequest != null)
                    {
                        this.tmpAllConditionDrawRequests.Add(conditionDrawRequest.Value);
                    }
                }
                return this.tmpAllConditionDrawRequests;
            }
        }

        public bool AnyForcedAction
        {
            get
            {
                List<Condition> allConditions = this.AllConditions;
                for (int i = 0; i < allConditions.Count; i++)
                {
                    if (allConditions[i].AnyForcedAction)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public Condition FirstConditionWithForcedAction
        {
            get
            {
                List<Condition> allConditions = this.AllConditions;
                for (int i = 0; i < allConditions.Count; i++)
                {
                    if (allConditions[i].AnyForcedAction)
                    {
                        return allConditions[i];
                    }
                }
                return null;
            }
        }

        public bool CanFly
        {
            get
            {
                List<Condition> allConditions = this.AllConditions;
                bool flag = false;
                for (int i = 0; i < allConditions.Count; i++)
                {
                    if (allConditions[i].DisableFlying)
                    {
                        return false;
                    }
                    if (allConditions[i].CanFly)
                    {
                        flag = true;
                    }
                }
                return flag;
            }
        }

        public bool CanJumpOffLedge
        {
            get
            {
                List<Condition> allConditions = this.AllConditions;
                for (int i = 0; i < allConditions.Count; i++)
                {
                    if (allConditions[i].CanJumpOffLedge)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public bool DisableCanJumpOffLedge
        {
            get
            {
                List<Condition> allConditions = this.AllConditions;
                for (int i = 0; i < allConditions.Count; i++)
                {
                    if (allConditions[i].DisableCanJumpOffLedge)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public bool AllowPathingIntoAIAvoids
        {
            get
            {
                List<Condition> allConditions = this.AllConditions;
                for (int i = 0; i < allConditions.Count; i++)
                {
                    if (allConditions[i].AllowPathingIntoAIAvoids)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public int MaxHPOffset
        {
            get
            {
                int num = 0;
                List<Condition> allConditions = this.AllConditions;
                for (int i = 0; i < allConditions.Count; i++)
                {
                    num += allConditions[i].MaxHPOffset;
                }
                return num;
            }
        }

        public float MaxHPFactor
        {
            get
            {
                float num = 1f;
                List<Condition> allConditions = this.AllConditions;
                for (int i = 0; i < allConditions.Count; i++)
                {
                    num *= allConditions[i].MaxHPFactor;
                }
                return num;
            }
        }

        public int MaxManaOffset
        {
            get
            {
                int num = 0;
                List<Condition> allConditions = this.AllConditions;
                for (int i = 0; i < allConditions.Count; i++)
                {
                    num += allConditions[i].MaxManaOffset;
                }
                return num;
            }
        }

        public int MaxStaminaOffset
        {
            get
            {
                int num = 0;
                List<Condition> allConditions = this.AllConditions;
                for (int i = 0; i < allConditions.Count; i++)
                {
                    num += allConditions[i].MaxStaminaOffset;
                }
                return num;
            }
        }

        public int SequencePerTurn
        {
            get
            {
                int num = 12;
                List<Condition> allConditions = this.AllConditions;
                for (int i = 0; i < allConditions.Count; i++)
                {
                    if (!allConditions[i].SequencePerTurnMultiplierInvert)
                    {
                        num *= allConditions[i].SequencePerTurnMultiplier;
                    }
                }
                for (int j = 0; j < allConditions.Count; j++)
                {
                    if (allConditions[j].SequencePerTurnMultiplierInvert)
                    {
                        num /= allConditions[j].SequencePerTurnMultiplier;
                    }
                }
                return Math.Max(num, 1);
            }
        }

        public float SequencePerMoveMultiplier
        {
            get
            {
                float num = 1f;
                List<Condition> allConditions = this.AllConditions;
                for (int i = 0; i < allConditions.Count; i++)
                {
                    num *= allConditions[i].SequencePerMoveMultiplier;
                }
                return num;
            }
        }

        public int SeeRangeOffset
        {
            get
            {
                int num = 0;
                List<Condition> allConditions = this.AllConditions;
                for (int i = 0; i < allConditions.Count; i++)
                {
                    num += allConditions[i].SeeRangeOffset;
                }
                return num;
            }
        }

        public int SeeRangeCap
        {
            get
            {
                int num = 10;
                List<Condition> allConditions = this.AllConditions;
                for (int i = 0; i < allConditions.Count; i++)
                {
                    num = Math.Min(num, allConditions[i].SeeRangeCap);
                }
                return num;
            }
        }

        public bool ImmuneToPoison
        {
            get
            {
                List<Condition> allConditions = this.AllConditions;
                for (int i = 0; i < allConditions.Count; i++)
                {
                    if (allConditions[i].ImmuneToPoison)
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
                List<Condition> allConditions = this.AllConditions;
                for (int i = 0; i < allConditions.Count; i++)
                {
                    if (allConditions[i].ImmuneToFire)
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
                List<Condition> allConditions = this.AllConditions;
                for (int i = 0; i < allConditions.Count; i++)
                {
                    if (allConditions[i].ImmuneToDisease)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public bool InvertHostile
        {
            get
            {
                List<Condition> allConditions = this.AllConditions;
                for (int i = 0; i < allConditions.Count; i++)
                {
                    if (allConditions[i].InvertHostile)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public bool StopIdentification
        {
            get
            {
                List<Condition> allConditions = this.AllConditions;
                for (int i = 0; i < allConditions.Count; i++)
                {
                    if (allConditions[i].StopIdentification)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public bool DisableNativeHPRegen
        {
            get
            {
                List<Condition> allConditions = this.AllConditions;
                for (int i = 0; i < allConditions.Count; i++)
                {
                    if (allConditions[i].DisableNativeHPRegen)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public bool DisableNativeStaminaRegen
        {
            get
            {
                List<Condition> allConditions = this.AllConditions;
                for (int i = 0; i < allConditions.Count; i++)
                {
                    if (allConditions[i].DisableNativeStaminaRegen)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public bool DisableNativeManaRegen
        {
            get
            {
                List<Condition> allConditions = this.AllConditions;
                for (int i = 0; i < allConditions.Count; i++)
                {
                    if (allConditions[i].DisableNativeManaRegen)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public int NativeStaminaRegenIntervalFactor
        {
            get
            {
                int num = 1;
                List<Condition> allConditions = this.AllConditions;
                for (int i = 0; i < allConditions.Count; i++)
                {
                    num *= allConditions[i].NativeStaminaRegenIntervalFactor;
                }
                return num;
            }
        }

        public bool StopDanceAnimation
        {
            get
            {
                List<Condition> allConditions = this.AllConditions;
                for (int i = 0; i < allConditions.Count; i++)
                {
                    if (allConditions[i].StopDanceAnimation)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public bool Lying
        {
            get
            {
                List<Condition> allConditions = this.AllConditions;
                for (int i = 0; i < allConditions.Count; i++)
                {
                    if (allConditions[i].Lying)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public bool DisableDamageDealtToPlayer
        {
            get
            {
                List<Condition> allConditions = this.AllConditions;
                for (int i = 0; i < allConditions.Count; i++)
                {
                    if (allConditions[i].DisableDamageDealtToPlayer)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public Color? ActorOverlayColor
        {
            get
            {
                List<Condition> allConditions = this.AllConditions;
                for (int i = 0; i < allConditions.Count; i++)
                {
                    if (allConditions[i].Spec.ActorOverlayColor != null && !allConditions[i].Spec.ActorOverlayColorScreenOnly)
                    {
                        return allConditions[i].Spec.ActorOverlayColor;
                    }
                }
                return null;
            }
        }

        public Color? ActorOverlayColorForScreen
        {
            get
            {
                List<Condition> allConditions = this.AllConditions;
                for (int i = 0; i < allConditions.Count; i++)
                {
                    if (allConditions[i].Spec.ActorOverlayColor != null)
                    {
                        return allConditions[i].Spec.ActorOverlayColor;
                    }
                }
                return null;
            }
        }

        public ValueTuple<SoundSpec, float> SoundLoop
        {
            get
            {
                List<Condition> allConditions = this.AllConditions;
                for (int i = 0; i < allConditions.Count; i++)
                {
                    if (allConditions[i].Spec.SoundLoop != null)
                    {
                        return new ValueTuple<SoundSpec, float>(allConditions[i].Spec.SoundLoop, allConditions[i].Spec.SoundLoopMaxDistance);
                    }
                }
                return default(ValueTuple<SoundSpec, float>);
            }
        }

        public float ActorScaleFactor
        {
            get
            {
                float num = 1f;
                List<Condition> allConditions = this.AllConditions;
                for (int i = 0; i < allConditions.Count; i++)
                {
                    num *= allConditions[i].ActorScaleFactor;
                }
                return num;
            }
        }

        public bool MovingDisallowed
        {
            get
            {
                List<Condition> allConditions = this.AllConditions;
                bool? flag = null;
                for (int i = 0; i < allConditions.Count; i++)
                {
                    if (allConditions[i].MovingDisallowed)
                    {
                        return true;
                    }
                    if (allConditions[i].MovingDisallowedIfCantFly)
                    {
                        if (flag == null)
                        {
                            flag = new bool?(this.actor.CanFly);
                        }
                        if (!flag.Value)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }

        public bool MovingDisallowedIfCantFly
        {
            get
            {
                List<Condition> allConditions = this.AllConditions;
                for (int i = 0; i < allConditions.Count; i++)
                {
                    if (allConditions[i].MovingDisallowedIfCantFly)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public bool LevitatingDisabledByBodyParts
        {
            get
            {
                List<Condition> allConditions = this.AllConditions;
                for (int i = 0; i < allConditions.Count; i++)
                {
                    if (allConditions[i].LevitatingDisabledByBodyParts)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public float MinMissChanceOverride
        {
            get
            {
                float num = 0f;
                List<Condition> allConditions = this.AllConditions;
                for (int i = 0; i < allConditions.Count; i++)
                {
                    num = Math.Max(num, allConditions[i].MinMissChanceOverride);
                }
                return num;
            }
        }

        public Condition MinMissChanceOverrideCondition
        {
            get
            {
                float num = 0f;
                Condition condition = null;
                List<Condition> allConditions = this.AllConditions;
                for (int i = 0; i < allConditions.Count; i++)
                {
                    if (allConditions[i].MinMissChanceOverride > num)
                    {
                        num = allConditions[i].MinMissChanceOverride;
                        condition = allConditions[i];
                    }
                }
                return condition;
            }
        }

        public float HungerRateMultiplier
        {
            get
            {
                float num = 1f;
                List<Condition> allConditions = this.AllConditions;
                for (int i = 0; i < allConditions.Count; i++)
                {
                    num *= allConditions[i].HungerRateMultiplier;
                }
                return num;
            }
        }

        public float IdentificationRateMultiplier
        {
            get
            {
                float num = 1f;
                List<Condition> allConditions = this.AllConditions;
                for (int i = 0; i < allConditions.Count; i++)
                {
                    num *= allConditions[i].IdentificationRateMultiplier;
                }
                return num;
            }
        }

        public float CritChanceFactor
        {
            get
            {
                float num = 1f;
                List<Condition> allConditions = this.AllConditions;
                for (int i = 0; i < allConditions.Count; i++)
                {
                    num *= allConditions[i].CritChanceFactor;
                }
                return num;
            }
        }

        public float MissChanceFactor
        {
            get
            {
                float num = 1f;
                List<Condition> allConditions = this.AllConditions;
                for (int i = 0; i < allConditions.Count; i++)
                {
                    num *= allConditions[i].MissChanceFactor;
                }
                return num;
            }
        }

        public ConditionsAccumulated(Actor actor)
        {
            this.actor = actor;
        }

        public Action GetForcedAction()
        {
            List<Condition> allConditions = this.AllConditions;
            for (int i = 0; i < allConditions.Count; i++)
            {
                Action forcedAction = allConditions[i].GetForcedAction(this.actor);
                if (forcedAction != null)
                {
                    return forcedAction;
                }
            }
            return null;
        }

        public Condition GetFirstOfSpec(ConditionSpec spec)
        {
            List<Condition> allConditions = this.AllConditions;
            for (int i = 0; i < allConditions.Count; i++)
            {
                if (allConditions[i].Spec == spec)
                {
                    return allConditions[i];
                }
            }
            return null;
        }

        public bool AnyOfSpec(ConditionSpec spec)
        {
            return this.GetFirstOfSpec(spec) != null;
        }

        public static void SetCachedConditionsDirty()
        {
            ConditionsAccumulated.conditionsVersion++;
        }

        private Actor actor;

        private List<Condition> allConditionsCached = new List<Condition>();

        private int allConditionsCachedForVersion = -1;

        private List<ConditionDrawRequest> tmpActorConditionDrawRequests = new List<ConditionDrawRequest>();

        private List<ConditionDrawRequest> tmpAllConditionDrawRequests = new List<ConditionDrawRequest>();

        private static int conditionsVersion;
    }
}