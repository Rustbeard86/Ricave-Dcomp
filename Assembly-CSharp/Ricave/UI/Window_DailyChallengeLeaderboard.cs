using System;
using System.Collections.Generic;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public class Window_DailyChallengeLeaderboard : Window
    {
        public Window_DailyChallengeLeaderboard(WindowSpec spec, int ID)
            : base(spec, ID)
        {
        }

        public void Init(string dateString)
        {
            this.dateString = dateString;
            this.isLoading = true;
            this.loadingStartTime = Time.realtimeSinceStartup;
            this.LoadLeaderboardData();
        }

        private void LoadLeaderboardData()
        {
            this.leaderboardEntries = DailyChallengeUtility.GetLeaderboard(this.dateString, false);
            this.friendsLeaderboardEntries = DailyChallengeUtility.GetLeaderboard(this.dateString, true);
            this.myGlobalRank = DailyChallengeUtility.GetMyLeaderboardPlace(this.dateString, false);
            this.myFriendsRank = DailyChallengeUtility.GetMyLeaderboardPlace(this.dateString, true);
            this.isLoading = false;
        }

        protected override void DoWindowContents(Rect inRect)
        {
            Rect rect = base.MainArea(inRect);
            Widgets.FontSizeScalable = 20;
            float num = 0f;
            Widgets.Label(rect.x, rect.width, ref num, this.dateString, true);
            Widgets.ResetFontSize();
            num += 10f;
            Widgets.Tabs(new Rect(rect.x, num, rect.width, 32f), this.tabs, ref this.currentTab, true, null, null);
            num += 42f;
            if (!this.isLoading)
            {
                int num2 = ((this.currentTab == 1) ? this.myFriendsRank : this.myGlobalRank);
                if (num2 > 0)
                {
                    GUI.color = new Color(0.6f, 1f, 0.6f);
                    Widgets.Label(rect.x, rect.width, ref num, "YourLeaderboardRank".Translate(num2), true);
                    GUI.color = Color.white;
                    num += 5f;
                }
            }
            Rect rect2 = new Rect(rect.x, num, rect.width, rect.yMax - num);
            if (this.isLoading)
            {
                if (Time.realtimeSinceStartup - this.loadingStartTime > 10f)
                {
                    GUI.color = Color.gray;
                    Widgets.Align = TextAnchor.MiddleCenter;
                    Widgets.Label(rect2, "LeaderboardLoadTimeout".Translate(), true, null, null, false);
                    Widgets.ResetAlign();
                    GUI.color = Color.white;
                }
                else
                {
                    GUI.color = Color.gray;
                    Widgets.Align = TextAnchor.MiddleCenter;
                    Widgets.Label(rect2, "LoadingLeaderboard".Translate(), true, null, null, false);
                    Widgets.ResetAlign();
                    GUI.color = Color.white;
                }
            }
            else
            {
                this.DoLeaderboardList(rect2);
            }
            if (base.DoBottomButton("Close".Translate(), inRect, 0, 1, true, null))
            {
                Get.WindowManager.Close(this, true);
            }
        }

        private void DoLeaderboardList(Rect rect)
        {
            ValueTuple<string, int>[] array = ((this.currentTab == 1) ? this.friendsLeaderboardEntries : this.leaderboardEntries);
            if (array == null || array.Length == 0)
            {
                GUI.color = Color.gray;
                Widgets.Align = TextAnchor.MiddleCenter;
                Widgets.Label(rect, (this.currentTab == 1) ? "NoFriendsPlayed".Translate() : "NoLeaderboardData".Translate(), true, null, null, false);
                Widgets.ResetAlign();
                GUI.color = Color.white;
                return;
            }
            Rect rect2 = Widgets.BeginScrollView(rect, null);
            for (int i = 0; i < array.Length; i++)
            {
                Rect rect3 = new Rect(0f, (float)i * 35f, rect2.width, 35f);
                if (Widgets.VisibleInScrollView(rect3))
                {
                    this.DoLeaderboardRow(rect3, i + 1, array[i].Item1, array[i].Item2, i);
                }
            }
            Widgets.EndScrollView(rect, (float)array.Length * 35f, false, false);
        }

        private void DoLeaderboardRow(Rect rect, int rank, string playerName, int score, int index)
        {
            if (index % 2 == 1)
            {
                Widgets.AltRowHighlight(rect);
            }
            GUIExtra.DrawHighlightIfMouseover(rect, true, true, true, true, true);
            Rect rect2 = rect.LeftPart(60f);
            Widgets.FontSizeScalable = 16;
            if (rank == 1)
            {
                GUI.color = new Color(1f, 0.84f, 0f);
            }
            else if (rank == 2)
            {
                GUI.color = new Color(0.75f, 0.75f, 0.75f);
            }
            else if (rank == 3)
            {
                GUI.color = new Color(0.8f, 0.5f, 0.2f);
            }
            Widgets.LabelCentered(rect2, "#{0}".Formatted(rank), true, null, null);
            GUI.color = Color.white;
            Widgets.ResetFontSize();
            Widgets.LabelCenteredV(rect.CutFromLeft(65f).RightPart(rect.width - 180f).LeftCenter(), playerName.TruncateToLength(50), true, null, null, false);
            Rect rect3 = rect.RightPart(100f);
            Widgets.FontSizeScalable = 16;
            GUI.color = new Color(0.6f, 1f, 0.6f);
            Widgets.LabelCentered(rect3, score.ToStringCached(), true, null, null);
            GUI.color = Color.white;
            Widgets.ResetFontSize();
        }

        private ValueTuple<string, int>[] leaderboardEntries;

        private ValueTuple<string, int>[] friendsLeaderboardEntries;

        private int myGlobalRank = -1;

        private int myFriendsRank = -1;

        private int currentTab;

        private List<string> tabs = new List<string>
        {
            "GlobalLeaderboard".Translate(),
            "FriendsLeaderboard".Translate()
        };

        private string dateString;

        private bool isLoading = true;

        private float loadingStartTime;

        private const float RowHeight = 35f;

        private const float TabHeight = 32f;

        private const float LoadingTimeout = 10f;
    }
}