using System;
using System.Collections.Generic;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public class Window_Stats : Window
    {
        public Window_Stats(WindowSpec spec, int ID)
            : base(spec, ID)
        {
        }

        public override void OnOpen()
        {
            base.OnOpen();
            this.actorSpecsInOrder.Clear();
            foreach (EntitySpec entitySpec in Get.Specs.GetAll<EntitySpec>())
            {
                if (entitySpec.IsActor)
                {
                    this.actorSpecsInOrder.Add(entitySpec);
                }
            }
            this.actorSpecsInOrder.StableSort<EntitySpec, string>((EntitySpec x) => x.SpecID);
            this.actorSpecsInOrder.StableSort<EntitySpec, int>((EntitySpec x) => x.Actor.GenerateMinFloor);
            this.actorSpecsInOrder.StableSort<EntitySpec, int>((EntitySpec x) => x.Actor.KilledExperience);
        }

        protected override void DoWindowContents(Rect inRect)
        {
            Rect rect = base.MainArea(inRect);
            float num = 0f;
            Rect rect2 = Widgets.BeginScrollView(rect, new int?(712197330));
            if (this.DoStatButton("PlayerName".Translate(), Get.Progress.PlayerName, ref num, rect2.width, null))
            {
                Get.WindowManager.Open(Get.Window_ChangePlayerName, true);
            }
            this.DoDivider(ref num, rect2.width);
            this.DoStat("Runs".Translate(), Get.Progress.Runs.ToStringCached(), ref num, rect2.width);
            this.DoStat("Playtime".Translate(), Get.Progress.TotalPlaytime.SecondsToTimeStr(), ref num, rect2.width);
            this.DoStat("MaxFloorReached".Translate(), Get.Progress.MaxFloorReached.ToStringCached(), ref num, rect2.width);
            this.DoStat("AverageFloorReached".Translate(), Get.Progress.AverageFloorReached.ToStringCached(), ref num, rect2.width);
            this.DoStat("Score".Translate(), Get.Progress.Score.ToStringCached(), ref num, rect2.width);
            this.DoStat("QuestsCompleted".Translate(), Get.QuestManager.CompletedQuests.Count.ToStringCached(), ref num, rect2.width);
            this.DoStat("NotesCollected".Translate(), "{0} / {1}".Formatted(Get.Progress.CollectedNoteSpecs.Count, Get.Specs.GetAll<NoteSpec>().Count), ref num, rect2.width);
            this.DoStat("ChestsOpened".Translate(), Get.Progress.TotalChestsOpened.ToStringCached(), ref num, rect2.width);
            this.DoStat("RecipesUsed".Translate(), Get.Progress.TotalRecipesUsed.ToStringCached(), ref num, rect2.width);
            this.DoStat("SpiritsSetFree".Translate(), Get.Progress.TotalSpiritsSetFree.ToStringCached(), ref num, rect2.width);
            this.DoStat("KillCount".Translate(), Get.Progress.TotalKillCounter.KillCount.ToStringCached(), ref num, rect2.width);
            foreach (EntitySpec entitySpec in this.actorSpecsInOrder)
            {
                int killCount = Get.Progress.TotalKillCounter.GetKillCount(entitySpec);
                if (killCount != 0)
                {
                    this.DoStat("  {0}".Formatted(entitySpec.LabelAdjustedCap), killCount.ToStringCached(), ref num, rect2.width);
                }
            }
            this.DoStat("BossKillCount".Translate(), Get.Progress.TotalKillCounter.BossKillCount.ToStringCached(), ref num, rect2.width);
            foreach (EntitySpec entitySpec2 in this.actorSpecsInOrder)
            {
                int bossKillCount = Get.Progress.TotalKillCounter.GetBossKillCount(entitySpec2);
                if (bossKillCount != 0)
                {
                    this.DoStat("  {0}".Formatted(entitySpec2.LabelAdjustedCap), bossKillCount.ToStringCached(), ref num, rect2.width);
                }
            }
            Widgets.EndScrollView(rect, num, false, false);
            if (base.DoBottomButton("Close".Translate(), inRect, 0, 1, true, null))
            {
                Get.WindowManager.Close(this, true);
            }
        }

        private void DoStat(string label, string value, ref float y, float width)
        {
            Rect rect = new Rect(0f, y, width, 30f);
            if (Widgets.VisibleInScrollView(rect))
            {
                GUIExtra.DrawHighlightIfMouseover(rect, true, true, true, true, true);
                Widgets.Label(rect, label, true, null, null, false);
                Widgets.Align = TextAnchor.UpperRight;
                Widgets.Label(rect.CutFromRight(10f), value, true, null, null, false);
                Widgets.ResetAlign();
            }
            y += 30f;
        }

        private bool DoStatButton(string label, string value, ref float y, float width, string tip = null)
        {
            Rect rect = new Rect(0f, y, width, 30f);
            bool flag;
            if (Widgets.VisibleInScrollView(rect))
            {
                Widgets.Label(rect, label, true, null, null, false);
                Rect rect2 = rect.CutFromRight(10f).RightPart(Widgets.CalcSize(value).x + 22f);
                flag = Widgets.Button(rect2, value, true, null, null, true, true, true, true, null, true, true, null, false, null, null);
                if (!tip.NullOrEmpty())
                {
                    Get.Tooltips.RegisterTip(rect2, tip, null);
                }
            }
            else
            {
                flag = false;
            }
            y += 30f;
            return flag;
        }

        private void DoDivider(ref float y, float width)
        {
            y += 3f;
            GUIExtra.DrawHorizontalLine(new Vector2(0f, y), width, Color.gray, 1f);
            y += 9f;
        }

        private List<EntitySpec> actorSpecsInOrder = new List<EntitySpec>();

        private const int StatHeight = 30;
    }
}