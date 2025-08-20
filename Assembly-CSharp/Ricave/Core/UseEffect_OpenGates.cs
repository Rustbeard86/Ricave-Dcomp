using System;
using System.Collections.Generic;
using System.Linq;
using Ricave.UI;
using UnityEngine;

namespace Ricave.Core
{
    public class UseEffect_OpenGates : UseEffect
    {
        protected UseEffect_OpenGates()
        {
        }

        public UseEffect_OpenGates(UseEffectSpec spec)
            : base(spec, 1f, 0, null, false)
        {
        }

        public override void CopyFieldsTo(UseEffect clone)
        {
            ((UseEffect_OpenGates)clone).setEscapingMode = this.setEscapingMode;
        }

        public override IEnumerable<Instruction> MakeUseInstructions(Actor user, IUsable usable, Target target, Target originalTarget, BodyPart targetBodyPart)
        {
            bool playedSoundNearby = false;
            foreach (Entity gate in Get.World.GetEntitiesOfSpec(Get.Entity_Gate).Concat<Entity>(Get.World.GetEntitiesOfSpec(Get.Entity_GateReinforced)).ToTemporaryList<Entity>())
            {
                if (!playedSoundNearby)
                {
                    yield return new Instruction_Sound(Get.Sound_OpenGate, new Vector3?(gate.Position), 1f, 1f);
                    if (gate.Position.GetGridDistance(Get.NowControlledActor.Position) <= 4)
                    {
                        playedSoundNearby = true;
                    }
                }
                foreach (Instruction instruction in InstructionSets_Entity.DeSpawn(gate, true))
                {
                    yield return instruction;
                }
                IEnumerator<Instruction> enumerator2 = null;
                gate = null;
            }
            List<Entity>.Enumerator enumerator = default(List<Entity>.Enumerator);
            foreach (Entity entity in Get.World.GetEntitiesOfSpec(Get.Entity_Lever).ToTemporaryList<Entity>())
            {
                foreach (Instruction instruction2 in InstructionSets_Entity.DeSpawn(entity, false))
                {
                    yield return instruction2;
                }
                IEnumerator<Instruction> enumerator2 = null;
            }
            enumerator = default(List<Entity>.Enumerator);
            yield return new Instruction_PlayLog("GateOpens".Translate(RichText.Label(Get.Entity_Gate, false)));
            if (this.setEscapingMode && Get.RunSpec != Get.Run_Tutorial && Get.PlaceSpec != Get.Place_GoogonsLair && Get.PlaceSpec != Get.Place_DemonsLair)
            {
                yield return new Instruction_SetEscapingMode(true);
            }
            yield break;
            yield break;
        }

        [Saved]
        private bool setEscapingMode;
    }
}