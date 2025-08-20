using System;
using Ricave.Core;
using UnityEngine;

namespace Ricave.Rendering
{
    public static class App
    {
        public static bool IsDebugBuild
        {
            get
            {
                return Debug.isDebugBuild;
            }
        }

        public static string Name
        {
            get
            {
                return Application.productName;
            }
        }

        public static string Version
        {
            get
            {
                return Application.version;
            }
        }

        public static bool InEditor
        {
            get
            {
                return Application.isEditor;
            }
        }

        public static string DataPath
        {
            get
            {
                return Application.dataPath;
            }
        }

        public static string PersistentDataPath
        {
            get
            {
                return Application.persistentDataPath;
            }
        }

        public static int FramerateCap
        {
            get
            {
                return Application.targetFrameRate;
            }
            set
            {
                Application.targetFrameRate = value;
            }
        }

        public static bool VSync
        {
            get
            {
                return QualitySettings.vSyncCount == 1;
            }
            set
            {
                QualitySettings.vSyncCount = (value ? 1 : 0);
            }
        }

        public static int MaxLights
        {
            get
            {
                return QualitySettings.pixelLightCount;
            }
            set
            {
                QualitySettings.pixelLightCount = value;
            }
        }

        public static void HideDeveloperConsole()
        {
            Debug.developerConsoleVisible = false;
        }

        public static void Quit()
        {
            Application.Quit();
        }

        public static void SetQualityLevel()
        {
            QualitySettings.SetQualityLevel(6);
        }

        public static void SetShadows_Low()
        {
            QualitySettings.shadows = ShadowQuality.HardOnly;
            QualitySettings.shadowResolution = ShadowResolution.Low;
        }

        public static void SetShadows_Medium()
        {
            QualitySettings.shadows = ShadowQuality.HardOnly;
            QualitySettings.shadowResolution = ShadowResolution.Medium;
        }

        public static void SetShadows_High()
        {
            QualitySettings.shadows = ShadowQuality.All;
            QualitySettings.shadowResolution = ShadowResolution.Medium;
        }

        public static void SetSSAO_Disabled()
        {
            SSAOPro componentInChildren = Camera.main.GetComponentInChildren<SSAOPro>(true);
            if (componentInChildren != null)
            {
                componentInChildren.enabled = false;
            }
        }

        public static void SetSSAO_Low()
        {
            SSAOPro componentInChildren = Camera.main.GetComponentInChildren<SSAOPro>(true);
            if (componentInChildren != null)
            {
                componentInChildren.enabled = true;
                componentInChildren.Samples = SSAOPro.SampleCount.Medium;
                componentInChildren.Downsampling = 3;
            }
        }

        public static void SetSSAO_Medium()
        {
            SSAOPro componentInChildren = Camera.main.GetComponentInChildren<SSAOPro>(true);
            if (componentInChildren != null)
            {
                componentInChildren.enabled = true;
                componentInChildren.Samples = SSAOPro.SampleCount.Medium;
                componentInChildren.Downsampling = 2;
            }
        }

        public static void SetSSAO_High()
        {
            SSAOPro componentInChildren = Camera.main.GetComponentInChildren<SSAOPro>(true);
            if (componentInChildren != null)
            {
                componentInChildren.enabled = true;
                componentInChildren.Samples = SSAOPro.SampleCount.High;
                componentInChildren.Downsampling = 1;
            }
        }

        public static void SetVolume(float volume)
        {
            AudioListener.volume = volume;
        }

        public static void SetDownsampling(float downsampling)
        {
            DownsamplingGOC componentInChildren = Camera.main.GetComponentInChildren<DownsamplingGOC>(true);
            if (componentInChildren != null)
            {
                if (Calc.Approximately(downsampling, 1f))
                {
                    componentInChildren.enabled = false;
                    return;
                }
                componentInChildren.enabled = true;
                componentInChildren.Scale = 1f / downsampling;
            }
        }

        public static readonly bool IsDemo;
    }
}