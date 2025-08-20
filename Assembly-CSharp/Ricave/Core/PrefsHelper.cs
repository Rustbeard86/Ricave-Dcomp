using System;
using Ricave.Rendering;
using Ricave.UI;

namespace Ricave.Core
{
    public static class PrefsHelper
    {
        public static float UIScale
        {
            get
            {
                return PrefsHelper.uiScale;
            }
            set
            {
                if (value == PrefsHelper.uiScale)
                {
                    return;
                }
                PrefsHelper.uiScale = value;
                Prefs.SetFloat("UIScale", PrefsHelper.uiScale);
                Prefs.Save();
                PrefsHelper.Apply();
            }
        }

        public static bool Fullscreen
        {
            get
            {
                return PrefsHelper.fullscreen;
            }
            set
            {
                if (value == PrefsHelper.fullscreen)
                {
                    return;
                }
                PrefsHelper.fullscreen = value;
                Prefs.SetInt("Fullscreen", PrefsHelper.fullscreen ? 1 : 0);
                Prefs.Save();
                PrefsHelper.Apply();
            }
        }

        public static float FOV
        {
            get
            {
                return PrefsHelper.fov;
            }
            set
            {
                if (value == PrefsHelper.fov)
                {
                    return;
                }
                PrefsHelper.fov = value;
                Prefs.SetFloat("FOV", PrefsHelper.fov);
                Prefs.Save();
                PrefsHelper.Apply();
            }
        }

        public static bool VSync
        {
            get
            {
                return PrefsHelper.vsync;
            }
            set
            {
                if (value == PrefsHelper.vsync)
                {
                    return;
                }
                PrefsHelper.vsync = value;
                Prefs.SetInt("VSync", PrefsHelper.vsync ? 1 : 0);
                Prefs.Save();
                PrefsHelper.Apply();
            }
        }

        public static int TargetFramerate
        {
            get
            {
                return PrefsHelper.targetFramerate;
            }
            set
            {
                if (value == PrefsHelper.targetFramerate)
                {
                    return;
                }
                PrefsHelper.targetFramerate = value;
                Prefs.SetInt("TargetFramerate", PrefsHelper.targetFramerate);
                Prefs.Save();
                PrefsHelper.Apply();
            }
        }

        public static bool ShowFPS
        {
            get
            {
                return PrefsHelper.showFPS;
            }
            set
            {
                if (value == PrefsHelper.showFPS)
                {
                    return;
                }
                PrefsHelper.showFPS = value;
                Prefs.SetInt("ShowFPS", PrefsHelper.showFPS ? 1 : 0);
                Prefs.Save();
                PrefsHelper.Apply();
            }
        }

        public static float Volume
        {
            get
            {
                return PrefsHelper.volume;
            }
            set
            {
                if (value == PrefsHelper.volume)
                {
                    return;
                }
                PrefsHelper.volume = value;
                Prefs.SetFloat("Volume", PrefsHelper.volume);
                Prefs.Save();
                PrefsHelper.Apply();
            }
        }

        public static float MusicVolume
        {
            get
            {
                return PrefsHelper.musicVolume;
            }
            set
            {
                if (value == PrefsHelper.musicVolume)
                {
                    return;
                }
                PrefsHelper.musicVolume = value;
                Prefs.SetFloat("MusicVolume", PrefsHelper.musicVolume);
                Prefs.Save();
                PrefsHelper.Apply();
            }
        }

        public static float MouseSensitivity
        {
            get
            {
                return PrefsHelper.mouseSensitivity;
            }
            set
            {
                if (value == PrefsHelper.mouseSensitivity)
                {
                    return;
                }
                PrefsHelper.mouseSensitivity = value;
                Prefs.SetFloat("MouseSensitivity", PrefsHelper.mouseSensitivity);
                Prefs.Save();
                PrefsHelper.Apply();
            }
        }

        public static bool InvertY
        {
            get
            {
                return PrefsHelper.invertY;
            }
            set
            {
                if (value == PrefsHelper.invertY)
                {
                    return;
                }
                PrefsHelper.invertY = value;
                Prefs.SetInt("InvertY", PrefsHelper.invertY ? 1 : 0);
                Prefs.Save();
                PrefsHelper.Apply();
            }
        }

