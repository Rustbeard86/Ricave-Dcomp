using System;
using UnityEngine;

namespace Ricave.Core
{
    public static class Clock
    {
        public static int Frame
        {
            get
            {
                return global::UnityEngine.Time.frameCount;
            }
        }

        public static float Time
        {
            get
            {
                return global::UnityEngine.Time.time;
            }
        }

        public static float DeltaTime
        {
            get
            {
                return global::UnityEngine.Time.deltaTime;
            }
        }

        public static float FixedDeltaTime
        {
            get
            {
                return global::UnityEngine.Time.fixedDeltaTime;
            }
        }

        public static float TimeSinceSceneLoad
        {
            get
            {
                return global::UnityEngine.Time.timeSinceLevelLoad;
            }
        }

        public static bool InFixedTimeStep
        {
            get
            {
                return global::UnityEngine.Time.inFixedTimeStep;
            }
        }

        public static float TimeScale
        {
            get
            {
                return global::UnityEngine.Time.timeScale;
            }
            set
            {
                global::UnityEngine.Time.timeScale = value;
            }
        }

        public static float UnscaledDeltaTime
        {
            get
            {
                return global::UnityEngine.Time.unscaledDeltaTime;
            }
        }

        public static float UnscaledTime
        {
            get
            {
                return global::UnityEngine.Time.unscaledTime;
            }
        }

        public static float FixedUnscaledDeltaTime
        {
            get
            {
                return global::UnityEngine.Time.fixedUnscaledDeltaTime;
            }
        }

        public static float FixedUnscaledTime
        {
            get
            {
                return global::UnityEngine.Time.fixedUnscaledTime;
            }
        }

        public static float TrailerFakeAdjustment
        {
            get
            {
                return 1f;
            }
        }
    }
}