using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Ricave.Core
{
    public class AINode_DestroyBlockers : AINode
    {
        public override IEnumerable<Instruction> MakeThinkInstructions(Actor actor)
        {
            Vector3Int? lastDestroyedBlockerAt = actor.AIMemory.LastDestroyedBlockerAt;
            if (lastDestroyedBlockerAt != null)
            {
                if (lastDestroyedBlockerAt.Value == actor.Position || lastDestroyedBlockerAt.Value.GetGridDistance(actor.Position) > 3)
                {
                    yield return new Instruction_SetLastDestroyedBlockerAt(actor, null);
                }
                else if (Get.PathFinder.FindPath(actor.Position, lastDestroyedBlockerAt.Value, new PathFinder.Request(PathFinder.Request.Mode.ToCell, actor), 5, null) == null)
                {
                    yield return new Instruction_SetLastDestroyedBlockerAt(actor, null);
                }
            }
            yield break;
        }

        public override Action GetNextAction(Actor actor)
        {
            Vector3Int? lastDestroyedBlockerAt = actor.AIMemory.LastDestroyedBlockerAt;
            if (lastDestroyedBlockerAt != null && lastDestroyedBlockerAt.Value != actor.Position)
            {
                List<Vector3Int> list = Get.PathFinder.FindPath(actor.Position, lastDestroyedBlockerAt.Value, new PathFinder.Request(PathFinder.Request.Mode.ToCell, actor), 5, null);
                if (list != null)
                {
                    Vector3Int vector3Int = list[list.Count - 2];
                    return new Action_MoveSelf(Get.Action_MoveSelf, actor, actor.Position, vector3Int, null, false);
                }
            }
            Structure blockerToDestroy = this.GetBlockerToDestroy(actor);
            if (blockerToDestroy == null)
            {
                return null;
            }
            IUsable blockerDestroyerWeaponToUse = AINode_DestroyBlockers.GetBlockerDestroyerWeaponToUse(actor);
            if (blockerDestroyerWeaponToUse == null)
            {
                return null;
            }
            if (actor.CanUseOn(blockerDestroyerWeaponToUse, blockerToDestroy, null, false, null))
            {
                return new Action_Use(Get.Action_Use, actor, blockerDestroyerWeaponToUse, blockerToDestroy, null);
            }
            if (Get.World.CanTouch(actor.Position, blockerToDestroy.Position, actor))
            {
                return null;
            }
            List<Vector3Int> list2 = Get.PathFinder.FindPath(actor.Position, blockerToDestroy.Position, new PathFinder.Request(PathFinder.Request.Mode.Touch, actor), 5, null);
            if (list2 != null)
            {
                Vector3Int vector3Int2 = list2[list2.Count - 2];
                return new Action_MoveSelf(Get.Action_MoveSelf, actor, actor.Position, vector3Int2, null, false);
            }
            return null;
        }

        private Structure GetBlockerToDestroy(Actor forActor)
        {
            Actor player = Get.NowControlledActor;
            if (player == null || !player.Spawned || forActor.Position.GetGridDistance(player.Position) > forActor.SeeRange + 2 || !forActor.IsHostile(player))
            {
                return null;
            }
            int? lastSeenPlayerSequence = forActor.AIMemory.LastSeenPlayerSequence;
            if (lastSeenPlayerSequence == null || lastSeenPlayerSequence.Value < forActor.Sequence - forActor.SequencePerTurn * 6)
            {
                return null;
            }
            IUsable weapon = AINode_DestroyBlockers.GetBlockerDestroyerWeaponToUse(forActor);
            if (weapon == null)
            {
                return null;
            }
            Get.BFSCache.InitFor(forActor.Position, forActor, Math.Min(forActor.SeeRange + 1, 5));
            Func<Vector3Int, bool> <> 9__2;
            Entity entity;
            if (Get.VisibilityCache.EntitiesSeen_Ordered.Where<Entity>(delegate (Entity x)
            {
                if (!(x is Structure) || x.Spec.CanPassThrough || !x.Spec.Structure.Fragile || !x.Spec.Structure.IsFilled || x.Position.GetGridDistance(player.Position) > 3 || forActor.Position.GetGridDistance(x.Position) > forActor.Position.GetGridDistance(player.Position) || (!GravityUtility.IsAltitudeEqual(x.Position, forActor.Position, forActor.Gravity) && !forActor.CanFly) || !forActor.Sees(x, null) || !forActor.CanUseOn(weapon, x, null, true, null) || (!forActor.CanUseOn(weapon, x, null, false, null) && !Get.BFSCache.CanReach(x.Position, PathFinder.Request.Mode.Touch)))
                {
                    return false;
                }
                if (!Get.World.CanTouch(x.Position, player.Position, forActor))
                {
                    IEnumerable<Vector3Int> enumerable = x.Position.AdjacentCardinalCells();
                    Func<Vector3Int, bool> func;
                    if ((func = <> 9__2) == null)
                    {
                        func = (<> 9__2 = (Vector3Int y) => Get.World.CanTouch(y, player.Position, forActor));
                    }
                    return enumerable.Any<Vector3Int>(func);
                }
                return true;
            }).TryGetMinBy<Entity, int>((Entity x) => x.Position.GetGridDistance(forActor.Position), out entity))
            {
                return (Structure)entity;
            }
            return null;
        }

        public static IUsable GetBlockerDestroyerWeaponToUse(Actor actor)
        {
            if (actor.Inventory.EquippedWeapon == null)
            {
                foreach (NativeWeapon nativeWeapon in actor.NativeWeapons)
                {
                    if (actor.CanUse(nativeWeapon, null, true, null) && AINode_DestroyBlockers.< GetBlockerDestroyerWeaponToUse > g__CanDestroyFragile | 4_0(nativeWeapon))
                    {
                        return nativeWeapon;
                    }
                }
                return null;
            }
            if (actor.CanUse(actor.Inventory.EquippedWeapon, null, true, null) && AINode_DestroyBlockers.< GetBlockerDestroyerWeaponToUse > g__CanDestroyFragile | 4_0(actor.Inventory.EquippedWeapon))
            {
                return actor.Inventory.EquippedWeapon;
            }
            return null;
        }

        [CompilerGenerated]
        internal static bool <GetBlockerDestroyerWeaponToUse>g__CanDestroyFragile|4_0(IUsable usable)
		{
			using (List<UseEffect>.Enumerator enumerator = usable.UseEffects.All.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current is UseEffect_DestroyFragile)
					{
						return true;
					}
				}
}
return false;
		}

		private const int MaxDistToBlocker = 5;
	}
}