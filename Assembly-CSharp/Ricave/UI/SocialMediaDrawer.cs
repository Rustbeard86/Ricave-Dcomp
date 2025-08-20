using System;
using Ricave.Core;
using Ricave.Rendering;
using UnityEngine;

namespace Ricave.UI
{
    public static class SocialMediaDrawer
    {
        public static void DrawAt(Texture2D icon, string title, string subtitle, string URL, Color highlightColor, Vector2 pos)
        {
            Rect rect = new Rect(pos.x, pos.y, 55f, 55f);
            Widgets.FontSize = 28;
            float x = Widgets.CalcSize(title).x;
            Widgets.FontSize = 20;
            float x2 = Widgets.CalcSize(subtitle).x;
            Rect rect2 = new Rect(pos.x, pos.y, rect.width + 10f + Math.Max(x, x2), 55f);
            GUI.DrawTexture(rect.ContractedByPct(0.1f), icon);
            if (Mouse.Over(rect2))
            {
                GUI.color = highlightColor;
            }
            else
            {
                GUI.color = new Color(0.6f, 0.6f, 0.6f);
            }
            float num = pos.x + rect.width + 10f;
            Widgets.FontSize = 28;
            Widgets.LabelCenteredV(new Vector2(num, pos.y + 15f), title, true, null, null, false);
            Widgets.FontSize = 20;
            Widgets.LabelCenteredV(new Vector2(num, pos.y + 40f), subtitle, true, null, null, false);
            GUI.color = Color.white;
            Widgets.ResetFontSize();
            if (Widgets.ButtonInvisible(rect2, true, false))
            {
                Get.Sound_Click.PlayOneShot(null, 1f, 1f);
                Sys.OpenURL(URL);
            }
        }

        public const float Height = 55f;

        private const int TitleFontSize = 28;

        private const int SubtitleFontSize = 20;

        private const float GapAfterIcon = 10f;
    }
}