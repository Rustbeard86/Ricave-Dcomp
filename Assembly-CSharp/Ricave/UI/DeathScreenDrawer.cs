using System;
using System.Collections.Generic;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public class DeathScreenDrawer
    {
        public ScoreProgressBar ProgressBar
        {
            get
            {
                return this.progressBar;
            }
        }

        public bool ShouldShow
        {
            get
            {
                return (Get.MainActor != null && !Get.MainActor.Spawned && Get.MainActor.HP <= 0) || Get.RunInfo.ReturnedToLobby || Get.RunInfo.FinishedRun;
            }
        }

        public void OnGUI()
        {
            if (Event.current.type != EventType.Repaint && Event.current.type != EventType.KeyDown)
            {
                return;
            }
            if (!this.ShouldShow)
            {
                this.shown = false;
                this.progressBar.Reset();
                if (Get.ScreenFader.ShowYouDiedText)
                {
                    Widgets.FontSizeScalable = 54;
                    Widgets.FontBold = true;
                    GUI.color = new Color(1f, 0.8f, 0.8f);
                    Widgets.LabelCentered(Widgets.ScreenCenter.WithY(Widgets.VirtualHeight * 0.2f), "YouDied".Translate(), true, null, null, false, false, false, null);
                    GUI.color = Color.white;
                    Widgets.FontBold = false;
                    Widgets.ResetFontSize();
                }
                return;
            }
            if (!this.shown)
            {
                this.shown = true;
                this.startTime = Clock.UnscaledTime;
                CachedGUI.SetDirty(6);
            }
            if (Clock.UnscaledTime - this.startTime > 1f && !this.progressBar.AnimationStarted)
            {
                this.progressBar.StartAnimation();
            }
            float num = Widgets.VirtualHeight * 0.04f;
            float num2 = Calc.ResolveFadeIn(Clock.UnscaledTime - this.startTime, 1.5f);
            GUIExtra.DrawRect(Widgets.ScreenRect, new Color(0f, 0f, 0f, num2 * 0.65f));
            float num3 = Widgets.VirtualHeight * 0.15f;
            Rect rect = new Rect(Widgets.ScreenCenter.x - num3 / 2f, num, num3, num3);
            this.progressBar.Do(rect, num2);
            this.DoNextReward(new Rect(rect.x - 370f, rect.y, 370f, rect.height));
            float num4 = rect.y;
            this.DoStats(rect.xMax + 30f, ref num4);
            this.DoSpiritsSetFree(rect.xMax + 30f + 285f, rect.center.y);
            num4 += 29f;
            num4 = Math.Max(num4, rect.yMax + 30f);
            this.DoKills(num4);
            this.DoBossesSlain(num4);
            if (!Get.RunConfig.ProgressDisabled)
            {
                this.DoLobbyItemsCollected(num4);
            }
            bool flag = Clock.UnscaledTime - this.startTime > 1.2f;
            bool flag2 = !Get.RunConfig.IsDailyChallenge && Get.MainActor.HP <= 0;
            if (flag)
            {
                Widgets.FontSizeScalable = 19;
                if (flag2)
                {
                    Widgets.LabelCentered(Widgets.ScreenCenter.WithY(Widgets.VirtualHeight - 195f).MovedBy(-200f, 0f), "PressAcceptToContinueToLobby".Translate().FormattedKeyBindings(), true, null, null, false, false, false, null);
                    Widgets.LabelCentered(Widgets.ScreenCenter.WithY(Widgets.VirtualHeight - 195f).MovedBy(200f, 0f), "PressWaitToRestart".Translate().FormattedKeyBindings(), true, null, null, false, false, false, null);
                }
                else
                {
                    Widgets.LabelCentered(Widgets.ScreenCenter.WithY(Widgets.VirtualHeight - 195f), "PressAcceptToContinue".Translate().FormattedKeyBindings(), true, null, null, false, false, false, null);
                }
                Widgets.ResetFontSize();
            }
            float num5 = Calc.ResolveFadeInStayOut(Clock.UnscaledTime - this.startTime - 0.1f, 3.1f, 1.3f, 2.8f);
            if (num5 > 0f)
            {
                Widgets.FontSizeScalable = 54;
                Widgets.FontBold = true;
                if (Get.MainActor != null && Get.MainActor.HP <= 0)
                {
                    GUI.color = new Color(1f, 0.8f, 0.8f, num5);
                    Widgets.LabelCentered(Widgets.ScreenCenter, "YouDied".Translate(), true, null, null, false, false, false, null);
                }
                else if (Get.RunInfo.ReturnedToLobby)
                {
                    GUI.color = new Color(0.8f, 1f, 0.8f, num5);
                    Widgets.LabelCentered(Widgets.ScreenCenter, "YouEscaped".Translate(), true, null, null, false, false, false, null);
                }
                else if (Get.RunInfo.FinishedRun)
                {
                    GUI.color = new Color(0.8f, 1f, 0.8f, num5);
                    Widgets.LabelCentered(Widgets.ScreenCenter, "RunCompleted".Translate(), true, null, null, false, false, false, null);
                }
                GUI.color = Color.white;
                Widgets.FontBold = false;
                Widgets.ResetFontSize();
            }
            if (flag && !Get.UI.InventoryOpen && !Get.UI.IsEscapeMenuOpen && !Get.ScreenFader.AnyActionQueued && !Get.WindowManager.AnyWindowOpen && !Root.ChangingScene)
            {
                if (Get.KeyBinding_Accept.JustPressed)
                {
                    Get.ScreenFader.FadeOutAndExecute(delegate
                    {
                        Get.Progress.ApplyCurrentRunProgress();
                        if (Get.RunSpec == Get.Run_Tutorial)
                        {
                            Root.LoadPlayScene(RootOnSceneChanged.StartNewRun(new RunConfig(Get.Run_Tutorial, Rand.Int, Get.Difficulty_Normal, null, "Current", null, false, null, false, false, null, null)));
                            return;
                        }
                        Root.LoadPlayScene(RootOnSceneChanged.StartNewRun(new RunConfig(Get.Run_Lobby, Rand.Int, Get.Difficulty_Normal, null, "Current", null, false, null, false, false, null, null)));
                    }, null, true, true, false);
                }
                if (flag2 && Get.KeyBinding_Wait.JustPressed)
                {
                    Window_EscapeMenu.QuickRestart();
                }
            }
        }

        private void DoNextReward(Rect rect)
        {
            UnlockableSpec unlockableSpec = null;
            foreach (UnlockableSpec unlockableSpec2 in Get.Specs.GetAll<UnlockableSpec>())
            {
                if (!Get.UnlockableManager.IsDirectlyUnlocked(unlockableSpec2) && unlockableSpec2.AutoUnlockAtScoreLevel != null)
                {
                    int? num;
                    if (unlockableSpec != null)
                    {
                        int? autoUnlockAtScoreLevel = unlockableSpec.AutoUnlockAtScoreLevel;
                        num = unlockableSpec2.AutoUnlockAtScoreLevel;
                        if ((autoUnlockAtScoreLevel.GetValueOrDefault() < num.GetValueOrDefault()) & ((autoUnlockAtScoreLevel != null) & (num != null)))
                        {
                            continue;
                        }
                    }
                    num = unlockableSpec2.AutoUnlockAtScoreLevel;
                    int curAnimationLevel = this.progressBar.CurAnimationLevel;
                    if (!((num.GetValueOrDefault() <= curAnimationLevel) & (num != null)))
                    {
                        unlockableSpec = unlockableSpec2;
                    }
                }
            }
            if (unlockableSpec == null)
            {
                return;
            }
            GUI.color = new Color(0.88f, 0.88f, 0.88f, this.GetElementAlpha(1));
            Widgets.FontSizeScalable = 19;
            Widgets.LabelCenteredH(rect.TopCenter().MovedBy(0f, 10f), "NextRewardAtScoreLevel".Translate(unlockableSpec.AutoUnlockAtScoreLevel.Value), true, null, null, false);
            Widgets.ResetFontSize();
            GUI.color = Color.white;
            Rect rect2 = rect.CutFromTop(50f).CutFromBottom(10f).ContractedToSquare();
            GUIExtra.DrawHighlightIfMouseover(rect2, true, true, true, true, true);
            Get.Tooltips.RegisterTip(rect2, unlockableSpec, null, null);
            GUI.color = new Color(1f, 1f, 1f, this.GetElementAlpha(1));
            GUI.DrawTexture(rect2, unlockableSpec.Icon);
            GUI.color = Color.white;
        }

        private void DoStats(float x, ref float curY)
        {
            string text = Get.Floor.ToStringCached();
            if (Get.Floor > Get.Progress.MaxFloorReached)
            {
                text = text.AppendedWithSpace(RichText.Yellow("({0})".Formatted("NewRecord".Translate())));
            }
            string text2 = Get.Player.Score.ToStringCached();
            if (Get.Player.Score > Get.Progress.MaxScore)
            {
                text2 = text2.AppendedWithSpace(RichText.Yellow("({0})".Formatted("NewRecord".Translate())));
            }
            string text3 = Get.KillCounter.KillCount.ToStringCached();
            if (Get.KillCounter.KillCount > Get.Progress.MaxKillCount)
            {
                text3 = text3.AppendedWithSpace(RichText.Yellow("({0})".Formatted("NewRecord".Translate())));
            }
            this.DoStat("Score".Translate(), text2, x, ref curY, 0);
            this.DoStat("FloorReached".Translate(), text, x, ref curY, 1);
            this.DoStat("KillCount".Translate(), text3, x, ref curY, 2);
            this.DoStat("CombatRating".Translate(), CombatRatingUtility.CombatRating.GetLabel(), x, ref curY, 3);
            this.DoStat("Playtime".Translate(), "{0} / {1}".Formatted(Get.Player.Playtime.SecondsToShortTimeRoughStr(), StringUtility.TurnsString(Get.TurnManager.CurrentSequence / 12)), x, ref curY, 4);
        }

        private void DoStat(string label, string value, float x, ref float curY, int elementIndex)
        {
            label = label.Concatenated(": ");
            GUI.color = new Color(1f, 1f, 1f, this.GetElementAlpha(elementIndex));
            Widgets.FontSizeScalable = 25;
            Widgets.Label(new Rect(x, curY, 500f, 500f), RichText.Grayed(label).Concatenated(RichText.Bold(value)), true, null, null, false);
            curY += 34f;
            Widgets.ResetFontSize();
            GUI.color = Color.white;
        }

        private void DoSpiritsSetFree(float x, float y)
        {
            int spiritsCount = Get.RunSpec.SpiritsCount;
            if (spiritsCount <= 0)
            {
                return;
            }
            for (int i = 0; i < spiritsCount; i++)
            {
                Rect rect = new Rect(x + (float)(i * 90), y - 40f, 80f, 80f);
                bool flag = Get.Player.SpiritsSetFree > i;
                GUI.color = Get.Entity_Spirit.IconColorAdjusted.MultipliedColor(flag ? 1f : 0.5f).WithAlphaFactor(this.GetElementAlpha(1));
                GUI.DrawTexture(rect, Get.Entity_Spirit.IconAdjusted);
                GUI.color = Color.white;
                Widgets.DoCheck(rect.ContractedByPct(0.05f), flag, new Color(1f, 1f, 1f, 0.5f * this.GetElementAlpha(1)));
            }
        }

        private void DoHeader(Vector2 pos, string label, ref float curY)
        {
            GUI.color = new Color(0.75f, 0.75f, 0.75f, this.GetElementAlpha(5));
            Widgets.FontSizeScalable = 25;
            Widgets.LabelCentered(pos, label, true, null, null, false, false, false, null);
            Widgets.ResetFontSize();
            GUI.color = Color.white;
            curY += 49f;
        }

        private void DoKills(float y)
        {
            Vector2 vector = new Vector2(Widgets.ScreenCenter.x - 455f, y);
            float num = y;
            this.DoHeader(vector, "Kills".Translate(), ref num);
            bool flag = false;
            int num2 = 6;
            foreach (ValueTuple<EntitySpec, int> valueTuple in Get.KillCounter.KillCountsOrderedByExp)
            {
                if (valueTuple.Item2 > 0)
                {
                    flag = true;
                    this.DoIconAndLabel(vector.x, ref num, valueTuple.Item1.IconAdjusted, valueTuple.Item1.IconColorAdjusted, "{0} x{1}".Formatted(valueTuple.Item1.LabelAdjustedCap, valueTuple.Item2.ToStringCached()), num2);
                    num2++;
                    if (num >= Widgets.VirtualHeight - 300f)
                    {
                        this.DoAndMoreLabel(vector.x, ref num, num2);
                        num2++;
                        break;
                    }
                }
            }
            if (!flag)
            {
                this.DoNoneLabel(vector.x, ref num, num2);
            }
        }

        private void DoBossesSlain(float y)
        {
            Vector2 vector = Widgets.ScreenCenter.WithY(y);
            float num = y;
            this.DoHeader(vector, "BossesSlain".Translate(), ref num);
            bool flag = false;
            int num2 = 6;
            foreach (TotalKillCounter.KilledBoss killedBoss in Get.KillCounter.KilledBossesOrderedByKillTime)
            {
                flag = true;
                this.DoIconAndLabel(vector.x, ref num, killedBoss.ActorSpec.IconAdjusted, killedBoss.ActorSpec.IconColorAdjusted, killedBoss.Name, num2);
                num2++;
                if (num >= Widgets.VirtualHeight - 300f)
                {
                    this.DoAndMoreLabel(vector.x, ref num, num2);
                    num2++;
                    break;
                }
            }
            if (!flag)
            {
                this.DoNoneLabel(vector.x, ref num, num2);
            }
        }

        private void DoLobbyItemsCollected(float y)
        {
            Vector2 vector = new Vector2(Widgets.ScreenCenter.x + 455f, y);
            float num = y;
            this.DoHeader(vector, "LobbyItemsCollected".Translate(), ref num);
            bool flag = false;
            int num2 = 6;
            DeathScreenDrawer.tmpLobbyItems.Clear();
            DeathScreenDrawer.tmpLobbyItems.AddRange(Get.ThisRunLobbyItems.LobbyItems);
            DeathScreenDrawer.tmpLobbyItems.AddRange(Get.ThisRunPrivateRoomStructures.CollectedStructures);
            foreach (KeyValuePair<EntitySpec, int> keyValuePair in DeathScreenDrawer.tmpLobbyItems)
            {
                ItemProps item = keyValuePair.Key.Item;
                if ((item == null || !item.HideLobbyItem) && keyValuePair.Value > 0)
                {
                    flag = true;
                    this.DoIconAndLabel(vector.x, ref num, keyValuePair.Key.IconAdjusted, keyValuePair.Key.IconColorAdjusted, "{0} x{1}".Formatted(keyValuePair.Key.LabelAdjustedCap, keyValuePair.Value.ToStringCached()), num2);
                    num2++;
                    if (num >= Widgets.VirtualHeight - 300f)
                    {
                        this.DoAndMoreLabel(vector.x, ref num, num2);
                        num2++;
                        break;
                    }
                }
            }
            if (!flag)
            {
                this.DoNoneLabel(vector.x, ref num, num2);
            }
        }

        private void DoIconAndLabel(float x, ref float curY, Texture2D icon, Color iconColor, string label, int elementIndex)
        {
            Widgets.FontSizeScalable = 21;
            Widgets.FontBold = true;
            float x2 = Widgets.CalcSize(label).x;
            float num = 58f + x2;
            Rect rect = RectUtility.CenteredAt(x, curY, num, 48f);
            float elementAlpha = this.GetElementAlpha(elementIndex);
            Rect rect2 = rect.LeftPart(48f);
            GUI.color = iconColor.WithAlphaFactor(elementAlpha);
            GUI.DrawTexture(rect2, icon);
            GUI.color = Color.white;
            GUI.color = new Color(1f, 1f, 1f, elementAlpha);
            Widgets.LabelCentered(rect.RightPart(x2).center, label, true, null, null, false, false, false, null);
            GUI.color = Color.white;
            Widgets.ResetFontSize();
            Widgets.FontBold = false;
            curY += 48f;
        }

        private void DoNoneLabel(float x, ref float curY, int elementIndex)
        {
            Widgets.FontSizeScalable = 21;
            GUI.color = new Color(0.75f, 0.75f, 0.75f, this.GetElementAlpha(elementIndex));
            Widgets.LabelCentered(new Vector2(x, curY), "({0})".Formatted("NoneLower".Translate()), true, null, null, false, false, false, null);
            GUI.color = Color.white;
            Widgets.ResetFontSize();
            curY += 48f;
        }

        private void DoAndMoreLabel(float x, ref float curY, int elementIndex)
        {
            Widgets.FontSizeScalable = 21;
            GUI.color = new Color(0.75f, 0.75f, 0.75f, this.GetElementAlpha(elementIndex));
            Widgets.LabelCentered(new Vector2(x, curY), "({0})".Formatted("AndMore".Translate()), true, null, null, false, false, false, null);
            GUI.color = Color.white;
            Widgets.ResetFontSize();
            curY += 48f;
        }

        private float GetElementAlpha(int elementIndex)
        {
            return Calc.ResolveFadeIn(Clock.UnscaledTime - this.startTime - (float)elementIndex * 0.2f - 1f, 0.3f);
        }

        private ScoreProgressBar progressBar = new ScoreProgressBar(6);

        private float startTime = -99999f;

        private bool shown;

        private static List<KeyValuePair<EntitySpec, int>> tmpLobbyItems = new List<KeyValuePair<EntitySpec, int>>();
    }
}