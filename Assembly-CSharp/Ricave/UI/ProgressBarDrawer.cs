using System;
using System.Collections.Generic;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public class ProgressBarDrawer
    {
        public void Draw(Rect rect, int value, int maxValue, Color color, float alpha = 1f, float labelAlpha = 1f, bool doBackground = true, bool doLabel = true, int? ID = null, ProgressBarDrawer.ValueChangeDirection valueChangeDirection = ProgressBarDrawer.ValueChangeDirection.Constant, IntRange? flashingRange = null, bool tl = true, bool tr = true, bool br = true, bool bl = true, bool bump = true, bool labelBold = false, string customLabel = null)
        {
            this.Draw(rect, (float)value / (float)maxValue, color, customLabel ?? (doLabel ? this.labelsCache.Get(new ValueTuple<int, int>(value, maxValue)) : null), alpha, labelAlpha, doBackground, ID, valueChangeDirection, (flashingRange != null) ? new FloatRange?(new FloatRange((float)flashingRange.Value.from / (float)maxValue, (float)flashingRange.Value.to / (float)maxValue)) : null, tl, tr, br, bl, bump, labelBold);
        }

        public void Draw(Rect rect, float progress, Color color, string label = null, float alpha = 1f, float labelAlpha = 1f, bool doBackground = true, int? ID = null, ProgressBarDrawer.ValueChangeDirection valueChangeDirection = ProgressBarDrawer.ValueChangeDirection.Constant, FloatRange? flashingRange = null, bool tl = true, bool tr = true, bool br = true, bool bl = true, bool bump = true, bool labelBold = false)
        {
            progress = Calc.Clamp01(progress);
            if (doBackground)
            {
                if (bump)
                {
                    GUIExtra.DrawRoundedRectBump(rect, new Color(0.4f, 0.4f, 0.4f, alpha), false, tl, tr, br, bl, null);
                }
                else
                {
                    GUIExtra.DrawRoundedRect(rect, new Color(0.4f, 0.4f, 0.4f, alpha), tl, tr, br, bl, null);
                }
            }
            float num = progress;
            float num2 = progress;
            if (ID != null)
            {
                bool flag = false;
                for (int i = 0; i < this.animatedProgress.Count; i++)
                {
                    int id = this.animatedProgress[i].ID;
                    int? num3 = ID;
                    if ((id == num3.GetValueOrDefault()) & (num3 != null))
                    {
                        ProgressBarDrawer.AnimationProgress animationProgress = this.animatedProgress[i];
                        num = Math.Min(progress, animationProgress.animated);
                        num2 = Math.Max(progress, animationProgress.animated);
                        animationProgress.target = progress;
                        animationProgress.lastFrameDrawn = Clock.Frame;
                        this.animatedProgress[i] = animationProgress;
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    this.animatedProgress.Add(new ProgressBarDrawer.AnimationProgress
                    {
                        ID = ID.Value,
                        target = progress,
                        animated = progress,
                        lastFrameDrawn = Clock.Frame
                    });
                }
            }
            if (num2 > 1E-45f)
            {
                color.a *= alpha;
                float num4 = ((num >= 1f) ? rect.width : Calc.Floor(num * rect.width));
                float num5 = ((num2 >= 1f) ? rect.width : Calc.Floor(num2 * rect.width));
                if (num5 - num4 >= 1f)
                {
                    if (bump)
                    {
                        GUIExtra.DrawRoundedRectBump(new Rect(rect.x + num4, rect.y, num5 - num4, rect.height), color.Darker(0.15f).WithAlphaFactor(0.53f), false, tl && Calc.Approximately(num4, 0f), tr && num2 >= 0.995f, br && num2 >= 0.995f, bl && Calc.Approximately(num4, 0f), null);
                    }
                    else
                    {
                        GUIExtra.DrawRoundedRect(new Rect(rect.x + num4, rect.y, num5 - num4, rect.height), color.Darker(0.15f).WithAlphaFactor(0.53f), tl && Calc.Approximately(num4, 0f), tr && num2 >= 0.995f, br && num2 >= 0.995f, bl && Calc.Approximately(num4, 0f), null);
                    }
                }
                if (!Calc.Approximately(num4, 0f))
                {
                    if (bump)
                    {
                        GUIExtra.DrawRoundedRectBump(new Rect(rect.x, rect.y, num4, rect.height), color, false, tl, tr && num >= 0.995f, br && num >= 0.995f, bl, null);
                    }
                    else
                    {
                        GUIExtra.DrawRoundedRect(new Rect(rect.x, rect.y, num4, rect.height), color, tl, tr && num >= 0.995f, br && num >= 0.995f, bl, null);
                    }
                }
            }
            if (flashingRange != null && flashingRange.Value != default(FloatRange))
            {
                float num6 = Math.Max(progress - flashingRange.Value.to, 0f);
                float num7 = Math.Max(progress - flashingRange.Value.from, 0f);
                float num8 = Calc.Sin(Clock.UnscaledTime * 8f) * 0.2f;
                GUI.color = new Color(0.8f, 0.8f, 0.8f, 0.8f + num8);
                GUIExtra.DrawTexture(new Rect(rect.x + num6 * rect.width - 4f, rect.yMax - 8f, 8f, 8f), ProgressBarDrawer.SmallArrowUp);
                GUIExtra.DrawTexture(new Rect(rect.x + num7 * rect.width - 4f, rect.y, 8f, 8f), ProgressBarDrawer.SmallArrowDown);
                GUI.color = Color.white;
            }
            float num9 = alpha * labelAlpha;
            if (num9 > 0f)
            {
                Texture2D texture2D;
                if (valueChangeDirection == ProgressBarDrawer.ValueChangeDirection.Up)
                {
                    texture2D = ProgressBarDrawer.SmallArrowUp;
                    GUI.color = new Color(0.73f, 0.87f, 0.73f, num9);
                }
                else if (valueChangeDirection == ProgressBarDrawer.ValueChangeDirection.Down)
                {
                    texture2D = ProgressBarDrawer.SmallArrowDown;
                    GUI.color = new Color(0.87f, 0.73f, 0.73f, num9);
                }
                else
                {
                    texture2D = null;
                }
                if (texture2D != null)
                {
                    GUIExtra.DrawTexture(rect.RightPart(rect.height).ContractedBy(rect.height * 0.32f), texture2D);
                    GUI.color = Color.white;
                }
                if (!label.NullOrEmpty())
                {
                    GUI.color = new Color(0.9f, 0.9f, 0.9f, num9);
                    if (labelBold)
                    {
                        Widgets.FontBold = true;
                    }
                    Widgets.LabelCentered(rect.center, label, true, null, null, false, false, false, null);
                    if (labelBold)
                    {
                        Widgets.FontBold = false;
                    }
                    GUI.color = Color.white;
                }
            }
        }

        public void FixedUpdate()
        {
            for (int i = this.animatedProgress.Count - 1; i >= 0; i--)
            {
                ProgressBarDrawer.AnimationProgress animationProgress = this.animatedProgress[i];
                if (Clock.Frame > animationProgress.lastFrameDrawn + 1)
                {
                    this.animatedProgress.RemoveAt(i);
                }
                else
                {
                    if (Math.Abs(animationProgress.animated - animationProgress.target) < 0.0001f)
                    {
                        animationProgress.animated = animationProgress.target;
                    }
                    else
                    {
                        animationProgress.animated = Calc.Lerp(animationProgress.animated, animationProgress.target, 0.1f);
                    }
                    this.animatedProgress[i] = animationProgress;
                }
            }
        }

        private readonly CalculationCache<ValueTuple<int, int>, string> labelsCache = new CalculationCache<ValueTuple<int, int>, string>((ValueTuple<int, int> x) => x.Item1.ToStringCached() + " / " + x.Item2.ToStringCached(), 300);

        private List<ProgressBarDrawer.AnimationProgress> animatedProgress = new List<ProgressBarDrawer.AnimationProgress>();

        private const float AnimationLerpSpeed = 0.1f;

        private static readonly Texture2D SmallArrowUp = Assets.Get<Texture2D>("Textures/UI/SmallArrowUp");

        private static readonly Texture2D SmallArrowDown = Assets.Get<Texture2D>("Textures/UI/SmallArrowDown");

        private struct AnimationProgress
        {
            public int ID;

            public float target;

            public float animated;

            public int lastFrameDrawn;
        }

        public enum ValueChangeDirection
        {
            Constant,

            Down,

            Up
        }
    }
}