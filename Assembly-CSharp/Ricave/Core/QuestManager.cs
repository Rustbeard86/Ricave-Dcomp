using System;
using System.Collections.Generic;
using System.Linq;

namespace Ricave.Core
{
    public class QuestManager
    {
        public List<QuestSpec> CompletedQuests
        {
            get
            {
                return this.completedQuests;
            }
        }

        public List<QuestSpec> ActiveQuests
        {
            get
            {
                return this.activeQuests;
            }
        }

        public IEnumerable<QuestSpec> AllVisibleQuests
        {
            get
            {
                return from x in Get.Specs.GetAll<QuestSpec>()
                       where x.Visible
                       orderby x.IsMainQuestline descending
                       select x;
            }
        }

        public int VisibleQuestsCount
        {
            get
            {
                int num = 0;
                using (List<QuestSpec>.Enumerator enumerator = Get.Specs.GetAll<QuestSpec>().GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        if (enumerator.Current.Visible)
                        {
                            num++;
                        }
                    }
                }
                return num;
            }
        }

        public int AvailableToTakeQuestsCount
        {
            get
            {
                int num = 0;
                foreach (QuestSpec questSpec in Get.Specs.GetAll<QuestSpec>())
                {
                    if (questSpec.Visible && !questSpec.IsCompleted() && !questSpec.IsActive())
                    {
                        num++;
                    }
                }
                return num;
            }
        }

        public int HistoricalQuestsCount
        {
            get
            {
                int num = 0;
                using (List<QuestSpec>.Enumerator enumerator = this.completedQuests.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        if (enumerator.Current.IsCompletedAndClaimed())
                        {
                            num++;
                        }
                    }
                }
                return num;
            }
        }

        public int ClaimableQuestsCount
        {
            get
            {
                int num = 0;
                using (List<QuestSpec>.Enumerator enumerator = this.completedQuests.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        if (!enumerator.Current.IsCompletedAndClaimed())
                        {
                            num++;
                        }
                    }
                }
                return num;
            }
        }

        public bool IsCompleted(QuestSpec questSpec)
        {
            return this.completedQuests.Contains(questSpec);
        }

        public bool IsActive(QuestSpec questSpec)
        {
            return (Get.InMainMenu || Get.InLobby || !Get.RunConfig.IsDailyChallenge) && this.activeQuests.Contains(questSpec);
        }

        public bool IsRewardClaimed(QuestSpec questSpec)
        {
            return this.claimedReward.Contains(questSpec);
        }

        public void SetActive(QuestSpec questSpec)
        {
            if (questSpec == null)
            {
                Log.Error("Tried to set null quest as active.", false);
                return;
            }
            if (this.IsActive(questSpec))
            {
                Log.Error("Tried to set an already active quest as active.", false);
                return;
            }
            if (this.IsCompleted(questSpec))
            {
                Log.Error("Tried to set a completed quest as active.", false);
                return;
            }
            if (!questSpec.Visible)
            {
                Log.Error("Tried to set unavailable quest as active.", false);
                return;
            }
            this.activeQuests.Add(questSpec);
        }

        public void SetInactive(QuestSpec questSpec)
        {
            if (questSpec == null)
            {
                Log.Error("Tried to set null quest as inactive.", false);
                return;
            }
            if (!this.IsActive(questSpec))
            {
                Log.Error("Tried to set an already inactive quest as inactive.", false);
                return;
            }
            this.activeQuests.Remove(questSpec);
        }

        public void Complete(QuestSpec questSpec)
        {
            if (questSpec == null)
            {
                Log.Error("Tried to complete null quest.", false);
                return;
            }
            if (this.IsCompleted(questSpec))
            {
                Log.Error("Tried to complete an already completed quest.", false);
                return;
            }
            if (!this.IsActive(questSpec))
            {
                Log.Error("Tried to complete an inactive quest.", false);
                return;
            }
            if (Get.RunConfig.ProgressDisabled)
            {
                return;
            }
            this.activeQuests.Remove(questSpec);
            this.completedQuests.Add(questSpec);
        }

        public void ClaimReward(QuestSpec questSpec)
        {
            if (questSpec == null)
            {
                Log.Error("Tried to claim reward for null quest.", false);
                return;
            }
            if (this.IsRewardClaimed(questSpec))
            {
                Log.Error("Tried to claim reward for a quest but the reward was already claimed.", false);
                return;
            }
            if (!this.IsCompleted(questSpec))
            {
                Log.Error("Tried to claim reward for a quest which was not completed.", false);
                return;
            }
            this.claimedReward.Add(questSpec);
            Get.Progress.OnQuestRewardClaimed(questSpec);
        }

        public void Apply(ThisRunCompletedQuests completedQuests)
        {
            foreach (QuestSpec questSpec in completedQuests.CompletedQuests)
            {
                this.Complete(questSpec);
            }
        }

        [Saved(Default.New, true)]
        private List<QuestSpec> completedQuests = new List<QuestSpec>();

        [Saved(Default.New, true)]
        private List<QuestSpec> activeQuests = new List<QuestSpec>();

        [Saved(Default.New, true)]
        private List<QuestSpec> claimedReward = new List<QuestSpec>();

        public const int MemoryPiece1Floor = 3;

        public const int MemoryPiece2Floor = 4;

        public const int MemoryPiece3Floor = 6;

        public const int MemoryPiece4Floor = 8;

        public const int NightmareLordFloor = 1;

        public const int RatInfestationFloor = 2;

        public const int PhantomSwarmFloor = 3;

        public const int MoonlightMedallionsFloor = 2;

        public const int MoonlightMedallions = 4;

        public const int HypnorakFloor = 3;

        public const int PhantasmosFloor = 4;

        public const int RelicFloor = 4;

        public const int LobbyShopkeeperRescueFloor = 2;
    }
}