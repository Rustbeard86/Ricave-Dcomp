using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ricave.Core
{
    public class RoomPart_Enemies : RoomPart
    {
        public override void Generate(Room room, WorldGenMemory memory)
        {
            World world = Get.World;
            int num = Rand.RangeInclusive(1, 2);
            if (Rand.Chance(0.05f))
            {
                num = 3;
            }
            if (GenPass_RoomLayout.HasUpperStorey(memory) && room.Storey.IsMainStorey)
            {
                num = Math.Min(num, memory.unusedBaseActors.Count - (int)(0.4f * (float)memory.baseActors.Count));
            }
            Vector3Int? vector3Int = null;
            List<Vector3Int> list = new List<Vector3Int>();
            foreach (Vector3Int vector3Int2 in room.Shape.InnerCuboid(1).BottomSurfaceCuboid)
            {
                if (world.GetEntitiesAt(vector3Int2).Any<Entity>(delegate (Entity x)
                {
                    if (!(x is Item))
                    {
                        Structure structure = x as Structure;
                        return structure != null && structure.Spec.Structure.IsSpecial && !structure.Spec.Structure.AutoUseOnDestroyed;
                    }
                    return true;
                }))
                {
                    list.Add(vector3Int2);
                }
                if (world.AnyEntityOfSpecAt(vector3Int2, Get.Entity_PressurePlate))
                {
                    vector3Int = new Vector3Int?(vector3Int2);
                }
            }
            int num2 = 0;
            Actor actor;
            while (num2 < num && memory.unusedBaseActors.TryGetRandomElement<Actor>(out actor))
            {
                Vector3Int vector3Int3;
                if (!list.TryGetRandomElement<Vector3Int>(out vector3Int3))
                {
                    goto IL_0141;
                }
                if (!this.ChooseBestCandidateAndSpawn(vector3Int3.AdjacentCellsXZ(), actor, room, memory, ref vector3Int))
                {
                    list.Remove(vector3Int3);
                    goto IL_0141;
                }
                list.Remove(vector3Int3);
            IL_016C:
                num2++;
                continue;
            IL_0141:
                this.ChooseBestCandidateAndSpawn(room.Shape.InnerCuboid(1).BottomSurfaceCuboid, actor, room, memory, ref vector3Int);
                goto IL_016C;
            }
        }

        private bool ChooseBestCandidateAndSpawn(IEnumerable<Vector3Int> candidates, Actor actor, Room room, WorldGenMemory memory, ref Vector3Int? pressurePlatePos)
        {
            World world = Get.World;
            candidates = candidates.Where<Vector3Int>((Vector3Int x) => !world.AnyEntityAt(x) && world.CellsInfo.IsFloorUnderNoActors(x) && !room.Shape.IsOnEdgeXZ(x));
            Func<Vector3Int, bool> <> 9__2;
            IEnumerable<Vector3Int> enumerable = candidates.Where<Vector3Int>(delegate (Vector3Int x)
            {
                IEnumerable<Vector3Int> enumerable2 = x.AdjacentCellsXZAndInside();
                Func<Vector3Int, bool> func;
                if ((func = <> 9__2) == null)
                {
                    func = (<> 9__2 = (Vector3Int y) => room.IsEntrance(y, true, true) || world.CellsInfo.AnyLadderAt(y.Below()));
                }
                return !enumerable2.Any<Vector3Int>(func);
            });
            if (pressurePlatePos != null)
            {
                Vector3Int? pressurePlatePosLocal = pressurePlatePos;
                Vector3Int vector3Int;
                if (enumerable.Where<Vector3Int>((Vector3Int x) => x.GetGridDistance(pressurePlatePosLocal.Value) <= 4 && LineOfSight.IsLineOfSight(x, pressurePlatePosLocal.Value)).TryGetRandomElement<Vector3Int>(out vector3Int))
                {
                    pressurePlatePos = null;
                    actor.Spawn(vector3Int);
                    memory.unusedBaseActors.Remove(actor);
                    return true;
                }
            }
            Vector3Int vector3Int2;
            if (enumerable.TryGetRandomElement<Vector3Int>(out vector3Int2))
            {
                actor.Spawn(vector3Int2);
                memory.unusedBaseActors.Remove(actor);
                return true;
            }
            Vector3Int vector3Int3;
            if (candidates.TryGetRandomElement<Vector3Int>(out vector3Int3))
            {
                actor.Spawn(vector3Int3);
                memory.unusedBaseActors.Remove(actor);
                return true;
            }
            return false;
        }
    }
}