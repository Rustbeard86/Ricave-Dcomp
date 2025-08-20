using System;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public static class CircularProgressBarDrawer
    {
        public static void Draw(Rect rect, float pct, float alpha = 1f, int? GUIcachedID = null)
        {
            if (alpha <= 0f)
            {
                return;
            }
            if (pct <= 0f)
            {
                GUI.color = CircularProgressBarDrawer.NotFilledColor.WithAlphaFactor(alpha);
                GUI.DrawTexture(rect, CircularProgressBarDrawer.Texture);
                GUI.color = Color.white;
                return;
            }
            if (pct >= 1f)
            {
                GUI.color = CircularProgressBarDrawer.FilledColor.WithAlphaFactor(alpha);
                GUI.DrawTexture(rect, CircularProgressBarDrawer.Texture);
                GUI.color = Color.white;
                return;
            }
            bool flag = GUIcachedID == null || CachedGUI.BeginCachedGUI(rect.ExpandedBy(2f), GUIcachedID.Value, true);
            if (flag)
            {
                GUI.color = CircularProgressBarDrawer.NotFilledColor;
                GUIExtra.DrawTextureWithTexCoords(rect.RightHalf().MovedBy(-1f, 0f), CircularProgressBarDrawer.Texture, new Rect(0.5f, 0f, 0.5f, 1f));
                GUIExtra.DrawTextureWithTexCoords(rect.RightHalf().MovedBy(1f, 0f), CircularProgressBarDrawer.Texture, new Rect(0.5f, 0f, 0.5f, 1f));
                GUIExtra.DrawTextureWithTexCoords(rect.RightHalf().MovedBy(0f, -1f), CircularProgressBarDrawer.Texture, new Rect(0.5f, 0f, 0.5f, 1f));
                GUIExtra.DrawTextureWithTexCoords(rect.RightHalf().MovedBy(0f, 1f), CircularProgressBarDrawer.Texture, new Rect(0.5f, 0f, 0.5f, 1f));
                GUI.color = CircularProgressBarDrawer.FilledColor;
                GUIExtra.DrawTextureWithTexCoordsRotated(rect.LeftHalf(), CircularProgressBarDrawer.Texture, new Rect(0f, 0f, 0.5f, 1f), Math.Min(pct / 0.5f * 180f, 180f), new Vector2?(rect.center));
                GUI.color = CircularProgressBarDrawer.NotFilledColor;
                GUIExtra.DrawTextureWithTexCoords(rect.LeftHalf().MovedBy(-1f, 0f), CircularProgressBarDrawer.Texture, new Rect(0f, 0f, 0.5f, 1f));
                GUIExtra.DrawTextureWithTexCoords(rect.LeftHalf().MovedBy(1f, 0f), CircularProgressBarDrawer.Texture, new Rect(0f, 0f, 0.5f, 1f));
                GUIExtra.DrawTextureWithTexCoords(rect.LeftHalf().MovedBy(0f, -1f), CircularProgressBarDrawer.Texture, new Rect(0f, 0f, 0.5f, 1f));
                GUIExtra.DrawTextureWithTexCoords(rect.LeftHalf().MovedBy(0f, 1f), CircularProgressBarDrawer.Texture, new Rect(0f, 0f, 0.5f, 1f));
                if (pct > 0.5f)
                {
                    GUI.color = CircularProgressBarDrawer.FilledColor;
                    GUIExtra.DrawTextureWithTexCoordsRotated(rect.RightHalf(), CircularProgressBarDrawer.Texture, new Rect(0.5f, 0f, 0.5f, 1f), Math.Min((pct - 0.5f) / 0.5f * 180f, 180f), new Vector2?(rect.center));
                }
                GUI.color = Color.white;
            }
            if (GUIcachedID != null)
            {
                CachedGUI.EndCachedGUI(alpha, 1f);
            }
        }

        private static readonly Color FilledColor = new Color(0.95f, 0.95f, 0.95f);

        private static readonly Color NotFilledColor = new Color(0.25f, 0.25f, 0.25f);

        public static readonly Texture2D Texture = Assets.Get<Texture2D>("Textures/UI/CircularProgressBar");
    }
}