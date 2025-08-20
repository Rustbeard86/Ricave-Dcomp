using System;
using System.Collections.Generic;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public class ScoreGainEffects
    {
        public int Add(string label, int score)
        {
            int num = this.nextID;
            this.nextID = num + 1;
            int num2 = num;
            this.entries.Add(new ScoreGainEffects.Entry
            {
                ID = num2,
                label = label,
                score = score
            });
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
            this.DrawEntry(0);
            if (this.entries.Count >= 2 && this.entries[0].animationPct >= 0.8f)
            {
                this.DrawEntry(1);
            }
            if (this.entries[0].animationPct >= 1f)
            {
                this.entries.RemoveAt(0);
            }
        }

        private void DrawEntry(int entryIndex)
        {
            ScoreGainEffects.Entry entry = this.entries[entryIndex];
            float num = Calc.ResolveFadeInStayOut(entry.animationPct, 0.3f, 0.48f, 0.22f);
            float num2 = Calc.LerpDouble(0.78f, 1f, 0f, -25f, entry.animationPct);
            GUI.color = new Color(0.9f, 0.9f, 0.9f, num);
            Widgets.LabelCentered(new Vector2(Widgets.ScreenCenter.x, 20f + num2), "{0}: {1} {2}".Formatted(entry.label, "Score".Translate(), entry.score.ToStringOffset(true)), true, null, null, false, false, false, null);
            GUI.color = Color.white;
            if (Event.current.type == EventType.Repaint)
            {
                entry.animationPct += Clock.UnscaledDeltaTime * 0.6f;
                this.entries[entryIndex] = entry;
            }
        }

        private List<ScoreGainEffects.Entry> entries = new List<ScoreGainEffects.Entry>();

        private int nextID = 1;

        private const float AnimationSpeed = 0.6f;

        private const float NextAnimationPctOverlap = 0.8f;

        private struct Entry
        {
            public int ID;

            public string label;

            public int score;

            public float animationPct;
        }
    }
}