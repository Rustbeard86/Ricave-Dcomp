using System;

namespace Ricave.Core
{
    public class Instruction_ScoreGainEffect : Instruction
    {
        public string Label
        {
            get
            {
                return this.label;
            }
        }

        public int Score
        {
            get
            {
                return this.score;
            }
        }

        protected Instruction_ScoreGainEffect()
        {
        }

        public Instruction_ScoreGainEffect(string label, int score)
        {
            this.label = label;
            this.score = score;
        }

        protected override void DoImpl()
        {
            this.entryID = Get.ScoreGainEffects.Add(this.label, this.score);
        }

        protected override void UndoImpl()
        {
            Get.ScoreGainEffects.TryRemove(this.entryID);
        }

        [Saved]
        private string label;

        [Saved]
        private int score;

        [Saved]
        private int entryID;
    }
}