        public static int SSAOQuality
        {
            get
            {
                return PrefsHelper.ssaoQuality;
            }
            set
            {
                if (value == PrefsHelper.ssaoQuality)
                {
                    return;
                }
                PrefsHelper.ssaoQuality = value;
                Prefs.SetInt("SSAOQuality", PrefsHelper.ssaoQuality);
                Prefs.Save();
                PrefsHelper.Apply();
            }
        }

        public static int ShadowQuality
        {
            get
            {
                return PrefsHelper.shadowQuality;
            }
            set
            {
                if (value == PrefsHelper.shadowQuality)
                {
                    return;
                }
                PrefsHelper.shadowQuality = value;
                Prefs.SetInt("ShadowQuality", PrefsHelper.shadowQuality);
                Prefs.Save();
                PrefsHelper.Apply();
            }
        }

        public static float Downsampling
        {
            get
            {
                return PrefsHelper.downsampling;
            }
            set
            {
                if (value == PrefsHelper.downsampling)
                {
                    return;
                }
                PrefsHelper.downsampling = value;
                Prefs.SetFloat("DownsamplingLevel", PrefsHelper.downsampling);
                Prefs.Save();
                PrefsHelper.Apply();
            }
        }

        public static string Outline
        {
            get
            {
                return PrefsHelper.outline;
            }
            set
            {
                if (value == PrefsHelper.outline)
                {
                    return;
                }
                PrefsHelper.outline = value;
                Prefs.SetString("Outline", PrefsHelper.outline);
                Prefs.Save();
                PrefsHelper.Apply();
            }
        }

        public static int MaxLights
        {
            get
            {
                return PrefsHelper.maxLights;
            }
            set
            {
                if (value == PrefsHelper.maxLights)
                {
                    return;
                }
                PrefsHelper.maxLights = value;
                Prefs.SetInt("MaxLights", PrefsHelper.maxLights);
                Prefs.Save();
                PrefsHelper.Apply();
            }
        }

        public static bool DevMode
        {
            get
            {
                return PrefsHelper.devMode;
            }
            set
            {
                if (value == PrefsHelper.devMode)
                {
                    return;
                }
                PrefsHelper.devMode = value;
                Prefs.SetInt("DevMode", PrefsHelper.devMode ? 1 : 0);
                Prefs.Save();
                PrefsHelper.Apply();
            }
        }

        static PrefsHelper()
        {
            PrefsHelper.ReadAll();
        }

        public static void Apply()
        {
            if ((PrefsHelper.resolutionWidth != 0 && PrefsHelper.resolutionHeight != 0) || PrefsHelper.fullscreen != Sys.FullScreen)
            {
                if (PrefsHelper.resolutionWidth == 0 || PrefsHelper.resolutionHeight == 0)
                {
                    Sys.SetResolution(Sys.Resolution.x, Sys.Resolution.y, PrefsHelper.fullscreen);
                }
                else
                {
                    Sys.SetResolution(PrefsHelper.resolutionWidth, PrefsHelper.resolutionHeight, PrefsHelper.fullscreen);
                }
            }
            Widgets.UIScaleFactor = PrefsHelper.uiScale;
            App.SetQualityLevel();
            if (PrefsHelper.vsync)
            {
                App.VSync = true;
                App.FramerateCap = 0;
            }
            else
            {
                App.VSync = false;
                App.FramerateCap = PrefsHelper.targetFramerate;
            }
            App.MaxLights = PrefsHelper.maxLights;
            App.SetVolume(PrefsHelper.volume);
            switch (PrefsHelper.ssaoQuality)
            {
                case 0:
                    App.SetSSAO_Disabled();
                    break;
                case 1:
                    App.SetSSAO_Low();
                    break;
                case 2:
                    App.SetSSAO_Medium();
                    break;
                case 3:
                    App.SetSSAO_High();
                    break;
            }
            switch (PrefsHelper.shadowQuality)
            {
                case 0:
                    App.SetShadows_Low();
                    break;
                case 1:
                    App.SetShadows_Medium();
                    break;
                case 2:
                    App.SetShadows_High();
                    break;
            }
            App.SetDownsampling(PrefsHelper.downsampling);
        }

