using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ricave.Core
{
    public class Condition_SpawnOnMove : Condition
    {
        public override GoodBadNeutral GoodBadNeutral
        {
            get
            {
                return GoodBadNeutral.Neutral;
            }
        }

        public EntitySpec EntitySpec
        {
            get
            {
                return this.entitySpec;
            }
        }

        public override string LabelBase
        {
            get
            {
                return base.Spec.LabelFormat.Formatted(this.entitySpec);
            }
        }

        protected Condition_SpawnOnMove()
        {
        }

        public Condition_SpawnOnMove(ConditionSpec spec)
            : base(spec)
        {
        }

        public override void CopyFieldsTo(Condition clone)
        {
            ((Condition_SpawnOnMove)clone).entitySpec = this.entitySpec;
        }

        public override IEnumerable<Instruction> MakeResolveConditionInstructions()
        {
            return Enumerable.Empty<Instruction>();
        }

        public override IEnumerable<Instruction> MakeAffectedActorMovedInstructions(Vector3Int prevPos)
        {
            if (SpawnPositionFinder.CanSpawnAt(this.entitySpec, prevPos, null, false, null))
            {
                Entity entity = Maker.Make(this.entitySpec, null, false, false, true);
                foreach (Instruction instruction in InstructionSets_Entity.Spawn(entity, prevPos, null))
                {
                    yield return instruction;
                }
                IEnumerator<Instruction> enumerator = null;
            }
            yield break;
            yield break;
        }

        [Saved]
        private EntitySpec entitySpec;
    }
}