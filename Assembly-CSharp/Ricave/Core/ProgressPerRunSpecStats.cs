using System;

namespace Ricave.Core
{
    public class ProgressPerRunSpecStats
    {
        public double TotalPlaytime
        {
            get
            {
                return this.totalPlaytime;
            }
        }

        public int MaxScore
        {
            get
            {
                return this.maxScore;
            }
        }

        public int MaxFloorReached
        {
            get
            {
                return this.maxFloorReached;
            }
        }

        public int FloorReachedSum
        {
            get
            {
                return this.floorReachedSum;
            }
        }

        public float AverageFloorReached
        {
            get
            {
                if (this.runs != 0)
                {
                    return (float)this.floorReachedSum / (float)this.runs;
                }
                return 0f;
            }
        }

        public int Runs
        {
            get
            {
                return this.runs;
            }
        }

        public int TotalStardustCollected
        {
            get
            {
                return this.totalStardustCollected;
            }
        }

        public int TotalScoreGained
        {
            get
            {
                return this.totalScoreGained;
            }
        }

        public int KillCount
        {
            get
            {
                return this.killCount;
            }
        }

        public int MaxKillCount
        {
            get
            {
                return this.maxKillCount;
            }
        }

        public int ChestsOpened
        {
            get
            {
                return this.chestsOpened;
            }
        }

        public int RecipesUsed
        {
            get
            {
                return this.recipesUsed;
            }
        }

        public int MaxSpiritsSetFree
        {
            get
            {
                return this.maxSpiritsSetFree;
            }
        }

        public int TotalSpiritsSetFree
        {
            get
            {
                return this.totalSpiritsSetFree;
            }
        }

        public int Wins
        {
            get
            {
                return this.wins;
            }
        }

        public int Losses
        {
            get
            {
                return this.losses;
            }
        }

        public bool Completed
        {
            get
            {
                return this.completed;
            }
        }

        public DifficultySpec CompletedHighestDifficulty
        {
            get
            {
                return this.completedHighestDifficulty;
            }
        }

        public void ApplyCurrentRunProgress()
        {
            this.totalPlaytime += Get.Player.Playtime;
            this.maxFloorReached = Math.Max(this.maxFloorReached, Get.Floor);
            this.maxScore = Math.Max(this.maxScore, Get.Player.Score);
            this.floorReachedSum += Get.Floor;
            this.runs++;
            this.totalScoreGained += Get.Player.Score;
            this.totalStardustCollected += Get.ThisRunLobbyItems.Stardust;
            this.killCount += Get.KillCounter.KillCount;
            this.maxKillCount = Math.Max(this.maxKillCount, Get.KillCounter.KillCount);
            this.chestsOpened += Get.Player.ChestsOpened;
            this.recipesUsed += Get.Player.RecipesUsed;
            this.maxSpiritsSetFree = Math.Max(this.maxSpiritsSetFree, Get.Player.SpiritsSetFree);
            this.totalSpiritsSetFree += Get.Player.SpiritsSetFree;
            if (Get.RunInfo.FinishedRun)
            {
                this.completed = true;
                this.wins++;
                if (this.completedHighestDifficulty == null || Get.Difficulty.Order > this.completedHighestDifficulty.Order)
                {
                    this.completedHighestDifficulty = Get.Difficulty;
                    return;
                }
            }
            else if (!Get.RunInfo.ReturnedToLobby)
            {
                this.losses++;
            }
        }

        public void OnBackCompatSetCompleted()
        {
            this.completed = true;
        }

        [Saved]
        private double totalPlaytime;

        [Saved]
        private int maxScore;

        [Saved]
        private int maxFloorReached;

        [Saved]
        private int floorReachedSum;

        [Saved]
        private int runs;

        [Saved]
        private int totalStardustCollected;

        [Saved]
        private int totalScoreGained;

        [Saved]
        private int killCount;

        [Saved]
        private int maxKillCount;

        [Saved]
        private int chestsOpened;

        [Saved]
        private int recipesUsed;

        [Saved]
        private int maxSpiritsSetFree;

        [Saved]
        private int totalSpiritsSetFree;

        [Saved]
        private int wins;

        [Saved]
        private int losses;

        [Saved]
        private bool completed;

        [Saved]
        private DifficultySpec completedHighestDifficulty;
    }
}