using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ricave.Core
{
    public class RetractableComp : EntityComp
    {
        public new RetractableCompProps Props
        {
            get
            {
                return (RetractableCompProps)base.Props;
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

        public bool Active
        {
            get
            {
                int num = this.Props.TurnsNotRetracted + this.Props.TurnsRetracted;
                return this.turnsPassed % num < this.Props.TurnsNotRetracted;
            }
        }

        protected RetractableComp()
        {
        }

        public RetractableComp(Entity parent)
            : base(parent)
        {
            if (!(parent is SequenceableStructure))
            {
                Log.Warning("RetractableComp requires entity of type SequenceableStructure. Entity: " + ((parent != null) ? parent.ToString() : null), false);
            }
        }

        public override IEnumerable<Instruction> MakeResolveStructureInstructions()
        {
            bool prevActive = this.Active;
            yield return new Instruction_Retractable_ChangeTurnsPassed(this, 1);
            if (prevActive != this.Active)
            {
                bool flag = Get.NowControlledActor.Spawned && base.Parent.Position.GetGridDistance(Get.NowControlledActor.Position) <= 4;
                if (prevActive)
                {
                    if (flag && this.Props.RetractSound != null)
                    {
                        yield return new Instruction_Sound(this.Props.RetractSound, new Vector3?(base.Parent.Position), 1f, 1f);
                    }
                }
                else
                {
                    if (flag && this.Props.AppearSound != null)
                    {
                        yield return new Instruction_Sound(this.Props.AppearSound, new Vector3?(base.Parent.Position), 1f, 1f);
                    }
                    Structure structure = base.Parent as Structure;
                    if (structure != null)
                    {
                        foreach (Instruction instruction in InstructionSets_Usable.CheckAutoUseStructureOnActors(structure))
                        {
                            yield return instruction;
                        }
                        IEnumerator<Instruction> enumerator = null;
                    }
                }
            }
            yield break;
            yield break;
        }

        [Saved]
        private int turnsPassed;
    }
}