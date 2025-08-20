using System;
using UnityEngine;

namespace Ricave.Rendering
{
    public static class Sys
    {
        public static bool IsLaptop
        {
            get
            {
                return SystemInfo.batteryStatus > BatteryStatus.Unknown;
            }
        }

        public static bool InstancingSupported
        {
            get
            {
                return SystemInfo.supportsInstancing;
            }
        }

        public static Vector2Int Resolution
        {
            get
            {
                return new Vector2Int(Screen.width, Screen.height);
            }
        }

        public static bool FullScreen
        {
            get
            {
                return Screen.fullScreen;
            }
        }

        public static void OpenURL(string url)
        {
            Application.OpenURL(url);
        }

        public static void SetResolution(int width, int height, bool fullscreen)
        {
            Screen.SetResolution(width, height, fullscreen);
        }
    }
}