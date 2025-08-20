using System;
using Ricave.Core;
using UnityEngine;

namespace Ricave.UI
{
    public static class FPSCounter
    {
        public static void OnGUI()
        {
            if (!PrefsHelper.ShowFPS)
            {
                return;
            }
            if (Event.current.type != EventType.Repaint)
            {
                return;
            }
            FPSCounter.totalTimeThisBatch += Clock.UnscaledDeltaTime;
            FPSCounter.framesThisBatch++;
            if (FPSCounter.totalTimeThisBatch >= 0.15f && FPSCounter.framesThisBatch >= 3)
            {
                FPSCounter.lastAverageTimePerFrame = FPSCounter.totalTimeThisBatch / (float)FPSCounter.framesThisBatch;
                FPSCounter.lastAverageFPSStr = Calc.RoundToInt(1f / FPSCounter.lastAverageTimePerFrame).ToStringCached();
                FPSCounter.totalTimeThisBatch = 0f;
                FPSCounter.framesThisBatch = 0;
            }
            Widgets.Align = TextAnchor.UpperRight;
            Widgets.Label(new Rect(Widgets.VirtualWidth - 50f - 2f, 2f, 50f, 50f), FPSCounter.lastAverageFPSStr, true, null, null, false);
            Widgets.ResetAlign();
        }

        private static float lastAverageTimePerFrame;

        private static string lastAverageFPSStr;

        private static float totalTimeThisBatch;

        private static int framesThisBatch;

        private const float Interval = 0.15f;

        private const int Margin = 2;
    }
}