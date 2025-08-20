using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ricave.Core
{
    public class UseEffect_Kick : UseEffect
    {
        protected UseEffect_Kick()
        {
        }

        public UseEffect_Kick(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            Entity entity = target.Entity;
            Structure structure = entity as Structure;
            if (structure == null || structure.Spec != Get.Entity_Door || !structure.Spawned)
            {
                yield break;
            }
            IEnumerator<Instruction> enumerator;
            if (user != null)
            {
                foreach (Instruction instruction in InstructionSets_Entity.PreViolentlyAttackedBy(target.Entity, user))
                {
                    yield return instruction;
                }
                enumerator = null;
            }
            Vector3Int? vector3Int = DamageUtility.DeduceImpactSource(user, target, originalTarget);
            foreach (Instruction instruction2 in InstructionSets_Entity.Destroy(structure, vector3Int, user))
            {
                yield return instruction2;
            }
            enumerator = null;
            if (user != null)
            {
                foreach (Instruction instruction3 in InstructionSets_Entity.PostViolentlyAttackedBy(target.Entity, user))
                {
                    yield return instruction3;
                }
                enumerator = null;
            }
            if (user != null && user.IsNowControlledActor)
            {
                yield return new Instruction_VisualEffect(Get.VisualEffect_PlayerKicksDoor, structure.Position);
            }
            else
            {
                yield return new Instruction_Sound(Get.Sound_KickDoor, new Vector3?(structure.Position), 1f, 1f);
            }
            if (user != null)
            {
                yield return new Instruction_StartStrikeAnimation(user, structure.Position);
            }
            if (user != null)
            {
                Vector3Int vector3Int2 = structure.Position + structure.DirectionCardinal.RightDir();
                Vector3Int vector3Int3 = structure.Position - structure.DirectionCardinal.RightDir();
                if (user.Position.GetGridDistance(vector3Int2) != user.Position.GetGridDistance(vector3Int3) && vector3Int2.InBounds() && vector3Int3.InBounds())
                {
                    Vector3Int otherSide;
                    Vector3Int mySide;
                    if (user.Position.GetGridDistance(vector3Int2) < user.Position.GetGridDistance(vector3Int3))
                    {
                        otherSide = vector3Int3;
                        mySide = vector3Int2;
                    }
                    else
                    {
                        otherSide = vector3Int2;
                        mySide = vector3Int3;
                    }
                    RetainedRoomInfo.RoomInfo roomInfo = otherSide.GetRooms().FirstOrDefault<RetainedRoomInfo.RoomInfo>();
                    if (roomInfo != null)
                    {
                        foreach (Vector3Int vector3Int4 in roomInfo.Shape)
                        {
                            if (Get.World.AnyEntityAt(vector3Int4))
                            {
                                foreach (Entity entity2 in Get.World.GetEntitiesAt(vector3Int4).ToTemporaryList<Entity>())
                                {
                                    Actor actor2 = entity2 as Actor;
                                    if (actor2 != null && actor2.Spawned && actor2.Conditions.AnyOfSpec(Get.Condition_Sleeping))
                                    {
                                        foreach (Instruction instruction4 in InstructionSets_Actor.WakeUp(actor2, "WakeUpReason_KickedDoor".Translate()))
                                        {
                                            yield return instruction4;
                                        }
                                        enumerator = null;
                                    }
                                }
                                List<Entity>.Enumerator enumerator3 = default(List<Entity>.Enumerator);
                            }
                        }
                    }
                    IEnumerable<Vector3Int> enumerable = from x in structure.Position.GetCellsAtDist(1)
                                                         where x.InBounds() && x.GetGridDistance(otherSide) < x.GetGridDistance(mySide) && LineOfSight.IsLineOfFire(structure.Position, x)
                                                         select x;
                    List<Actor> list = (from x in structure.Position.GetCellsAtDist(2)
                                        where x.InBounds() && x.GetGridDistance(otherSide) < x.GetGridDistance(mySide) && LineOfSight.IsLineOfFire(structure.Position, x)
                                        select x).SelectMany<Vector3Int, Entity>((Vector3Int x) => Get.World.GetEntitiesAt(x)).OfType<Actor>().ToTemporaryList<Actor>();
                    List<Actor> actorsAtDist = enumerable.SelectMany<Vector3Int, Entity>((Vector3Int x) => Get.World.GetEntitiesAt(x)).OfType<Actor>().ToTemporaryList<Actor>();
                    foreach (Actor actor in list)
                    {
                        if (actor.Spawned)
                        {
                            if (actor.Conditions.AnyOfSpec(Get.Condition_Sleeping))
                            {
                                foreach (Instruction instruction5 in InstructionSets_Actor.WakeUp(actor, "WakeUpReason_KickedDoor".Translate()))
                                {
                                    yield return instruction5;
                                }
                                enumerator = null;
                            }
                            foreach (Instruction instruction6 in InstructionSets_Entity.Push(actor, actor.Position - structure.Position, 1, false, true, true))
                            {
                                yield return instruction6;
                            }
                            enumerator = null;
                            actor = null;
                        }
                    }
                    List<Actor>.Enumerator enumerator4 = default(List<Actor>.Enumerator);
                    foreach (Actor actor in actorsAtDist)
                    {
                        if (actor.Spawned)
                        {
                            if (actor.Conditions.AnyOfSpec(Get.Condition_Sleeping))
                            {
                                foreach (Instruction instruction7 in InstructionSets_Actor.WakeUp(actor, "WakeUpReason_KickedDoor".Translate()))
                                {
                                    yield return instruction7;
                                }
                                enumerator = null;
                            }
                            foreach (Instruction instruction8 in InstructionSets_Entity.Push(actor, actor.Position - structure.Position, 2, false, true, true))
                            {
                                yield return instruction8;
                            }
                            enumerator = null;
                            if (actor.Spawned)
                            {
                                int num = DamageUtility.ApplyDamageProtectionAndClamp(actor, 1, Get.DamageType_Crush);
                                foreach (Instruction instruction9 in InstructionSets_Entity.Damage(actor, num, Get.DamageType_Crush, null, new Vector3Int?(structure.Position), false, false, null, null, false, true))
                                {
                                    yield return instruction9;
                                }
                                enumerator = null;
                            }
                            actor = null;
                        }
                    }
                    enumerator4 = default(List<Actor>.Enumerator);
                    actorsAtDist = null;
                }
            }
            yield break;
            yield break;
        }
    }
}