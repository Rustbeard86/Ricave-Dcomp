using System;
using System.Collections.Generic;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public class RandomTip
    {
        private static string TipToShow
        {
            get
            {
                if (DebugUI.TrailerMode)
                {
                    return null;
                }
                if (Get.LessonManager.CurrentLesson != null)
                {
                    return null;
                }
                if (Get.TotalKillCounter.KillCount == 0)
                {
                    return "FirstTip".Translate().FormattedKeyBindings();
                }
                List<TipsSpec> all = Get.Specs.GetAll<TipsSpec>();
                int num = 0;
                for (int i = 0; i < all.Count; i++)
                {
                    List<string> allowedTips = RandomTip.GetAllowedTips(all[i]);
                    num += allowedTips.Count;
                }
                if (num == 0)
                {
                    return null;
                }
                int num2 = Rand.RangeInclusiveSeeded(0, num - 1, Calc.CombineHashes<int, int, int>(Get.RunInfo.GlobalRandomID, Get.WorldConfig.WorldSeed, 952786091));
                for (int j = 0; j < all.Count; j++)
                {
                    List<string> allowedTips2 = RandomTip.GetAllowedTips(all[j]);
                    if (num2 < allowedTips2.Count)
                    {
                        return allowedTips2[num2].FormattedKeyBindings();
                    }
                    num2 -= allowedTips2.Count;
                }
                Log.Error("Could not find the chosen tip.", false);
                return null;
            }
        }

        public void Show()
        {
            this.startTime = Clock.UnscaledTime;
        }

        public void OnGUI()
        {
            if (Event.current.type != EventType.Repaint)
            {
                return;
            }
            float num = Calc.ResolveFadeInStayOut(Clock.UnscaledTime - this.startTime, 3f, 7.9f, 4.2f);
            if (num <= 0f)
            {
                return;
            }
            string tipToShow = RandomTip.TipToShow;
            if (tipToShow.NullOrEmpty())
            {
                return;
            }
            if (RandomTip.lastTip != tipToShow)
            {
                RandomTip.lastTip = tipToShow;
                CachedGUI.SetDirty(10);
            }
            Rect rect = RectUtility.CenteredAt(Widgets.ScreenCenter.WithY(Widgets.VirtualHeight - 210f), 900f, 100f);
            if (CachedGUI.BeginCachedGUI(rect, 10, true))
            {
                Widgets.FontSizeScalable = 19;
                Widgets.LabelCentered(rect, "RandomTip".Translate(tipToShow), true, null, null);
                Widgets.ResetFontSize();
            }
            CachedGUI.EndCachedGUI(num, 1f);
        }

        private static List<string> GetAllowedTips(TipsSpec tips)
        {
            if (!ControllerUtility.InControllerMode)
            {
                return tips.Tips;
            }
            if (tips == Get.Tips_Interface)
            {
                return EmptyLists<string>.Get();
            }
            List<string> list = FrameLocalPool<List<string>>.Get();
            List<string> tips2 = tips.Tips;
            for (int i = 0; i < tips2.Count; i++)
            {
                if (!KeyCodeUtility.HasKeyBindingSymbols(tips2[i]))
                {
                    list.Add(tips2[i]);
                }
            }
            return list;
        }

        private float startTime = -9999f;

        private const float FadeInTime = 3f;

        private const float StayTime = 7.9f;

        private const float FadeOutTime = 4.2f;

        private static string lastTip;
    }
}