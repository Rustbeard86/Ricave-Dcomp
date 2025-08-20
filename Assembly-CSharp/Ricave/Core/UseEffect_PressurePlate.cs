using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class UseEffect_PressurePlate : UseEffect
    {
        protected UseEffect_PressurePlate()
        {
        }

        public UseEffect_PressurePlate(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            Entity usableEntity = usable as Entity;
            if (usableEntity == null)
            {
                yield break;
            }
            Vector3Int entrance;
            Vector3Int dir;
            if (!this.TryFindEntrance(usableEntity.Position, out entrance, out dir))
            {
                yield break;
            }
            IEnumerator<Instruction> enumerator2;
            foreach (Entity entity in Get.World.GetEntitiesAt(entrance).ToTemporaryList<Entity>())
            {
                if (entity is Structure && entity.Spec.Structure.IsDoor)
                {
                    foreach (Instruction instruction in InstructionSets_Entity.DeSpawn(entity, false))
                    {
                        yield return instruction;
                    }
                    enumerator2 = null;
                }
            }
            List<Entity>.Enumerator enumerator = default(List<Entity>.Enumerator);
            foreach (Instruction instruction2 in InstructionSets_Entity.Spawn(Maker.Make(Get.Entity_TemporaryGate, null, false, false, true), entrance, new Quaternion?(dir.CardinalDirToQuaternion())))
            {
                yield return instruction2;
            }
            enumerator2 = null;
            foreach (Instruction instruction3 in this.PushActorsAway(entrance, usableEntity.Position))
            {
                yield return instruction3;
            }
            enumerator2 = null;
            Vector3Int above = entrance.Above();
            if (Get.CellsInfo.AnyStairsAt(entrance) && above.InBounds() && Get.CellsInfo.CanPassThroughNoActors(above))
            {
                foreach (Instruction instruction4 in InstructionSets_Entity.Spawn(Maker.Make(Get.Entity_TemporaryGate, null, false, false, true), above, new Quaternion?(dir.CardinalDirToQuaternion())))
                {
                    yield return instruction4;
                }
                enumerator2 = null;
                foreach (Instruction instruction5 in this.PushActorsAway(above, usableEntity.Position))
                {
                    yield return instruction5;
                }
                enumerator2 = null;
            }
            Vector3Int below = entrance.Below();
            if (below.InBounds() && Get.CellsInfo.AnyStairsAt(below) && Get.CellsInfo.CanPassThroughNoActors(below))
            {
                foreach (Instruction instruction6 in InstructionSets_Entity.Spawn(Maker.Make(Get.Entity_TemporaryGate, null, false, false, true), below, new Quaternion?(dir.CardinalDirToQuaternion())))
                {
                    yield return instruction6;
                }
                enumerator2 = null;
                foreach (Instruction instruction7 in this.PushActorsAway(below, usableEntity.Position))
                {
                    yield return instruction7;
                }
                enumerator2 = null;
            }
            yield return new Instruction_PlayLog("GateClosesBehindYou".Translate());
            yield break;
            yield break;
        }

        private bool TryFindEntrance(Vector3Int pressurePlatePos, out Vector3Int entrance, out Vector3Int dir)
        {
            for (int i = 0; i < 2; i++)
            {
                bool flag = i == 1;
                foreach (Vector3Int vector3Int in pressurePlatePos.AdjacentCardinalCellsXZ())
                {
                    if (vector3Int.InBounds() && Get.CellsInfo.CanPassThroughNoActors(vector3Int))
                    {
                        List<RetainedRoomInfo.RoomInfo> rooms = vector3Int.GetRooms();
                        if (rooms.Count != 0 && rooms[0].Shape.IsOnEdgeXZ(vector3Int))
                        {
                            entrance = vector3Int;
                            dir = rooms[0].Shape.GetEdge(entrance).RightDir();
                            if (flag || ((entrance + dir).InBounds() && Get.CellsInfo.IsFilled(entrance + dir) && (entrance - dir).InBounds() && Get.CellsInfo.IsFilled(entrance - dir)))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            entrance = pressurePlatePos;
            dir = default(Vector3Int);
            return false;
        }

        private IEnumerable<Instruction> PushActorsAway(Vector3Int at, Vector3Int pressurePlatePos)
        {
            Vector3Int vector3Int = at;
            vector3Int.y = pressurePlatePos.y;
            Vector3Int vector3Int2 = pressurePlatePos - vector3Int;
            Vector3Int targetPos = at - vector3Int2;
            if (Get.CellsInfo.AnyStairsAt(at))
            {
                int y = targetPos.y;
                targetPos.y = y + 1;
            }
            if (!Get.CellsInfo.CanPassThroughNoActors(targetPos))
            {
                targetPos = at;
            }
            foreach (Entity entity in Get.World.GetEntitiesAt(at).ToTemporaryList<Entity>())
            {
                Actor actor = entity as Actor;
                if (actor != null)
                {
                    Vector3Int vector3Int3 = SpawnPositionFinder.Near(targetPos, entity, false, false, null);
                    if (!(vector3Int3 == entity.Position))
                    {
                        foreach (Instruction instruction in InstructionSets_Entity.Move(actor, vector3Int3, true, true))
                        {
                            yield return instruction;
                        }
                        IEnumerator<Instruction> enumerator2 = null;
                    }
                }
            }
            List<Entity>.Enumerator enumerator = default(List<Entity>.Enumerator);
            yield break;
            yield break;
        }
    }
}