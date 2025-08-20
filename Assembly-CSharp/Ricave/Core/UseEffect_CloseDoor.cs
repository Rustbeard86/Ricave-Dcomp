using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class UseEffect_CloseDoor : UseEffect
    {
        protected UseEffect_CloseDoor()
        {
        }

        public UseEffect_CloseDoor(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
        }

        public override bool PreventEntireUse(Actor user, IUsable usable, Target target, StringSlot outReason = null)
        {
            Entity entity = usable as Entity;
            if (entity == null)
            {
                return true;
            }
            foreach (Entity entity2 in Get.World.GetEntitiesAt(entity.Position))
            {
                if (entity2 != entity)
                {
                    if (entity2 == user)
                    {
                        if (!this.CanPushAway(user))
                        {
                            if (outReason != null)
                            {
                                outReason.Set("CantCloseBlockedDoor".Translate());
                            }
                            return true;
                        }
                    }
                    else if (entity2 is Actor || entity2 is Item || entity2 is Structure)
                    {
                        if (outReason != null)
                        {
                            outReason.Set("CantCloseBlockedDoor".Translate());
                        }
                        return true;
                    }
                }
            }
            return false;
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            Entity usableEntity = usable as Entity;
            if (usableEntity == null || !usableEntity.Spawned)
            {
                yield break;
            }
            foreach (Instruction instruction in InstructionSets_Entity.DeSpawn(usableEntity, false))
            {
                yield return instruction;
            }
            IEnumerator<Instruction> enumerator = null;
            Entity door = Maker.Make(Get.Entity_Door, null, false, false, true);
            foreach (Instruction instruction2 in InstructionSets_Entity.Spawn(door, usableEntity.Position, new Quaternion?(usableEntity.Rotation)))
            {
                yield return instruction2;
            }
            enumerator = null;
            if (user.Position == door.Position)
            {
                foreach (Instruction instruction3 in this.TryPushAway(user))
                {
                    yield return instruction3;
                }
                enumerator = null;
            }
            yield return new Instruction_Sound(Get.Sound_CloseDoor, new Vector3?(door.Position), 1f, 1f);
            yield break;
            yield break;
        }

        private IEnumerable<Instruction> TryPushAway(Actor actor)
        {
            foreach (Vector3Int vector3Int in Vector3IntUtility.DirectionsXZCardinal.InRandomOrder<Vector3Int>())
            {
                Vector3Int vector3Int2 = actor.Position + vector3Int;
                if (vector3Int2.InBounds() && Get.CellsInfo.CanPassThrough(vector3Int2))
                {
                    foreach (Instruction instruction in InstructionSets_Entity.Move(actor, vector3Int2, true, true))
                    {
                        yield return instruction;
                    }
                    IEnumerator<Instruction> enumerator2 = null;
                    break;
                }
            }
            IEnumerator<Vector3Int> enumerator = null;
            yield break;
            yield break;
        }

        private bool CanPushAway(Actor actor)
        {
            foreach (Vector3Int vector3Int in Vector3IntUtility.DirectionsXZCardinal)
            {
                Vector3Int vector3Int2 = actor.Position + vector3Int;
                if (vector3Int2.InBounds() && Get.CellsInfo.CanPassThrough(vector3Int2))
                {
                    return true;
                }
            }
            return false;
        }
    }
}