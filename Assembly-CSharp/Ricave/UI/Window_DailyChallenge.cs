using System;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public class Window_DailyChallenge : Window
    {
        public Window_DailyChallenge(WindowSpec spec, int ID)
            : base(spec, ID)
        {
        }

        protected override void DoWindowContents(Rect inRect)
        {
            Rect rect = base.MainArea(inRect);
            ValueTuple<int, string> currentDailyChallengeRunSeed = DailyChallengeUtility.CurrentDailyChallengeRunSeed;
            int item = currentDailyChallengeRunSeed.Item1;
            string item2 = currentDailyChallengeRunSeed.Item2;
            int? lastDailyChallengeRunSeed = Get.Progress.LastDailyChallengeRunSeed;
            int num = item;
            bool flag = (lastDailyChallengeRunSeed.GetValueOrDefault() == num) & (lastDailyChallengeRunSeed != null);
            float num2 = 0f;
            Widgets.FontSizeScalable = 20;
            GUI.color = new Color(1f, 1f, 0.6f);
            Widgets.Label(inRect.x, inRect.width, ref num2, "DailyChallengeTitle".Translate(), true);
            GUI.color = Color.white;
            Widgets.ResetFontSize();
            num2 += 15f;
            Widgets.Label(inRect.x, inRect.width, ref num2, "DailyChallengeExplanation".Translate(RichText.Stardust(StringUtility.StardustString(20))), true);
            num2 += 15f;
            ValueTuple<int, int, int> timeToNextDailyChallenge = DailyChallengeUtility.TimeToNextDailyChallenge;
            Widgets.FontSizeScalable = 20;
            GUI.color = new Color(0.75f, 0.75f, 0.75f);
            Widgets.Label(inRect.x, inRect.width, ref num2, "DailyChallengeTimeLeft".Translate(timeToNextDailyChallenge.Item1.ToString("00"), timeToNextDailyChallenge.Item2.ToString("00"), timeToNextDailyChallenge.Item3.ToString("00")), true);
            GUI.color = Color.white;
            Widgets.ResetFontSize();
            Widgets.FontSizeScalable = 20;
            GUI.color = new Color(0.75f, 0.75f, 0.75f);
            Widgets.Label(inRect.x, inRect.width, ref num2, "AllDailyChallengesBestScore".Translate(Get.Progress.AllDailyChallengesBestScore), true);
            GUI.color = Color.white;
            Widgets.ResetFontSize();
            num2 += 30f;
            Rect rect2 = RectUtility.CenteredAt(new Vector2(rect.center.x - 160f, rect.height - 50f), 300f, 80f);
            if (Get.Progress.ScoreLevel < 4)
            {
                Widgets.DisabledButton(rect2, "DailyChallengeRequiresLevel".Translate(4));
            }
            else if (Get.ModManager.ActiveMods.Count != 0)
            {
                Widgets.DisabledButton(rect2, "DailyChallengeModsActive".Translate());
            }
            else if (flag)
            {
                Widgets.DisabledButton(rect2, "DailyChallengeAlreadyPlayed".Translate(Get.Progress.LastDailyChallengeScore));
            }
            else if (Widgets.Button(rect2, "{0} ({1})".Formatted("DailyChallengeStart".Translate(), item2), true, null, null, true, true, true, true, null, true, true, null, false, null, null) && !Root.ChangingScene && !Get.ScreenFader.AnyActionQueued)
            {
                RunConfig runConfig = new RunConfig(Get.Run_Main9, item, Get.Difficulty_Normal, DailyChallengeUtility.GetClassForSeed(item), "Current", null, true, null, false, false, item2, DailyChallengeUtility.GetChooseablePartyMemberForSeed(item));
                Get.ScreenFader.FadeOutAndExecute(delegate
                {
                    Root.LoadPlayScene(RootOnSceneChanged.StartNewRun(runConfig));
                }, new float?(2.5f), false, true, false);
                Get.WindowManager.Close(this, true);
            }
            if (Widgets.Button(RectUtility.CenteredAt(new Vector2(rect.center.x + 160f, rect.height - 50f), 300f, 80f), "ViewDailyLeaderboard".Translate(), true, null, null, true, true, true, true, null, true, true, null, false, null, null))
            {
                ((Window_DailyChallengeLeaderboard)Get.WindowManager.Open(Get.Window_DailyChallengeLeaderboard, true)).Init(item2);
            }
            if (base.DoBottomButton("Cancel".Translate(), inRect, 0, 1, true, null))
            {
                Get.WindowManager.Close(this, true);
            }
        }
    }
}