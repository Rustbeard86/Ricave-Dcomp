using System;
using System.Collections.Generic;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public class PlayerHitMarkers
    {
        public void OnGUI()
        {
            if (Event.current.type != EventType.Repaint)
            {
                return;
            }
            if (this.markers.Count == 0)
            {
                return;
            }
            float num = Math.Min(Widgets.VirtualWidth, Widgets.VirtualHeight) * 0.7f;
            float num2 = num / 2048f * (float)PlayerHitMarkers.Texture.width;
            float num3 = num2 * ((float)PlayerHitMarkers.Texture.height / (float)PlayerHitMarkers.Texture.width);
            Vector2 vector = Widgets.ScreenCenter.WithAddedY(-num / 2f);
            Rect rect = new Rect(vector.x - num2 / 2f, vector.y, num2, num3);
            for (int i = this.markers.Count - 1; i >= 0; i--)
            {
                PlayerHitMarkers.Marker marker = this.markers[i];
                if (Clock.UnscaledTime - marker.startTime >= 2f)
                {
                    this.markers.RemoveAt(i);
                }
                else
                {
                    float angleXZFromCamera_Accurate = Vector3Utility.GetAngleXZFromCamera_Accurate(marker.position);
                    float alpha = this.GetAlpha(marker);
                    GUI.color = marker.color.WithAlphaFactor(alpha);
                    GUIExtra.DrawTextureRotated(rect, PlayerHitMarkers.Texture, angleXZFromCamera_Accurate, new Vector2?(Widgets.ScreenCenter));
                }
            }
            GUI.color = Color.white;
        }

        public void Add(Vector3 position, Color color)
        {
            PlayerHitMarkers.Marker marker = default(PlayerHitMarkers.Marker);
            marker.startTime = Clock.UnscaledTime;
            marker.position = position;
            marker.color = color;
            this.markers.Add(marker);
        }

        private float GetAlpha(PlayerHitMarkers.Marker marker)
        {
            return Calc.ResolveFadeInStayOut(Clock.UnscaledTime - marker.startTime, 0f, 0.5f, 1.5f);
        }

        public void OnSwitchedNowControlledActor()
        {
            this.markers.Clear();
        }

        private List<PlayerHitMarkers.Marker> markers = new List<PlayerHitMarkers.Marker>();

        private const float FadeInTime = 0f;

        private const float StayTime = 0.5f;

        private const float FadeOutTime = 1.5f;

        private const int OriginalFullCircleTextureSize = 2048;

        private const float CircleSizeScreenPct = 0.7f;

        private static readonly Texture2D Texture = Assets.Get<Texture2D>("Textures/UI/PlayerHitMarker");

        private struct Marker
        {
            public Vector3 position;

            public float startTime;

            public Color color;
        }
    }
}