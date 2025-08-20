using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class UseEffect_CheckCollapseCeilingSingle : UseEffect
    {
        public override string LabelBase
        {
            get
            {
                return base.Spec.LabelFormat.Formatted(4.ToStringCached(), Get.DamageType_Physical.Adjective);
            }
        }

        protected UseEffect_CheckCollapseCeilingSingle()
        {
        }

        public UseEffect_CheckCollapseCeilingSingle(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            Entity support = usable as Entity;
            if (support == null)
            {
                yield break;
            }
            this.GetCollapsedCeiling(support.Position, this.tmpCollapsedCeiling);
            if (!this.tmpCollapsedCeiling.Any())
            {
                yield break;
            }
            foreach (Vector3Int vector3Int in this.tmpCollapsedCeiling)
            {
                yield return new Instruction_VisualEffect(Get.VisualEffect_CollapsingCeilingCell, vector3Int);
            }
            List<Vector3Int>.Enumerator enumerator = default(List<Vector3Int>.Enumerator);
            foreach (Entity entity in this.GetEntitiesToDamage(this.tmpCollapsedCeiling, support))
            {
                if (entity.Spawned)
                {
                    int num = DamageUtility.ApplyDamageProtectionAndClamp(entity, 4, Get.DamageType_Physical);
                    foreach (Instruction instruction in InstructionSets_Entity.Damage(entity, num, Get.DamageType_Physical, null, new Vector3Int?(entity.Position.Above()), false, false, null, null, false, true))
                    {
                        yield return instruction;
                    }
                    IEnumerator<Instruction> enumerator3 = null;
                }
            }
            List<Entity>.Enumerator enumerator2 = default(List<Entity>.Enumerator);
            yield return new Instruction_VisualEffect(Get.VisualEffect_CeilingCollapseSmall, support.Position);
            yield break;
            yield break;
        }

        private List<Entity> GetEntitiesToDamage(List<Vector3Int> collapsedCeiling, Entity support)
        {
            List<Entity> list = new List<Entity>();
            this.GetAffectedCells(collapsedCeiling, this.tmpAffectedCells, support);
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

        public void GetAffectedCellsForUI(Entity support, List<Vector3Int> outCells)
        {
            this.GetCollapsedCeiling(support.Position, this.tmpCollapsedCeilingForUI);
            this.GetAffectedCells(this.tmpCollapsedCeilingForUI, outCells, support);
        }

        public void GetAffectedCells(IList<Vector3Int> collapsedCeiling, List<Vector3Int> outCells, Entity support)
        {
            outCells.Clear();
            Predicate<Vector3Int> <> 9__0;
            foreach (Vector3Int vector3Int in collapsedCeiling)
            {
                Vector3Int down = Vector3IntUtility.Down;
                Predicate<Vector3Int> predicate;
                if ((predicate = <> 9__0) == null)
                {
                    predicate = (<> 9__0 = (Vector3Int x) => (Get.CellsInfo.CanProjectilesPassThrough(x) && Get.CellsInfo.CanPassThroughNoActors(x)) || x == support.Position);
                }
                foreach (Vector3Int vector3Int2 in vector3Int.SelectLineOnePastLastInBounds(down, predicate))
                {
                    outCells.Add(vector3Int2);
                }
            }
        }

        public void GetCollapsedCeiling(Vector3Int supportPos, List<Vector3Int> outCells)
        {
            outCells.Clear();
            Vector3Int vector3Int = supportPos.Above();
            if (!vector3Int.InBounds() || !Get.CellsInfo.IsFilled(vector3Int))
            {
                return;
            }
            outCells.Add(supportPos);
        }

        private List<Vector3Int> tmpCollapsedCeiling = new List<Vector3Int>();

        private List<Vector3Int> tmpCollapsedCeilingForUI = new List<Vector3Int>();

        private List<Vector3Int> tmpAffectedCells = new List<Vector3Int>();

        public const int Damage = 4;
    }
}