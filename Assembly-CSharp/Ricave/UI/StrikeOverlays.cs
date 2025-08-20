using System;
using System.Collections.Generic;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public class StrikeOverlays
    {
        public void OnGUI()
        {
            if (Event.current.type != EventType.Repaint)
            {
                return;
            }
            float num = Widgets.VirtualHeight * 1.2f;
            float num2 = (float)StrikeOverlays.StrikeOverlay.width / (float)StrikeOverlays.StrikeOverlay.height * num;
            for (int i = this.overlays.Count - 1; i >= 0; i--)
            {
                StrikeOverlays.Overlay overlay = this.overlays[i];
                if (Clock.Time - overlay.startTime >= 0.28100002f)
                {
                    this.overlays.RemoveAt(i);
                }
            }
            for (int j = 0; j < this.overlays.Count; j++)
            {
                StrikeOverlays.Overlay overlay2 = this.overlays[j];
                float sizeFactor = this.GetSizeFactor(overlay2);
                float num3;
                Rect rect;
                Vector2? vector;
                Rect rect2;
                if (overlay2.horizontal)
                {
                    num3 = (overlay2.mirror ? 270f : 90f);
                    rect = new Rect(Widgets.ScreenCenter.x - num2 / 2f, Widgets.ScreenCenter.y - num / 2f, num2, num * sizeFactor);
                    vector = new Vector2?(new Vector2(Widgets.ScreenCenter.x, Widgets.ScreenCenter.y));
                    rect2 = new Rect(0f, 1f - sizeFactor, 1f, sizeFactor);
                }
                else
                {
                    num3 = 0f;
                    vector = null;
                    float num4 = -0.235f * num2;
                    if (overlay2.mirror)
                    {
                        num4 = -num4;
                    }
                    rect = new Rect(Widgets.ScreenCenter.x - num2 / 2f + num4, Widgets.ScreenCenter.y - num / 2f - 0.1f * num, num2, num * sizeFactor);
                    rect2 = new Rect(overlay2.mirror ? 1f : 0f, 1f - sizeFactor, overlay2.mirror ? (-1f) : 1f, sizeFactor);
                }
                GUI.color = new Color(1f, 1f, 1f, this.GetAlpha(overlay2));
                GUIExtra.DrawTextureWithTexCoordsRotated(rect, StrikeOverlays.StrikeOverlay, rect2, num3, vector);
            }
            GUI.color = Color.white;
        }

        public void Strike(bool horizontal = false)
        {
            StrikeOverlays.Overlay overlay = default(StrikeOverlays.Overlay);
            overlay.startTime = Clock.Time;
            overlay.startUnscaledTime = Clock.UnscaledTime;
            overlay.mirror = this.nextMirror;
            overlay.horizontal = horizontal;
            this.overlays.Add(overlay);
            this.ToggleNextMirror();
        }

        public void ToggleNextMirror()
        {
            this.nextMirror = !this.nextMirror;
        }

        private float GetSizeFactor(StrikeOverlays.Overlay overlay)
        {
            return Math.Min((Clock.UnscaledTime - overlay.startUnscaledTime) / 0.1f, 1f);
        }

        private float GetAlpha(StrikeOverlays.Overlay overlay)
        {
            float num = Clock.Time - overlay.startTime;
            float num2 = Calc.ResolveFadeInStayOut(num, 0.1f, 0.001f, 0.18f);
            if (num < 0.101f)
            {
                float num3 = Clock.UnscaledTime - overlay.startUnscaledTime;
                num2 = Math.Max(num2, Calc.ResolveFadeIn(num3, 0.1f));
            }
            return num2;
        }

        public void OnSwitchedNowControlledActor()
        {
            this.overlays.Clear();
        }

        private List<StrikeOverlays.Overlay> overlays = new List<StrikeOverlays.Overlay>();

        private bool nextMirror;

        private const float HeightScreenPct = 1.2f;

        private const float FullSizeAfterSeconds = 0.1f;

        private const float FadeInTime = 0.1f;

        private const float StayTime = 0.001f;

        private const float FadeOutTime = 0.18f;

        private const float OffsetXPctMirrorable = 0.235f;

        private const float OffsetYPct = 0.1f;

        private static readonly Texture2D StrikeOverlay = Assets.Get<Texture2D>("Textures/UI/Strike");

        private struct Overlay
        {
            public bool mirror;

            public bool horizontal;

            public float startTime;

            public float startUnscaledTime;
        }
    }
}