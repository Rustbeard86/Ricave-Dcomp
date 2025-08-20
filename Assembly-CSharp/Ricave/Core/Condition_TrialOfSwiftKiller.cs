using System;
using System.Collections.Generic;
using System.Linq;

namespace Ricave.Core
{
    public class Condition_TrialOfSwiftKiller : Condition
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
                return base.Spec.LabelFormat.Formatted(this.enemiesKilled, 4);
            }
        }

        public override string Description
        {
            get
            {
                return base.Spec.DescriptionFormat.Formatted(4, 100);
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

        protected Condition_TrialOfSwiftKiller()
        {
        }

        public Condition_TrialOfSwiftKiller(ConditionSpec spec)
            : base(spec, 100)
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
                yield return new Instruction_ChangeTrialOfSwiftKillerEnemiesKilled(this, 1);
                if (this.enemiesKilled >= 4)
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

        public override void CopyFieldsTo(Condition clone)
        {
            ((Condition_TrialOfSwiftKiller)clone).enemiesKilled = this.enemiesKilled;
        }

        public static IEnumerable<Instruction> TrialCompletedInstructions(Condition trial)
        {
            Actor actor = trial.AffectedActor;
            foreach (Instruction instruction in InstructionSets_Misc.RemoveCondition(trial, false, false))
            {
                yield return instruction;
            }
            IEnumerator<Instruction> enumerator = null;
            Rand.PushState(Calc.CombineHashes<int, int, int>(Get.WorldSeed, trial.MyStableHash, 917461998));
            Item reward = ItemGenerator.Reward(true);
            Rand.PopState();
            foreach (Instruction instruction2 in InstructionSets_Actor.AddToInventoryOrSpawnNear(actor, reward))
            {
                yield return instruction2;
            }
            enumerator = null;
            if (actor.IsPlayerParty || actor.IsNowControlledActor)
            {
                yield return new Instruction_PlayLog("TrialCompleted".Translate(reward));
                yield return new Instruction_Sound(Get.Sound_TrialCompleted, null, 1f, 1f);
                foreach (Instruction instruction3 in InstructionSets_Misc.GainScore(50, "ScoreGain_TrialCompleted".Translate(), true, true, true))
                {
                    yield return instruction3;
                }
                enumerator = null;
                if (!Get.Achievement_CompleteTrial.IsCompleted)
                {
                    yield return new Instruction_CompleteAchievement(Get.Achievement_CompleteTrial);
                }
            }
            yield break;
            yield break;
        }

        [Saved]
        private int enemiesKilled;

        private const int EnemiesToKill = 4;

        private const int DurationTurns = 100;
    }
}