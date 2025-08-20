using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ricave.Core
{
    public class AINode_GoToNonBlockingCell : AINode
    {
        public override IEnumerable<Instruction> MakeThinkInstructions(Actor actor)
        {
            return Enumerable.Empty<Instruction>();
        }

        public override Action GetNextAction(Actor actor)
        {
            if (!AINode_GoToNonBlockingCell.IsBlocking(actor.Position))
            {
                return null;
            }
            Vector3Int vector3Int;
            if (!this.TryFindGotoPos(actor, out vector3Int) || vector3Int == actor.Position)
            {
                return null;
            }
            List<Vector3Int> pathTo = Get.ShortestPathsCache.GetPathTo(vector3Int, PathFinder.Request.Mode.ToCell);
            if (pathTo.NullOrEmpty<Vector3Int>() || pathTo.Count == 1)
            {
                return null;
            }
            Vector3Int vector3Int2 = pathTo[pathTo.Count - 2];
            return new Action_MoveSelf(Get.Action_MoveSelf, actor, actor.Position, vector3Int2, null, false);
        }

        private bool TryFindGotoPos(Actor actor, out Vector3Int pos)
        {
            Get.ShortestPathsCache.InitFor(actor.Position, actor, 6);
            foreach (Vector3Int vector3Int in Get.ShortestPathsCache.AllReachableWithToCellModeOrderedByDistance)
            {
                if (this.IsGoodDestination(vector3Int))
                {
                    pos = vector3Int;
                    return true;
                }
            }
            pos = actor.Position;
            return false;
        }

        private bool IsGoodDestination(Vector3Int pos)
        {
            return !Get.CellsInfo.AnyAIAvoidsAt(pos) && !Get.CellsInfo.AnyWaterAt(pos) && !AINode_GoToNonBlockingCell.IsBlocking(pos);
        }

        public static bool IsBlocking(Vector3Int pos)
        {
            if (Get.CellsInfo.AnyLadderAt(pos) || (pos.Below().InBounds() && Get.CellsInfo.AnyLadderAt(pos.Below())))
            {
                return true;
            }
            if (Get.CellsInfo.AnyAutoUseOnPassingActors(pos))
            {
                return true;
            }
            if (Get.CellsInfo.AnyStairsAt(pos))
            {
                return true;
            }
            if (Get.CellsInfo.AnyItemAt(pos))
            {
                return true;
            }
            Vector3Int? vector3Int = null;
            for (int i = 0; i < Vector3IntUtility.DirectionsXZ.Length; i++)
            {
                Vector3Int vector3Int2 = pos + Vector3IntUtility.DirectionsXZ[i];
                if ((!vector3Int2.InBounds() || Get.PlaceSpec != Get.Place_Shelter || !Get.World.AnyEntityOfSpecAt(vector3Int2, Get.Entity_AltarOfRedemption)) && vector3Int2.InBounds() && (!Get.CellsInfo.CanPassThroughNoActors(vector3Int2) || (!Get.CellsInfo.IsFloorUnderNoActors(vector3Int2) && (!vector3Int2.Below().InBounds() || (!Get.CellsInfo.AnyStairsAt(vector3Int2.Below()) && !Get.CellsInfo.AnyLadderAt(vector3Int2.Below())))) || Get.CellsInfo.AnyAIAvoidsAt(vector3Int2)))
                {
                    if (vector3Int == null)
                    {
                        vector3Int = new Vector3Int?(vector3Int2);
                    }
                    else if (!vector3Int2.IsAdjacentCardinalXZ(vector3Int.Value))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private const int MaxSearchDist = 6;
    }
}