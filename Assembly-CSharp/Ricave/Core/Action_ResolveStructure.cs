using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class Action_ResolveStructure : Action
    {
        public SequenceableStructure Structure
        {
            get
            {
                return this.structure;
            }
        }

        protected override int RandSeedPart
        {
            get
            {
                return Calc.CombineHashes<int, int>(this.structure.MyStableHash, 287091539);
            }
        }

        public override string Description
        {
            get
            {
                return base.Spec.DescriptionVerbose.Formatted(this.structure);
            }
        }

        protected Action_ResolveStructure()
        {
        }

        public Action_ResolveStructure(ActionSpec spec, SequenceableStructure structure)
            : base(spec)
        {
            this.structure = structure;
        }

        public override bool CanDo(bool ignoreActorState = false)
        {
            return this.structure.Spawned;
        }

        protected override bool CalculateConcernsPlayer()
        {
            return false;
        }

        protected override IEnumerable<Instruction> MakeInstructions()
        {
            foreach (EntityComp entityComp in this.structure.AllComps)
            {
                IEnumerable<Instruction> enumerable = null;
                try
                {
                    enumerable = entityComp.MakeResolveStructureInstructions();
                }
                catch (Exception ex)
                {
                    Log.Error("Error in EntityComp.MakeResolveStructureInstructions().", ex);
                }
                if (enumerable != null)
                {
                    foreach (Instruction instruction in enumerable)
                    {
                        yield return instruction;
                    }
                    IEnumerator<Instruction> enumerator2 = null;
                }
            }
            List<EntityComp>.Enumerator enumerator = default(List<EntityComp>.Enumerator);
            yield return new Instruction_AddSequence(this.structure, 12);
            yield break;
            yield break;
        }

        [Saved]
        private SequenceableStructure structure;
    }
}