using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class Action_ErrorRecoveryAddSequence : Action
    {
        public ISequenceable Sequenceable
        {
            get
            {
                return this.sequenceable;
            }
        }

        public int SequenceToAdd
        {
            get
            {
                return this.sequenceToAdd;
            }
        }

        protected override int RandSeedPart
        {
            get
            {
                return 109808461;
            }
        }

        public override string Description
        {
            get
            {
                return base.Spec.DescriptionVerbose.Formatted(this.sequenceable);
            }
        }

        protected Action_ErrorRecoveryAddSequence()
        {
        }

        public Action_ErrorRecoveryAddSequence(ActionSpec spec, ISequenceable sequenceable, int sequenceToAdd)
            : base(spec)
        {
            this.sequenceable = sequenceable;
            this.sequenceToAdd = sequenceToAdd;
        }

        public override bool CanDo(bool ignoreActorState = false)
        {
            return true;
        }

        protected override bool CalculateConcernsPlayer()
        {
            return false;
        }

        protected override IEnumerable<Instruction> MakeInstructions()
        {
            yield return new Instruction_AddSequence(this.sequenceable, this.sequenceToAdd);
            yield return new Instruction_Immediate(delegate
            {
                string text = "Error recovery: Added ";
                string text2 = this.sequenceToAdd.ToString();
                string text3 = " sequence to ";
                ISequenceable sequenceable = this.sequenceable;
                Log.Message(text + text2 + text3 + ((sequenceable != null) ? sequenceable.ToString() : null));
            });
            yield break;
        }

        [Saved]
        private ISequenceable sequenceable;

        [Saved]
        private int sequenceToAdd;
    }
}