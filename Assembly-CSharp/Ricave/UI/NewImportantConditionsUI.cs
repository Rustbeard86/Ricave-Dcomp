using System;
using System.Collections.Generic;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public class NewImportantConditionsUI
    {
        public int Add(Condition condition)
        {
            int num = this.nextID;
            this.nextID = num + 1;
            int num2 = num;
            NewImportantConditionsUI.Entry entry = new NewImportantConditionsUI.Entry
            {
                ID = num2,
                icon = condition.Icon,
                iconColor = condition.IconColor,
                label = condition.LabelCap,
                startTime = Clock.UnscaledTime
            };
            this.entries.Add(entry);
            return num2;
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
            for (int i = this.entries.Count - 1; i >= 0; i--)
            {
                NewImportantConditionsUI.Entry entry = this.entries[i];
                float num = Clock.UnscaledTime - entry.startTime;
                float num2 = Calc.ResolveFadeInStayOut(num, 0.3f, 0.32f, 0.9f);
                if (num > 2f)
                {
                    this.entries.RemoveAt(i);
                }
                else if (num2 > 0f)
                {
                    float num3 = 0.78f + Calc.ResolveFadeIn(num, 0.17f) * 0.22f;
                    float num4 = Math.Max(num - 0.35f, 0f) * 0.03f;
                    GUI.color = entry.iconColor.WithAlphaFactor(num2 * 0.18f);
                    GUI.DrawTexture(Widgets.ScreenRect.ContractedByPct(0.15f).ExpandedByPct(num3 - 1f).ContractedToSquare()
                        .MovedByPct(0f, num4), entry.icon);
                    GUI.color = Color.white;
                }
            }
        }

        private List<NewImportantConditionsUI.Entry> entries = new List<NewImportantConditionsUI.Entry>();

        private int nextID = 1;

        private struct Entry
        {
            public int ID;

            public Texture2D icon;

            public Color iconColor;

            public string label;

            public float startTime;
        }
    }
}