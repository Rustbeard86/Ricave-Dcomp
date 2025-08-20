using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ricave.Core
{
    public class AINode_Wander : AINode
    {
        public override IEnumerable<Instruction> MakeThinkInstructions(Actor actor)
        {
            if (actor.AIMemory.WanderDest == null || actor.AIMemory.WanderDest.Value == actor.Position || Get.PathFinder.FindPath(actor.Position, actor.AIMemory.WanderDest.Value, new PathFinder.Request(PathFinder.Request.Mode.ToCell, actor), 14, null) == null)
            {
                yield return new Instruction_SetWanderDest(actor, this.TryGetNewWanderDest(actor));
            }
            yield break;
        }

        public override Action GetNextAction(Actor actor)
        {
            if (actor.AIMemory.WanderDest == null || actor.AIMemory.WanderDest.Value == actor.Position)
            {
                return null;
            }
            List<Vector3Int> list = Get.PathFinder.FindPath(actor.Position, actor.AIMemory.WanderDest.Value, new PathFinder.Request(PathFinder.Request.Mode.ToCell, actor), 14, null);
            if (list == null)
            {
                return null;
            }
            return new Action_MoveSelf(Get.Action_MoveSelf, actor, actor.Position, list[list.Count - 2], null, false);
        }

        private Vector3Int? TryGetNewWanderDest(Actor actor)
        {
            Get.ShortestPathsCache.InitFor(actor.Position, actor, 13);
            float maxCostFound = 0f;
            List<Vector3Int> allReachableWithToCellMode = Get.ShortestPathsCache.AllReachableWithToCellMode;
            for (int i = 0; i < allReachableWithToCellMode.Count; i++)
            {
                float costTo = Get.ShortestPathsCache.GetCostTo(allReachableWithToCellMode[i], PathFinder.Request.Mode.ToCell);
                if (costTo > maxCostFound && !AINode_Wander.DestinationHasKnownDanger(allReachableWithToCellMode[i], actor))
                {
                    maxCostFound = costTo;
                }
            }
            if (maxCostFound <= 0f)
            {
                return null;
            }
            Vector3Int vector3Int;
            if (Get.ShortestPathsCache.AllReachableWithToCellMode.Where<Vector3Int>((Vector3Int x) => Get.ShortestPathsCache.GetCostTo(x, PathFinder.Request.Mode.ToCell) == maxCostFound && !AINode_Wander.DestinationHasKnownDanger(x, actor)).TryGetRandomElement<Vector3Int>(out vector3Int))
            {
                return new Vector3Int?(vector3Int);
            }
            return null;
        }

        private static bool DestinationHasKnownDanger(Vector3Int pos, Actor forActor)
        {
            return !forActor.AllowPathingIntoAIAvoids && Get.CellsInfo.AnyAIAvoidsAt(pos);
        }

        private const int MaxWanderDestinationDist = 13;
    }
}