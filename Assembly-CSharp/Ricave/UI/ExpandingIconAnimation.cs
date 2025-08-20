using System;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public static class ExpandingIconAnimation
    {
        public static void Do(Rect rect, Texture2D icon, Color iconColor, float startTime, float scale, float duration = 0.6f, float alphaFactor = 0.55f)
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
            GUI.color = iconColor.WithAlphaFactor((1f - num) * alphaFactor);
            GUIExtra.DrawTexture(rect.ExpandedBy(num * scale * 100f), icon);
            GUI.color = Color.white;
        }

        private const float DefaultDuration = 0.6f;
    }
}