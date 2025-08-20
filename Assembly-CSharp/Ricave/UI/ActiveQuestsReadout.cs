using System;
using System.Collections.Generic;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public class ActiveQuestsReadout
    {
        private float Alpha
        {
            get
            {
                return Calc.ResolveFadeInStayOut(Clock.UnscaledTime - this.startTime, 1f, 3.3f, 1.6f);
            }
        }

        public void Show()
        {
            this.startTime = Clock.UnscaledTime;
        }

        public void OnGUI()
        {
            if (Event.current.type != EventType.Repaint)
            {
                return;
            }
            if (Get.InLobby)
            {
                return;
            }
            if (Get.RunConfig.ProgressDisabled)
            {
                return;
            }
            float alpha = this.Alpha;
            if (alpha <= 0f)
            {
                return;
            }
            this.tmpActiveQuests.Clear();
            this.tmpActiveQuests.AddRange(Get.QuestManager.ActiveQuests);
            foreach (QuestSpec questSpec in Get.ThisRunCompletedQuests.CompletedQuests)
            {
                this.tmpActiveQuests.Remove(questSpec);
            }
            if (this.tmpActiveQuests.Count == 0)
            {
                return;
            }
            float num = Widgets.VirtualHeight / 2f + 40f;
            Widgets.FontSizeScalable = 22;
            for (int i = this.tmpActiveQuests.Count; i >= 0; i--)
            {
                int num2 = this.tmpActiveQuests.Count - i;
                string text;
                if (i == this.tmpActiveQuests.Count)
                {
                    text = "{0}:".Formatted("ActiveQuests".Translate());
                }
                else
                {
                    text = this.tmpActiveQuests[i].LabelCap;
                    if (!this.tmpActiveQuests[i].ExtraLabelFormat.NullOrEmpty())
                    {
                        text = text.AppendedWithSpace(RichText.Grayed("({0})".Formatted(this.tmpActiveQuests[i].ExtraLabelFormat.FormattedQuestsState())));
                    }
                }
                float num3 = Clock.UnscaledTime - this.startTime - 0.12f - (float)num2 * 0.2f;
                float num4 = Calc.SmoothStep(0f, 1f, Math.Min(num3 * 1.2f, 1f));
                float num5 = Calc.SmoothStep(0f, 20f, Math.Min(num3 * 1.2f, 1f));
                GUI.color = new Color(0.9f, 0.9f, 0.9f, alpha * num4);
                float num6 = 0f;
                if (i != this.tmpActiveQuests.Count)
                {
                    GUI.DrawTexture(new Rect(num5, num - 15f, 30f, 30f), ActiveQuestsReadout.NotCompleted);
                    num6 = 35f;
                }
                Widgets.FontBold = i != this.tmpActiveQuests.Count;
                Widgets.LabelCenteredV(new Vector2(num5 + num6, num), text, true, null, null, false);
                Widgets.FontBold = false;
                num += 28f;
            }
            GUI.color = Color.white;
            Widgets.ResetFontSize();
        }

        private float startTime = -99999f;

        private List<QuestSpec> tmpActiveQuests = new List<QuestSpec>();

        private const float Duration_FadingIn = 1f;

        private const float Duration = 3.3f;

        private const float Duration_FadingOut = 1.6f;

        private const float Delay = 0.12f;

        private const float Margin = 20f;

        private static readonly Texture2D NotCompleted = Assets.Get<Texture2D>("Textures/UI/NotCompleted");
    }
}