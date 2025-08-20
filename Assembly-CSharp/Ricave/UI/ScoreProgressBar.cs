using System;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public class ScoreProgressBar
    {
        private int StartScore
        {
            get
            {
                return Get.Progress.Score;
            }
        }

        private float StartPct
        {
            get
            {
                return (float)Get.Progress.ScoreSinceLeveling / (float)Get.Progress.ScoreBetweenLevels;
            }
        }

        private int StartLevel
        {
            get
            {
                return Get.Progress.ScoreLevel;
            }
        }

        private int EndScore
        {
            get
            {
                if (!Get.RunConfig.ProgressDisabled && !Get.InLobby)
                {
                    return Get.Progress.Score + Get.Player.Score;
                }
                return Get.Progress.Score;
            }
        }

        private float EndPct
        {
            get
            {
                return (float)(this.EndLevel - this.StartLevel) + (float)ScoreLevelUtility.GetScoreSinceLeveling(this.EndScore) / (float)(ScoreLevelUtility.GetTotalScoreRequired(this.EndLevel + 1) - ScoreLevelUtility.GetTotalScoreRequired(this.EndLevel));
            }
        }

        private int EndLevel
        {
            get
            {
                return ScoreLevelUtility.GetLevel(this.EndScore);
            }
        }

        public int CurAnimationLevel
        {
            get
            {
                return this.StartLevel + (int)this.curAnimationPct;
            }
        }

        public bool AnimationStarted
        {
            get
            {
                return this.animationStarted;
            }
        }

        public ScoreProgressBar(int GUIcachedID)
        {
            this.GUIcachedID = GUIcachedID;
            this.curAnimationPct = this.StartPct;
        }

        public void StartAnimation()
        {
            if (Get.Player.Score == 0 || this.EndScore == this.StartScore)
            {
                return;
            }
            this.animationStartTime = Clock.UnscaledTime;
            this.animationStarted = true;
            this.animationFinished = false;
            this.curAnimationPct = this.StartPct;
            CachedGUI.SetDirty(this.GUIcachedID);
        }

        public void Reset()
        {
            this.animationStarted = false;
            this.animationFinished = false;
            this.curAnimationPct = this.StartPct;
        }

        public void FinishInstantly()
        {
            this.animationStartTime = Clock.UnscaledTime;
            this.animationStarted = true;
            this.animationFinished = true;
            this.curAnimationPct = this.EndPct;
            CachedGUI.SetDirty(this.GUIcachedID);
        }

        public void Do(Rect rect, float alpha = 1f)
        {
            if (Event.current.type != EventType.Repaint)
            {
                return;
            }
            CircularProgressBarDrawer.Draw(rect, this.curAnimationPct % 1f, alpha, new int?(this.GUIcachedID));
            string text = this.CurAnimationLevel.ToStringCached();
            int levelFontSize = ScoreProgressBar.GetLevelFontSize(this.CurAnimationLevel, rect.height);
            ExpandingIconAnimation.Do(rect, CircularProgressBarDrawer.Texture, Color.white, this.levelUpStartTime, 0.5f, 0.6f, 0.55f);
            ExpandingLabelAnimation.Do(rect.center, text, levelFontSize, Color.white, this.levelUpStartTime, 0.5f, 0.6f, 0.55f, true);
            Widgets.FontSizeScalable = levelFontSize;
            Widgets.FontBold = true;
            GUI.color = new Color(1f, 1f, 1f, alpha);
            Widgets.LabelCentered(rect.center, text, true, null, null, false, false, false, null);
            GUI.color = Color.white;
            Widgets.FontBold = false;
            Widgets.ResetFontSize();
            if (this.animationStarted && !this.animationFinished && Event.current.type == EventType.Repaint)
            {
                int curAnimationLevel = this.CurAnimationLevel;
                float num = this.curAnimationPct;
                float num2 = ((this.EndLevel - this.StartLevel >= 2) ? 1.5f : 0.82f);
                this.curAnimationPct = Calc.StepTowards(this.curAnimationPct, this.EndPct, num2 * Clock.UnscaledDeltaTime);
                CachedGUI.SetDirty(this.GUIcachedID);
                if (Calc.RoundToInt(this.curAnimationPct / 0.05f) != Calc.RoundToInt(num / 0.05f))
                {
                    Get.Sound_ScoreTick.PlayOneShot(null, 1f, 1f);
                }
                if (curAnimationLevel != this.CurAnimationLevel)
                {
                    this.levelUpStartTime = Clock.UnscaledTime;
                    Get.Sound_ScoreLevelUp.PlayOneShot(null, 1f, 1f);
                }
                if (!this.animationFinished && this.curAnimationPct >= this.EndPct)
                {
                    this.animationFinished = true;
                }
            }
        }

        private static int GetLevelFontSize(int level, float rectHeight)
        {
            string text = level.ToStringCached();
            float num;
            if (text.Length <= 1)
            {
                num = 0.465f;
            }
            else if (text.Length == 2)
            {
                num = 0.27f;
            }
            else
            {
                num = 0.21f;
            }
            return Widgets.GetFontSizeToFitInHeight(rectHeight * num);
        }

        private int GUIcachedID;

        private float animationStartTime = -99999f;

        private bool animationStarted;

        private bool animationFinished;

        private float curAnimationPct;

        private float levelUpStartTime = -99999f;
    }
}