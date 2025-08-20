using System;

namespace Ricave.Core
{
    public static class InterpolationUtility
    {
        public static float TimeToNextFixedUpdate
        {
            get
            {
                if (Clock.InFixedTimeStep)
                {
                    return Clock.FixedDeltaTime;
                }
                return Math.Max(Clock.FixedDeltaTime - (Clock.Time - Root.LastFixedUpdateTime), 0.0001f);
            }
        }
    }
}