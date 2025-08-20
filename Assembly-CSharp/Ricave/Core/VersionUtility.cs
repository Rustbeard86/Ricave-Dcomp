using System;
using Ricave.Rendering;

namespace Ricave.Core
{
    public static class VersionUtility
    {
        public static int Major
        {
            get
            {
                return VersionUtility.major;
            }
        }

        public static int Minor
        {
            get
            {
                return VersionUtility.minor;
            }
        }

        public static int Patch
        {
            get
            {
                return VersionUtility.patch;
            }
        }

        public static string CurrentVersionLabel
        {
            get
            {
                if (VersionUtility.currentVersionLabelCached == null)
                {
                    VersionUtility.currentVersionLabelCached = string.Concat(new string[]
                    {
                        VersionUtility.major.ToString(),
                        ".",
                        VersionUtility.minor.ToString(),
                        ".",
                        VersionUtility.patch.ToString()
                    });
                    if (App.IsDebugBuild)
                    {
                        VersionUtility.currentVersionLabelCached += " (debug)";
                    }
                }
                return VersionUtility.currentVersionLabelCached;
            }
        }

        public static string CurrentVersionLabelWithAppName
        {
            get
            {
                if (VersionUtility.currentVersionLabelWithAppNameCached == null)
                {
                    VersionUtility.currentVersionLabelWithAppNameCached = App.Name + " " + VersionUtility.CurrentVersionLabel;
                }
                return VersionUtility.currentVersionLabelWithAppNameCached;
            }
        }

        static VersionUtility()
        {
            string[] array = App.Version.Split('.', StringSplitOptions.None);
            VersionUtility.major = int.Parse(array[0]);
            VersionUtility.minor = int.Parse(array[1]);
            VersionUtility.patch = int.Parse(array[2]);
        }

        private static int major;

        private static int minor;

        private static int patch;

        private static string currentVersionLabelCached;

        private static string currentVersionLabelWithAppNameCached;
    }
}