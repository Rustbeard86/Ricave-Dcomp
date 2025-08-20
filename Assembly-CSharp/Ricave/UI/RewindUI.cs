using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public class RewindUI
    {
        public static string RewindTipBase
        {
            get
            {
                return "RewindTip".Translate(3).FormattedKeyBindings();
            }
        }

        public void StartRewindAnimation()
        {
            this.lastRewindTime = Clock.UnscaledTime;
        }

        public void StartReplenishAnimation()
        {
            this.lastReplenishTime = Clock.UnscaledTime;
        }

        public void OnGUI()
        {
            if (Event.current.type != EventType.Repaint && Event.current.type != EventType.MouseDown)
            {
                return;
            }
            if (!Get.Player.HasWatch)
            {
                return;
            }
            bool flag;
            if (Clock.UnscaledTime - Get.CameraEffects.LastRewindTime < 0.3f)
            {
                this.centeredRewindIconAlpha = Math.Min(this.centeredRewindIconAlpha + Clock.UnscaledDeltaTime * 5f, 1f);
                flag = true;
            }
            else
            {
                this.centeredRewindIconAlpha = Math.Max(this.centeredRewindIconAlpha - Clock.UnscaledDeltaTime * 2f, 0f);
                flag = false;
            }
            if (this.centeredRewindIconAlpha > 0f)
            {
                float num = Math.Min(Widgets.VirtualWidth, Widgets.VirtualHeight) * 0.2f;
                float num2 = (flag ? 0f : ((1f - this.centeredRewindIconAlpha) * -0.15f));
                GUI.color = new Color(1f, 1f, 1f, this.centeredRewindIconAlpha);
                GUI.DrawTexture(RectUtility.CenteredAt(Widgets.ScreenCenter, num).MovedByPct(num2, 0f), RewindUI.CenteredIcon);
                GUI.color = Color.white;
            }
            Rect rect = new Rect(25f, Widgets.VirtualHeight - 25f - 80f - 42f, 80f, 122f).ExpandedBy(12f, 0f);
            GUIExtra.DrawHighlightIfMouseover(rect, true, true, true, true, true);
            if (Mouse.Over(rect))
            {
                string text = RewindUI.RewindTipBase;
                if (Get.Player.TurnsCanRewind < Get.Player.MaxTurnsCanRewind)
                {
                    text = text.AppendedInDoubleNewLine("RewindReplenishTip".Translate(RichText.Turns(StringUtility.TurnsString(3 - Get.Player.ReplenishTurnsCanRewindTurnsPassed))));
                }
                Get.Tooltips.RegisterTip(rect, text, null);
            }
            float num3 = Math.Max(1f - (Clock.UnscaledTime - this.lastReplenishTime) / 0.3f, 0f);
            if (num3 != this.cachedForReplenishAnimation || this.cachedForTurnsCanRewind != Get.Player.TurnsCanRewind || this.cachedForMaxTurnsCanRewind != Get.Player.MaxTurnsCanRewind)
            {
                this.cachedForReplenishAnimation = num3;
                this.cachedForTurnsCanRewind = Get.Player.TurnsCanRewind;
                this.cachedForMaxTurnsCanRewind = Get.Player.MaxTurnsCanRewind;
                CachedGUI.SetDirty(5);
            }
            Rect rect2 = new Rect(25f, Widgets.VirtualHeight - 25f - 80f, 80f, 80f);
            if (CachedGUI.BeginCachedGUI(rect, 5, true))
            {
                if (Get.Player.TurnsCanRewind <= 0)
                {
                    GUI.color = new Color(0.9f, 0.35f, 0.35f);
                }
                GUIExtra.DrawTexture(rect2, RewindUI.Icon1);
                string text2 = this.labelsCache.Get(new ValueTuple<int, int>(Get.Player.TurnsCanRewind, Get.Player.MaxTurnsCanRewind));
                Widgets.FontSizeScalable = 22;
                Widgets.FontBold = true;
                if (Get.Player.TurnsCanRewind <= 0)
                {
                    GUI.color = new Color(0.9f, 0.35f, 0.35f);
                }
                else
                {
                    GUI.color = new Color(0.85f, 0.85f, 0.85f).Lighter(num3 * 0.15f);
                }
                Vector2 vector = rect2.center.WithAddedY(-65f);
                vector.y -= num3 * 4f;
                Widgets.LabelCentered(vector, text2, true, null, null, false, false, false, null);
                GUI.color = Color.white;
                Widgets.FontBold = false;
                Widgets.ResetFontSize();
            }
            CachedGUI.EndCachedGUI(1f, 1f);
            ExpandingIconAnimation.Do(rect2, RewindUI.Icon2, Color.white, this.lastRewindTime, 2f, 0.3f, 1f);
            if (Event.current.type == EventType.MouseDown && Event.current.button == 1 && Mouse.Over(rect))
            {
                List<ValueTuple<string, Action>> list = new List<ValueTuple<string, Action>>();
                if (Get.Player.TurnsCanRewind > 0)
                {
                    list.Add(new ValueTuple<string, Action>("RewindTurns".Translate(StringUtility.TurnsString(1)), delegate
                    {
                        RewindUI.< OnGUI > g__Rewind | 19_0(1);
                    }));
                    if (Get.Player.TurnsCanRewind > 1)
                    {
                        list.Add(new ValueTuple<string, Action>("RewindTurns".Translate(StringUtility.TurnsString(Get.Player.TurnsCanRewind)), delegate
                        {
                            RewindUI.< OnGUI > g__Rewind | 19_0(Get.Player.TurnsCanRewind);
                        }));
                    }
                }
                Get.WindowManager.OpenContextMenu(list, Get.Entity_Watch.LabelAdjustedCap);
                Event.current.Use();
            }
        }

        [CompilerGenerated]
        internal static void <OnGUI>g__Rewind|19_0(int turns)
		{
			Get.Sound_Click.PlayOneShot(null, 1f, 1f);
			ActionViaInterfaceHelper.TryDo(() => new Action_RewindTime(Get.Action_RewindTime, turns));
		}

		private float lastRewindTime = -9999f;

        private float lastReplenishTime = -9999f;

        private readonly CalculationCache<ValueTuple<int, int>, string> labelsCache = new CalculationCache<ValueTuple<int, int>, string>((ValueTuple<int, int> x) => x.Item1.ToStringCached() + " / " + x.Item2.ToStringCached(), 300);

        private float cachedForReplenishAnimation = -1f;

        private int cachedForTurnsCanRewind = -1;

        private int cachedForMaxTurnsCanRewind = -1;

        private float centeredRewindIconAlpha;

        private const int IconSize = 80;

        private const int Margin = 25;

        private const float RewindAnimationDuration = 0.3f;

        private const float ReplenishAnimationDuration = 0.3f;

        private static readonly Texture2D CenteredIcon = Assets.Get<Texture2D>("Textures/UI/Rewind");

        private static readonly Texture2D Icon1 = Assets.Get<Texture2D>("Textures/UI/Rewind1");

        private static readonly Texture2D Icon2 = Assets.Get<Texture2D>("Textures/UI/Rewind2");

        private const float CenteredRewindIconSizeScreenPct = 0.2f;
    }
}