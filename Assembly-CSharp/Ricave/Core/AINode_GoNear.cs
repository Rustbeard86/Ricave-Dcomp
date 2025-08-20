using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ricave.Core
{
    public class AINode_GoNear : AINode
    {
        public override IEnumerable<Instruction> MakeThinkInstructions(Actor actor)
        {
            return Enumerable.Empty<Instruction>();
        }

        public override Action GetNextAction(Actor actor)
        {
            if (!Get.World.AnyEntityOfSpec(this.entitySpec))
            {
                return null;
            }
            if (this.IsGoodCell(actor.Position, false))
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
            Get.ShortestPathsCache.InitFor(actor.Position, actor, 15);
            foreach (Vector3Int vector3Int in Get.ShortestPathsCache.AllReachableWithToCellModeOrderedByDistance)
            {
                if (this.IsGoodCell(vector3Int, false))
                {
                    pos = vector3Int;
                    return true;
                }
            }
            foreach (Vector3Int vector3Int2 in Get.ShortestPathsCache.AllReachableWithToCellModeOrderedByDistance)
            {
                if (this.IsGoodCell(vector3Int2, true))
                {
                    pos = vector3Int2;
                    return true;
                }
            }
            pos = actor.Position;
            return false;
        }

        private bool IsGoodCell(Vector3Int pos, bool desperate)
        {
            if (Get.CellsInfo.AnyAIAvoidsAt(pos))
            {
                return false;
            }
            bool flag = false;
            List<Entity> entitiesOfSpec = Get.World.GetEntitiesOfSpec(this.entitySpec);
            if (entitiesOfSpec.Count < 26)
            {
                for (int i = 0; i < entitiesOfSpec.Count; i++)
                {
                    if (entitiesOfSpec[i].Position.IsAdjacent(pos) && (LineOfSight.IsLineOfSight(pos, entitiesOfSpec[i].Position) || LineOfSight.IsLineOfFire(pos, entitiesOfSpec[i].Position)))
                    {
                        flag = true;
                        break;
                    }
                }
            }
            else
            {
                for (int j = 0; j < Vector3IntUtility.Directions.Length; j++)
                {
                    Vector3Int vector3Int = pos + Vector3IntUtility.Directions[j];
                    if (vector3Int.InBounds() && Get.World.AnyEntityOfSpecAt(vector3Int, this.entitySpec) && (LineOfSight.IsLineOfSight(pos, vector3Int) || LineOfSight.IsLineOfFire(pos, vector3Int)))
                    {
                        flag = true;
                        break;
                    }
                }
            }
            return flag && (desperate || !AINode_GoToNonBlockingCell.IsBlocking(pos));
        }

        [Saved]
        private EntitySpec entitySpec;

        private const int MaxSearchDist = 15;
    }
}