using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class ThisRunCompletedQuests
    {
        public List<QuestSpec> CompletedQuests
        {
            get
            {
                return this.completedQuests;
            }
        }

        public bool IsMarkedCompleted(QuestSpec questSpec)
        {
            return this.completedQuests.Contains(questSpec);
        }

        public void MarkCompleted(QuestSpec questSpec)
        {
            if (questSpec == null)
            {
                Log.Error("Tried to mark null QuestSpec as completed.", false);
                return;
            }
            if (this.completedQuests.Contains(questSpec))
            {
                Log.Error("Tried to mark the same QuestSpec as completed twice.", false);
                return;
            }
            this.completedQuests.Add(questSpec);
        }

        public void MarkNotCompleted(QuestSpec questSpec)
        {
            if (questSpec == null)
            {
                Log.Error("Tried to mark null QuestSpec as not completed.", false);
                return;
            }
            if (!this.completedQuests.Contains(questSpec))
            {
                Log.Error("Tried to mark the same QuestSpec as not completed twice.", false);
                return;
            }
            this.completedQuests.Remove(questSpec);
        }

        [Saved(Default.New, true)]
        private List<QuestSpec> completedQuests = new List<QuestSpec>();
    }
}