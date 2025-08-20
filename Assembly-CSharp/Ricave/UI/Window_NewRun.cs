using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public class Window_NewRun : Window
    {
        public Action<RunConfig> OnAccept { get; set; }

        public Window_NewRun(WindowSpec spec, int ID)
            : base(spec, ID)
        {
        }

        public override void OnOpen()
        {
            base.OnOpen();
            this.difficultiesInUIOrder = (from x in Get.Specs.GetAll<DifficultySpec>()
                                          orderby x.Order
                                          select x).ToList<DifficultySpec>();
            this.classesInUIOrder = (from x in Get.Specs.GetAll<ClassSpec>()
                                     orderby x.UIOrder
                                     select x).ToList<ClassSpec>();
            this.badModifiersInUIOrder = (from m in Get.Specs.GetAll<DungeonModifierSpec>()
                                          where m.Category == DungeonModifierCategory.Bad
                                          orderby m.UIOrder
                                          select m).ToList<DungeonModifierSpec>();
            this.goodModifiersInUIOrder = (from m in Get.Specs.GetAll<DungeonModifierSpec>()
                                           where m.Category == DungeonModifierCategory.Good
                                           orderby m.UIOrder
                                           select m).ToList<DungeonModifierSpec>();
            Window_NewRun.cachedTipForRun = null;
        }

        protected override void DoWindowContents(Rect inRect)
        {
            Window_NewRun.Page page = this.curPage;
            if (page == Window_NewRun.Page.Character)
            {
                this.DoCharacterPage(inRect);
                return;
            }
            if (page != Window_NewRun.Page.Dungeon)
            {
                return;
            }
            this.DoDungeonPage(inRect);
        }

        private void DoCharacterPage(Rect inRect)
        {
            Rect rect = base.MainArea(inRect);
            Rect rect2 = rect.LeftHalf().CutFromRight(base.Spec.Padding / 2f);
            Rect rect3 = rect.RightHalf().CutFromLeft(base.Spec.Padding / 2f);
            this.DoLeftColumn(rect2);
            this.DoRightColumn(rect3);
            if (base.DoBottomButton("Cancel".Translate(), inRect, 0, 2, true, null))
            {
                Get.WindowManager.Close(this, true);
            }
            if (base.DoBottomButton("StartNewRun".Translate(), inRect, 1, 2, true, null) || Get.KeyBinding_Accept.JustPressed)
            {
                this.curPage = Window_NewRun.Page.Dungeon;
            }
        }

        private void DoLeftColumn(Rect left)
        {
            float num = left.y;
            Widgets.Section("Character".Translate(), left.x, left.width, ref num, false, 7f);
            Rect rect = new Rect(left.x, num, 50f, 50f);
            GUI.color = Get.Entity_Player.IconColorAdjusted;
            GUI.DrawTexture(rect, Get.Entity_Player.IconAdjusted);
            GUI.color = Color.white;
            Widgets.LabelCenteredV(rect.RightCenter().MovedBy(5f, 0f), Get.Progress.PlayerName, true, null, null, false);
            ChooseablePartyMemberSpec lastChosenPartyMember = Get.Progress.LastChosenPartyMember;
            if (lastChosenPartyMember != null)
            {
                Rect rect2 = new Rect(rect.xMax + Widgets.CalcSize(Get.Progress.PlayerName).x + 10f, num, 50f, 50f);
                GUI.color = lastChosenPartyMember.ActorSpec.IconColorAdjusted;
                GUI.DrawTexture(rect2, lastChosenPartyMember.ActorSpec.IconAdjusted);
                GUI.color = Color.white;
                Widgets.LabelCenteredV(rect2.RightCenter().MovedBy(5f, 0f), lastChosenPartyMember.ActorSpec.LabelAdjustedCap, true, null, null, false);
            }
            num += 50f;
            num += 12f;
            this.DoClassSelection(left, ref num);
            num += 12f;
            Widgets.Section("Traits".Translate(), left.x, left.width, ref num, false, 7f);
            HashSet<TraitSpec> chosen = Get.TraitManager.Chosen;
            if (chosen.Count == 0)
            {
                GUI.color = new Color(0.7f, 0.7f, 0.7f);
                Widgets.Label(left.x, left.width, ref num, "None".Translate(), true);
                GUI.color = Color.white;
            }
            else
            {
                float num2 = left.x;
                foreach (TraitSpec traitSpec in chosen)
                {
                    Window_Traits.DoTrait(new Rect(num2, num, 50f, 50f), traitSpec, true, false);
                    num2 += 60f;
                }
                num += 50f;
            }
            num += 12f;
            Widgets.Section("ActiveQuests".Translate(), left.x, left.width, ref num, false, 7f);
            if (Get.QuestManager.ActiveQuests.Count == 0)
            {
                GUI.color = new Color(0.7f, 0.7f, 0.7f);
                Widgets.Label(left.x, left.width, ref num, "None".Translate(), true);
                GUI.color = Color.white;
            }
            else
            {
                Widgets.Label(left.x, left.width, ref num, this.GetActiveQuestsInfo(), true);
            }
            num += 12f;
            Widgets.Section("Boosts".Translate(), left.x, left.width, ref num, false, 7f);
            string singleUseBonusesInfo = this.GetSingleUseBonusesInfo();
            if (singleUseBonusesInfo.NullOrEmpty())
            {
                GUI.color = new Color(0.7f, 0.7f, 0.7f);
                Widgets.Label(left.x, left.width, ref num, "None".Translate(), true);
                GUI.color = Color.white;
                return;
            }
            Widgets.Label(left.x, left.width, ref num, singleUseBonusesInfo, true);
        }

        private void DoRightColumn(Rect right)
        {
            float num = right.y;
            Widgets.Section("Difficulty".Translate(), right.x, right.width, ref num, false, 7f);
            DifficultySpec difficultySpec = this.chosenDifficulty;
            for (int i = 0; i < this.difficultiesInUIOrder.Count; i++)
            {
                DifficultySpec difficultySpec2 = this.difficultiesInUIOrder[i];
                Rect rect;
                if (Widgets.Radio(new Vector2(right.x, num), this.chosenDifficulty == difficultySpec2, difficultySpec2.LabelCap, out rect))
                {
                    this.chosenDifficulty = difficultySpec2;
                }
                if (Mouse.Over(rect))
                {
                    difficultySpec = difficultySpec2;
                }
                num += 30f;
            }
            num += 6f;
            Widgets.FontSizeScalable = 30;
            Widgets.FontBold = true;
            Widgets.Label(right.CutFromTop(num), difficultySpec.LabelCap, true, null, null, false);
            num += 52f;
            Widgets.FontBold = false;
            Widgets.ResetFontSize();
            Widgets.Label(right.CutFromTop(num), difficultySpec.Details, true, null, null, false);
        }

        private void DoDungeonPage(Rect inRect)
        {
            Rect rect = base.MainArea(inRect);
            float num = rect.width * 0.25f;
            float num2 = rect.width - num - 15f;
            Rect rect2 = new Rect(rect.x, rect.y, num2, rect.height);
            this.DoDungeonSelection(rect2);
            Rect rect3 = new Rect(rect2.xMax + 15f, rect.y, num, rect.height);
            this.DoDungeonModifiers(rect3);
            if (SteamManager.Initialized && AchievementsUtility.CompletedAchievementsCount > 0)
            {
                Rect rect4 = inRect.BottomPart(45f).RightPart(45f);
                Rect rect5 = new Rect(rect4.x - 200f, rect4.y, 191f, rect4.height);
                Widgets.Align = TextAnchor.MiddleRight;
                GUI.color = Color.gray;
                Widgets.Label(rect5, "{0}/{1}".Formatted(AchievementsUtility.CompletedAchievementsCount.ToStringCached(), AchievementsUtility.AchievementsCount.ToStringCached()), true, null, null, false);
                GUI.color = Color.white;
                Widgets.ResetAlign();
                Rect rect6 = rect4;
                string text = "";
                bool flag = true;
                Texture2D achievementsIcon = Window_NewRun.AchievementsIcon;
                string description = Get.Window_Achievements.Description;
                if (Widgets.Button(rect6, text, flag, null, null, true, true, true, true, achievementsIcon, true, true, null, false, description, null))
                {
                    Get.WindowManager.Open(Get.Window_Achievements, true);
                }
            }
            if (base.DoBottomButton("Back".Translate(), inRect, 0, 1, true, null))
            {
                this.curPage = Window_NewRun.Page.Character;
            }
        }

        private void DoDungeonSelection(Rect inRect)
        {
            List<RunSpec> mainDungeonsInOrder = Get.Progress.MainDungeonsInOrder;
            Rect rect = inRect.CutFromTop(40f).CutFromBottom(15f);
            for (int i = 0; i < mainDungeonsInOrder.Count; i++)
            {
                RunSpec runSpec = mainDungeonsInOrder[i];
                Rect dungeonRect = this.GetDungeonRect(rect, i);
                if (i != mainDungeonsInOrder.Count - 1)
                {
                    Rect dungeonRect2 = this.GetDungeonRect(rect, i + 1);
                    GUIExtra.DrawLine(dungeonRect.center, dungeonRect2.center, new Color(0.35f, 0.35f, 0.35f), 4f);
                }
                bool flag = runSpec.IsUnlocked();
                Widgets.FontSizeScalable = 22;
                if (!flag)
                {
                    GUI.color = new Color(0.65f, 0.65f, 0.65f);
                }
                Widgets.LabelCentered(dungeonRect.TopCenter().MovedBy(0f, -27f), runSpec.LabelCap, true, null, null, false, false, false, null);
                GUI.color = Color.white;
                Widgets.ResetFontSize();
                GUIExtra.DrawRoundedRectBump(dungeonRect, flag ? Widgets.DefaultButtonColor : Widgets.DefaultButtonColor.Darker(0.05f), false, true, true, true, true, null);
                GUIExtra.DrawHighlightIfMouseover(dungeonRect, true, true, true, true, true);
                this.DoEnemyIcons(dungeonRect.ContractedBy(3f).CutFromBottom(7f), runSpec, flag);
                Widgets.Align = TextAnchor.LowerCenter;
                Widgets.Label(dungeonRect.CutFromBottom(2f), this.GetDungeonMainText(runSpec), true, null, null, false);
                Widgets.ResetAlign();
                this.DoBossIcons(dungeonRect, runSpec);
                if (Mouse.Over(dungeonRect))
                {
                    Get.Tooltips.RegisterTip(dungeonRect, Window_NewRun.GetTip(runSpec), null);
                }
                Rect rect2 = dungeonRect.ContractedToSquare().ContractedByPct(0.16f);
                if (Get.Progress.IsRunCompleted(runSpec))
                {
                    GUI.color = new Color(0.55f, 1f, 0.55f, 1f);
                    GUI.DrawTexture(rect2, Window_NewRun.CompletedIcon);
                    GUI.color = Color.white;
                }
                else if (!flag)
                {
                    GUI.color = new Color(1f, 1f, 1f, 1f);
                    GUI.DrawTexture(rect2, Window_NewRun.LockedIcon);
                    GUI.color = Color.white;
                }
                if (flag && Widgets.ButtonInvisible(dungeonRect, true, true))
                {
                    Get.Progress.LastChosenDifficulty = this.chosenDifficulty;
                    Get.Progress.LastChosenClass = this.chosenClass;
                    RunConfig runConfig;
                    if (runSpec == Get.Run_Main5 && !Get.Progress.GetRunStats(Get.Run_Cutscene).Completed)
                    {
                        runConfig = new RunConfig(Get.Run_Cutscene, Rand.Int, this.chosenDifficulty, this.chosenClass, "Current", null, false, null, false, false, null, null);
                    }
                    else
                    {
                        bool flag2;
                        if (Get.Progress.PetRatSatiation > 0)
                        {
                            Progress progress = Get.Progress;
                            int petRatSatiation = progress.PetRatSatiation;
                            progress.PetRatSatiation = petRatSatiation - 1;
                            flag2 = true;
                        }
                        else
                        {
                            flag2 = false;
                        }
                        runConfig = new RunConfig(runSpec, Rand.Int, this.chosenDifficulty, this.chosenClass, "Current", this.selectedModifiers, false, null, false, flag2, null, Get.Progress.LastChosenPartyMember);
                    }
                    this.OnAccept(runConfig);
                    Get.WindowManager.Close(this, true);
                }
            }
        }

        private void DoDungeonModifiers(Rect inRect)
        {
            Window_NewRun.<> c__DisplayClass28_0 CS$<> 8__locals1;
            CS$<> 8__locals1.<> 4__this = this;
            bool flag = Get.Progress.ScoreLevel < 2;
            if (flag)
            {
                Widgets.AbsorbClicks(inRect);
            }
            Widgets.FontSizeScalable = 17;
            Widgets.FontBold = true;
            Widgets.Label(inRect, "DungeonModifiersTitle".Translate(), true, null, null, false);
            Widgets.FontBold = false;
            Widgets.ResetFontSize();
            CS$<> 8__locals1.column = inRect.CutFromTop(39f);
            CS$<> 8__locals1.curY = CS$<> 8__locals1.column.y;
            for (int i = 0; i < this.badModifiersInUIOrder.Count; i++)
            {
                this.< DoDungeonModifiers > g__DoModifier | 28_0(this.badModifiersInUIOrder[i], false, ref CS$<> 8__locals1);
            }
            CS$<> 8__locals1.curY += 12f;
            for (int j = 0; j < this.goodModifiersInUIOrder.Count; j++)
            {
                this.< DoDungeonModifiers > g__DoModifier | 28_0(this.goodModifiersInUIOrder[j], true, ref CS$<> 8__locals1);
            }
            float num;
            float num2;
            DungeonModifiersUtility.GetStardustAndScoreFactors(this.selectedModifiers, out num, out num2, false);
            CS$<> 8__locals1.curY += 40f;
            Window_NewRun.< DoDungeonModifiers > g__SetGUIColorFromFactor | 28_1(num);
            Widgets.LabelCenteredV(new Vector2(CS$<> 8__locals1.column.x, CS$<> 8__locals1.curY), "{0}: {1}".Formatted("StardustFactor".Translate(), num.ToStringPercent(true)), true, null, null, false);
            CS$<> 8__locals1.curY += 20f;
            Window_NewRun.< DoDungeonModifiers > g__SetGUIColorFromFactor | 28_1(num2);
            Widgets.LabelCenteredV(new Vector2(CS$<> 8__locals1.column.x, CS$<> 8__locals1.curY), "{0}: {1}".Formatted("ScoreFactor".Translate(), num2.ToStringPercent(true)), true, null, null, false);
            GUI.color = Color.white;
            if (flag)
            {
                GUIExtra.DrawRoundedRect(inRect, new Color(0.08f, 0.08f, 0.08f, 0.74f), true, true, true, true, null);
                Widgets.FontBold = true;
                Widgets.FontSizeScalable = 20;
                Widgets.LabelCentered(inRect, "DungeonModifiersLocked".Translate(2), true, null, null);
                Widgets.ResetFontSize();
                Widgets.FontBold = false;
            }
        }

        private Rect GetDungeonRect(Rect mainRect, int i)
        {
            int num = i / 3;
            int num2 = i % 3;
            if (num % 2 == 1)
            {
                num2 = 3 - num2 - 1;
            }
            return mainRect.StackHorizontally(num2, 3, 170f).StackVertically(num, 3, 100f);
        }

        private string GetSingleUseBonusesInfo()
        {
            if (this.cachedSingleUseBonusesInfo != null)
            {
                return this.cachedSingleUseBonusesInfo;
            }
            StringBuilder stringBuilder = new StringBuilder();
            if (Get.Progress.NextRunRetainedExp != 0)
            {
                stringBuilder.Append("NextRunRetainedExp".Translate());
                stringBuilder.Append(": ");
                stringBuilder.AppendLine(Get.Progress.NextRunRetainedExp.ToStringCached());
            }
            if (Get.Progress.NextRunRetainedGold != 0)
            {
                stringBuilder.Append("NextRunRetainedGold".Translate());
                stringBuilder.Append(": ");
                stringBuilder.AppendLine(Get.Progress.NextRunRetainedGold.ToStringCached());
            }
            if (Get.Progress.PetRatSatiation > 0)
            {
                string text = ((Get.Progress.PetRatSatiation == 1) ? "PetRatOneRunLeft".Translate() : "PetRatRunsLeft".Translate(Get.Progress.PetRatSatiation));
                stringBuilder.AppendLine("{0} ({1})".Formatted(Get.Entity_PetRat.LabelCap, text));
            }
            foreach (UnlockableSpec unlockableSpec in Get.UnlockableManager.DirectlyUnlocked)
            {
                if (unlockableSpec.ResetAfterRun)
                {
                    stringBuilder.AppendLine(unlockableSpec.LabelCap);
                }
            }
            this.cachedSingleUseBonusesInfo = stringBuilder.ToString();
            return this.cachedSingleUseBonusesInfo;
        }

        private string GetActiveQuestsInfo()
        {
            if (this.cachedActiveQuestsInfo != null)
            {
                return this.cachedActiveQuestsInfo;
            }
            StringBuilder stringBuilder = new StringBuilder();
            List<QuestSpec> activeQuests = Get.QuestManager.ActiveQuests;
            for (int i = 0; i < activeQuests.Count; i++)
            {
                QuestSpec questSpec = activeQuests[i];
                if (i != 0)
                {
                    stringBuilder.AppendLine();
                }
                stringBuilder.Append(RichText.Yellow(RichText.Bold(questSpec.LabelCap)));
            }
            this.cachedActiveQuestsInfo = stringBuilder.ToString();
            return this.cachedActiveQuestsInfo;
        }

        public static string GetTip(RunSpec runSpec)
        {
            if (runSpec == Window_NewRun.cachedTipForRun)
            {
                return Window_NewRun.cachedTip;
            }
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(RichText.Bold(runSpec.LabelCap));
            if (runSpec.FloorCount != null)
            {
                stringBuilder.AppendLine();
                stringBuilder.AppendLine();
                stringBuilder.Append(RichText.Grayed("Floors".Translate()));
                stringBuilder.Append(RichText.Grayed(": "));
                stringBuilder.Append(runSpec.FloorCount.Value.ToStringCached());
            }
            if (runSpec.FloorCount == null)
            {
                stringBuilder.AppendLine();
            }
            stringBuilder.AppendLine();
            stringBuilder.Append(RichText.Grayed("RoomsPerFloor".Translate()));
            stringBuilder.Append(RichText.Grayed(": "));
            stringBuilder.Append(runSpec.RoomsPerFloor.ToStringCached());
            stringBuilder.AppendLine();
            stringBuilder.Append(RichText.Grayed("HasMiniBosses".Translate()));
            stringBuilder.Append(RichText.Grayed(": "));
            stringBuilder.Append(runSpec.HasMiniBosses.ToStringYesNo());
            stringBuilder.AppendLine();
            stringBuilder.Append(RichText.Grayed("HasShelters".Translate()));
            stringBuilder.Append(RichText.Grayed(": "));
            stringBuilder.Append(runSpec.HasShelters.ToStringYesNo());
            if (!runSpec.Enemies.NullOrEmpty<EntitySpec>())
            {
                stringBuilder.AppendLine();
                stringBuilder.AppendLine();
                stringBuilder.Append("Enemies".Translate());
                stringBuilder.Append(": ");
                foreach (EntitySpec entitySpec in runSpec.Enemies)
                {
                    bool flag = Get.Progress.SeenActorSpecs.Contains(entitySpec);
                    stringBuilder.AppendLine();
                    stringBuilder.Append("- ");
                    if (flag)
                    {
                        stringBuilder.Append(entitySpec.LabelCap);
                    }
                    else
                    {
                        stringBuilder.Append("UndiscoveredEnemy".Translate());
                    }
                }
            }
            if (!runSpec.IsUnlocked())
            {
                stringBuilder.AppendLine();
                stringBuilder.AppendLine();
                stringBuilder.Append(RichText.Grayed("Locked".Translate()));
            }
            else
            {
                bool flag2 = false;
                DifficultySpec highestCompletedDifficulty = Get.Progress.GetHighestCompletedDifficulty(runSpec);
                if (highestCompletedDifficulty != null)
                {
                    flag2 = true;
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine();
                    stringBuilder.Append(RichText.Grayed("CompletedOnDifficulty".Translate()));
                    stringBuilder.Append(RichText.Grayed(": "));
                    stringBuilder.Append(RichText.Grayed(RichText.Bold(highestCompletedDifficulty.LabelCap)));
                }
                if (runSpec.SpiritsCount != 0)
                {
                    if (!flag2)
                    {
                        stringBuilder.AppendLine();
                        flag2 = true;
                    }
                    int maxSpiritsSetFree = Get.Progress.GetRunStats(runSpec).MaxSpiritsSetFree;
                    stringBuilder.AppendLine();
                    stringBuilder.Append(RichText.Grayed("SpiritsSetFree".Translate()));
                    stringBuilder.Append(RichText.Grayed(": "));
                    stringBuilder.Append(RichText.Bold(maxSpiritsSetFree.ToStringCached()));
                    stringBuilder.Append(RichText.Bold(" / "));
                    stringBuilder.Append(RichText.Bold(runSpec.SpiritsCount.ToStringCached()));
                }
                if (runSpec.FloorCount != null)
                {
                    ProgressPerRunSpecStats runStats = Get.Progress.GetRunStats(runSpec);
                    if (runStats.Runs != 0)
                    {
                        if (!flag2)
                        {
                            stringBuilder.AppendLine();
                        }
                        stringBuilder.AppendLine();
                        stringBuilder.Append(RichText.LightGreen("RunWins".Translate()));
                        stringBuilder.Append(RichText.LightGreen(": "));
                        stringBuilder.Append(RichText.LightGreen(RichText.Bold(runStats.Wins.ToStringCached())));
                        stringBuilder.AppendLine();
                        stringBuilder.Append(RichText.LightRed("RunLosses".Translate()));
                        stringBuilder.Append(RichText.LightRed(": "));
                        stringBuilder.Append(RichText.LightRed(RichText.Bold(runStats.Losses.ToStringCached())));
                    }
                }
            }
            Window_NewRun.cachedTip = stringBuilder.ToString();
            Window_NewRun.cachedTipForRun = runSpec;
            return Window_NewRun.cachedTip;
        }

        private string GetDungeonMainText(RunSpec runSpec)
        {
            string text = "";
            if (runSpec.FloorCount != null)
            {
                text = "FloorCount".Translate(RichText.Bold(runSpec.FloorCount.Value.ToStringCached()));
            }
            return text;
        }

        private void DoEnemyIcons(Rect rect, RunSpec runSpec, bool unlocked)
        {
            List<EntitySpec> enemies = runSpec.Enemies;
            if (enemies.NullOrEmpty<EntitySpec>())
            {
                return;
            }
            int num = 0;
            foreach (Rect rect2 in RectUtility.GetPackedRects(rect, enemies.Count, 45f, 5f))
            {
                EntitySpec entitySpec = enemies[num];
                if (Get.Progress.SeenActorSpecs.Contains(entitySpec))
                {
                    GUI.color = entitySpec.IconColorAdjusted.MultipliedColor(unlocked ? 1f : 0.8f);
                    GUI.DrawTexture(rect2, entitySpec.IconAdjusted);
                }
                else
                {
                    GUI.color = Color.white.MultipliedColor(unlocked ? 1f : 0.8f);
                    GUI.DrawTexture(rect2, Window_NewRun.UndiscoveredEnemyIcon);
                }
                GUI.color = Color.white;
                num++;
            }
        }

        private void DoBossIcons(Rect buttonRect, RunSpec runSpec)
        {
            int? num = runSpec.FloorCount;
            int num2 = 5;
            bool flag = ((num.GetValueOrDefault() >= num2) & (num != null)) || runSpec.FloorCount == null;
            num = runSpec.FloorCount;
            num2 = 9;
            bool flag2 = ((num.GetValueOrDefault() >= num2) & (num != null)) || runSpec.FloorCount == null;
            int num3 = (flag ? 1 : 0) + (flag2 ? 1 : 0);
            if (num3 <= 0)
            {
                return;
            }
            int num4 = num3 * 30 + (num3 - 1) * 2;
            float num5 = buttonRect.center.x - (float)num4 / 2f;
            if (flag)
            {
                Window_NewRun.< DoBossIcons > g__DoBoss | 39_0(new Rect(num5, buttonRect.yMax + 1f, 30f, 30f), Get.Entity_Googon);
                num5 += 32f;
            }
            if (flag2)
            {
                Window_NewRun.< DoBossIcons > g__DoBoss | 39_0(new Rect(num5, buttonRect.yMax + 1f, 30f, 30f), Get.Entity_Demon);
                num5 += 32f;
            }
        }

        private void DoClassSelection(Rect left, ref float curY)
        {
            bool flag = Get.Progress.ScoreLevel < 4;
            Widgets.Section("Class".Translate(), left.x, left.width, ref curY, false, 7f);
            if (flag)
            {
                Widgets.AbsorbClicks(new Rect(left.x, curY, left.width, 60f));
            }
            float num = curY;
            float num2 = left.x + 3f;
            Rect rect = new Rect(num2, curY, 50f, 50f);
            if (this.chosenClass == null && !flag)
            {
                GUIExtra.DrawRoundedRect(rect.ExpandedBy(2f), Color.white, true, true, true, true, null);
            }
            string text = "NoClass".Translate().AppendedInDoubleNewLine("MaxFloorReachedWithoutClassTip".Translate(Get.Progress.MaxFloorReachedWithoutClass));
            Rect rect2 = rect;
            string text2 = "";
            bool flag2 = true;
            Texture2D texture2D = Window_NewRun.NoClassIcon;
            string text3 = text;
            Color? color = new Color?(ClassSpec.ClassColor);
            if (Widgets.Button(rect2, text2, flag2, null, null, true, true, true, true, texture2D, true, true, null, false, text3, color) && !flag)
            {
                this.chosenClass = null;
            }
            num2 += 60f;
            foreach (ClassSpec classSpec in this.classesInUIOrder)
            {
                Rect rect3 = new Rect(num2, curY, 50f, 50f);
                if (this.chosenClass == classSpec)
                {
                    GUIExtra.DrawRoundedRect(rect3.ExpandedBy(2f), Color.white, true, true, true, true, null);
                }
                Rect rect4 = rect3;
                string text4 = "";
                bool flag3 = true;
                texture2D = classSpec.Icon;
                color = new Color?(classSpec.IconColor);
                if (Widgets.Button(rect4, text4, flag3, null, null, true, true, true, true, texture2D, true, true, null, false, null, color) && !flag)
                {
                    this.chosenClass = classSpec;
                }
                Get.Tooltips.RegisterTip(rect3, classSpec, null, null);
                num2 += 60f;
            }
            curY += 50f;
            if (flag)
            {
                Rect rect5 = new Rect(left.x, num, left.width, curY - num);
                GUIExtra.DrawRoundedRect(rect5, new Color(0.08f, 0.08f, 0.08f, 0.74f), true, true, true, true, null);
                Widgets.FontBold = true;
                Widgets.FontSizeScalable = 20;
                Widgets.LabelCentered(rect5, "ClassesLocked".Translate(4), true, null, null);
                Widgets.ResetFontSize();
                Widgets.FontBold = false;
            }
        }

        [CompilerGenerated]
        private void <DoDungeonModifiers>g__DoModifier|28_0(DungeonModifierSpec mod, bool good, ref Window_NewRun.<>c__DisplayClass28_0 A_3)
		{
            Rect rect = new Rect(A_3.column.x, A_3.curY, A_3.column.width, 25f);
        bool flag = this.selectedModifiers.Contains(mod);
        bool flag2 = Widgets.CheckboxLabeled(rect, mod.LabelCap, flag, new Color?(good ? new Color(0.8f, 1f, 0.8f) : new Color(1f, 0.8f, 0.8f)), mod.Description);
			if (flag2 != flag)
			{
				if (flag2)
				{
					this.selectedModifiers.Add(mod);
				}
				else
				{
                    this.selectedModifiers.Remove(mod);
    }
}
A_3.curY += 30f;
		}

		[CompilerGenerated]
internal static void < DoDungeonModifiers > g__SetGUIColorFromFactor | 28_1(float factor)

        {
    if (Calc.Approximately(factor, 1f))
    {
        GUI.color = Color.white;
        return;
    }
    if (factor > 1f)
    {
        GUI.color = new Color(0.6f, 1f, 0.6f);
        return;
    }
    GUI.color = new Color(1f, 0.6f, 0.6f);
}

[CompilerGenerated]
internal static void < DoBossIcons > g__DoBoss | 39_0(Rect rect, EntitySpec bossSpec)

        {
    GUIExtra.DrawHighlightIfMouseover(rect, true, true, true, true, true);
    GUI.color = bossSpec.IconColorAdjusted;
    GUI.DrawTexture(rect, bossSpec.IconAdjusted);
    GUI.color = Color.white;
    Get.Tooltips.RegisterTip(rect, "NewRunScreenBossLabel".Translate(bossSpec.LabelCap), null);
}

private Window_NewRun.Page curPage;

private DifficultySpec chosenDifficulty = Get.Progress.LastChosenDifficulty ?? Get.Difficulty_Normal;

private ClassSpec chosenClass = Get.Progress.LastChosenClass;

private List<DifficultySpec> difficultiesInUIOrder;

private List<ClassSpec> classesInUIOrder;

private List<DungeonModifierSpec> badModifiersInUIOrder;

private List<DungeonModifierSpec> goodModifiersInUIOrder;

private List<DungeonModifierSpec> selectedModifiers = new List<DungeonModifierSpec>();

private static readonly Texture2D CompletedIcon = Assets.Get<Texture2D>("Textures/UI/Completed");

private static readonly Texture2D LockedIcon = Assets.Get<Texture2D>("Textures/UI/Lock");

private static readonly Texture2D AchievementsIcon = Assets.Get<Texture2D>("Textures/UI/Achievements");

private static readonly Texture2D NoClassIcon = Assets.Get<Texture2D>("Textures/UI/Classes/NoClass");

private static readonly Texture2D UndiscoveredEnemyIcon = Assets.Get<Texture2D>("Textures/UI/UnexploredRoom");

private const int DungeonModifiersUnlockedAtLevel = 2;

private const int ClassesUnlockedAtLevel = 4;

private string cachedSingleUseBonusesInfo;

private string cachedActiveQuestsInfo;

private static string cachedTip;

private static RunSpec cachedTipForRun;

private enum Page
{
    Character,

    Dungeon
}
	}
}