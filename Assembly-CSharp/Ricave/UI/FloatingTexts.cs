using System;
using System.Collections.Generic;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public class FloatingTexts
    {
        public void Add(string text, Vector3 pos, Color color, float scale = 1f, float offset = 0f)
        {
            this.texts.Add(new FloatingTexts.FloatingText
            {
                text = text,
                startTime = Clock.Time,
                pos = pos,
                color = color,
                scale = scale,
                offset = offset
            });
        }

        public void OnGUI()
        {
            if (Event.current.type != EventType.Repaint)
            {
                return;
            }
            if (this.texts.Count == 0)
            {
                return;
            }
            if (this.texts.Count >= 2)
            {
                this.texts.Sort(FloatingTexts.ByDistToCamera);
            }
            int i = 0;
            while (i < this.texts.Count)
            {
                FloatingTexts.FloatingText floatingText = this.texts[i];
                floatingText.pos += this.GetMoveDir(floatingText) * 0.75f * Clock.DeltaTime;
                float num = Clock.Time - floatingText.startTime;
                if (num > 0.18f)
                {
                    floatingText.pos = floatingText.pos.WithAddedY(Clock.DeltaTime * Calc.LerpDouble(0f, 3f, 0f, -6.3f, num));
                }
                if (num <= 0.45f)
                {
                    floatingText.alpha += 2f * Clock.DeltaTime;
                    goto IL_011D;
                }
                floatingText.alpha -= 1.8f * Clock.DeltaTime;
                if (floatingText.alpha > 0f)
                {
                    goto IL_011D;
                }
                this.texts.RemoveAt(i);
                i--;
            IL_01EE:
                i++;
                continue;
            IL_011D:
                floatingText.alpha = Calc.Clamp01(floatingText.alpha);
                this.texts[i] = floatingText;
                float num2 = 0.26f * floatingText.scale * Calc.Pow(floatingText.alpha, 0.3f);
                Rect rect;
                if (WorldRectToUIRectUtility.GetUIRect(floatingText.pos, new Vector2(num2, num2), out rect))
                {
                    rect.y += floatingText.offset;
                    GUI.color = new Color(floatingText.color.r, floatingText.color.g, floatingText.color.b, floatingText.color.a);
                    Widgets.FontSize = Widgets.GetFontSizeToFitInHeight(rect.height);
                    Widgets.LabelCentered(rect.center, floatingText.text, true, null, null, false, false, false, null);
                    goto IL_01EE;
                }
                goto IL_01EE;
            }
            Widgets.ResetFontSize();
            GUI.color = Color.white;
        }

        private Vector3 GetMoveDir(FloatingTexts.FloatingText text)
        {
            float num = Clock.Time / 3f;
            float num2 = text.startTime * 10f + (float)(Calc.AbsSafe(text.text.StableHashCode()) % 1000);
            return new Vector3(Noise.PerlinNoise(num, num2) * 2f - 1f, Noise.PerlinNoise(num, num2 + 1000f) * 2f - 1f + 1f, Noise.PerlinNoise(num, num2 + 2000f) * 2f - 1f).normalized;
        }

        public void OnWorldAboutToRegenerate()
        {
            this.texts.Clear();
        }

        private List<FloatingTexts.FloatingText> texts = new List<FloatingTexts.FloatingText>();

        private const float Duration = 0.45f;

        private const float FadeInSpeed = 2f;

        private const float FadeOutSpeed = 1.8f;

        private const float MoveSpeed = 0.75f;

        private const float SizeInWorldUnits = 0.26f;

        private static readonly Comparison<FloatingTexts.FloatingText> ByDistToCamera = (FloatingTexts.FloatingText a, FloatingTexts.FloatingText b) => (b.pos - Get.CameraPosition).sqrMagnitude.CompareTo((a.pos - Get.CameraPosition).sqrMagnitude);

        private struct FloatingText
        {
            public string text;

            public float startTime;

            public Vector3 pos;

            public float alpha;

            public Color color;

            public float scale;

            public float offset;
        }
    }
}