using System;
using System.Collections.Generic;
using Ricave.UI;
using UnityEngine;

namespace Ricave.Core
{
    public class LifespanComp : EntityComp
    {
        public new LifespanCompProps Props
        {
            get
            {
                return (LifespanCompProps)base.Props;
            }
        }

        public int TurnsPassed
        {
            get
            {
                return this.turnsPassed;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.turnsPassed = value;
            }
        }

        public int TurnsLeft
        {
            get
            {
                return this.Props.LifespanTurns - this.turnsPassed;
            }
        }

        public override string ExtraTip
        {
            get
            {
                return "DisappearsIn".Translate(RichText.Turns(StringUtility.TurnsString(this.TurnsLeft)));
            }
        }

        protected LifespanComp()
        {
        }

        public LifespanComp(Entity parent)
            : base(parent)
        {
            if (!(parent is SequenceableStructure))
            {
                Log.Warning("LifespanComp requires entity of type SequenceableStructure. Entity: " + ((parent != null) ? parent.ToString() : null), false);
            }
        }

        public override IEnumerable<Instruction> MakeResolveStructureInstructions()
        {
            if (this.turnsPassed >= this.Props.LifespanTurns - 1)
            {
                if (this.turnsPassed != 0)
                {
                    yield return new Instruction_Lifespan_ChangeTurnsPassed(this, -this.turnsPassed);
                }
                if (this.Props.ExpiredSound != null)
                {
                    yield return new Instruction_Sound(this.Props.ExpiredSound, new Vector3?(base.Parent.Position), 1f, 1f);
                }
                foreach (Instruction instruction in InstructionSets_Entity.DeSpawn(base.Parent, true))
                {
                    yield return instruction;
                }
                IEnumerator<Instruction> enumerator = null;
            }
            else
            {
                yield return new Instruction_Lifespan_ChangeTurnsPassed(this, 1);
            }
            yield break;
            yield break;
        }

        [Saved]
        private int turnsPassed;
    }
}