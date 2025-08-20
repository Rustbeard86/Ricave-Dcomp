using System;
using Ricave.Core;
using Steamworks;
using UnityEngine;

namespace Ricave.UI
{
    public static class VersionUI
    {
        public static void DrawVersion()
        {
            Widgets.FontBold = true;
            Widgets.Align = TextAnchor.LowerLeft;
            Rect rect = new Rect(3f, Widgets.VirtualHeight - 50f - 3f, 700f, 50f);
            Widgets.Label(rect, VersionUtility.CurrentVersionLabelWithAppName, true, null, null, false);
            try
            {
                if (SteamManager.Initialized)
                {
                    if (VersionUI.steamUserName == null)
                    {
                        VersionUI.steamUserName = SteamFriends.GetPersonaName();
                    }
                    if (!VersionUI.steamUserName.NullOrEmpty())
                    {
                        rect.y -= 23f;
                        Widgets.Label(rect, "LoggedInAs".Translate(VersionUI.steamUserName), true, null, null, false);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Could not show logged user name.", ex);
            }
            if (Log.AnyError)
            {
                rect.y -= 23f;
                Widgets.Label(rect, RichText.Error("ApplicationErrorsInfo".Translate()), true, null, null, false);
            }
            Widgets.ResetAlign();
            Widgets.FontBold = false;
        }

        private static string steamUserName;

        private const int Margin = 3;
    }
}