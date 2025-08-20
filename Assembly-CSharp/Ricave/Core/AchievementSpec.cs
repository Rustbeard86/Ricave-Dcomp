using System;
using Steamworks;
using UnityEngine;

namespace Ricave.Core
{
    public class AchievementSpec : Spec, ISaveableEventsReceiver
    {
        private string SteamAchievementID
        {
            get
            {
                return base.SpecID;
            }
        }

        public bool IsCompleted
        {
            get
            {
                if (!SteamManager.Initialized)
                {
                    return false;
                }
                if (this.completedCached != null)
                {
                    return this.completedCached.Value;
                }
                try
                {
                    this.completedCached = new bool?(false);
                    bool flag;
                    SteamUserStats.GetAchievement(this.SteamAchievementID, out flag);
                    this.completedCached = new bool?(flag);
                }
                catch (Exception ex)
                {
                    Log.Error("Could not check achievement.", ex);
                }
                return this.completedCached.GetValueOrDefault();
            }
        }

        public Texture2D CompletedIcon
        {
            get
            {
                return this.completedIcon;
            }
        }

        public Texture2D LockedIcon
        {
            get
            {
                return this.lockedIcon;
            }
        }

        public void TryComplete()
        {
            if (!SteamManager.Initialized)
            {
                return;
            }
            if (this.IsCompleted)
            {
                return;
            }
            try
            {
                SteamUserStats.SetAchievement(this.SteamAchievementID);
                SteamUserStats.StoreStats();
            }
            catch (Exception ex)
            {
                Log.Error("Could not complete achievement.", ex);
            }
            this.completedCached = null;
        }

        public void OnLoaded()
        {
            if (!this.completedIconPath.NullOrEmpty())
            {
                this.completedIcon = Assets.Get<Texture2D>(this.completedIconPath);
            }
            else
            {
                Log.Error("AchievementSpec " + base.SpecID + " has no completed icon.", false);
            }
            if (!this.lockedIconPath.NullOrEmpty())
            {
                this.lockedIcon = Assets.Get<Texture2D>(this.lockedIconPath);
                return;
            }
            Log.Error("AchievementSpec " + base.SpecID + " has no locked icon.", false);
        }

        public void OnSaved()
        {
        }

        [Saved]
        private string completedIconPath;

        [Saved]
        private string lockedIconPath;

        private Texture2D completedIcon;

        private Texture2D lockedIcon;

        private bool? completedCached;
    }
}