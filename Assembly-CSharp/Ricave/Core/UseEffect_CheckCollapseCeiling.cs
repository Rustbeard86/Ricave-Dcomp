using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ricave.Core
{
    public class UseEffect_CheckCollapseCeiling : UseEffect
    {
        public override string LabelBase
        {
            get
            {
                return base.Spec.LabelFormat.Formatted(5.ToStringCached(), Get.DamageType_Physical.Adjective);
            }
        }

        protected UseEffect_CheckCollapseCeiling()
        {
        }

        public UseEffect_CheckCollapseCeiling(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            Entity pillar = usable as Entity;
            if (pillar == null)
            {
                yield break;
            }
            this.GetCollapsedCeiling(pillar.Position, this.tmpCollapsedCeiling);
            if (!this.tmpCollapsedCeiling.Any())
            {
                yield break;
            }
            foreach (Vector3Int vector3Int in this.tmpCollapsedCeiling)
            {
                yield return new Instruction_VisualEffect(Get.VisualEffect_CollapsingCeilingCell, vector3Int);
            }
            List<Vector3Int>.Enumerator enumerator = default(List<Vector3Int>.Enumerator);
            foreach (Entity entity in this.GetEntitiesToDamage(this.tmpCollapsedCeiling))
            {
                if (entity.Spawned)
                {
                    int num = DamageUtility.ApplyDamageProtectionAndClamp(entity, 5, Get.DamageType_Physical);
                    foreach (Instruction instruction in InstructionSets_Entity.Damage(entity, num, Get.DamageType_Physical, null, new Vector3Int?(entity.Position.Above()), false, false, null, null, false, true))
                    {
                        yield return instruction;
                    }
                    IEnumerator<Instruction> enumerator3 = null;
                }
            }
            List<Entity>.Enumerator enumerator2 = default(List<Entity>.Enumerator);
            yield return new Instruction_VisualEffect(Get.VisualEffect_CeilingCollapse, pillar.Position);
            yield break;
            yield break;
        }

        private List<Entity> GetEntitiesToDamage(List<Vector3Int> collapsedCeiling)
        {
            List<Entity> list = new List<Entity>();
            this.GetAffectedCells(collapsedCeiling, this.tmpAffectedCells);
            foreach (Vector3Int vector3Int in this.tmpAffectedCells)
            {
                foreach (Entity entity in Get.World.GetEntitiesAt(vector3Int))
                {
                    if (entity.MaxHP != 0)
                    {
                        list.Add(entity);
                    }
                }
            }
            return list;
        }

        public void GetAffectedCellsForUIAssumingChainedPillarDestroy(Entity pillar, List<Vector3Int> outCells)
        {
            Vector3Int vector3Int = pillar.Position;
            while (vector3Int.Above().InBounds() && Get.World.AnyEntityOfSpecAt(vector3Int.Above(), Get.Entity_Pillar))
            {
                vector3Int = vector3Int.Above();
            }
            this.GetCollapsedCeiling(vector3Int, this.tmpCollapsedCeilingForUI);
            this.GetAffectedCells(this.tmpCollapsedCeilingForUI, outCells);
        }

        public void GetAffectedCells(IList<Vector3Int> collapsedCeiling, List<Vector3Int> outCells)
        {
            outCells.Clear();
            using (IEnumerator<Vector3Int> enumerator = collapsedCeiling.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    foreach (Vector3Int vector3Int in enumerator.Current.SelectLineOnePastLastInBounds(Vector3IntUtility.Down, (Vector3Int x) => Get.CellsInfo.CanProjectilesPassThrough(x) && Get.CellsInfo.CanPassThroughNoActors(x)))
                    {
                        outCells.Add(vector3Int);
                    }
                }
            }
        }

        public void GetCollapsedCeiling(Vector3Int pillarPos, List<Vector3Int> outCells)
        {
            outCells.Clear();
            Vector3Int vector3Int = pillarPos.Above();
            if (!vector3Int.InBounds() || !Get.CellsInfo.IsFilled(vector3Int))
            {
                return;
            }
            List<RetainedRoomInfo.RoomInfo> rooms = pillarPos.GetRooms();
            Func<RetainedRoomInfo.RoomInfo, bool> <> 9__2;
            FloodFiller.FloodFill(pillarPos, delegate (Vector3Int x)
            {
                if (x.y == pillarPos.y && Get.CellsInfo.IsFilled(x.Above()) && !Get.CellsInfo.IsFilled(x))
                {
                    IEnumerable<RetainedRoomInfo.RoomInfo> rooms2 = x.GetRooms();
                    Func<RetainedRoomInfo.RoomInfo, bool> func;
                    if ((func = <> 9__2) == null)
                    {
                        func = (<> 9__2 = (RetainedRoomInfo.RoomInfo y) => rooms.Contains(y));
                    }
                    return rooms2.Any<RetainedRoomInfo.RoomInfo>(func);
                }
                return false;
            }, delegate (Vector3Int x)
            {
                outCells.Add(x);
            });
            bool flag = false;
            foreach (Vector3Int vector3Int2 in outCells)
            {
                if (vector3Int2 != pillarPos && Get.World.AnyEntityOfSpecAt(vector3Int2, Get.Entity_Pillar))
                {
                    flag = true;
                    break;
                }
            }
            if (flag)
            {
                outCells.Clear();
                return;
            }
        }

        private List<Vector3Int> tmpCollapsedCeiling = new List<Vector3Int>();

        private List<Vector3Int> tmpCollapsedCeilingForUI = new List<Vector3Int>();

        private List<Vector3Int> tmpAffectedCells = new List<Vector3Int>();

        public const int Damage = 5;
    }
}