        public static void ResetToDefault()
        {
            Prefs.DeleteKey("ResolutionWidth");
            Prefs.DeleteKey("ResolutionHeight");
            Prefs.DeleteKey("UIScale");
            Prefs.DeleteKey("Fullscreen");
            Prefs.DeleteKey("FOV");
            Prefs.DeleteKey("VSync");
            Prefs.DeleteKey("TargetFramerate");
            Prefs.DeleteKey("ShowFPS");
            Prefs.DeleteKey("Volume");
            Prefs.DeleteKey("MusicVolume");
            Prefs.DeleteKey("MouseSensitivity");
            Prefs.DeleteKey("InvertY");
            Prefs.DeleteKey("SSAOQuality");
            Prefs.DeleteKey("ShadowQuality");
            Prefs.DeleteKey("DownsamplingLevel");
            Prefs.DeleteKey("Outline");
            Prefs.DeleteKey("MaxLights");
            Prefs.DeleteKey("DevMode");
            Prefs.Save();
            PrefsHelper.ReadAll();
            PrefsHelper.Apply();
        }

        public static void SetResolution(int width, int height)
        {
            PrefsHelper.resolutionWidth = width;
            PrefsHelper.resolutionHeight = height;
            Prefs.SetInt("ResolutionWidth", PrefsHelper.resolutionWidth);
            Prefs.SetInt("ResolutionHeight", PrefsHelper.resolutionHeight);
            Prefs.Save();
            PrefsHelper.Apply();
        }

        private static void ReadAll()
        {
            PrefsHelper.resolutionWidth = Prefs.GetInt("ResolutionWidth", 0);
            PrefsHelper.resolutionHeight = Prefs.GetInt("ResolutionHeight", 0);
            PrefsHelper.uiScale = Prefs.GetFloat("UIScale", 1f);
            PrefsHelper.fullscreen = Prefs.GetInt("Fullscreen", 1) != 0;
            PrefsHelper.fov = Prefs.GetFloat("FOV", 75f);
            PrefsHelper.vsync = Prefs.GetInt("VSync", 0) != 0;
            PrefsHelper.targetFramerate = Prefs.GetInt("TargetFramerate", 90);
            PrefsHelper.showFPS = Prefs.GetInt("ShowFPS", 0) != 0;
            PrefsHelper.volume = Prefs.GetFloat("Volume", 1f);
            PrefsHelper.musicVolume = Prefs.GetFloat("MusicVolume", 1f);
            PrefsHelper.mouseSensitivity = Prefs.GetFloat("MouseSensitivity", 1f);
            PrefsHelper.invertY = Prefs.GetInt("InvertY", 0) != 0;
            PrefsHelper.ssaoQuality = Prefs.GetInt("SSAOQuality", 2);
            PrefsHelper.shadowQuality = Prefs.GetInt("ShadowQuality", 1);
            PrefsHelper.downsampling = Prefs.GetFloat("DownsamplingLevel", 1f);
            PrefsHelper.outline = Prefs.GetString("Outline", "Single");
            PrefsHelper.maxLights = Prefs.GetInt("MaxLights", 4);
            PrefsHelper.devMode = Prefs.GetInt("DevMode", 0) != 0;
            if (App.IsDebugBuild)
            {
                PrefsHelper.devMode = true;
            }
            PrefsHelper.outline = "Single";
        }

        private static int resolutionWidth;

        private static int resolutionHeight;

        private static float uiScale;

        private static bool fullscreen;

        private static float fov;

        private static bool vsync;

        private static int targetFramerate;

        private static bool showFPS;

        private static float volume;

        private static float musicVolume;

        private static float mouseSensitivity;

        private static bool invertY;

        private static int ssaoQuality;

        private static int shadowQuality;

        private static float downsampling;

        private static string outline;

        private static int maxLights;

        private static bool devMode;

        public const string KeyBindingKeyPrefix = "KeyBinding_";

        private const string ResolutionWidthKey = "ResolutionWidth";

        private const string ResolutionHeightKey = "ResolutionHeight";

        private const string UIScaleKey = "UIScale";

        private const string FullscreenKey = "Fullscreen";

        private const string FOVKey = "FOV";

        private const string VSyncKey = "VSync";

        private const string TargetFramerateKey = "TargetFramerate";

        private const string ShowFPSKey = "ShowFPS";

        private const string VolumeKey = "Volume";

        private const string MusicVolumeKey = "MusicVolume";

        private const string MouseSensitivityKey = "MouseSensitivity";

        private const string InvertYKey = "InvertY";

        private const string SSAOQualityKey = "SSAOQuality";

        private const string ShadowQualityKey = "ShadowQuality";

        private const string DownsamplingLevelKey = "DownsamplingLevel";

        private const string OutlineKey = "Outline";

        private const string MaxLightsKey = "MaxLights";

        private const string DevModeKey = "DevMode";
    }
}