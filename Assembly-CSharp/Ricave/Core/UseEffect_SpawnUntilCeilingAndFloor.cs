using System;
using System.Collections.Generic;
using Ricave.UI;
using UnityEngine;

namespace Ricave.Core
{
    public class UseEffect_SpawnUntilCeilingAndFloor : UseEffect
    {
        public EntitySpec EntitySpec
        {
            get
            {
                return this.entitySpec;
            }
        }

        public override string LabelBase
        {
            get
            {
                return StringUtility.Concat(base.Spec.LabelFormat.Formatted(this.entitySpec), (this.SpawnedEntityLifespan > 0) ? RichText.Turns(" ({0})".Formatted(StringUtility.TurnsString(this.SpawnedEntityLifespan))) : "");
            }
        }

        public override Texture2D Icon
        {
            get
            {
                return this.entitySpec.IconAdjusted;
            }
        }

        public override Color IconColor
        {
            get
            {
                return this.entitySpec.IconColorAdjusted;
            }
        }

        private int SpawnedEntityLifespan
        {
            get
            {
                foreach (EntityCompProps entityCompProps in this.entitySpec.AllCompProps)
                {
                    LifespanCompProps lifespanCompProps = entityCompProps as LifespanCompProps;
                    if (lifespanCompProps != null)
                    {
                        return lifespanCompProps.LifespanTurns;
                    }
                }
                return -1;
            }
        }

        protected UseEffect_SpawnUntilCeilingAndFloor()
        {
        }

        public UseEffect_SpawnUntilCeilingAndFloor(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
            UseEffect_SpawnUntilCeilingAndFloor useEffect_SpawnUntilCeilingAndFloor = (UseEffect_SpawnUntilCeilingAndFloor)clone;
            useEffect_SpawnUntilCeilingAndFloor.entitySpec = this.entitySpec;
            useEffect_SpawnUntilCeilingAndFloor.onlyIfTargetIsCell = this.onlyIfTargetIsCell;
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            if (this.onlyIfTargetIsCell && !target.IsLocation)
            {
                yield break;
            }
            if (!this.entitySpec.IsStructure || this.entitySpec.Structure.FallBehavior == StructureFallBehavior.None)
            {
                if (!SpawnPositionFinder.CanSpawnAt(this.entitySpec, target.Position, null, false, null))
                {
                    yield break;
                }
                Vector3Int cur = target.Position;
                do
                {
                    foreach (Instruction instruction in this.SpawnAt(cur))
                    {
                        yield return instruction;
                    }
                    IEnumerator<Instruction> enumerator = null;
                    cur += Vector3IntUtility.Up;
                }
                while (cur.InBounds() && SpawnPositionFinder.CanSpawnAt(this.entitySpec, cur, null, false, null));
                cur = target.Position;
                do
                {
                    foreach (Instruction instruction2 in this.SpawnAt(cur))
                    {
                        yield return instruction2;
                    }
                    IEnumerator<Instruction> enumerator = null;
                    cur += Vector3IntUtility.Down;
                }
                while (cur.InBounds() && SpawnPositionFinder.CanSpawnAt(this.entitySpec, cur, null, false, null));
                cur = default(Vector3Int);
            }
            else
            {
                Vector3Int cur;
                Vector3Int vector3Int;
                if (!UseEffect_SpawnUntilCeilingAndFloor.TryFindStartSpawnPos(this.entitySpec, target.Position, out vector3Int, out cur))
                {
                    yield break;
                }
                Vector3Int cur2 = vector3Int;
                do
                {
                    foreach (Instruction instruction3 in this.SpawnAt(cur2))
                    {
                        yield return instruction3;
                    }
                    IEnumerator<Instruction> enumerator = null;
                    cur2 -= cur;
                }
                while (cur2.InBounds() && SpawnPositionFinder.CanSpawnAt(this.entitySpec, cur2, null, false, null));
                cur = default(Vector3Int);
                cur2 = default(Vector3Int);
            }
            yield break;
            yield break;
        }

        private IEnumerable<Instruction> SpawnAt(Vector3Int at)
        {
            Entity entity = Maker.Make(this.entitySpec, null, false, false, true);
            foreach (Instruction instruction in InstructionSets_Entity.Spawn(entity, at, null))
            {
                yield return instruction;
            }
            IEnumerator<Instruction> enumerator = null;
            yield break;
            yield break;
        }

        public static bool TryFindStartSpawnPos(EntitySpec entitySpec, Vector3Int from, out Vector3Int pos, out Vector3Int dir)
        {
            if (!entitySpec.IsStructure || entitySpec.Structure.FallBehavior == StructureFallBehavior.None)
            {
                dir = Vector3Int.zero;
                if (SpawnPositionFinder.CanSpawnAt(entitySpec, from, null, false, null))
                {
                    pos = from;
                    return true;
                }
                pos = default(Vector3Int);
                return false;
            }
            else
            {
                dir = ((entitySpec.IsStructure && entitySpec.Structure.AttachesToCeiling) ? Vector3IntUtility.Up : Vector3IntUtility.Down);
                CellCuboid cellCuboid = from.SelectLineInBounds(dir, (Vector3Int x) => Get.CellsInfo.CanPassThroughNoActors(x));
                if (cellCuboid.Empty)
                {
                    pos = default(Vector3Int);
                    return false;
                }
                Vector3Int? vector3Int = null;
                foreach (Vector3Int vector3Int2 in cellCuboid)
                {
                    if (SpawnPositionFinder.CanSpawnAt(entitySpec, vector3Int2, null, false, null))
                    {
                        vector3Int = new Vector3Int?(vector3Int2);
                    }
                }
                if (vector3Int == null)
                {
                    pos = default(Vector3Int);
                    return false;
                }
                pos = vector3Int.Value;
                return true;
            }
        }

        [Saved]
        private EntitySpec entitySpec;

        [Saved]
        private bool onlyIfTargetIsCell;
    }
}