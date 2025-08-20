using System;
using System.Collections.Generic;
using Ricave.UI;
using UnityEngine;

namespace Ricave.Core
{
    public class FireSpreaderComp : EntityComp
    {
        public new FireSpreaderCompProps Props
        {
            get
            {
                return (FireSpreaderCompProps)base.Props;
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
                if (this.turnsPassed >= this.Props.SpreadAfterTurns)
                {
                    return null;
                }
                return "SpreadsIn".Translate(RichText.Turns(StringUtility.TurnsString(this.Props.SpreadAfterTurns - this.turnsPassed)));
            }
        }

        protected FireSpreaderComp()
        {
        }

        public FireSpreaderComp(Entity parent)
            : base(parent)
        {
            if (!(parent is SequenceableStructure))
            {
                Log.Warning("FireSpreaderComp requires entity of type SequenceableStructure. Entity: " + ((parent != null) ? parent.ToString() : null), false);
            }
        }

        public override IEnumerable<Instruction> MakeResolveStructureInstructions()
        {
            bool spreadsNow = this.turnsPassed == this.Props.SpreadAfterTurns - 1;
            yield return new Instruction_FireSpreader_ChangeTurnsPassed(this, 1);
            if (spreadsNow)
            {
                foreach (Vector3Int vector3Int in base.Parent.Position.AdjacentCellsAndInside())
                {
                    if (vector3Int.InBounds())
                    {
                        foreach (Entity entity in Get.World.GetEntitiesAt(vector3Int).ToTemporaryList<Entity>())
                        {
                            Structure structure = entity as Structure;
                            if (structure != null && entity.Spec.Structure.Flammable)
                            {
                                foreach (Instruction instruction in UseEffect_SetOnFire.TrySetStructureOnFireInstructions(structure, null))
                                {
                                    yield return instruction;
                                }
                                IEnumerator<Instruction> enumerator3 = null;
                            }
                        }
                        List<Entity>.Enumerator enumerator2 = default(List<Entity>.Enumerator);
                    }
                }
                IEnumerator<Vector3Int> enumerator = null;
            }
            yield break;
            yield break;
        }

        [Saved]
        private int turnsPassed;
    }
}