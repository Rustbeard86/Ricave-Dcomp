using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class Action_BuyPrivateRoomStructure : Action
    {
        public EntitySpec StructureSpec
        {
            get
            {
                return this.structureSpec;
            }
        }

        protected override int RandSeedPart
        {
            get
            {
                return Calc.CombineHashes<int, int>(this.structureSpec.MyStableHash, 906841276);
            }
        }

        public override string Description
        {
            get
            {
                return base.Spec.DescriptionVerbose.Formatted(Get.MainActor, this.structureSpec);
            }
        }

        protected Action_BuyPrivateRoomStructure()
        {
        }

        public Action_BuyPrivateRoomStructure(ActionSpec spec, EntitySpec structureSpec)
            : base(spec)
        {
            this.structureSpec = structureSpec;
        }

        public override bool CanDo(bool ignoreActorState = false)
        {
            if (!Get.InLobby)
            {
                return false;
            }
            if (!ignoreActorState)
            {
                Actor mainActor = Get.MainActor;
                if (!mainActor.Spawned)
                {
                    return false;
                }
                if (!this.structureSpec.Structure.PrivateRoomPriceTag.CanAfford(mainActor))
                {
                    return false;
                }
            }
            return true;
        }

        protected override bool CalculateConcernsPlayer()
        {
            return true;
        }

        protected override IEnumerable<Instruction> MakeInstructions()
        {
            foreach (Instruction instruction in this.structureSpec.Structure.PrivateRoomPriceTag.MakePayInstructions(Get.MainActor, null))
            {
                yield return instruction;
            }
            IEnumerator<Instruction> enumerator = null;
            yield return new Instruction_Sound(Get.Sound_BoughtItem, null, 1f, 1f);
            yield return new Instruction_PlayLog("YouBoughtItem".Translate(this.structureSpec));
            yield return new Instruction_ChangePrivateRoomStructures(this.structureSpec, 1);
            yield break;
            yield break;
        }

        [Saved]
        private EntitySpec structureSpec;
    }
}