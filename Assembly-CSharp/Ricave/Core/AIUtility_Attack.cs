using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Ricave.Core
{
    public static class AIUtility_Attack
    {
        public static IEnumerable<Actor> GetJustSeenHostilesInAttackPriority(Actor actor)
        {
            return from x in AIUtility.GetJustSeenHostiles(actor)
                   orderby x.Position.GetGridDistance(actor.Position), x.HP, x.IsPlayerParty descending, x.MyStableHash
                   select x;
        }

        public static Actor TryGetActorToAttackFromCurrentPos(Actor actor, IUsable weapon)
        {
            if (!weapon.UseEffects.Any)
            {
                return null;
            }
            foreach (Actor actor2 in AIUtility_Attack.GetJustSeenHostilesInAttackPriority(actor))
            {
                if (actor.CanUseOn(weapon, actor2, null, false, null))
                {
                    return actor2;
                }
            }
            return null;
        }

        public static Action TryGetAttackAnyFromCurrentPosAction(Actor actor, IUsable weapon)
        {
            Actor actor2 = AIUtility_Attack.TryGetActorToAttackFromCurrentPos(actor, weapon);
            if (actor2 == null || !actor.CanUseOn(weapon, actor2, null, false, null))
            {
                return null;
            }
            return new Action_Use(Get.Action_Use, actor, weapon, actor2, null);
        }

        public static Action TryGetAttackOrApproachAction(Actor actor, IUsable weapon, Actor target, Vector3Int? lastKnownTargetPos, bool keepDistance)
        {
            if (!AIUtility_Attack.IsAttackTargetValidAndReachableFor(target, actor, weapon, lastKnownTargetPos, keepDistance))
            {
                return null;
            }
            if (actor.CanUseOn(weapon, target, null, false, null))
            {
                return new Action_Use(Get.Action_Use, actor, weapon, target, null);
            }
            if (AIUtility.SeesOrJustSeen(actor, target))
            {
                if (keepDistance && actor.Position.GetGridDistance(target.Position) <= 2)
                {
                    return new Action_Wait(Get.Action_Wait, actor, null, null);
                }
                if (Get.World.CanTouch(actor.Position, target.Position, actor))
                {
                    return null;
                }
                List<Vector3Int> list = Get.PathFinder.FindPath(actor.Position, target.Position, new PathFinder.Request(PathFinder.Request.Mode.Touch, actor), 15, null);
                if (list != null)
                {
                    Vector3Int vector3Int = list[list.Count - 2];
                    if (keepDistance && AIUtility.AnyAdjacentHostileActorCanTouchMeAt(vector3Int, actor))
                    {
                        return new Action_Wait(Get.Action_Wait, actor, null, null);
                    }
                    return new Action_MoveSelf(Get.Action_MoveSelf, actor, actor.Position, vector3Int, null, false);
                }
                else
                {
                    if (weapon.UseRange <= 1)
                    {
                        return null;
                    }
                    List<Vector3Int> list2;
                    if (AIUtility_Attack.TryGetShootPos(actor, target, weapon, keepDistance, out list2) && list2.Count >= 2)
                    {
                        return new Action_MoveSelf(Get.Action_MoveSelf, actor, actor.Position, list2[list2.Count - 2], null, false);
                    }
                    return null;
                }
            }
            else
            {
                if (lastKnownTargetPos == null)
                {
                    return null;
                }
                if (actor.Position == lastKnownTargetPos)
                {
                    return null;
                }
                if (keepDistance && actor.Position.GetGridDistance(lastKnownTargetPos.Value) <= 2)
                {
                    return null;
                }
                PathFinder.Request.Mode mode = ((!Get.CellsInfo.CanPassThrough(lastKnownTargetPos.Value) || Get.CellsInfo.IsFallingAt(lastKnownTargetPos.Value, actor, false)) ? PathFinder.Request.Mode.Touch : PathFinder.Request.Mode.ToCell);
                if (mode == PathFinder.Request.Mode.Touch && Get.World.CanTouch(actor.Position, lastKnownTargetPos.Value, actor))
                {
                    return null;
                }
                List<Vector3Int> list3 = Get.PathFinder.FindPath(actor.Position, lastKnownTargetPos.Value, new PathFinder.Request(mode, actor), 15, null);
                if (list3 == null)
                {
                    return null;
                }
                return new Action_MoveSelf(Get.Action_MoveSelf, actor, actor.Position, list3[list3.Count - 2], null, false);
            }
        }

        public static Actor GetNewBestAttackTargetFor(Actor actor)
        {
            IUsable weaponToUse = AIUtility_Attack.GetWeaponToUse(actor, true);
            if (weaponToUse == null)
            {
                return null;
            }
            if (!weaponToUse.UseEffects.Any)
            {
                return null;
            }
            Get.ShortestPathsCache.InitFor(actor.Position, actor, Math.Min(Math.Max(actor.SeeRange + 1, 5), 15));
            foreach (Actor actor2 in AIUtility_Attack.GetJustSeenHostilesInAttackPriority(actor))
            {
                List<Vector3Int> list;
                if (actor2.Spawned && actor.CanUseOn(weaponToUse, actor2, null, true, null) && (!Get.World.CanTouch(actor.Position, actor2.Position, actor) || actor.CanUseOn(weaponToUse, actor2, null, false, null)) && (Get.ShortestPathsCache.CanReach(actor2.Position, PathFinder.Request.Mode.Touch) || (weaponToUse.UseRange > 1 && AIUtility_Attack.TryGetShootPos(actor, actor2, weaponToUse, false, out list))))
                {
                    return actor2;
                }
            }
            return null;
        }

        public static bool IsAttackTargetValidAndReachableFor(Actor attackTarget, Actor forActor, IUsable weaponToUse, Vector3Int? lastKnownTargetPos, bool keepDistance)
        {
            if (!attackTarget.Spawned)
            {
                return false;
            }
            if (!forActor.CanUseOn(weaponToUse, attackTarget, null, true, null))
            {
                return false;
            }
            if (!forActor.IsHostile(attackTarget))
            {
                return false;
            }
            if (forActor.CanUseOn(weaponToUse, attackTarget, null, false, null))
            {
                return true;
            }
            if (AIUtility.SeesOrJustSeen(forActor, attackTarget))
            {
                List<Vector3Int> list;
                return Get.PathFinder.FindPath(forActor.Position, attackTarget.Position, new PathFinder.Request(PathFinder.Request.Mode.Touch, forActor), 15, null) != null || (weaponToUse.UseRange > 1 && AIUtility_Attack.TryGetShootPos(forActor, attackTarget, weaponToUse, keepDistance, out list));
            }
            if (lastKnownTargetPos == null)
            {
                return false;
            }
            if (forActor.Position == lastKnownTargetPos)
            {
                return false;
            }
            if (keepDistance && forActor.Position.GetGridDistance(lastKnownTargetPos.Value) <= 2)
            {
                return false;
            }
            PathFinder.Request.Mode mode = ((!Get.CellsInfo.CanPassThrough(lastKnownTargetPos.Value) || Get.CellsInfo.IsFallingAt(lastKnownTargetPos.Value, forActor, false)) ? PathFinder.Request.Mode.Touch : PathFinder.Request.Mode.ToCell);
            return (mode != PathFinder.Request.Mode.Touch || !Get.World.CanTouch(forActor.Position, lastKnownTargetPos.Value, forActor)) && Get.PathFinder.FindPath(forActor.Position, lastKnownTargetPos.Value, new PathFinder.Request(mode, forActor), 15, null) != null;
        }

        public static bool TryGetShootPos(Actor forActor, Actor toShoot, IUsable weapon, bool tryKeepDistance, out List<Vector3Int> path)
        {
            if (forActor.CanUseOn(weapon, toShoot, null, false, null))
            {
                path = new List<Vector3Int> { forActor.Position };
                return true;
            }
            Get.ShortestPathsCache.InitFor(forActor.Position, forActor, 5);
            if (tryKeepDistance)
            {
                Func<Vector3Int, bool> <> 9__0;
                foreach (IEnumerable<Vector3Int> enumerable in Get.ShortestPathsCache.AllReachableWithToCellMode.GetGroupedByDistTo(forActor.Position))
                {
                    Func<Vector3Int, bool> func;
                    if ((func = <> 9__0) == null)
                    {
                        func = (<> 9__0 = (Vector3Int x) => !x.IsAdjacent(toShoot.Position) && !AIUtility.AnyAdjacentHostileActorCanTouchMeAt(x, forActor) && forActor.CanUseOn(weapon, toShoot, new Vector3Int?(x), false, null));
                    }
                    Vector3Int vector3Int;
                    if (enumerable.Where<Vector3Int>(func).TryGetRandomElement<Vector3Int>(out vector3Int))
                    {
                        path = Get.ShortestPathsCache.GetPathTo(vector3Int, PathFinder.Request.Mode.ToCell);
                        if (path == null)
                        {
                            Log.Error("ShortestPathsCache.AllReachableWithToCellMode returned an unreachable cell.", false);
                        }
                        return true;
                    }
                }
            }
            Func<Vector3Int, bool> <> 9__1;
            foreach (IEnumerable<Vector3Int> enumerable2 in Get.ShortestPathsCache.AllReachableWithToCellMode.GetGroupedByDistTo(forActor.Position))
            {
                Func<Vector3Int, bool> func2;
                if ((func2 = <> 9__1) == null)
                {
                    func2 = (<> 9__1 = (Vector3Int x) => forActor.CanUseOn(weapon, toShoot, new Vector3Int?(x), false, null));
                }
                Vector3Int vector3Int2;
                if (enumerable2.Where<Vector3Int>(func2).TryGetRandomElement<Vector3Int>(out vector3Int2))
                {
                    path = Get.ShortestPathsCache.GetPathTo(vector3Int2, PathFinder.Request.Mode.ToCell);
                    if (path == null)
                    {
                        Log.Error("ShortestPathsCache.AllReachableWithToCellMode returned an unreachable cell.", false);
                    }
                    return true;
                }
            }
            path = null;
            return false;
        }

        public static IUsable GetWeaponToUse(Actor actor, bool allowSpells = true)
        {
            AIUtility_Attack.<> c__DisplayClass9_0 CS$<> 8__locals1;
            CS$<> 8__locals1.actor = actor;
            if (allowSpells && !CS$<> 8__locals1.actor.IsPlayerParty)
			{
                foreach (Spell spell in CS$<> 8__locals1.actor.Spells.All)
				{
                    AIUtility_Attack.<> c__DisplayClass9_1 CS$<> 8__locals2;
                    CS$<> 8__locals2.spell = spell;
                    if (!CS$<> 8__locals2.spell.Spec.IsSupportSpellForAI && AIUtility_Attack.< GetWeaponToUse > g__FilterAllowsAnyPlayerParty | 9_0(ref CS$<> 8__locals1, ref CS$<> 8__locals2) && CS$<> 8__locals1.actor.CanUse(CS$<> 8__locals2.spell, null, true, null))
					{
                        return CS$<> 8__locals2.spell;
                    }
                }
            }
            if (CS$<> 8__locals1.actor.Inventory.EquippedWeapon == null)
			{
                foreach (NativeWeapon nativeWeapon in CS$<> 8__locals1.actor.NativeWeapons)
				{
                    if (CS$<> 8__locals1.actor.CanUse(nativeWeapon, null, true, null))
					{
                        return nativeWeapon;
                    }
                }
                return null;
            }
            if (CS$<> 8__locals1.actor.CanUse(CS$<> 8__locals1.actor.Inventory.EquippedWeapon, null, true, null))
			{
                return CS$<> 8__locals1.actor.Inventory.EquippedWeapon;
            }
            return null;
        }

        public static IntRange? GetPredictedDealtDamageRangeToPlayerForUI(Actor enemy)
        {
            IUsable usable = AIUtility_Attack.GetWeaponToUse(enemy, true);
            if (usable == null)
            {
                return null;
            }
            IntRange? intRange = UsableUtility.GetDealtDamageRangeForUI(enemy, usable, Get.NowControlledActor, null);
            if (intRange != null)
            {
                return intRange;
            }
            if (usable is Spell)
            {
                usable = AIUtility_Attack.GetWeaponToUse(enemy, false);
                if (usable == null)
                {
                    return null;
                }
                intRange = UsableUtility.GetDealtDamageRangeForUI(enemy, usable, Get.NowControlledActor, null);
                if (intRange != null)
                {
                    return intRange;
                }
            }
            return null;
        }

        [CompilerGenerated]
        internal static bool <GetWeaponToUse>g__FilterAllowsAnyPlayerParty|9_0(ref AIUtility_Attack.<>c__DisplayClass9_0 A_0, ref AIUtility_Attack.<>c__DisplayClass9_1 A_1)
		{
			foreach (Actor actor in Get.PlayerParty)
			{
				if (A_1.spell.UseFilter.Allows(actor, A_0.actor))
				{
					return true;
				}
			}
			return false;
		}

private const int MaxTargetSearchDist = 15;

private const int MaxDistToShootPos = 5;
	}
}