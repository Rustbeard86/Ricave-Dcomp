using System;
using System.Collections.Generic;
using Ricave.UI;
using UnityEngine;

namespace Ricave.Core
{
    public class ProximityTriggerComp : EntityComp
    {
        public new ProximityTriggerCompProps Props
        {
            get
            {
                return (ProximityTriggerCompProps)base.Props;
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

        public bool Triggered
        {
            get
            {
                return this.triggered;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.triggered = value;
            }
        }

        public int TurnsLeft
        {
            get
            {
                return this.Props.Turns - this.turnsPassed;
            }
        }

        public override string ExtraTip
        {
            get
            {
                if (!this.triggered)
                {
                    return null;
                }
                return "TriggerTurnsLeft".Translate(RichText.Turns(StringUtility.TurnsString(this.TurnsLeft)));
            }
        }

        protected ProximityTriggerComp()
        {
        }

        public ProximityTriggerComp(Entity parent)
            : base(parent)
        {
            if (!(parent is SequenceableStructure))
            {
                Log.Warning("ProximityTriggerComp requires entity of type SequenceableStructure. Entity: " + ((parent != null) ? parent.ToString() : null), false);
            }
        }

        public override IEnumerable<Instruction> MakeResolveStructureInstructions()
        {
            if (!this.triggered)
            {
                yield break;
            }
            yield return new Instruction_ProximityTrigger_ChangeTurnsPassed(this, 1);
            yield return new Instruction_Sound(Get.Sound_TriggerTick, new Vector3?(base.Parent.Position), 1f, 1f);
            if (this.turnsPassed >= this.Props.Turns)
            {
                foreach (Instruction instruction in this.CountdownPassed())
                {
                    yield return instruction;
                }
                IEnumerator<Instruction> enumerator = null;
            }
            else
            {
                yield return new Instruction_PlayLog("TriggerWillActivateIn".Translate(base.Parent, RichText.Turns(StringUtility.TurnsString(this.TurnsLeft))));
                yield return new Instruction_AddFloatingText(base.Parent, "{0}...".Formatted(this.TurnsLeft), new Color(0.8f, 0.8f, 0.8f), 0.4f, 0f, 0f, null);
            }
            yield break;
            yield break;
        }

        public IEnumerable<Instruction> MakePlayerActorAdjacentInstructions()
        {
            if (this.triggered)
            {
                yield break;
            }
            yield return new Instruction_ProximityTrigger_SetTriggered(this, true);
            yield return new Instruction_Sound(Get.Sound_TriggerTriggered, new Vector3?(base.Parent.Position), 1f, 1f);
            if (this.Props.Turns == 0)
            {
                foreach (Instruction instruction in this.CountdownPassed())
                {
                    yield return instruction;
                }
                IEnumerator<Instruction> enumerator = null;
            }
            yield break;
            yield break;
        }

        private IEnumerable<Instruction> CountdownPassed()
        {
            Structure structure = base.Parent as Structure;
            if (structure != null && structure.UseEffects.Any)
            {
                foreach (Instruction instruction in InstructionSets_Usable.Use(null, structure, structure, null))
                {
                    yield return instruction;
                }
                IEnumerator<Instruction> enumerator = null;
            }
            yield break;
            yield break;
        }

        [Saved]
        private int turnsPassed;

        [Saved]
        private bool triggered;
    }
}