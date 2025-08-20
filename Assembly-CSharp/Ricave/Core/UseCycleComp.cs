using System;
using System.Collections.Generic;
using Ricave.UI;

namespace Ricave.Core
{
    public class UseCycleComp : EntityComp
    {
        public new UseCycleCompProps Props
        {
            get
            {
                return (UseCycleCompProps)base.Props;
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

        public override string ExtraTip
        {
            get
            {
                return "UseCycleUseIn".Translate(RichText.Turns(StringUtility.TurnsString(this.Props.IntervalTurns - this.turnsPassed)));
            }
        }

        protected UseCycleComp()
        {
        }

        public UseCycleComp(Entity parent)
            : base(parent)
        {
            if (!(parent is SequenceableStructure))
            {
                Log.Warning("UseCycleComp requires entity of type SequenceableStructure. Entity: " + ((parent != null) ? parent.ToString() : null), false);
            }
        }

        public override IEnumerable<Instruction> MakeResolveStructureInstructions()
        {
            if (this.turnsPassed >= this.Props.IntervalTurns - 1)
            {
                Entity parent = base.Parent;
                Structure structure = parent as Structure;
                if (structure != null && structure.UseEffects.Any)
                {
                    yield return new Instruction_UseCycle_ChangeTurnsPassed(this, -this.turnsPassed);
                    foreach (Instruction instruction in InstructionSets_Usable.Use(null, structure, structure, null))
                    {
                        yield return instruction;
                    }
                    IEnumerator<Instruction> enumerator = null;
                    goto IL_013A;
                }
            }
            yield return new Instruction_UseCycle_ChangeTurnsPassed(this, 1);
        IL_013A:
            yield break;
            yield break;
        }

        [Saved]
        private int turnsPassed;
    }
}