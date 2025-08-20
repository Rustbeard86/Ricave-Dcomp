using System;
using System.Collections.Generic;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public class Tooltips
    {
        public bool Any
        {
            get
            {
                return this.tips.Count != 0 || this.showingTip != null;
            }
        }

        private Tooltips.Tip? ShowingTip
        {
            get
            {
                return this.showingTip;
            }
        }

        public void RegisterTip(Rect rect, string text, int? customID = null)
        {
            this.RegisterTip(rect, null, text, customID);
        }

        public void RegisterTip(Rect rect, ITipSubject subject, string extraText = null, int? customID = null)
        {
            if (Event.current.type != EventType.Repaint)
            {
                return;
            }
            if (Get.LessonManager.CurrentLesson == Get.Lesson_Tooltips && !(subject is Item))
            {
                GUIExtra.DrawLessonHint(rect);
            }
            if (!Mouse.Over(rect))
            {
                return;
            }
            if (subject == null && extraText.NullOrEmpty())
            {
                return;
            }
            if (Get.WindowManager.IsContextMenuOpen && !(Window.Current is Window_ContextMenu))
            {
                return;
            }
            if (Get.DragAndDrop.Dragging)
            {
                return;
            }
            Vector2 vector = GUIUtility.GUIToScreenPoint(rect.position / Widgets.UIScale);
            Rect rect2 = new Rect(vector, rect.size);
            int i = 0;
            Tooltips.Tip tip;
            while (i < this.tips.Count)
            {
                if (customID == null)
                {
                    tip = this.tips[i];
                    if (tip.customID == null && this.tips[i].rect == rect2 && this.tips[i].subject.EqualsSafe(subject) && this.tips[i].extraText == extraText)
                    {
                        goto IL_0147;
                    }
                }
                if (customID != null)
                {
                    int? customID2 = this.tips[i].customID;
                    int? num = customID;
                    if ((customID2.GetValueOrDefault() == num.GetValueOrDefault()) & (customID2 != null == (num != null)))
                    {
                        goto IL_0147;
                    }
                }
                i++;
                continue;
            IL_0147:
                Tooltips.Tip tip2 = this.tips[i];
                tip2.rect = rect2;
                tip2.subject = subject;
                tip2.extraText = extraText;
                tip2.lastRegisteredFrame = Clock.Frame;
                this.tips[i] = tip2;
                return;
            }
            List<Tooltips.Tip> list = this.tips;
            tip = new Tooltips.Tip
            {
                rect = rect2,
                subject = subject,
                extraText = extraText,
                lastRegisteredFrame = Clock.Frame,
                customID = customID
            };
            list.Add(tip);
            Get.MouseAttachmentDrawerGOC.CheckEnableDisable();
        }

        public void OnGUI()
        {
            if (Event.current.type != EventType.Repaint)
            {
                return;
            }
            bool flag = false;
            for (int i = this.tips.Count - 1; i >= 0; i--)
            {
                if (this.tips[i].lastRegisteredFrame < Clock.Frame - 1)
                {
                    this.tips.RemoveAt(i);
                    flag = true;
                }
            }
            if (flag)
            {
                Get.MouseAttachmentDrawerGOC.CheckEnableDisable();
            }
            for (int j = 0; j < this.tips.Count; j++)
            {
                Tooltips.Tip tip = this.tips[j];
                if (this.tips[j].rect.Contains(Event.current.MousePositionOrCenter()))
                {
                    tip.mouseoverTime += Clock.UnscaledDeltaTime;
                }
                else
                {
                    tip.mouseoverTime = 0f;
                }
                this.tips[j] = tip;
            }
        }

        public void DrawTooltips()
        {
            if (Event.current.type != EventType.Repaint)
            {
                return;
            }
            bool flag = false;
            float num = this.showingTipAlpha;
            for (int i = this.tips.Count - 1; i >= 0; i--)
            {
                if (this.tips[i].mouseoverTime >= 0.45f)
                {
                    this.showingTip = new Tooltips.Tip?(this.tips[i]);
                    this.showingTipAlpha = Math.Min(this.showingTipAlpha + Clock.UnscaledDeltaTime * 19f, 1f);
                    flag = true;
                    break;
                }
            }
            if (!flag)
            {
                this.showingTipAlpha = Math.Max(this.showingTipAlpha - Clock.UnscaledDeltaTime * 14f, 0f);
                if (this.showingTipAlpha <= 0f)
                {
                    this.showingTip = null;
                }
            }
            if (this.showingTip == null)
            {
                return;
            }
            if (num != this.showingTipAlpha)
            {
                CachedGUI.SetDirty(7);
            }
            ITipSubject subject = this.showingTip.Value.subject;
            Vector2 size;
            if (subject != null)
            {
                size = TipSubjectDrawer.GetSize(subject, this.showingTip.Value.extraText);
            }
            else
            {
                size = new Vector2(Math.Min(Widgets.CalcSize(this.showingTip.Value.extraText).x + 20f, 350f), Widgets.CalcHeight(this.showingTip.Value.extraText, 330f) + 20f);
            }
            Vector2 vector = Event.current.MousePositionOrCenter() + new Vector2(0f, 30f);
            bool flag2 = false;
            if (vector.x + size.x > Widgets.VirtualWidth)
            {
                vector.x = Event.current.MousePositionOrCenter().x - size.x;
                vector.y = Event.current.MousePositionOrCenter().y;
                flag2 = true;
            }
            if (vector.y + size.y > Widgets.VirtualHeight)
            {
                if (Event.current.MousePositionOrCenter().y - size.y >= 0f)
                {
                    vector.y = Event.current.MousePositionOrCenter().y - size.y;
                    if (!flag2)
                    {
                        vector.x = Event.current.MousePositionOrCenter().x;
                    }
                }
                else
                {
                    vector.y = Widgets.VirtualHeight - size.y;
                }
            }
            if (Clock.Frame % 4 == 2)
            {
                CachedGUI.SetDirty(7);
            }
            if (CachedGUI.BeginCachedGUI(new Rect(vector, size).ExpandedBy(2f), 7, false))
            {
                if (subject != null)
                {
                    TipSubjectDrawer.Draw(subject, vector, this.showingTip.Value.extraText);
                }
                else
                {
                    Rect rect = new Rect(vector, size);
                    GUIExtra.DrawRoundedRectBump(rect, new Color(0.15f, 0.15f, 0.15f), false, true, true, true, true, null);
                    Widgets.Label(rect.ContractedBy(10f), this.showingTip.Value.extraText, true, null, null, false);
                }
            }
            CachedGUI.EndCachedGUI(this.showingTipAlpha, 1f);
            if (!(subject is Item))
            {
                Get.LessonManager.FinishIfCurrent(Get.Lesson_Tooltips);
            }
        }

        public void OnCursorLockStateChanged()
        {
            this.showingTip = null;
            this.showingTipAlpha = 0f;
        }

        private List<Tooltips.Tip> tips = new List<Tooltips.Tip>();

        private Tooltips.Tip? showingTip;

        private float showingTipAlpha;

        private const float TimeToShow = 0.45f;

        private const int Pad = 10;

        private const int DefaultWidth = 350;

        public const float MouseOffset = 30f;

        private const float FadeInSpeed = 19f;

        private const float FadeOutSpeed = 14f;

        private struct Tip
        {
            public Rect rect;

            public ITipSubject subject;

            public string extraText;

            public float mouseoverTime;

            public int lastRegisteredFrame;

            public int? customID;
        }
    }
}