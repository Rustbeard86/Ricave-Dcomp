using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class Condition_HatMissing : Condition
    {
        public override GoodBadNeutral GoodBadNeutral
        {
            get
            {
                return GoodBadNeutral.Bad;
            }
        }

        public override bool ImmuneToFire
        {
            get
            {
                return true;
            }
        }

        protected Condition_HatMissing()
        {
        }

        public Condition_HatMissing(ConditionSpec spec)
            : base(spec)
        {
        }

        public override void CopyFieldsTo(Condition clone)
        {
        }

        public override IEnumerable<Instruction> MakeResolveConditionInstructions()
        {
            Actor affectedActor = base.AffectedActor;
            if (SpawnPositionFinder.CanSpawnAt(Get.Entity_Fire, affectedActor.Position, null, false, null))
            {
                Entity entity = Maker.Make(Get.Entity_Fire, null, false, false, true);
                foreach (Instruction instruction in InstructionSets_Entity.Spawn(entity, affectedActor.Position, null))
                {
                    yield return instruction;
                }
                IEnumerator<Instruction> enumerator = null;
            }
            yield break;
            yield break;
        }
    }
}