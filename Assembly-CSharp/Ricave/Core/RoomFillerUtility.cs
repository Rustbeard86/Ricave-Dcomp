using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ricave.Core
{
    public static class RoomFillerUtility
    {
        public static List<Entity> DoPatchOf(Room room, WorldGenMemory memory, EntitySpec entitySpec, int count, bool ensureAccessToInterestingEntities = true, bool allMustBeAdjacentToFirst = false, bool neverSpawnAtEntrance = false, Predicate<Vector3Int> extraValidator = null)
        {
            RoomFillerUtility.<> c__DisplayClass0_0 CS$<> 8__locals1 = new RoomFillerUtility.<> c__DisplayClass0_0();
            CS$<> 8__locals1.entitySpec = entitySpec;
            CS$<> 8__locals1.room = room;
            CS$<> 8__locals1.ensureAccessToInterestingEntities = ensureAccessToInterestingEntities;
            CS$<> 8__locals1.allMustBeAdjacentToFirst = allMustBeAdjacentToFirst;
            CS$<> 8__locals1.neverSpawnAtEntrance = neverSpawnAtEntrance;
            CS$<> 8__locals1.extraValidator = extraValidator;
            World world = Get.World;
            List<Entity> list = new List<Entity>();
            CS$<> 8__locals1.spawnedAt = new HashSet<Vector3Int>();
            CS$<> 8__locals1.firstPos = default(Vector3Int);
            int i = 0;
            Vector3Int vector3Int;
            while (i < count && CS$<> 8__locals1.room.FreeOnFloor.Where<Vector3Int>(delegate (Vector3Int x)
            {
            if (i != 0)
            {
                IEnumerable<Vector3Int> enumerable = x.AdjacentCellsXZ();
                Func<Vector3Int, bool> func;
                if ((func = CS$<> 8__locals1.<> 9__1) == null)
					{
                func = (CS$<> 8__locals1.<> 9__1 = (Vector3Int y) => CS$<> 8__locals1.spawnedAt.Contains(y));
            }
            if (!enumerable.Any<Vector3Int>(func))
            {
                return false;
            }
        }
				if (((CS$<>8__locals1.entitySpec.CanPassThrough && (!CS$<>8__locals1.entitySpec.IsStructure || !CS$<>8__locals1.entitySpec.Structure.AIAvoids)) || !CS$<>8__locals1.room.WouldPotentiallyBlockPath(x, CS$<>8__locals1.ensureAccessToInterestingEntities)) && (!CS$<>8__locals1.allMustBeAdjacentToFirst || i == 0 || x.IsAdjacentXZ(CS$<>8__locals1.firstPos)) && (!CS$<>8__locals1.neverSpawnAtEntrance || !CS$<>8__locals1.room.IsEntranceCellToAvoidBlocking(x, true)))
				{
					return CS$<>8__locals1.extraValidator == null || CS$<>8__locals1.extraValidator(x);
				}
				return false;
			}).TryGetRandomElement<Vector3Int>(out vector3Int))
			{
    Entity entity;
    if (CS$<> 8__locals1.entitySpec == Get.Entity_Crates && !memory.generatedBigCrate && Rand.Chance(0.3f))
				{
        entity = Maker.Make(Get.Entity_BigCrate, null, false, false, true);
        memory.generatedBigCrate = true;
    }

                else if (CS$<> 8__locals1.entitySpec == Get.Entity_Crates && !memory.generatedBarrel && Rand.Chance(0.3f))
				{
        entity = Maker.Make(Get.Entity_Barrel, null, false, false, true);
        memory.generatedBarrel = true;
    }

                else
    {
        entity = Maker.Make(CS$<> 8__locals1.entitySpec, null, false, false, true);
    }
    entity.Spawn(vector3Int);
    list.Add(entity);
    CS$<> 8__locals1.spawnedAt.Add(vector3Int);
    if (i == 0)
    {
        CS$<> 8__locals1.firstPos = vector3Int;
    }
    int j = i;
    i = j + 1;
}
return list;
		}
	}
}