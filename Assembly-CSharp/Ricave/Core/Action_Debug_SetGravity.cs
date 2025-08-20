using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class Action_Debug_SetGravity : Action
    {
        public Actor Actor
        {
            get
            {
                return this.actor;
            }
        }

        public Vector3Int NewGravity
        {
            get
            {
                return this.newGravity;
            }
        }

        protected override int RandSeedPart
        {
            get
            {
                return Calc.CombineHashes<int, Vector3Int, int>(this.actor.MyStableHash, this.newGravity, 906138657);
            }
        }

        public override string Description
        {
            get
            {
                return base.Spec.DescriptionVerbose.Formatted(this.actor);
            }
        }

        protected Action_Debug_SetGravity()
        {
        }

        public Action_Debug_SetGravity(ActionSpec spec, Actor actor, Vector3Int newGravity)
            : base(spec)
        {
            this.actor = actor;
            this.newGravity = newGravity;
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
            yield return new Instruction_SetGravity(this.actor, this.newGravity);
            yield break;
        }

        [Saved]
        private Actor actor;

        [Saved]
        private Vector3Int newGravity;
    }
}