using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class WorldEventSpecBehavior
    {
        public WorldEventSpec WorldEventSpec
        {
            get
            {
                return this.worldEventSpec;
            }
            set
            {
                this.worldEventSpec = value;
            }
        }

        public WorldEventSpecBehavior(WorldEventSpec worldEventSpec)
        {
            this.worldEventSpec = worldEventSpec;
        }

        protected WorldEventSpecBehavior()
        {
        }

        public virtual IEnumerable<Instruction> MakeEventInstructions()
        {
            if (this.worldEventSpec.AddSituation != null)
            {
                WorldSituation worldSituation = Maker.Make(this.worldEventSpec.AddSituation);
                foreach (Instruction instruction in InstructionSets_Misc.AddWorldSituation(worldSituation))
                {
                    yield return instruction;
                }
                IEnumerator<Instruction> enumerator = null;
            }
            yield break;
            yield break;
        }

        private WorldEventSpec worldEventSpec;
    }
}