using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class UseEffect_Push : UseEffect
    {
        public int Distance
        {
            get
            {
                return this.distance;
            }
        }

        public bool OriginalTargetIsImpactSource
        {
            get
            {
                return this.originalTargetIsImpactSource;
            }
        }

        public bool UseAoEToCalculateDistance
        {
            get
            {
                return this.useAoEToCalculateDistance;
            }
        }

        public override string LabelBase
        {
            get
            {
                return base.Spec.LabelFormat.Formatted(StringUtility.CellsString(this.useAoEToCalculateDistance ? base.AoERadius.Value : this.distance));
            }
        }

        protected UseEffect_Push()
        {
        }

        public UseEffect_Push(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public UseEffect_Push(UseEffectSpec spec, int distance, float chance = 1f, int usesLeft = 0)
            : base(spec, chance, usesLeft, null, false)
        {
            this.distance = distance;
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
            UseEffect_Push useEffect_Push = (UseEffect_Push)clone;
            useEffect_Push.distance = this.distance;
            useEffect_Push.originalTargetIsImpactSource = this.originalTargetIsImpactSource;
            useEffect_Push.useAoEToCalculateDistance = this.useAoEToCalculateDistance;
            useEffect_Push.allowStructures = this.allowStructures;
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            if (!target.IsEntity || !target.Spawned)
            {
                yield break;
            }
            if (this.allowStructures)
            {
                if (!(target.Entity is Structure) && !(target.Entity is Actor))
                {
                    yield break;
                }
                Structure structure = target.Entity as Structure;
                if (structure != null && structure.Spec.Structure.IsGlass)
                {
                    yield break;
                }
            }
            else if (!(target.Entity is Actor))
            {
                yield break;
            }
            Actor actor = target.Entity as Actor;
            if (actor != null && actor.ImmuneToPushing)
            {
                yield break;
            }
            Vector3Int? vector3Int = (this.originalTargetIsImpactSource ? new Vector3Int?(originalTarget.Position) : DamageUtility.DeduceImpactSource(user, target, originalTarget));
            if (vector3Int == null)
            {
                yield break;
            }
            if (target.Position != vector3Int)
            {
                int num = (this.useAoEToCalculateDistance ? (base.AoERadius.Value - target.Position.GetGridDistance(originalTarget.Position) + 1) : this.distance);
                foreach (Instruction instruction in InstructionSets_Entity.Push(target.Entity, target.Position - vector3Int.Value, num, false, true, true))
                {
                    yield return instruction;
                }
                IEnumerator<Instruction> enumerator = null;
            }
            yield break;
            yield break;
        }

        [Saved]
        private int distance;

        [Saved]
        private bool originalTargetIsImpactSource;

        [Saved]
        private bool useAoEToCalculateDistance;

        [Saved]
        private bool allowStructures;
    }
}