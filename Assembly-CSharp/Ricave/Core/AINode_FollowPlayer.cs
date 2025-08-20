using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ricave.Core
{
    public class AINode_FollowPlayer : AINode
    {
        private Actor PlayerToFollow
        {
            get
            {
                if (!Get.NowControlledActor.IsPlayerParty)
                {
                    return Get.MainActor;
                }
                return Get.NowControlledActor;
            }
        }

        public override IEnumerable<Instruction> MakeThinkInstructions(Actor actor)
        {
            return Enumerable.Empty<Instruction>();
        }

        public override Action GetNextAction(Actor actor)
        {
            Actor playerToFollow = this.PlayerToFollow;
            if (playerToFollow == null || !playerToFollow.Spawned || actor == playerToFollow)
            {
                return null;
            }
            if (actor.Position.GetGridDistance(playerToFollow.Position) <= 1)
            {
                foreach (Vector3Int vector3Int in actor.Position.AdjacentCells().InRandomOrder<Vector3Int>())
                {
                    if (vector3Int.InBounds() && !vector3Int.IsAdjacentOrInside(playerToFollow.Position) && Get.World.CanMoveFromTo(actor.Position, vector3Int, actor))
                    {
                        return new Action_MoveSelf(Get.Action_MoveSelf, actor, actor.Position, vector3Int, null, false);
                    }
                }
                foreach (Vector3Int vector3Int2 in actor.Position.AdjacentCells().InRandomOrder<Vector3Int>())
                {
                    if (vector3Int2.InBounds() && Get.World.CanMoveFromTo(actor.Position, vector3Int2, actor))
                    {
                        return new Action_MoveSelf(Get.Action_MoveSelf, actor, actor.Position, vector3Int2, null, false);
                    }
                }
                return null;
            }
            if (this.PossiblyInBlockingCluster(actor))
            {
                foreach (Vector3Int vector3Int3 in actor.Position.AdjacentCells().InRandomOrder<Vector3Int>())
                {
                    if (vector3Int3.InBounds() && !vector3Int3.IsAdjacentOrInside(playerToFollow.Position) && Get.World.CanMoveFromTo(actor.Position, vector3Int3, actor))
                    {
                        return new Action_MoveSelf(Get.Action_MoveSelf, actor, actor.Position, vector3Int3, null, false);
                    }
                }
                return null;
            }
            if ((AINode_WasHarmedRecently.WasHarmedRecently(actor, 1) || AINode_WasHarmedRecently.WasHarmedRecently(playerToFollow, 1)) && actor.Position.GetGridDistance(playerToFollow.Position) == 2)
            {
                bool flag = actor.Sees(playerToFollow, null);
                foreach (Vector3Int vector3Int4 in actor.Position.AdjacentCells().InRandomOrder<Vector3Int>())
                {
                    if (vector3Int4.InBounds() && !vector3Int4.IsAdjacentOrInside(playerToFollow.Position))
                    {
                        if (AINode_WasHarmedRecently.WasHarmedRecently(actor, 1))
                        {
                            if (vector3Int4.GetGridDistance(playerToFollow.Position) != 2 && vector3Int4.GetGridDistance(playerToFollow.Position) != 3)
                            {
                                continue;
                            }
                        }
                        else if (vector3Int4.GetGridDistance(playerToFollow.Position) != 2)
                        {
                            continue;
                        }
                        if (Get.World.CanMoveFromTo(actor.Position, vector3Int4, actor) && (!flag || actor.Sees(playerToFollow, new Vector3Int?(vector3Int4))) && (!AINode_WasHarmedRecently.WasHarmedRecently(actor, 1) || !AIUtility.AnyAdjacentHostileActorCanTouchMeAt(vector3Int4, actor)))
                        {
                            return new Action_MoveSelf(Get.Action_MoveSelf, actor, actor.Position, vector3Int4, null, false);
                        }
                    }
                }
                return null;
            }
            if (actor.Position.GetGridDistance(playerToFollow.Position) == 2 && actor.Sees(playerToFollow, null))
            {
                return null;
            }
            List<Vector3Int> list = Get.PathFinder.FindPath(actor.Position, playerToFollow.Position, new PathFinder.Request(PathFinder.Request.Mode.Touch, actor), 15, null);
            if (list != null)
            {
                Vector3Int vector3Int5 = list[list.Count - 2];
                return new Action_MoveSelf(Get.Action_MoveSelf, actor, actor.Position, vector3Int5, null, false);
            }
            return null;
        }

        private bool PossiblyInBlockingCluster(Actor actor)
        {
            Actor playerToFollow = this.PlayerToFollow;
            if (actor.Position.GetGridDistance(playerToFollow.Position) > 3)
            {
                return false;
            }
            bool flag = false;
            foreach (Vector3Int vector3Int in playerToFollow.Position.AdjacentCells())
            {
                if (vector3Int.InBounds())
                {
                    foreach (Entity entity in Get.World.GetEntitiesAt(vector3Int))
                    {
                        Actor actor2 = entity as Actor;
                        if (actor2 != null && actor2 != actor && (actor2.AI == Get.AI_PlayerFollower || actor2.AI == Get.AI_PartyMember || actor2.AI == Get.AI_PartyMember_Stay))
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (flag)
                    {
                        break;
                    }
                }
            }
            if (!flag)
            {
                return false;
            }
            bool flag2 = false;
            foreach (Vector3Int vector3Int2 in actor.Position.AdjacentCells())
            {
                if (vector3Int2.InBounds())
                {
                    foreach (Entity entity2 in Get.World.GetEntitiesAt(vector3Int2))
                    {
                        Actor actor3 = entity2 as Actor;
                        if (actor3 != null && actor3 != playerToFollow && (actor3.AI == Get.AI_PlayerFollower || actor3.AI == Get.AI_PartyMember || actor3.AI == Get.AI_PartyMember_Stay))
                        {
                            flag2 = true;
                            break;
                        }
                    }
                    if (flag2)
                    {
                        break;
                    }
                }
            }
            return flag2;
        }

        private const int MaxPlayerSearchDist = 15;
    }
}