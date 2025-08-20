using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class Action_Debug_SetScale : Action
    {
        public Entity Entity
        {
            get
            {
                return this.entity;
            }
        }

        public Vector3 NewScale
        {
            get
            {
                return this.newScale;
            }
        }

        protected override int RandSeedPart
        {
            get
            {
                return Calc.CombineHashes<int, Vector3, int>(this.entity.MyStableHash, this.newScale, 61324571);
            }
        }

        public override string Description
        {
            get
            {
                return base.Spec.DescriptionVerbose.Formatted(this.entity);
            }
        }

        protected Action_Debug_SetScale()
        {
        }

        public Action_Debug_SetScale(ActionSpec spec, Entity entity, Vector3 newScale)
            : base(spec)
        {
            this.entity = entity;
            this.newScale = newScale;
        }

        public override bool CanDo(bool ignoreActorState = false)
        {
            return true;
        }

        protected override bool CalculateConcernsPlayer()
        {
            return true;
        }

        protected override IEnumerable<Instruction> MakeInstructions()
        {
            yield return new Instruction_SetScale(this.entity, this.newScale);
            yield break;
        }

        [Saved]
        private Entity entity;

        [Saved]
        private Vector3 newScale;
    }
}