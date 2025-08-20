using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class UseEffect_Visual_Explosion : UseEffect
    {
        public VisualEffectSpec PerCellEffect
        {
            get
            {
                return this.perCellEffect;
            }
        }

        public override bool AoEHandledManually
        {
            get
            {
                return true;
            }
        }

        protected UseEffect_Visual_Explosion()
        {
        }

        public UseEffect_Visual_Explosion(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
            UseEffect_Visual_Explosion useEffect_Visual_Explosion = (UseEffect_Visual_Explosion)clone;
            useEffect_Visual_Explosion.perCellEffect = this.perCellEffect;
            useEffect_Visual_Explosion.onlyPerCellEffect = this.onlyPerCellEffect;
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            if (!this.onlyPerCellEffect)
            {
                yield return new Instruction_VisualEffect(Get.VisualEffect_Explosion, target.Position);
            }
            if (this.perCellEffect != null)
            {
                UseEffect_Visual_Explosion.tmpAoEArea.Clear();
                AoEUtility.GetAoEArea(target.Position, this, usable, UseEffect_Visual_Explosion.tmpAoEArea, null);
                foreach (Vector3Int vector3Int in UseEffect_Visual_Explosion.tmpAoEArea)
                {
                    if (!Get.CellsInfo.AnyFilledCantSeeThroughAt(vector3Int))
                    {
                        yield return new Instruction_VisualEffect(this.perCellEffect, vector3Int);
                    }
                }
                List<Vector3Int>.Enumerator enumerator = default(List<Vector3Int>.Enumerator);
            }
            yield break;
            yield break;
        }

        [Saved]
        private VisualEffectSpec perCellEffect;

        [Saved]
        private bool onlyPerCellEffect;

        private static List<Vector3Int> tmpAoEArea = new List<Vector3Int>();
    }
}