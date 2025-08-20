using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ricave.Core
{
    public class Condition_SeeRangeOffset : Condition
    {
        public override GoodBadNeutral GoodBadNeutral
        {
            get
            {
                if (this.Offset > 0)
                {
                    return GoodBadNeutral.Good;
                }
                if (this.Offset >= 0)
                {
                    return GoodBadNeutral.Neutral;
                }
                return GoodBadNeutral.Bad;
            }
        }

        public int Offset
        {
            get
            {
                return this.offset;
            }
        }

        public override string LabelBase
        {
            get
            {
                return base.Spec.LabelFormat.Formatted(this.offset.ToStringOffset(true));
            }
        }

        public override int SeeRangeOffset
        {
            get
            {
                return this.offset;
            }
        }

        protected Condition_SeeRangeOffset()
        {
        }

        public Condition_SeeRangeOffset(ConditionSpec spec)
            : base(spec)
        {
        }

        public Condition_SeeRangeOffset(ConditionSpec spec, int offset, int turnsLeft = 0)
            : base(spec, turnsLeft)
        {
            this.offset = offset;
        }

        public override IEnumerable<Instruction> MakeResolveConditionInstructions()
        {
            return Enumerable.Empty<Instruction>();
        }

        public override IEnumerable<Instruction> MakeOnNewlyAffectingActorInstructions()
        {
            foreach (Instruction instruction in base.MakeOnNewlyAffectingActorInstructions())
            {
                yield return instruction;
            }
            IEnumerator<Instruction> enumerator = null;
            foreach (Instruction instruction2 in Condition_SeeRangeOffset.SeeRangeChangedInstructions(base.AffectedActor))
            {
                yield return instruction2;
            }
            enumerator = null;
            yield break;
            yield break;
        }

        public override IEnumerable<Instruction> MakeOnNoLongerAffectingActorInstructions(Actor actor)
        {
            foreach (Instruction instruction in base.MakeOnNoLongerAffectingActorInstructions(actor))
            {
                yield return instruction;
            }
            IEnumerator<Instruction> enumerator = null;
            foreach (Instruction instruction2 in Condition_SeeRangeOffset.SeeRangeChangedInstructions(actor))
            {
                yield return instruction2;
            }
            enumerator = null;
            yield break;
            yield break;
        }

        public static IEnumerable<Instruction> SeeRangeChangedInstructions(Actor actor)
        {
            if (actor.IsNowControlledActor)
            {
                List<Vector3Int> cellsToNewlyUnfogAt = Get.FogOfWar.GetCellsToNewlyUnfogAt(actor.Position);
                if (cellsToNewlyUnfogAt.Count != 0)
                {
                    yield return new Instruction_Unfog(cellsToNewlyUnfogAt);
                }
            }
            yield break;
        }

        public override void CopyFieldsTo(Condition clone)
        {
            ((Condition_SeeRangeOffset)clone).offset = this.offset;
        }

        [Saved]
        private int offset;
    }
}