using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ricave.Core
{
    public static class AIUtility_Move
    {
        public static Action TryMoveInRoughDirAvoidingAdjacentHostiles(Actor actor, Vector3Int dir)
        {
            if (!dir.IsDirection())
            {
                Log.Error("Passed in direction is not a valid direction.", false);
                dir = dir.NormalizedToDir();
            }
            Func<Vector3Int, bool> <> 9__1;
            foreach (IEnumerable<Vector3Int> enumerable in dir.GetSimilarDirectionsGrouped())
            {
                Func<Vector3Int, bool> func;
                if ((func = <> 9__1) == null)
                {
                    func = (<> 9__1 = (Vector3Int x) => base.< TryMoveInRoughDirAvoidingAdjacentHostiles > g__ShouldMoveInDir | 0(x, true));
                }
                Vector3Int vector3Int;
                if (enumerable.Where<Vector3Int>(func).TryGetRandomElement<Vector3Int>(out vector3Int))
                {
                    return new Action_MoveSelf(Get.Action_MoveSelf, actor, actor.Position, actor.Position + vector3Int, null, false);
                }
            }
            Func<Vector3Int, bool> <> 9__2;
            foreach (IEnumerable<Vector3Int> enumerable2 in dir.GetSimilarDirectionsGrouped())
            {
                Func<Vector3Int, bool> func2;
                if ((func2 = <> 9__2) == null)
                {
                    func2 = (<> 9__2 = (Vector3Int x) => base.< TryMoveInRoughDirAvoidingAdjacentHostiles > g__ShouldMoveInDir | 0(x, false));
                }
                Vector3Int vector3Int2;
                if (enumerable2.Where<Vector3Int>(func2).TryGetRandomElement<Vector3Int>(out vector3Int2))
                {
                    return new Action_MoveSelf(Get.Action_MoveSelf, actor, actor.Position, actor.Position + vector3Int2, null, false);
                }
            }
            return null;
        }
    }
}