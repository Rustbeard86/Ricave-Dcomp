using System;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public static class ExpandingLabelAnimation
    {
        public static void Do(Vector2 pos, string label, int fontSize, Color labelColor, float startTime, float scale, float duration = 0.6f, float alphaFactor = 0.55f, bool bold = false)
        {
            if (Event.current.type != EventType.Repaint)
            {
                return;
            }
            float num = (Clock.UnscaledTime - startTime) / duration;
            if (num >= 1f)
            {
                return;
            }
            GUI.color = labelColor.WithAlpha((1f - num) * alphaFactor);
            Widgets.FontBold = bold;
            Widgets.FontSizeScalable = fontSize + (int)(num * 320f * scale);
            Widgets.LabelCentered(pos, label, true, null, null, false, false, false, null);
            Widgets.ResetFontSize();
            Widgets.FontBold = false;
            GUI.color = Color.white;
        }

        private const float DefaultDuration = 0.6f;
    }
}