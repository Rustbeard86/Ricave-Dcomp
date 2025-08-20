using System;
using System.Collections.Generic;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public class PlayLog
    {
        public int Add(string text)
        {
            int num = Get.UniqueIDsManager.NextPlayLogEntryID();
            PlayLog.Entry entry = new PlayLog.Entry
            {
                ID = num,
                text = text,
                startTime = Clock.UnscaledTime
            };
            this.entries.Add(entry);
            if (this.entries.Count > 15)
            {
                this.entries.RemoveRange(0, this.entries.Count - 15);
            }
            for (int i = 0; i < this.entries.Count - 1; i++)
            {
                PlayLog.Entry entry2 = this.entries[i];
                entry2.yOffsetAnimation.SetTarget(entry2.yOffsetAnimation.Target + 25f, entry2.yOffsetAnimation.Target + 25f);
                this.entries[i] = entry2;
            }
            return num;
        }

        public void TryRemove(int ID)
        {
            for (int i = 0; i < this.entries.Count; i++)
            {
                if (this.entries[i].ID == ID)
                {
                    this.entries.RemoveAt(i);
                    return;
                }
            }
        }

        public void OnGUI()
        {
            if (Event.current.type != EventType.Repaint)
            {
                return;
            }
            if (this.entries.Count == 0)
            {
                return;
            }
            float num = Widgets.VirtualHeight - 220f;
            Widgets.FontSizeScalable = 16;
            int i = this.entries.Count - 1;
            while (i >= 0)
            {
                PlayLog.Entry entry = this.entries[i];
                if (Clock.UnscaledTime - entry.startTime < 5f)
                {
                    entry.alpha = 1f;
                    goto IL_00A4;
                }
                entry.alpha -= 0.3f * Clock.UnscaledDeltaTime;
                if (entry.alpha > 0f)
                {
                    goto IL_00A4;
                }
                this.entries.RemoveAt(i);
            IL_0133:
                i--;
                continue;
            IL_00A4:
                float num2 = Math.Max(1f - (Clock.UnscaledTime - entry.startTime) / 0.25f, 0f);
                GUI.color = new Color(1f, 1f, 1f, entry.alpha * (1f - num2));
                Widgets.LabelCenteredV(new Vector2(20f + num2 * 45f, num + entry.yOffsetAnimation.CurrentValue), entry.text, true, null, null, false);
                num -= 25f;
                this.entries[i] = entry;
                goto IL_0133;
            }
            GUI.color = Color.white;
            Widgets.ResetFontSize();
        }

        public void FixedUpdate()
        {
            int i = 0;
            int count = this.entries.Count;
            while (i < count)
            {
                PlayLog.Entry entry = this.entries[i];
                if (entry.yOffsetAnimation.Target < 0.5f)
                {
                    entry.yOffsetAnimation.SetTarget(0f);
                }
                else
                {
                    entry.yOffsetAnimation.SetTarget(Calc.Lerp(entry.yOffsetAnimation.Target, 0f, 0.12f));
                }
                this.entries[i] = entry;
                i++;
            }
        }

        [Saved(Default.New, true)]
        private List<PlayLog.Entry> entries = new List<PlayLog.Entry>();

        private const int Margin = 20;

        private const int StartDistFromBottom = 220;

        private const int MaxEntries = 15;

        private const float EntryDuration = 5f;

        private const float FadeOutSpeed = 0.3f;

        private const int EntryHeight = 25;

        private const float XOffsetAnimationDuration = 0.25f;

        private const float XOffsetAnimationDistance = 45f;

        public const int FontSize = 16;

        private struct Entry
        {
            [Saved]
            public int ID;

            [Saved]
            public string text;

            public float startTime;

            [Saved]
            public float alpha;

            public InterpolatedFloat yOffsetAnimation;
        }
    }
}