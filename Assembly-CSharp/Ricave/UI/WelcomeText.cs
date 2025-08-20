using System;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public class WelcomeText
    {
        private float Alpha
        {
            get
            {
                return Calc.ResolveFadeInStayOut(Clock.UnscaledTime - this.startTime, 0.25f, Get.InLobby ? 2f : 3.2f, 1f);
            }
        }

        private string Text
        {
            get
            {
                string text;
                if (!Get.RunSpec.LabelCap.NullOrEmpty())
                {
                    text = Get.RunSpec.LabelCap;
                }
                else
                {
                    text = Get.WorldSpec.LabelCap;
                }
                if (Get.Place != null)
                {
                    string text2 = Get.Place.LabelCap;
                    if (Get.RunSpec.FloorCount != null)
                    {
                        text2 = "{0} ({1}/{2})".Formatted(text2, Get.Floor.ToStringCached(), Get.RunSpec.FloorCount.Value.ToStringCached());
                    }
                    text = text.AppendedInNewLine(RichText.FontSize(text2, 23));
                }
                if (Get.Difficulty != null && !Get.InLobby && Get.RunSpec != Get.Run_Tutorial)
                {
                    text = text.AppendedInNewLine(RichText.SubtleRed(RichText.FontSize(Get.Difficulty.LabelCap, 21)));
                }
                if (Get.RunConfig.ProgressDisabled)
                {
                    if (Get.RunConfig.IsDailyChallenge)
                    {
                        text = text.AppendedInNewLine(RichText.FontSize(RichText.Grayed("{0} ({1})".Formatted("DailyChallenge".Translate(), Get.RunConfig.DailyChallengeDate)), 21));
                    }
                    else
                    {
                        text = text.AppendedInNewLine(RichText.FontSize(RichText.Grayed("ProgressDisabled".Translate()), 21));
                    }
                }
                if (!Get.InLobby)
                {
                    bool flag = true;
                    foreach (UnlockableSpec unlockableSpec in Get.UnlockableManager.DirectlyUnlocked)
                    {
                        if (unlockableSpec.ResetAfterRun)
                        {
                            string text3 = RichText.FontSize(RichText.Yellow(unlockableSpec.LabelCap), 21);
                            if (flag)
                            {
                                flag = false;
                                text = text.AppendedInNewLine(text3);
                            }
                            else
                            {
                                text = text.AppendedWithComma(text3);
                            }
                        }
                    }
                }
                int activeCount = DungeonModifiersUtility.ActiveCount;
                if (activeCount >= 1)
                {
                    text = text.AppendedInNewLine(RichText.FontSize(RichText.Blue((activeCount == 1) ? "DungeonModifiersCount_One".Translate() : "DungeonModifiersCount_More".Translate(activeCount)), 21));
                }
                return text;
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
            float alpha = this.Alpha;
            if (alpha <= 0f)
            {
                return;
            }
            string text = this.Text;
            if (text.NullOrEmpty())
            {
                return;
            }
            Vector2 vector = new Vector2(Widgets.ScreenCenter.x, 0.25f * Widgets.VirtualHeight);
            GUI.color = new Color(0.9f, 0.9f, 0.9f, alpha);
            Widgets.FontSizeScalable = 35;
            Widgets.FontBold = true;
            Widgets.Align = TextAnchor.MiddleCenter;
            Widgets.Label(vector.CenteredRect(600f, 250f), text, true, null, null, false);
            float num = Widgets.CalcHeight(text, 600f);
            Widgets.ResetAlign();
            Widgets.FontBold = false;
            Widgets.ResetFontSize();
            Place place = Get.Place;
            Texture2D texture2D;
            Color color;
            if (((place != null) ? place.Icon : null) != null)
            {
                texture2D = Get.Place.Icon;
                color = Get.Place.IconColor;
            }
            else
            {
                texture2D = Get.WorldSpec.Icon;
                color = Get.WorldSpec.IconColor;
            }
            GUI.color = color.WithAlphaFactor(alpha);
            GUI.DrawTexture(vector.WithAddedY(-num / 2f - 34f).CenteredRect(60f), texture2D);
            GUI.color = Color.white;
        }

        private float startTime = -99999f;

        private const float Duration_FadingIn = 0.25f;

        private const float Duration_FadingOut = 1f;

        private const float DurationLobby = 2f;

        private const float DurationNonLobby = 3.2f;
    }
}