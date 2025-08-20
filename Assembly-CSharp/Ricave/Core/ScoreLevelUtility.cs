using System;

namespace Ricave.Core
{
    public static class ScoreLevelUtility
    {
        public static int GetLevel(int score)
        {
            int num = 1;
            while (score >= ScoreLevelUtility.GetTotalScoreRequired(num))
            {
                num++;
            }
            return num - 1;
        }

        public static int GetTotalScoreRequired(int level)
        {
            return (level - 1) * (level + 13) * 10;
        }

        public static int GetScoreSinceLeveling(int score)
        {
            return score - ScoreLevelUtility.GetTotalScoreRequired(ScoreLevelUtility.GetLevel(score));
        }
    }
}