using System;
using System.Collections.Generic;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public class Window_Quests : Window
    {
        public Window_Quests(WindowSpec spec, int ID)
            : base(spec, ID)
        {
        }

        protected override void DoWindowContents(Rect inRect)
        {
            float num = 0f;
            Rect rect = Widgets.BeginScrollView(inRect, new int?(734278954));
            this.DoQuestsView(rect, ref num);
            Widgets.EndScrollView(inRect, num, false, false);
        }

        private void DoQuestsView(Rect inRect, ref float curY)
        {
            List<QuestSpec> questsToShow = Window_Quests.GetQuestsToShow();
            Widgets.Section("MainQuests".Translate(), 0f, inRect.width, ref curY, false, 7f);
            bool flag = false;
            foreach (QuestSpec questSpec in questsToShow)
            {
                if (questSpec.IsMainQuestline)
                {
                    if (flag)
                    {
                        curY += 10f;
                    }
                    this.DoQuest(questSpec, inRect.width, ref curY);
                    flag = true;
                }
            }
            if (!flag)
            {
                GUI.color = Color.gray;
                Widgets.Label(0f, inRect.width, ref curY, "None".Translate(), true);
                GUI.color = Color.white;
            }
            curY += 17f;
            Widgets.Section("SideQuests".Translate(), 0f, inRect.width, ref curY, false, 7f);
            flag = false;
            foreach (QuestSpec questSpec2 in questsToShow)
            {
                if (!questSpec2.IsMainQuestline)
                {
                    if (flag)
                    {
                        curY += 10f;
                    }
                    this.DoQuest(questSpec2, inRect.width, ref curY);
                    flag = true;
                }
            }
            if (!flag)
            {
                GUI.color = Color.gray;
                Widgets.Label(0f, inRect.width, ref curY, "None".Translate(), true);
                GUI.color = Color.white;
            }
            curY += 17f;
            Widgets.Section("ExtraQuests".Translate(), 0f, inRect.width, ref curY, false, 7f);
            flag = false;
            foreach (ValueTuple<string, string> valueTuple in Window_Quests.GetExtraQuests())
            {
                if (flag)
                {
                    curY += 10f;
                }
                this.DoExtra(valueTuple.Item1, valueTuple.Item2, inRect.width, ref curY);
                flag = true;
            }
            if (!flag)
            {
                GUI.color = Color.gray;
                Widgets.Label(0f, inRect.width, ref curY, "None".Translate(), true);
                GUI.color = Color.white;
            }
            curY += 40f;
            GUI.color = Color.gray;
            Widgets.LabelCentered(0f, inRect.width, ref curY, "AcceptQuestsInLobbyInfo".Translate(), true);
            curY += 7f;
            Widgets.LabelCentered(0f, inRect.width, ref curY, "QuestsRequireDungeonFloorInfo".Translate(), true);
            GUI.color = Color.white;
        }

        private void DoQuest(QuestSpec quest, float width, ref float curY)
        {
            Rect rect = new Rect(0f, curY, width, Widgets.GetLinesHeight(2));
            GUIExtra.DrawHighlightIfMouseover(rect, true, true, true, true, true);
            Get.Tooltips.RegisterTip(rect, quest.LongDescription, null);
            Rect rect2 = new Rect(0f, curY, 40f, 40f);
            GUI.color = new Color(0.88f, 0.88f, 0.88f);
            GUI.DrawTexture(rect2, Get.MainTab_Quests.Icon);
            GUI.color = Color.white;
            Widgets.FontBold = true;
            Widgets.Label(45f, width - 45f, ref curY, quest.LabelCap, true);
            Widgets.FontBold = false;
            Widgets.Label(45f, width - 45f, ref curY, quest.Description, true);
        }

        private void DoExtra(string title, string shortDesc, float width, ref float curY)
        {
            GUI.color = new Color(0.88f, 0.853f, 1f);
            Widgets.FontBold = true;
            Widgets.Label(0f, width, ref curY, title, true);
            Widgets.FontBold = false;
            Widgets.Label(0f, width, ref curY, shortDesc, true);
            GUI.color = Color.white;
        }

        public static List<QuestSpec> GetQuestsToShow()
        {
            Window_Quests.tmpQuestsToShow.Clear();
            Window_Quests.tmpQuestsToShow.AddRange(Get.QuestManager.ActiveQuests);
            foreach (QuestSpec questSpec in Get.ThisRunCompletedQuests.CompletedQuests)
            {
                Window_Quests.tmpQuestsToShow.Remove(questSpec);
            }
            return Window_Quests.tmpQuestsToShow;
        }

        public static List<ValueTuple<string, string>> GetExtraQuests()
        {
            Window_Quests.tmpExtraQuests.Clear();
            if (Get.RunSpec.IsMain && !Get.RunConfig.ProgressDisabled)
            {
                RunSpec nextMainDungeon = Get.RunSpec.GetNextMainDungeon();
                if (nextMainDungeon != null && !nextMainDungeon.IsUnlocked() && Get.RunSpec.FloorCount != null)
                {
                    Window_Quests.tmpExtraQuests.Add(new ValueTuple<string, string>("ExtraQuest_UnlockNextDungeon".Translate(), "ExtraQuest_UnlockNextDungeonDesc".Translate(Get.RunSpec.FloorCount.Value)));
                }
            }
            if (Get.RunSpec.IsMain && !Get.RunConfig.ProgressDisabled && Get.ThisRunLobbyItems.GetCount(Get.Entity_PuzzlePiece) <= 0)
            {
                int count = Get.TotalLobbyItems.GetCount(Get.Entity_PuzzlePiece);
                int num = count + 1;
                if (count < 10)
                {
                    if (Get.RunSpec.FloorCount != null)
                    {
                        int? floorCount = Get.RunSpec.FloorCount;
                        int num2 = num;
                        if (!((floorCount.GetValueOrDefault() >= num2) & (floorCount != null)))
                        {
                            goto IL_012B;
                        }
                    }
                    Window_Quests.tmpExtraQuests.Add(new ValueTuple<string, string>("ExtraQuest_FindPuzzlePiece".Translate(), "ExtraQuest_FindPuzzlePieceDesc".Translate(num, count, 10)));
                }
            }
        IL_012B:
            if (Get.RunSpec.SpiritsCount > 0 && Get.Player.SpiritsSetFree < Get.RunSpec.SpiritsCount && Get.Progress.GetRunStats(Get.RunSpec).MaxSpiritsSetFree < Get.RunSpec.SpiritsCount)
            {
                string text = ((Get.RunSpec.SpiritsCount == 1) ? "ExtraQuest_SetSpiritsFreeDesc_One".Translate() : "ExtraQuest_SetSpiritsFreeDesc_Multiple".Translate(Get.RunSpec.SpiritsCount));
                Window_Quests.tmpExtraQuests.Add(new ValueTuple<string, string>("ExtraQuest_SetSpiritsFree".Translate(), text));
            }
            return Window_Quests.tmpExtraQuests;
        }

        private const float SpaceBeforeSection = 17f;

        private const float SpaceBetweenQuests = 10f;

        private static List<QuestSpec> tmpQuestsToShow = new List<QuestSpec>();

        private static List<ValueTuple<string, string>> tmpExtraQuests = new List<ValueTuple<string, string>>();
    }
}