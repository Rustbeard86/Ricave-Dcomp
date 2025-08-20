using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Ricave.Core
{
    public static class SpawnPositionFinder
    {
        public static bool CanSpawnAt(EntitySpec entity, Vector3Int at, Entity knownInstance = null, bool allowPlayerPosForItem = false, Vector3Int? dir = null)
        {
            if (!at.InBounds())
            {
                return false;
            }
            if (entity.IsEthereal)
            {
                return true;
            }
            if (Get.CellsInfo.AnyDoorAt(at))
            {
                return false;
            }
            Vector3Int? vector3Int = dir;
            if (!ItemOrStructureFallUtility.WouldHaveSupport(entity, at, (vector3Int != null) ? vector3Int : ((knownInstance != null) ? new Vector3Int?(SpawnPositionFinder.GetExpectedDir(knownInstance)) : null)))
            {
                return false;
            }
            if (entity.IsActor)
            {
                if (knownInstance != null)
                {
                    if (Get.CellsInfo.IsFallingAt(at, (Actor)knownInstance, false))
                    {
                        return false;
                    }
                }
                else if (Get.CellsInfo.IsFallingAt(at, Vector3IntUtility.Down, SpawnPositionFinder.CanFlyByDefault(entity), false, false))
                {
                    return false;
                }
            }
            if (entity.IsItem)
            {
                return Get.CellsInfo.CanPassThroughNoActors(at) && !Get.CellsInfo.AnyItemAt(at) && (allowPlayerPosForItem || !Get.NowControlledActor.Spawned || at != Get.NowControlledActor.Position);
            }
            if (entity.IsActor)
            {
                if (Get.CellsInfo.CanPassThrough(at) && !Get.CellsInfo.AnyAIAvoidsAt(at))
                {
                    return !Get.World.GetEntitiesAt(at).Any<Entity>(delegate (Entity x)
                    {
                        Structure structure = x as Structure;
                        return structure != null && structure.Spec.Structure.AutoUseOnActorsPassing && structure.UseEffects.AnyOfSpec(Get.UseEffect_TeleportRandomly);
                    });
                }
                return false;
            }
            else
            {
                if (!entity.IsStructure)
                {
                    return true;
                }
                if (Get.World.AnyEntityOfSpecAt(at, entity))
                {
                    return false;
                }
                if (entity.Structure.IsCollidingFloorEmplacement && Get.World.AnyEntityAt(at))
                {
                    if (Get.World.GetEntitiesAt(at).Any<Entity>((Entity x) => x is Structure && x.Spec.Structure.IsCollidingFloorEmplacement))
                    {
                        return false;
                    }
                }
                if (entity.Structure.IsCollidingCeilingEmplacement && Get.World.AnyEntityAt(at))
                {
                    if (Get.World.GetEntitiesAt(at).Any<Entity>((Entity x) => x is Structure && x.Spec.Structure.IsCollidingCeilingEmplacement))
                    {
                        return false;
                    }
                }
                if (entity.Structure.Extinguishable && Get.CellsInfo.AnyWaterAt(at))
                {
                    return false;
                }
                if (!entity.CanPassThrough)
                {
                    return (entity.HasLifespanComp || !Get.CellsInfo.AnyItemAt(at)) && Get.CellsInfo.CanPassThrough(at) && !Get.CellsInfo.AnyActorAt(at);
                }
                return Get.CellsInfo.CanPassThroughNoActors(at);
            }
        }

        public static Vector3Int Near(Vector3Int near, Entity forEntity, bool allowPlayerPosForItem = false, bool allowPlayerPosForItemIfDropedDirectlyOntoPlayer = false, Predicate<Vector3Int> extraValidator = null)
        {
            SpawnPositionFinder.<> c__DisplayClass1_0 CS$<> 8__locals1;
            CS$<> 8__locals1.forEntity = forEntity;
            CS$<> 8__locals1.extraValidator = extraValidator;
            CS$<> 8__locals1.allowPlayerPosForItem = allowPlayerPosForItem;
            CS$<> 8__locals1.allowPlayerPosForItemIfDropedDirectlyOntoPlayer = allowPlayerPosForItemIfDropedDirectlyOntoPlayer;
            CS$<> 8__locals1.root = near;
            if (ItemOrStructureFallUtility.WouldHaveSupport(CS$<> 8__locals1.forEntity.Spec, near, new Vector3Int?(SpawnPositionFinder.GetExpectedDir(CS$<> 8__locals1.forEntity))))
            {
                Actor actor = CS$<> 8__locals1.forEntity as Actor;
                if (actor == null || !Get.CellsInfo.IsFallingAt(near, actor, false))
                {
                    goto IL_00C4;
                }
            }
            Actor actor2 = CS$<> 8__locals1.forEntity as Actor;
            Vector3Int vector3Int = ((actor2 != null) ? actor2.Gravity : Vector3IntUtility.Down);
            while ((CS$<> 8__locals1.root + vector3Int).InBounds() && SpawnPositionFinder.< Near > g__CanPassThroughCheck | 1_1(CS$<> 8__locals1.root + vector3Int, ref CS$<> 8__locals1))
			{
                CS$<> 8__locals1.root += vector3Int;
            }
        IL_00C4:
            if (SpawnPositionFinder.< Near > g__IsGood | 1_0(CS$<> 8__locals1.root, false, ref CS$<> 8__locals1))
            {
                return CS$<> 8__locals1.root;
            }
            foreach (Vector3Int vector3Int2 in CS$<> 8__locals1.root.AdjacentCellsXZ().InRandomOrder<Vector3Int>())
			{
                if (SpawnPositionFinder.< Near > g__IsGood | 1_0(vector3Int2, false, ref CS$<> 8__locals1))
                {
                    return vector3Int2;
                }
            }
            foreach (Vector3Int vector3Int3 in CS$<> 8__locals1.root.AdjacentCells().InRandomOrder<Vector3Int>())
			{
                if (SpawnPositionFinder.< Near > g__IsGood | 1_0(vector3Int3, false, ref CS$<> 8__locals1))
                {
                    return vector3Int3;
                }
            }
            if (SpawnPositionFinder.< Near > g__IsGood | 1_0(CS$<> 8__locals1.root, true, ref CS$<> 8__locals1))
            {
                return CS$<> 8__locals1.root;
            }
            for (int i = 2; i <= 3; i++)
            {
                foreach (Vector3Int vector3Int4 in CS$<> 8__locals1.root.GetCellsAtDist(i - 1).InRandomOrder<Vector3Int>())
				{
                if (SpawnPositionFinder.< Near > g__IsGood | 1_0(vector3Int4, true, ref CS$<> 8__locals1))
                {
                    return vector3Int4;
                }
            }
            foreach (Vector3Int vector3Int5 in CS$<> 8__locals1.root.GetCellsAtDist(i).InRandomOrder<Vector3Int>())
				{
                if (SpawnPositionFinder.< Near > g__IsGood | 1_0(vector3Int5, false, ref CS$<> 8__locals1))
                {
                    return vector3Int5;
                }
            }
        }
        CS$<>8__locals1.extraValidator = null;
			for (int j = 1; j <= 6; j++)
			{
				if (j > 1)
				{
					foreach (Vector3Int vector3Int6 in CS$<>8__locals1.root.GetCellsAtDist(j - 1).InRandomOrder<Vector3Int>())
					{
						if (SpawnPositionFinder.<Near>g__IsGood|1_0(vector3Int6, true, ref CS$<>8__locals1))
						{
							return vector3Int6;
						}
}
				}
				foreach (Vector3Int vector3Int7 in CS$<> 8__locals1.root.GetCellsAtDist(j).InRandomOrder<Vector3Int>())
				{
    if (SpawnPositionFinder.< Near > g__IsGood | 1_0(vector3Int7, false, ref CS$<> 8__locals1))
    {
        return vector3Int7;
    }
}
			}
			return SpawnPositionFinder.Anywhere(CS$<> 8__locals1.forEntity);
		}

		private static bool CanFlyByDefault(EntitySpec entity)
{
    return entity.IsActor && entity.Actor.CanFlyByDefault;
}

private static Vector3Int GetExpectedDir(Entity entity)
{
    Structure structure = entity as Structure;
    if (structure == null || structure.Spawned)
    {
        return entity.DirectionCardinal;
    }
    Quaternion? autoRotateRotation = structure.GetAutoRotateRotation();
    if (autoRotateRotation != null)
    {
        return autoRotateRotation.Value.ToCardinalDir();
    }
    return entity.DirectionCardinal;
}

public static Vector3Int GetExpectedDir(EntitySpec spec, Vector3Int at)
{
    if (!spec.IsStructure)
    {
        return Vector3IntUtility.Forward;
    }
    Quaternion? entitySpecAutoRotateRotation = Structure.GetEntitySpecAutoRotateRotation(spec, at);
    if (entitySpecAutoRotateRotation != null)
    {
        return entitySpecAutoRotateRotation.Value.ToCardinalDir();
    }
    return Vector3IntUtility.Forward;
}

public static Vector3Int Anywhere(Entity forEntity)
{
    Vector3Int vector3Int;
    if (Get.RetainedRoomInfo.Rooms.SelectMany<RetainedRoomInfo.RoomInfo, Vector3Int>((RetainedRoomInfo.RoomInfo x) => x.Shape).TryGetRandomElementWhere<Vector3Int>((Vector3Int x) => SpawnPositionFinder.CanSpawnAt(forEntity.Spec, x, forEntity, false, null), out vector3Int))
    {
        return vector3Int;
    }
    Vector3Int vector3Int2;
    if (Get.RetainedRoomInfo.Rooms.SelectMany<RetainedRoomInfo.RoomInfo, Vector3Int>((RetainedRoomInfo.RoomInfo x) => x.Shape).TryGetRandomElementWhere<Vector3Int>((Vector3Int x) => !Get.World.AnyEntityAt(x), out vector3Int2))
    {
        return vector3Int2;
    }
    Vector3Int vector3Int3;
    if (Get.World.Bounds.TryGetRandomElementWhere<Vector3Int>((Vector3Int x) => !Get.World.AnyEntityAt(x), out vector3Int3))
    {
        return vector3Int3;
    }
    return Vector3Int.zero;
}

public static bool FromFloorBars(out Vector3Int pos)
{
    IEnumerable<Entity> enumerable = from x in Get.World.GetEntitiesOfSpec(Get.Entity_FloorBars).Concat<Entity>(Get.World.GetEntitiesOfSpec(Get.Entity_FloorBarsReinforced))
                                     where x.Position.Above().InBounds() && !Get.World.AnyEntityAt(x.Position.Above())
                                     select x;
    if (!enumerable.Any<Entity>())
    {
        pos = default(Vector3Int);
        return false;
    }
    bool playerOnGround = Get.CellsInfo.IsFloorUnderNoActors(Get.MainActor.Position);
    if (playerOnGround)
    {
        Get.BFSCache.InitFor(Get.MainActor.Position, new World.CanMoveFromToParams(Vector3IntUtility.Down, false, false, true, true, false, null), 26);
    }
    int tries;
    int tries2;
    for (tries = 0; tries < 3; tries = tries2 + 1)
    {
        int distThreshold = ((tries == 0) ? 10 : 5);
        Entity entity;
        if (enumerable.Where<Entity>((Entity x) => SpawnPositionFinder.< FromFloorBars > g__ClosestDistToPlayerPartyFrom | 6_1(x.Position.Above()) > distThreshold && (!playerOnGround || tries == 2 || Get.BFSCache.CanReach(x.Position.Above(), PathFinder.Request.Mode.ToCell))).TryGetRandomElement<Entity>(out entity))
        {
            pos = entity.Position.Above();
            return true;
        }
        tries2 = tries;
    }
    Entity entity2;
    if (enumerable.TryGetRandomElement<Entity>(out entity2))
    {
        pos = entity2.Position.Above();
        return true;
    }
    pos = default(Vector3Int);
    return false;
}

public static bool OutOfPlayerSight(EntitySpec forEntitySpec, out Vector3Int pos, bool mustBeFree = false, bool mustBeOnFloor = false, bool mustBeFilledBelow = false, bool allowInOptionalChallengeOrSecretRooms = true)
{
    IEnumerable<RetainedRoomInfo.RoomInfo> enumerable = Get.RetainedRoomInfo.Rooms;
    if (!allowInOptionalChallengeOrSecretRooms)
    {
        enumerable = enumerable.Where<RetainedRoomInfo.RoomInfo>((RetainedRoomInfo.RoomInfo x) => x.Role != Room.LayoutRole.OptionalChallenge && x.Role != Room.LayoutRole.Secret && x.Role != Room.LayoutRole.LockedBehindSilverDoor);
    }
    if (enumerable.SelectMany<RetainedRoomInfo.RoomInfo, Vector3Int>((RetainedRoomInfo.RoomInfo x) => x.Shape).TryGetRandomElementWhere<Vector3Int>(delegate (Vector3Int x)
    {
        if (mustBeFree && Get.World.AnyEntityAt(x))
        {
            return false;
        }
        if (mustBeOnFloor && !Get.CellsInfo.IsFloorUnderNoActors(x))
        {
            return false;
        }
        if (mustBeFilledBelow && (!x.Below().InBounds() || !Get.CellsInfo.IsFilled(x.Below())))
        {
            return false;
        }
        if (!SpawnPositionFinder.CanSpawnAt(forEntitySpec, x, null, false, null))
        {
            return false;
        }
        using (List<Actor>.Enumerator enumerator = Get.PlayerParty.GetEnumerator())
        {
            while (enumerator.MoveNext())
            {
                if (enumerator.Current.Sees(x, null))
                {
                    return false;
                }
            }
        }
        return true;
    }, out pos))
    {
        return true;
    }
    pos = default(Vector3Int);
    return false;
}

public static bool UnburrowPos(Actor forActor, Vector3Int burrowedAt, out Vector3Int pos)
{
    Func<Vector3Int, bool> <> 9__3;
    for (int i = 0; i < 2; i++)
    {
        int num = ((i == 0) ? 2 : 4);
        IEnumerable<Vector3Int> enumerable = from x in burrowedAt.GetCellsWithin(num).ClipToWorld()
                                             where x.InRoomsBounds() && Get.CellsInfo.CanPassThrough(x) && x.Below().InBounds() && Get.CellsInfo.AnyFilledImpassableAt(x.Below()) && !Get.CellsInfo.AnyAIAvoidsAt(x) && !Get.CellsInfo.AnyWaterAt(x) && !Get.CellsInfo.AnyDoorAt(x) && !Get.CellsInfo.AnyStairsAt(x)
                                             select x;
        IEnumerable<Vector3Int> enumerable2 = enumerable;
        Func<Vector3Int, bool> func;
        if ((func = <> 9__3) == null)
        {
            func = (<> 9__3 = (Vector3Int x) => !SpawnPositionFinder.< UnburrowPos > g__AnyAdjacentPlayerParty | 8_1(x) && base.< UnburrowPos > g__AnyHostileNearbyWithLineOfFire | 2(x));
        }
        if (enumerable2.Where<Vector3Int>(func).TryGetRandomElement<Vector3Int>(out pos))
        {
            return true;
        }
        if (enumerable.TryGetRandomElement<Vector3Int>(out pos))
        {
            return true;
        }
    }
    pos = burrowedAt;
    return false;
}

[CompilerGenerated]
internal static bool < Near > g__CanPassThroughCheck | 1_1(Vector3Int at, ref SpawnPositionFinder.<> c__DisplayClass1_0 A_1)

        {
    if (A_1.forEntity is Item)
    {
        return Get.CellsInfo.CanPassThroughNoActors(at);
    }
    return Get.CellsInfo.CanPassThrough(at);
}

[CompilerGenerated]
internal static bool < Near > g__IsGood | 1_0(Vector3Int pos, bool desperate, ref SpawnPositionFinder.<> c__DisplayClass1_0 A_2)

        {
    if (!pos.InBounds())
    {
        return false;
    }
    if (pos != A_2.root && A_2.root.InRoomsBounds() && !pos.InRoomsBounds())
    {
        return false;
    }
    if (!desperate && !LineOfSight.IsLineOfFire(A_2.root, pos))
    {
        return false;
    }
    if (!desperate && A_2.forEntity is Item && Get.CellsInfo.AnyAIAvoidsAt(pos))
    {
        return false;
    }
    if (A_2.extraValidator != null && !A_2.extraValidator(pos))
    {
        return false;
    }
    if (!desperate)
    {
        Actor actor = A_2.forEntity as Actor;
        if (actor != null && !actor.CanFly && pos.Below().InBounds() && Get.CellsInfo.AnyActorAt(pos.Below()) && Get.CellsInfo.CanPassThroughNoActors(pos.Below()))
        {
            return false;
        }
    }
    return SpawnPositionFinder.CanSpawnAt(A_2.forEntity.Spec, pos, A_2.forEntity, A_2.allowPlayerPosForItem || (A_2.allowPlayerPosForItemIfDropedDirectlyOntoPlayer && A_2.root == pos), null);
}

[CompilerGenerated]
internal static int < FromFloorBars > g__ClosestDistToPlayerPartyFrom | 6_1(Vector3Int from)

        {
    int num = from.GetGridDistance(Get.MainActor.Position);
    foreach (Actor actor in Get.PlayerParty)
    {
        int gridDistance = from.GetGridDistance(actor.Position);
        if (gridDistance < num)
        {
            num = gridDistance;
        }
    }
    return num;
}

[CompilerGenerated]
internal static bool < UnburrowPos > g__AnyAdjacentPlayerParty | 8_1(Vector3Int at)

        {
    using (List<Actor>.Enumerator enumerator = Get.PlayerParty.GetEnumerator())
    {
        while (enumerator.MoveNext())
        {
            if (enumerator.Current.Position.IsAdjacent(at))
            {
                return true;
            }
        }
    }
    return false;
}
	}
}