using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public static class FrameLocalPools
    {
        public static void OnNewFrame()
        {
            for (int i = 0; i < FrameLocalPools.onNewFrame.Count; i++)
            {
                FrameLocalPools.onNewFrame[i]();
            }
        }

        public static void Clear()
        {
            for (int i = 0; i < FrameLocalPools.clear.Count; i++)
            {
                FrameLocalPools.clear[i]();
            }
        }

        public static void RegisterPool(Action onNewFrame, Action clear)
        {
            FrameLocalPools.onNewFrame.Add(onNewFrame);
            FrameLocalPools.clear.Add(clear);
        }

        private static List<Action> onNewFrame = new List<Action>();

        private static List<Action> clear = new List<Action>();
    }
}