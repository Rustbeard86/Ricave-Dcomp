using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class UseEffect_Pull : UseEffect
    {
        public int Distance
        {
            get
            {
                return this.distance;
            }
        }

        public override string LabelBase
        {
            get
            {
                return base.Spec.LabelFormat.Formatted(StringUtility.CellsString(this.distance));
            }
        }

        protected UseEffect_Pull()
        {
        }

        public UseEffect_Pull(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public UseEffect_Pull(UseEffectSpec spec, int distance, float chance = 1f, int usesLeft = 0)
            : base(spec, chance, usesLeft, null, false)
        {
            this.distance = distance;
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
            ((UseEffect_Pull)clone).distance = this.distance;
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            if (!target.IsEntity || !target.Spawned)
            {
                yield break;
            }
            Actor actor = target.Entity as Actor;
            if (actor != null && actor.ImmuneToPushing)
            {
                yield break;
            }
            Vector3Int? vector3Int = DamageUtility.DeduceImpactSource(user, target, originalTarget);
            if (vector3Int == null)
            {
                yield break;
            }
            int num = Math.Min(this.distance, target.Position.GetGridDistance(vector3Int.Value));
            foreach (Instruction instruction in InstructionSets_Entity.Push(target.Entity, vector3Int.Value - target.Position, num, true, true, true))
            {
                yield return instruction;
            }
            IEnumerator<Instruction> enumerator = null;
            yield break;
            yield break;
        }

        [Saved]
        private int distance;
    }
}