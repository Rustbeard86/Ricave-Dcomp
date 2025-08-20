using System;
using System.Collections.Generic;
using System.Linq;

namespace Ricave.Core
{
    public class WorldEventsManager
    {
        private int DoRandomEventAfterTurns
        {
            get
            {
                return Rand.RangeInclusiveSeeded(250, 450, Calc.CombineHashes<int, int>(Get.WorldSeed, 813200948));
            }
        }

        public bool DidRandomEvent
        {
            get
            {
                return this.didRandomEvent;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.didRandomEvent = value;
            }
        }

        protected WorldEventsManager()
        {
        }

        public WorldEventsManager(World world)
        {
            this.world = world;
        }

        public IEnumerable<Instruction> MakeCheckDoEventInstructions()
        {
            if (!Get.RunSpec.HasRandomEvents || Get.WorldSpec.DisableRandomEvents || Get.DungeonModifier_NoRandomEvents.IsActiveAndAppliesToCurrentRun())
            {
                yield break;
            }
            if (!this.didRandomEvent && (Get.TurnManager.CurrentSequence - Get.TurnManager.InceptionSequence) / 12 >= this.DoRandomEventAfterTurns)
            {
                Rand.PushState(Calc.CombineHashes<int, int>(Get.WorldSeed, 613275996));
                WorldEventSpec eventSpec;
                bool flag = (from x in Get.Specs.GetAll<WorldEventSpec>()
                             where x.CanBeChosenRandomly && (x.AddSituation == null || !Get.WorldSituationsManager.AnyOfSpec(x.AddSituation))
                             select x).TryGetRandomElement<WorldEventSpec>(out eventSpec);
                Rand.PopState();
                if (flag)
                {
                    yield return new Instruction_SetWorldEventsManagerDidRandomEvent(true);
                    foreach (Instruction instruction in InstructionSets_Misc.DoWorldEvent(eventSpec))
                    {
                        yield return instruction;
                    }
                    IEnumerator<Instruction> enumerator = null;
                }
                eventSpec = null;
            }
            yield break;
            yield break;
        }

        [Saved]
        private World world;

        [Saved]
        private bool didRandomEvent;
    }
}