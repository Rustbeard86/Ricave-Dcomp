using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public static class UsableUtility
    {
        public static IntRange? GetDealtDamageRangeForUI(Actor user, IUsable usable, Entity target, BodyPart targetBodyPart)
        {
            IntRange? intRange = null;
            foreach (UseEffect useEffect in usable.UseEffects.All)
            {
                if (!useEffect.AoEExcludeCenter && ((useEffect.AoERadius == null && !useEffect.HasCustomAoEArea) || useEffect.AoEHandledManually || usable.UseFilterAoE.Allows(target, user)))
                {
                    UseEffect_Damage useEffect_Damage = useEffect as UseEffect_Damage;
                    int num;
                    int num2;
                    if (useEffect_Damage != null)
                    {
                        string text;
                        useEffect_Damage.GetFinalDamageAmount(user, target, targetBodyPart, false, out text, out num, out num2, true);
                    }
                    else if (useEffect is UseEffect_CheckCollapseCeiling)
                    {
                        num = 5;
                        num2 = 5;
                    }
                    else
                    {
                        if (!(useEffect is UseEffect_CheckCollapseCeilingSingle))
                        {
                            continue;
                        }
                        num = 4;
                        num2 = 4;
                    }
                    if (intRange != null)
                    {
                        intRange = new IntRange?(new IntRange(intRange.Value.from + num, intRange.Value.to + num2));
                    }
                    else
                    {
                        intRange = new IntRange?(new IntRange(num, num2));
                    }
                }
            }
            return intRange;
        }

        public static IntRange? GetAoEDealtDamageRangeForUI(Actor user, IUsable usable, Entity target, Target originalTarget)
        {
            IntRange? intRange = null;
            foreach (UseEffect useEffect in usable.UseEffects.All)
            {
                if (useEffect.AoERadius != null || useEffect.HasCustomAoEArea || useEffect is UseEffect_CheckCollapseCeiling || useEffect is UseEffect_CheckCollapseCeilingSingle)
                {
                    if (!useEffect.AoEHandledManually && !(useEffect is UseEffect_CheckCollapseCeiling) && !(useEffect is UseEffect_CheckCollapseCeilingSingle))
                    {
                        UsableUtility.tmpAoETargets.Clear();
                        AoEUtility.GetAoETargets(originalTarget.Position, useEffect, usable, user, UsableUtility.tmpAoETargets);
                        if (!UsableUtility.tmpAoETargets.Contains(target))
                        {
                            UsableUtility.tmpAoETargets.Clear();
                            continue;
                        }
                        UsableUtility.tmpAoETargets.Clear();
                    }
                    UseEffect_Damage useEffect_Damage = useEffect as UseEffect_Damage;
                    int num;
                    int num2;
                    if (useEffect_Damage != null)
                    {
                        string text;
                        useEffect_Damage.GetFinalDamageAmount(user, target, null, false, out text, out num, out num2, true);
                    }
                    else
                    {
                        UseEffect_CheckCollapseCeiling useEffect_CheckCollapseCeiling = useEffect as UseEffect_CheckCollapseCeiling;
                        if (useEffect_CheckCollapseCeiling != null)
                        {
                            UsableUtility.tmpCollapsedCeiling.Clear();
                            useEffect_CheckCollapseCeiling.GetAffectedCellsForUIAssumingChainedPillarDestroy(originalTarget.Entity, UsableUtility.tmpCollapsedCeiling);
                            if (!UsableUtility.tmpCollapsedCeiling.Contains(target.Position))
                            {
                                continue;
                            }
                            num = 5;
                            num2 = 5;
                        }
                        else
                        {
                            UseEffect_CheckCollapseCeilingSingle useEffect_CheckCollapseCeilingSingle = useEffect as UseEffect_CheckCollapseCeilingSingle;
                            if (useEffect_CheckCollapseCeilingSingle == null)
                            {
                                continue;
                            }
                            UsableUtility.tmpCollapsedCeiling.Clear();
                            useEffect_CheckCollapseCeilingSingle.GetAffectedCellsForUI(originalTarget.Entity, UsableUtility.tmpCollapsedCeiling);
                            if (!UsableUtility.tmpCollapsedCeiling.Contains(target.Position))
                            {
                                continue;
                            }
                            num = 4;
                            num2 = 4;
                        }
                    }
                    if (intRange != null)
                    {
                        intRange = new IntRange?(new IntRange(intRange.Value.from + num, intRange.Value.to + num2));
                    }
                    else
                    {
                        intRange = new IntRange?(new IntRange(num, num2));
                    }
                }
            }
            return intRange;
        }

        public static bool SeenByAnyHostile(Actor actor)
        {
            foreach (Actor actor2 in Get.World.Actors)
            {
                if (actor2 != actor && actor2.IsHostile(actor) && actor2.Sees(actor, null) && !ForcedActionsHelper.AnyForcedAction(actor2))
                {
                    return true;
                }
            }
            return false;
        }

        public static string CheckAppendDotsToUseLabel(string useLabel_self, IUsable usable)
        {
            if (useLabel_self.NullOrEmpty())
            {
                return useLabel_self;
            }
            if (useLabel_self[useLabel_self.Length - 1] == '.')
            {
                return useLabel_self;
            }
            if (usable.UsePrompt == null)
            {
                return useLabel_self;
            }
            return "{0}...".Formatted(useLabel_self);
        }

        private static List<Target> tmpAoETargets = new List<Target>();

        private static List<Vector3Int> tmpCollapsedCeiling = new List<Vector3Int>();
    }
}