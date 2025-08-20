using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public class Window_QuestLog : Window
    {
        public Window_QuestLog(WindowSpec spec, int ID)
            : base(spec, ID)
        {
        }

        public override void OnOpen()
        {
            base.OnOpen();
            if (Get.QuestManager.ActiveQuests.Count + Get.QuestManager.ClaimableQuestsCount == 0)
            {
                this.currentTab = 1;
            }
        }

        protected override void DoWindowContents(Rect inRect)
        {
            Get.WindowManager.FocusWindow(this);
            Rect rect = base.MainArea(inRect);
            if (this.selectedQuest != null)
            {
                float num = 20f;
                Rect rect2 = Widgets.BeginScrollView(rect, new int?(821540978));
                this.DoQuestDetails(rect2, ref num);
                Widgets.EndScrollView(rect, num, true, true);
            }
            else
            {
                this.tabs[0] = "{0} ({1})".Formatted("ActiveQuestsShort".Translate(), (Get.QuestManager.ActiveQuests.Count + Get.QuestManager.ClaimableQuestsCount).ToStringCached());
                this.tabs[1] = "{0} ({1})".Formatted("AvailableQuestsShort".Translate(), Get.QuestManager.AvailableToTakeQuestsCount.ToStringCached());
                this.tabs[2] = "{0} ({1})".Formatted("HistoricalQuestsShort".Translate(), Get.QuestManager.HistoricalQuestsCount.ToStringCached());
                if (this.currentTab == 0 && Get.QuestManager.ActiveQuests.Count + Get.QuestManager.ClaimableQuestsCount == 0 && Get.QuestManager.AvailableToTakeQuestsCount > 0)
                {
                    float num2 = Calc.PulseUnscaled(3f, 0.4f);
                    this.tabs[1] = RichText.Bold(this.tabs[1]);
                    this.tabs[1] = RichText.CreateColorTag(this.tabs[1], new Color(0.5f + num2, 0.5f + num2, 0.35f));
                }
                Widgets.Tabs(rect.TopPart(32f), this.tabs, ref this.currentTab, true, null, null);
                Rect rect3 = rect.CutFromTop(32f);
                Rect rect4 = Widgets.BeginScrollView(rect3, new int?(280492612));
                float num3 = this.DoQuestsList(rect4);
                Widgets.EndScrollView(rect3, num3, false, false);
            }
            if (this.selectedQuest != null)
            {
                bool flag = false;
                if (this.selectedQuest.IsCompleted())
                {
                    if (!this.selectedQuest.IsCompletedAndClaimed())
                    {
                        this.DoClaimButton(base.GetBottomButtonRect(inRect, 1, 2), this.selectedQuest);
                        flag = true;
                    }
                }
                else if (this.selectedQuest.IsActive())
                {
                    this.DoPostponeButton(base.GetBottomButtonRect(inRect, 1, 2), this.selectedQuest);
                    flag = true;
                }
                else if (this.selectedQuest.NoDialogueOrEnded)
                {
                    if (this.DoAcceptButton(base.GetBottomButtonRect(inRect, 1, 2), this.selectedQuest))
                    {
                        this.selectedQuest = null;
                    }
                    flag = true;
                }
                if (base.DoBottomButton("Back".Translate(), inRect, 0, flag ? 2 : 1, true, null))
                {
                    if (!this.selectedQuest.NoDialogueOrEnded)
                    {
                        Dialogue orCreateDialogue = Get.DialoguesManager.GetOrCreateDialogue(this.selectedQuest.Dialogue);
                        if (orCreateDialogue.IsInLastNode && !orCreateDialogue.CurNode.HasResponses)
                        {
                            orCreateDialogue.OnContinuedDialogue();
                        }
                    }
                    this.selectedQuest = null;
                    return;
                }
            }
            else if (base.DoBottomButton("Close".Translate(), inRect, 0, 1, true, null))
            {
                Get.WindowManager.Close(this, true);
            }
        }

        private float DoQuestsList(Rect viewRect)
        {
            Window_QuestLog.<> c__DisplayClass14_0 CS$<> 8__locals1;
            CS$<> 8__locals1.viewRect = viewRect;
            CS$<> 8__locals1.<> 4__this = this;
            CS$<> 8__locals1.curY = 0f;
            if (this.currentTab == 0)
            {
                this.tmpClaimableQuests.Clear();
                this.tmpClaimableQuests.AddRange(Get.QuestManager.CompletedQuests.Where<QuestSpec>((QuestSpec x) => !x.IsCompletedAndClaimed()));
                this.tmpClaimableQuests.Reverse();
                this.tmpActiveQuests.Clear();
                this.tmpActiveQuests.AddRange(Get.QuestManager.ActiveQuests);
                this.tmpActiveQuests.Reverse();
                if (this.tmpClaimableQuests.Count != 0)
                {
                    this.< DoQuestsList > g__DoSectionTitle | 14_1("CompletedQuests".Translate(), ref CS$<> 8__locals1);
                    this.< DoQuestsList > g__DoQuests | 14_0(this.tmpClaimableQuests, ref CS$<> 8__locals1);
                }
                this.< DoQuestsList > g__DoSectionTitle | 14_1("ActiveQuests".Translate(), ref CS$<> 8__locals1);
                this.< DoQuestsList > g__DoQuests | 14_0(this.tmpActiveQuests, ref CS$<> 8__locals1);
            }
            else if (this.currentTab == 1)
            {
                this.tmpAvailableQuests.Clear();
                this.tmpAvailableQuests.AddRange(Get.QuestManager.AllVisibleQuests.Where<QuestSpec>((QuestSpec x) => !x.IsCompleted() && !x.IsActive()));
                this.< DoQuestsList > g__DoSectionTitle | 14_1("AvailableQuests".Translate(), ref CS$<> 8__locals1);
                this.< DoQuestsList > g__DoQuests | 14_0(this.tmpAvailableQuests, ref CS$<> 8__locals1);
            }
            else
            {
                this.tmpHistoricalQuests.Clear();
                this.tmpHistoricalQuests.AddRange(Get.QuestManager.CompletedQuests.Where<QuestSpec>((QuestSpec x) => x.IsCompletedAndClaimed()));
                this.tmpHistoricalQuests.Reverse();
                this.< DoQuestsList > g__DoSectionTitle | 14_1("HistoricalQuests".Translate(), ref CS$<> 8__locals1);
                this.< DoQuestsList > g__DoQuests | 14_0(this.tmpHistoricalQuests, ref CS$<> 8__locals1);
            }
            return CS$<> 8__locals1.curY;
        }

        private void DoQuestRow(Rect rowRect, QuestSpec quest, bool anyAbove, bool anyBelow)
        {
            GUIExtra.DrawRoundedRectBump(rowRect, this.BackgroundColor.Lighter(0.09f), false, !anyAbove, !anyAbove, !anyBelow, !anyBelow, null);
            GUIExtra.DrawHighlightIfMouseover(rowRect, true, !anyAbove, !anyAbove, !anyBelow, !anyBelow);
            Rect rect = rowRect.ContractedBy(20f);
            Rect rect2 = new Rect(rect.xMax - 120f, rect.y + (rect.height - 50f) / 2f, 120f, 50f);
            Rect rect3 = rect;
            rect3.xMax = rect2.x - 10f;
            rect3.xMin += 58f;
            string text = quest.LabelCap;
            if (quest.NoDialogueOrEnded && quest.StardustReward != 0)
            {
                text = text.Concatenated(" {0}".Formatted(RichText.Stardust("{0}★".Formatted(quest.StardustReward))));
            }
            float num = rect3.y;
            GUI.color = new Color(0.88f, 0.88f, 0.88f);
            GUI.DrawTexture(rect.LeftPart(50f).ContractedToSquare().MovedBy(0f, -8f), Get.MainTab_Quests.Icon);
            GUI.color = Color.white;
            Widgets.FontSizeScalable = 16;
            Widgets.FontBold = true;
            Widgets.Label(new Rect(rect3.x, num, rect3.width, 30f), text, true, null, null, false);
            float x = Widgets.CalcSize(text).x;
            Widgets.FontBold = false;
            Widgets.ResetFontSize();
            if (quest.Dialogue != null)
            {
                float num2 = rect3.x + x + 10f;
                foreach (SpeakerSpec speakerSpec in quest.Dialogue.AllSpeakers)
                {
                    Rect rect4 = new Rect(num2, num - 2f, 24f, 24f);
                    GUIExtra.DrawHighlightIfMouseover(rect4, true, true, true, true, true);
                    if (Mouse.Over(rect4))
                    {
                        Get.Tooltips.RegisterTip(rect4, speakerSpec.LabelCap, null);
                    }
                    GUI.DrawTexture(rect4, speakerSpec.Portrait);
                    num2 += 29f;
                }
            }
            num += 28f;
            if (quest.NoDialogueOrEnded)
            {
                Widgets.Label(new Rect(rect3.x, num, rect3.width, 20f), quest.Description, true, null, null, false);
                num += 26f;
            }
            else
            {
                GUI.color = new Color(0.7f, 0.7f, 0.7f);
                Widgets.Label(new Rect(rect3.x, num, rect3.width, 20f), (ControllerUtility.InControllerMode ? "ClickToReadMoreController" : "ClickToReadMore").Translate(), true, null, null, false);
            }
            GUI.color = Color.white;
            if (quest.IsCompleted())
            {
                if (quest.IsCompletedAndClaimed())
                {
                    GUI.color = new Color(0.5f, 1f, 0.5f);
                    GUI.DrawTexture(rect2.ContractedByPct(0.1f), Window_QuestLog.QuestCompletedIcon, ScaleMode.ScaleToFit);
                    GUI.color = Color.white;
                }
                else
                {
                    this.DoClaimButton(rect2, quest);
                }
            }
            else if (quest.IsActive())
            {
                this.DoPostponeButton(rect2, quest);
            }
            else if (quest.NoDialogueOrEnded)
            {
                this.DoAcceptButton(rect2, quest);
            }
            if (Widgets.ButtonInvisible(rowRect, true, false))
            {
                this.selectedQuest = quest;
                this.letterShowingTimer = 0f;
                Get.Sound_Click.PlayOneShot(null, 1f, 1f);
            }
        }

        private void DoClaimButton(Rect buttonRect, QuestSpec quest)
        {
            string text = "Claim".Translate();
            bool flag = false;
            Color? color = new Color?(new Color(1f, 1f, 0.3f));
            if (Widgets.Button(buttonRect, text, flag, null, color, true, true, true, true, null, true, true, null, false, null, null))
            {
                Get.QuestManager.ClaimReward(quest);
                Get.Sound_QuestRewardClaimed.PlayOneShot(null, 1f, 1f);
            }
        }

        private void DoPostponeButton(Rect buttonRect, QuestSpec quest)
        {
            if (Widgets.Button(buttonRect, "Postpone".Translate(), true, null, null, true, true, true, true, null, true, true, null, false, null, null))
            {
                Get.QuestManager.SetInactive(quest);
            }
        }

        private bool DoAcceptButton(Rect buttonRect, QuestSpec quest)
        {
            if (Widgets.Button(buttonRect, "AcceptQuest".Translate(), false, null, null, true, true, true, true, null, true, true, null, false, null, null))
            {
                Get.Sound_QuestAccepted.PlayOneShot(null, 1f, 1f);
                Get.QuestManager.SetActive(quest);
                if (quest == Get.Quest_Introduction)
                {
                    Entity entity = Get.World.GetEntitiesOfSpec(Get.Entity_StaircaseLocked).FirstOrDefault<Entity>();
                    if (entity != null)
                    {
                        Vector3Int position = entity.Position;
                        Quaternion rotation = entity.Rotation;
                        foreach (Instruction instruction in InstructionSets_Entity.DeSpawn(entity, false))
                        {
                            instruction.Do();
                        }
                        foreach (Instruction instruction2 in InstructionSets_Entity.Spawn(Maker.Make(Get.Entity_NewRunStaircase, null, false, false, true), position, new Quaternion?(rotation)))
                        {
                            instruction2.Do();
                        }
                    }
                }
                return true;
            }
            return false;
        }

        private void DoQuestDetails(Rect viewRect, ref float curY)
        {
            Rect rect = viewRect.ContractedBy(20f);
            if (this.selectedQuest.Dialogue != null)
            {
                DialogueDrawer.Draw(this.selectedQuest.Dialogue, rect.x, rect.width, ref curY, ref this.letterShowingTimer);
            }
            if (this.selectedQuest.NoDialogueOrEnded)
            {
                if (this.selectedQuest.Dialogue != null)
                {
                    curY += 20f;
                    GUIExtra.DrawHorizontalLine(new Vector2(viewRect.x, curY), viewRect.width, new Color(0.35f, 0.35f, 0.35f), 1f);
                    curY += 20f;
                }
                Widgets.FontSizeScalable = 20;
                Widgets.FontBold = true;
                Widgets.Label(rect.x, rect.width, ref curY, "{0}: {1}".Formatted("Quest".Translate(), this.selectedQuest.LabelCap), true);
                Widgets.FontBold = false;
                Widgets.ResetFontSize();
                curY += 10f;
                Widgets.Label(rect.x, rect.width, ref curY, this.selectedQuest.LongDescription, true);
                if (this.selectedQuest.StardustReward != 0 || !this.selectedQuest.ExtraRewards.NullOrEmpty<string>())
                {
                    curY += 10f;
                    Widgets.Label(rect.x, rect.width, ref curY, "{0}:".Formatted("Rewards".Translate()), true);
                    if (this.selectedQuest.StardustReward != 0)
                    {
                        Widgets.Label(rect.x, rect.width, ref curY, "  - {0}".Formatted(RichText.Stardust(StringUtility.StardustString(this.selectedQuest.StardustReward))), true);
                    }
                    foreach (string text in this.selectedQuest.ExtraRewards)
                    {
                        Widgets.Label(rect.x, rect.width, ref curY, "  - {0}".Formatted(text), true);
                    }
                }
                if (this.selectedQuest.DungeonModifiers.Count != 0)
                {
                    curY += 10f;
                    GUI.color = new Color(0.75f, 0.75f, 0.9f);
                    Widgets.Label(rect.x, rect.width, ref curY, "{0}:".Formatted("DungeonModifiers".Translate()), true);
                    foreach (string text2 in this.selectedQuest.DungeonModifiers)
                    {
                        Widgets.Label(rect.x, rect.width, ref curY, "  - {0}".Formatted(text2), true);
                    }
                    GUI.color = Color.white;
                }
            }
            curY += 5f;
        }

        public override void OnClosed()
        {
            base.OnClosed();
            Get.Progress.Save();
        }

        [CompilerGenerated]
        private void <DoQuestsList>g__DoQuests|14_0(List<QuestSpec> quests, ref Window_QuestLog.<>c__DisplayClass14_0 A_2)
		{
			if (quests.Count == 0)
			{
				Rect rect = new Rect(A_2.viewRect.x, A_2.curY, A_2.viewRect.width, 30f).ContractedBy(20f, 0f);
        GUI.color = new Color(0.7f, 0.7f, 0.7f);
        Widgets.Label(rect, "None".Translate(), true, null, null, false);
				GUI.color = Color.white;
				A_2.curY += 30f;
				return;
			}
			for (int i = 0; i<quests.Count; i++)
			{
				if (i != 0)
				{
					float curY = A_2.curY;
    A_2.curY = curY - 1f;
				}
QuestSpec questSpec = quests[i];
Rect rect2 = new Rect(A_2.viewRect.x, A_2.curY, A_2.viewRect.width, 110f);
if (rect2.VisibleInScrollView())
{
    this.DoQuestRow(rect2, questSpec, i > 0, i < quests.Count - 1);
}
A_2.curY += 110f;
			}
		}

		[CompilerGenerated]
private void < DoQuestsList > g__DoSectionTitle | 14_1(string title, ref Window_QuestLog.<> c__DisplayClass14_0 A_2)

        {
    Rect rect = new Rect(A_2.viewRect.x, A_2.curY, A_2.viewRect.width, 60f).ContractedBy(20f);
    GUI.color = new Color(0.9f, 0.9f, 0.9f);
    Widgets.FontSizeScalable = 20;
    Widgets.FontBold = true;
    Widgets.Label(rect, title, true, null, null, false);
    Widgets.FontBold = false;
    Widgets.ResetFontSize();
    GUI.color = Color.white;
    A_2.curY += 60f;
}

private QuestSpec selectedQuest;

private float letterShowingTimer;

private List<QuestSpec> tmpClaimableQuests = new List<QuestSpec>();

private List<QuestSpec> tmpActiveQuests = new List<QuestSpec>();

private List<QuestSpec> tmpAvailableQuests = new List<QuestSpec>();

private List<QuestSpec> tmpHistoricalQuests = new List<QuestSpec>();

private int currentTab;

private List<string> tabs = new List<string> { "", "", "" };

private const int RowHeight = 110;

private const int Gap = 10;

private static readonly Texture2D QuestCompletedIcon = Assets.Get<Texture2D>("Textures/UI/Completed");
	}
}