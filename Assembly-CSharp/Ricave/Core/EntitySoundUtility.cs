using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public static class EntitySoundUtility
    {
        public static SoundSpec GetMoveSound(Actor actor, Vector3Int prevPos)
        {
            List<Entity> entitiesAt = Get.World.GetEntitiesAt(actor.Position);
            for (int i = 0; i < entitiesAt.Count; i++)
            {
                if (entitiesAt[i] is Structure)
                {
                    SoundSpec walkThroughSound = entitiesAt[i].Spec.Structure.WalkThroughSound;
                    if (walkThroughSound != null)
                    {
                        return walkThroughSound;
                    }
                }
            }
            if (actor.Spec.Actor.MoveSound != null && actor.AllowDanceAnimation && (!actor.Spec.Actor.UseMoveSoundOnlyIfCanFly || actor.CanFly))
            {
                return actor.Spec.Actor.MoveSound;
            }
            if (actor.CanUseLadders && !GravityUtility.IsAltitudeEqual(actor.Position, prevPos, actor.Gravity) && actor.Position.x == prevPos.x && actor.Position.z == prevPos.z && (Get.CellsInfo.AnyLadderAt(actor.Position) || Get.CellsInfo.IsLadderUnder(actor.Position, actor.Gravity)) && actor.AllowDanceAnimation)
            {
                Vector3Int vector3Int = (Get.CellsInfo.AnyLadderAt(actor.Position) ? actor.Position : (actor.Position + actor.Gravity));
                foreach (Entity entity in Get.World.GetEntitiesAt(vector3Int))
                {
                    if (entity is Structure && entity.Spec.Structure.IsLadder && entity.Spec.Structure.UsedLadderSound != null)
                    {
                        return entity.Spec.Structure.UsedLadderSound;
                    }
                }
                return Get.Sound_UseLadder;
            }
            if (!actor.IsNowControlledActor && actor.CanFly)
            {
                return Get.Sound_Fly;
            }
            if (Get.CellsInfo.IsFloorUnder(actor.Position, actor.Gravity) && (!actor.CanFly || !actor.ConditionsAccumulated.MovingDisallowedIfCantFly))
            {
                Vector3Int vector3Int2 = actor.Position + actor.Gravity;
                if (vector3Int2.InBounds())
                {
                    List<Entity> entitiesAt2 = Get.World.GetEntitiesAt(vector3Int2);
                    for (int j = 0; j < entitiesAt2.Count; j++)
                    {
                        if (entitiesAt2[j] is Structure && !entitiesAt2[j].Spec.Structure.IsLadder)
                        {
                            SoundSpec walkOnSound = entitiesAt2[j].Spec.Structure.WalkOnSound;
                            if (walkOnSound != null)
                            {
                                return walkOnSound;
                            }
                        }
                    }
                }
                return Get.Sound_Walk;
            }
            if (actor.CanUseLadders && (Get.CellsInfo.AnyLadderAt(actor.Position) || Get.CellsInfo.IsLadderUnder(actor.Position, actor.Gravity)) && (!actor.CanFly || !actor.ConditionsAccumulated.MovingDisallowedIfCantFly))
            {
                Vector3Int vector3Int3 = (Get.CellsInfo.AnyLadderAt(actor.Position) ? actor.Position : (actor.Position + actor.Gravity));
                foreach (Entity entity2 in Get.World.GetEntitiesAt(vector3Int3))
                {
                    if (entity2 is Structure && entity2.Spec.Structure.IsLadder && entity2.Spec.Structure.WalkOnSound != null)
                    {
                        return entity2.Spec.Structure.WalkOnSound;
                    }
                }
                return Get.Sound_Walk;
            }
            if (actor.CanFly)
            {
                return Get.Sound_Fly;
            }
            return null;
        }

        public static SoundSpec GetDestroySound(Entity entity)
        {
            Actor actor = entity as Actor;
            if (actor != null && actor.ConditionsAccumulated.AnyOfSpec(Get.Condition_Frozen))
            {
                return Get.Sound_DestroyFrozenActor;
            }
            if (entity.Spec.DestroySound != null)
            {
                return entity.Spec.DestroySound;
            }
            return Get.Sound_DestroyGeneric;
        }

        public static float GetPitchFromScale(Actor actor)
        {
            if (!(actor.ActorGOC != null))
            {
                return 1f;
            }
            Vector3 desiredGameObjectScaleWithoutCapAndAnimation = actor.ActorGOC.DesiredGameObjectScaleWithoutCapAndAnimation;
            float num = Math.Max(desiredGameObjectScaleWithoutCapAndAnimation.x, desiredGameObjectScaleWithoutCapAndAnimation.y);
            if (actor.IsBoss && actor.Spec.Actor.AlwaysBoss && num > 1f)
            {
                return 1f;
            }
            return 1f / num;
        }
    }
}