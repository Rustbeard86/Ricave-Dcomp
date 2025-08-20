using System;
using System.Collections.Generic;
using System.Linq;

namespace Ricave.Core
{
    public class Condition_TrialOfUnblemished : Condition
    {
        public override GoodBadNeutral GoodBadNeutral
        {
            get
            {
                return GoodBadNeutral.Neutral;
            }
        }

        public override string LabelBase
        {
            get
            {
                if (base.AffectedActor == null)
                {
                    return base.LabelBase;
                }
                return base.Spec.LabelFormat.Formatted(this.enemiesKilled, 2);
            }
        }

        public override string Description
        {
            get
            {
                return base.Spec.DescriptionFormat.Formatted(2);
            }
        }

        public int EnemiesKilled
        {
            get
            {
                return this.enemiesKilled;
            }
            set
            {
                Instruction.ThrowIfNotExecuting();
                this.enemiesKilled = value;
            }
        }

        protected Condition_TrialOfUnblemished()
        {
        }

        public Condition_TrialOfUnblemished(ConditionSpec spec)
            : base(spec)
        {
        }

        public override IEnumerable<Instruction> MakeResolveConditionInstructions()
        {
            return Enumerable.Empty<Instruction>();
        }

        public override IEnumerable<Instruction> MakeOtherActorDestroyedInstructions(Actor otherActor)
        {
            if (otherActor.KilledExperience != 0)
            {
                yield return new Instruction_ChangeTrialOfUnblemishedEnemiesKilled(this, 1);
                if (this.enemiesKilled >= 2)
                {
                    foreach (Instruction instruction in Condition_TrialOfSwiftKiller.TrialCompletedInstructions(this))
                    {
                        yield return instruction;
                    }
                    IEnumerator<Instruction> enumerator = null;
                }
            }
            yield break;
            yield break;
        }

        public override IEnumerable<Instruction> MakeAffectedActorLostHPInstructions(int damage, bool onBodyPart)
        {
            if (damage > 0)
            {
                Actor affectedActor = base.AffectedActor;
                foreach (Instruction instruction in InstructionSets_Misc.RemoveCondition(this, false, false))
                {
                    yield return instruction;
                }
                IEnumerator<Instruction> enumerator = null;
                if (affectedActor.IsPlayerParty || affectedActor.IsNowControlledActor)
                {
                    yield return new Instruction_PlayLog("TrialFailed".Translate());
                    yield return new Instruction_Sound(Get.Sound_TrialFailed, null, 1f, 1f);
                }
                affectedActor = null;
            }
            yield break;
            yield break;
        }

        public override void CopyFieldsTo(Condition clone)
        {
            ((Condition_TrialOfUnblemished)clone).enemiesKilled = this.enemiesKilled;
        }

        [Saved]
        private int enemiesKilled;

        private const int EnemiesToKill = 2;
    }
}