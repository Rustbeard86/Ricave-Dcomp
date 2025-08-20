using System;
using System.Collections.Generic;
using System.Linq;
using Ricave.UI;
using UnityEngine;

namespace Ricave.Core
{
    public class UseEffect_TeleportRandomly : UseEffect
    {
        protected UseEffect_TeleportRandomly()
        {
        }

        public UseEffect_TeleportRandomly(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public UseEffect_TeleportRandomly(UseEffectSpec spec, float chance, int usesLeft = 0)
            : base(spec, chance, usesLeft, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
            UseEffect_TeleportRandomly useEffect_TeleportRandomly = (UseEffect_TeleportRandomly)clone;
            useEffect_TeleportRandomly.alwaysSameDestination = this.alwaysSameDestination;
            useEffect_TeleportRandomly.rememberedDestination = this.rememberedDestination;
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            if (!target.IsEntity || !target.Entity.Spawned)
            {
                yield break;
            }
            Vector3Int dest;
            if (this.rememberedDestination != null)
            {
                dest = SpawnPositionFinder.Near(this.rememberedDestination.Value, target.Entity, false, false, null);
            }
            else
            {
                dest = this.FindTeleportDestination(target.Entity);
                if (this.alwaysSameDestination)
                {
                    this.rememberedDestination = new Vector3Int?(dest);
                }
            }
            if (dest == target.Entity.Position)
            {
                yield break;
            }
            if (ActionUtility.TargetConcernsPlayer(target.Entity))
            {
                if (this.alwaysSameDestination)
                {
                    yield return new Instruction_PlayLog("EntityTeleports".Translate(RichText.Label(target)));
                }
                else
                {
                    yield return new Instruction_PlayLog("EntityTeleportsRandomly".Translate(RichText.Label(target)));
                }
            }
            yield return new Instruction_Sound(Get.Sound_Teleport, new Vector3?(target.Position), 1f, 1f);
            foreach (Instruction instruction in InstructionSets_Entity.Move(target.Entity, dest, true, true))
            {
                yield return instruction;
            }
            IEnumerator<Instruction> enumerator = null;
            yield return new Instruction_Sound(Get.Sound_Teleport, new Vector3?(dest), 1f, 1f);
            yield break;
            yield break;
        }

        private Vector3Int FindTeleportDestination(Entity forEntity)
        {
            Actor actor = forEntity as Actor;
            bool flag = (actor != null && !actor.CanFly) || this.alwaysSameDestination;
            bool flag2 = actor == null && forEntity.Spec.MaxHP == 0 && !forEntity.Spec.CanPassThrough;
            IEnumerable<RetainedRoomInfo.RoomInfo> enumerable = Get.RetainedRoomInfo.Rooms.Where<RetainedRoomInfo.RoomInfo>((RetainedRoomInfo.RoomInfo x) => x.Role != Room.LayoutRole.LockedBehindSilverDoor && x.Role != Room.LayoutRole.OptionalChallenge && x.Role != Room.LayoutRole.Secret);
            if (forEntity.IsMainActor || this.alwaysSameDestination)
            {
                enumerable = enumerable.Where<RetainedRoomInfo.RoomInfo>((RetainedRoomInfo.RoomInfo x) => x.Role != Room.LayoutRole.LockedBehindGate);
            }
            IEnumerable<Vector3Int> enumerable2;
            if (flag)
            {
                enumerable2 = enumerable.SelectMany<RetainedRoomInfo.RoomInfo, Vector3Int>((RetainedRoomInfo.RoomInfo x) => x.Shape.InnerCuboid(1).BottomSurfaceCuboid);
            }
            else
            {
                enumerable2 = enumerable.SelectMany<RetainedRoomInfo.RoomInfo, Vector3Int>((RetainedRoomInfo.RoomInfo x) => x.Shape.InnerCuboid(1));
            }
            IEnumerable<Vector3Int> enumerable3 = enumerable2.Where<Vector3Int>((Vector3Int x) => x.GetGridDistance(forEntity.Position) >= 13).InRandomOrder<Vector3Int>();
            IEnumerable<Vector3Int> enumerable4 = enumerable2;
            Func<Vector3Int, int> <> 9__5;
            Func<Vector3Int, int> func;
            if ((func = <> 9__5) == null)
            {
                func = (<> 9__5 = (Vector3Int x) => x.GetGridDistance(forEntity.Position));
            }
            foreach (Vector3Int vector3Int in enumerable3.Concat<Vector3Int>(enumerable4.OrderByDescending<Vector3Int, int>(func)))
            {
                if (!Get.World.AnyEntityAt(vector3Int) && (!flag || Get.CellsInfo.IsFloorUnder(vector3Int)) && ItemOrStructureFallUtility.WouldHaveSupport(forEntity.Spec, vector3Int, new Vector3Int?(forEntity.DirectionCardinal)))
                {
                    if (flag2)
                    {
                        bool flag3 = false;
                        foreach (Vector3Int vector3Int2 in vector3Int.AdjacentCellsXZ())
                        {
                            if (!vector3Int2.InBounds() || !Get.CellsInfo.CanPassThroughNoActors(vector3Int2) || !Get.CellsInfo.IsFloorUnderNoActors(vector3Int2))
                            {
                                flag3 = true;
                                break;
                            }
                        }
                        if (flag3)
                        {
                            continue;
                        }
                    }
                    return vector3Int;
                }
            }
            return forEntity.Position;
        }

        [Saved]
        private bool alwaysSameDestination;

        [Saved]
        private Vector3Int? rememberedDestination;
    }
}