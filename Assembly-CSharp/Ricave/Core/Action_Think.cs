using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class Action_Think : Action
    {
        public Actor Actor
        {
            get
            {
                return this.actor;
            }
        }

        protected override int RandSeedPart
        {
            get
            {
                return Calc.CombineHashes<int, int>(this.actor.MyStableHash, 316576323);
            }
        }

        public override string Description
        {
            get
            {
                return base.Spec.DescriptionVerbose.Formatted(this.actor);
            }
        }

        protected Action_Think()
        {
        }

        public Action_Think(ActionSpec spec, Actor actor)
            : base(spec)
        {
            this.actor = actor;
        }

        public override bool CanDo(bool ignoreActorState = false)
        {
            return ignoreActorState || this.actor.Spawned;
        }

        protected override bool CalculateConcernsPlayer()
        {
            return false;
        }

        protected override IEnumerable<Instruction> MakeInstructions()
        {
            if (this.actor.AI != null)
            {
                foreach (Instruction instruction in this.actor.AI.Root.MakeThinkInstructions(this.actor))
                {
                    yield return instruction;
                }
                IEnumerator<Instruction> enumerator = null;
            }
            yield break;
            yield break;
        }

        [Saved]
        private Actor actor;
    }
}