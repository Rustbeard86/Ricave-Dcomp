using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Ricave.Core
{
    public static class AIUtility_Flee
    {
        public static Action TryGetFleeFromAllHostilesAction(Actor actor, bool canMoveToEquallyBadPositions)
        {
            List<Vector3Int> bestFleePath = AIUtility_Flee.GetBestFleePath(actor, canMoveToEquallyBadPositions);
            if (bestFleePath != null)
            {
                if (bestFleePath.Count == 1)
                {
                    return new Action_Wait(Get.Action_Wait, actor, null, null);
                }
                Vector3Int vector3Int = bestFleePath[bestFleePath.Count - 2];
                Action action = AIUtility_Move.TryMoveInRoughDirAvoidingAdjacentHostiles(actor, vector3Int - actor.Position);
                if (action != null)
                {
                    return action;
                }
            }
            return null;
        }

        private static List<Vector3Int> GetBestFleePath(Actor actor, bool canMoveToEquallyBadPositions)
        {
            AIUtility_Flee.<> c__DisplayClass1_0 CS$<> 8__locals1 = new AIUtility_Flee.<> c__DisplayClass1_0();
            CS$<> 8__locals1.actor = actor;
            Get.ShortestPathsCache.InitFor(CS$<> 8__locals1.actor.Position, CS$<> 8__locals1.actor, 12);
            float num = 0f;
            List<Vector3Int> allReachableWithToCellMode = Get.ShortestPathsCache.AllReachableWithToCellMode;
            for (int i = 0; i < allReachableWithToCellMode.Count; i++)
            {
                float costTo = Get.ShortestPathsCache.GetCostTo(allReachableWithToCellMode[i], PathFinder.Request.Mode.ToCell);
                num = Math.Max(num, costTo);
            }
            if (num <= 0f)
            {
                return null;
            }
            if (num >= 10f)
            {
                CS$<> 8__locals1.minCostToConsider = num - 2f;
            }
            else if (num >= 7f)
            {
                CS$<> 8__locals1.minCostToConsider = num - 1f;
            }
            else
            {
                CS$<> 8__locals1.minCostToConsider = num;
            }
            CS$<> 8__locals1.withMinX = null;
            CS$<> 8__locals1.withMaxX = null;
            CS$<> 8__locals1.withMinY = null;
            CS$<> 8__locals1.withMaxY = null;
            CS$<> 8__locals1.withMinZ = null;
            CS$<> 8__locals1.withMaxZ = null;
            IEnumerable<Vector3Int> enumerable = allReachableWithToCellMode;
            Func<Vector3Int, bool> func;
            if ((func = CS$<> 8__locals1.<> 9__4) == null)
			{
                func = (CS$<> 8__locals1.<> 9__4 = (Vector3Int x) => Get.ShortestPathsCache.GetCostTo(x, PathFinder.Request.Mode.ToCell) >= CS$<> 8__locals1.minCostToConsider);
            }
            foreach (Vector3Int vector3Int in enumerable.Where<Vector3Int>(func).InRandomOrder<Vector3Int>())
            {
                if (CS$<> 8__locals1.withMinX == null || vector3Int.x < CS$<> 8__locals1.withMinX.Value.x)
				{
                    CS$<> 8__locals1.withMinX = new Vector3Int?(vector3Int);
                }
                if (CS$<> 8__locals1.withMaxX == null || vector3Int.x > CS$<> 8__locals1.withMaxX.Value.x)
				{
                    CS$<> 8__locals1.withMaxX = new Vector3Int?(vector3Int);
                }
                if (CS$<> 8__locals1.withMinY == null || vector3Int.y < CS$<> 8__locals1.withMinY.Value.y)
				{
                    CS$<> 8__locals1.withMinY = new Vector3Int?(vector3Int);
                }
                if (CS$<> 8__locals1.withMaxY == null || vector3Int.y > CS$<> 8__locals1.withMaxY.Value.y)
				{
                    CS$<> 8__locals1.withMaxY = new Vector3Int?(vector3Int);
                }
                if (CS$<> 8__locals1.withMinZ == null || vector3Int.z < CS$<> 8__locals1.withMinZ.Value.z)
				{
                    CS$<> 8__locals1.withMinZ = new Vector3Int?(vector3Int);
                }
                if (CS$<> 8__locals1.withMaxZ == null || vector3Int.z > CS$<> 8__locals1.withMaxZ.Value.z)
				{
                    CS$<> 8__locals1.withMaxZ = new Vector3Int?(vector3Int);
                }
            }
            if (CS$<> 8__locals1.withMinX == null)
			{
                return null;
            }
            List<Actor> list = Get.World.Actors.Where<Actor>((Actor x) => x.Position.GetGridDistance(CS$<> 8__locals1.actor.Position) <= 24 && AIUtility_Flee.WantsToFleeFrom(CS$<> 8__locals1.actor, x)).ToTemporaryList<Actor>();
            if (list.Count == 0)
            {
                return null;
            }
            Get.BFSCache.InitFor(list, 12, true);
            CS$<> 8__locals1.bestDistToHostile = 0;
            foreach (Vector3Int vector3Int2 in CS$<> 8__locals1.< GetBestFleePath > g__GetResolvedCandidates | 1())
			{
                int num2 = AIUtility_Flee.< GetBestFleePath > g__GetDistToClosestHostileTouchFromPath | 1_2(vector3Int2);
                if (num2 > CS$<> 8__locals1.bestDistToHostile)
				{
                    CS$<> 8__locals1.bestDistToHostile = num2;
                    if (CS$<> 8__locals1.bestDistToHostile == 9999)
					{
                        break;
                    }
                }
            }
            int num3 = (Get.BFSCache.CanReach(CS$<> 8__locals1.actor.Position, PathFinder.Request.Mode.Touch) ? Get.BFSCache.GetDistTo(CS$<> 8__locals1.actor.Position, PathFinder.Request.Mode.Touch) : 9999);
            if (CS$<> 8__locals1.bestDistToHostile <= 1 && num3 <= 1 && CS$<> 8__locals1.bestDistToHostile <= num3)
			{
                Vector3Int? vector3Int3 = null;
                foreach (Vector3Int vector3Int4 in Vector3IntUtility.Directions.InRandomOrder<Vector3Int>())
                {
                    Vector3Int vector3Int5 = CS$<> 8__locals1.actor.Position + vector3Int4;
                    if (vector3Int5.InBounds() && Get.World.CanMoveFromTo(CS$<> 8__locals1.actor.Position, vector3Int5, CS$<> 8__locals1.actor))
                    {
                        int num4 = (Get.BFSCache.CanReach(vector3Int5, PathFinder.Request.Mode.Touch) ? Get.BFSCache.GetDistTo(vector3Int5, PathFinder.Request.Mode.Touch) : 9999);
                        if (num4 > num3)
                        {
                            return new List<Vector3Int>
                            {
                                vector3Int5,
                                CS$<>8__locals1.actor.Position
                            };
                        }
                        if (num4 == num3)
                        {
                            vector3Int3 = new Vector3Int?(vector3Int5);
                        }
                    }
                }
                if (AIUtility.AnyAdjacentHostileActorCanTouchMeAt(CS$<> 8__locals1.actor.Position, CS$<> 8__locals1.actor))
                {
                    return null;
                }
                if (vector3Int3 == null)
                {
                    return new List<Vector3Int> { CS$<> 8__locals1.actor.Position };
                }
                if (canMoveToEquallyBadPositions)
                {
                    return new List<Vector3Int>
                    {
                        vector3Int3.Value,
                        CS$<>8__locals1.actor.Position
                    };
                }
                return null;
            }
            Vector3Int vector3Int6;
            if ((from x in CS$<> 8__locals1.< GetBestFleePath > g__GetResolvedCandidates | 1()

                where AIUtility_Flee.< GetBestFleePath > g__GetDistToClosestHostileTouchFromPath | 1_2(x) == CS$<> 8__locals1.bestDistToHostile
                select x).TryGetRandomElement<Vector3Int>(out vector3Int6))
			{
                return Get.ShortestPathsCache.GetPathTo(vector3Int6, PathFinder.Request.Mode.ToCell);
            }
            return null;
        }

        private static bool WantsToFleeFrom(Actor actor, Actor from)
        {
            return from != actor && from.IsHostile(actor);
        }

        [CompilerGenerated]
        internal static int <GetBestFleePath>g__GetDistToClosestHostileTouchFromPath|1_2(Vector3Int pathDest)
		{
			int num = 9999;
        List<Vector3Int> pathTo = Get.ShortestPathsCache.GetPathTo(pathDest, PathFinder.Request.Mode.ToCell);
			for (int i = 0; i<pathTo.Count - 1; i++)
			{
				if (Get.BFSCache.CanReach(pathTo[i], PathFinder.Request.Mode.Touch))
				{
					int distTo = Get.BFSCache.GetDistTo(pathTo[i], PathFinder.Request.Mode.Touch);
					if (distTo<num)
					{
						num = distTo;
					}
}
			}
			return num;
		}
	}
}