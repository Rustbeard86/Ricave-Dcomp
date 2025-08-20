using System;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public static class InteractionDrawer
    {
        public static void Draw(Texture2D icon1, Color icon1Color, Texture2D icon2, Color icon2Color, int sequence, bool disabled = false, string disabledReason = null, IntRange? dealtDamageRange = null, int? targetHP = null, bool hasUsePrompt = false, float? arrowDirection = null, bool anyActionTakesMoreThan1Turn = false)
        {
            if (Event.current.type != EventType.Repaint)
            {
                return;
            }
            if (InteractionDrawer.lastFrameDrawn < Clock.Frame - 1 || InteractionDrawer.lastFrameDrawnIcon != icon1 || InteractionDrawer.lastFrameDrawnDisabled != disabled)
            {
                InteractionDrawer.animationStartTime = Clock.UnscaledTime;
            }
            InteractionDrawer.lastFrameDrawn = Clock.Frame;
            InteractionDrawer.lastFrameDrawnIcon = icon1;
            InteractionDrawer.lastFrameDrawnDisabled = disabled;
            float num = 0f;
            BodyPart highlightedBodyPart = Get.InteractionManager.HighlightedBodyPart;
            if (highlightedBodyPart != null)
            {
                targetHP = new int?(highlightedBodyPart.HP);
            }
            Texture2D texture2D = icon1;
            Color color = icon1Color;
            if (!disabled && (int)((Clock.UnscaledTime - InteractionDrawer.animationStartTime) * 2f) % 2 == 1)
            {
                if (icon2 != null)
                {
                    texture2D = icon2;
                    color = icon2Color;
                }
                else
                {
                    num = 15f;
                }
            }
            if (arrowDirection != null)
            {
                GUI.color = new Color(1f, 1f, 1f, 0.6f);
                GUIExtra.DrawTextureRotated(new Rect(Calc.Round(Widgets.VirtualWidth / 2f + 12f), Calc.Round((Widgets.VirtualHeight - 30f) / 2f), 30f, 30f), InteractionDrawer.ArrowTex, arrowDirection.Value + 180f, null);
                GUI.color = Color.white;
            }
            GUI.color = (disabled ? color.Darker(0.35f) : color);
            Rect rect = new Rect(Calc.Round(Widgets.VirtualWidth / 2f + 12f), Calc.Round((Widgets.VirtualHeight - 30f) / 2f), 30f, 30f);
            if (arrowDirection != null)
            {
                rect = rect.MovedBy(30f, 0f);
            }
            GUIExtra.DrawTextureRotated(rect, texture2D, num, null);
            GUI.color = Color.white;
            if (hasUsePrompt)
            {
                Widgets.LabelCentered(rect.BottomCenter().MovedBy(13f, -4f), "...", true, null, null, false, false, false, null);
            }
            Widgets.FontSizeScalable = 19;
            GUI.color = new Color(0.72f, 0.72f, 0.72f);
            Vector2 vector = new Vector2(Calc.Round(Widgets.VirtualWidth / 2f + 12f + 30f + 5f), Calc.Round(Widgets.VirtualHeight / 2f));
            string text = null;
            if (disabled)
            {
                text = RichText.CreateColorTag(disabledReason ?? "-", "#ffb7b7");
                if (Get.InteractionManager.LastTriedDisabledActionReason == disabledReason)
                {
                    float num2 = Clock.UnscaledTime - Get.InteractionManager.LastTimeTriedDisabledAction;
                    if (num2 < 0.33333f)
                    {
                        vector = vector.MovedBy(Calc.Sin(num2 * 3.1415927f * 12f) * 7f * (1f - num2 / 0.33333f), 0f);
                    }
                }
            }
            else if (arrowDirection == null)
            {
                text = ((float)sequence / 12f).ToStringCached();
                if (sequence == 0)
                {
                    text = RichText.CreateColorTag(text, "#adffb6");
                }
                if (dealtDamageRange != null)
                {
                    if (targetHP != null && dealtDamageRange.Value.from >= targetHP.Value)
                    {
                        text = text.AppendedWithSpace(RichText.Bold(RichText.LightRed("({0}-{1})".Formatted(dealtDamageRange.Value.from.ToStringCached(), dealtDamageRange.Value.to.ToStringCached()))));
                    }
                    else if (targetHP != null && dealtDamageRange.Value.to >= targetHP.Value)
                    {
                        text = "{0} ({1}-{2})".Formatted(text, dealtDamageRange.Value.from.ToStringCached(), RichText.Bold(RichText.LightRed(dealtDamageRange.Value.to.ToStringCached())));
                    }
                    else
                    {
                        text = "{0} ({1}-{2})".Formatted(text, dealtDamageRange.Value.from.ToStringCached(), dealtDamageRange.Value.to.ToStringCached());
                    }
                }
            }
            if (text != null)
            {
                Widgets.LabelCenteredV(vector, text, true, null, null, false);
            }
            if (Get.KeyBinding_StationaryActionsOnly.HeldDown)
            {
                Widgets.LabelCenteredV(vector.WithAddedY(25f), "Stationary".Translate(), true, null, null, false);
            }
            else if (Get.KeyBinding_OneStep.HeldDown)
            {
                Widgets.LabelCenteredV(vector.WithAddedY(25f), "OneStep".Translate(), true, null, null, false);
            }
            GUI.color = Color.white;
            if (highlightedBodyPart != null)
            {
                Widgets.LabelCenteredV(vector.WithAddedY(-25f), highlightedBodyPart.LabelCap, true, null, null, false);
            }
            Widgets.ResetFontSize();
        }

        private static int lastFrameDrawn;

        private static Texture2D lastFrameDrawnIcon;

        private static bool lastFrameDrawnDisabled;

        private static float animationStartTime;

        public const int IconSize = 30;

        public const int FontSize = 19;

        private static readonly Texture2D ArrowTex = Assets.Get<Texture2D>("Textures/UI/Arrow");
    }
}