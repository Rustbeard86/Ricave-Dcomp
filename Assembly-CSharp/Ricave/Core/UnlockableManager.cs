using System;
using System.Collections.Generic;
using Ricave.UI;

namespace Ricave.Core
{
    public class UnlockableManager
    {
        public List<UnlockableSpec> DirectlyUnlocked
        {
            get
            {
                return this.unlocked;
            }
        }

        public bool AnyDirectlyUnlockedSingleUseBoost
        {
            get
            {
                for (int i = 0; i < this.unlocked.Count; i++)
                {
                    if (this.unlocked[i].ResetAfterRun)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public bool IsUnlocked(UnlockableSpec unlockableSpec, int? assumeScoreLevel = null)
        {
            if (unlockableSpec == null)
            {
                return false;
            }
            if (!Get.InMainMenu && !Get.InLobby && Get.RunConfig.IsDailyChallenge)
            {
                return unlockableSpec == Get.Unlockable_ExtraSpear || unlockableSpec == Get.Unlockable_ExtraShield || unlockableSpec == Get.Unlockable_ExtraWatch;
            }
            if (unlockableSpec.AutoUnlockAtScoreLevel != null)
            {
                int num = assumeScoreLevel ?? Get.Progress.ScoreLevel;
                int? autoUnlockAtScoreLevel = unlockableSpec.AutoUnlockAtScoreLevel;
                if ((num >= autoUnlockAtScoreLevel.GetValueOrDefault()) & (autoUnlockAtScoreLevel != null))
                {
                    return true;
                }
            }
            return this.IsDirectlyUnlocked(unlockableSpec);
        }

        public bool IsDirectlyUnlocked(UnlockableSpec unlockableSpec)
        {
            return this.unlocked.Contains(unlockableSpec);
        }

        public void Unlock(UnlockableSpec unlockableSpec)
        {
            if (unlockableSpec == null)
            {
                Log.Error("Tried to unlock null unlockable.", false);
                return;
            }
            if (this.IsDirectlyUnlocked(unlockableSpec))
            {
                Log.Error("Tried to unlock the same unlockable twice.", false);
                return;
            }
            this.unlocked.Add(unlockableSpec);
            this.CheckGiveRewards(unlockableSpec);
            if (unlockableSpec == Get.Unlockable_PrivateRoom && !Get.Achievement_BuyPrivateRoom.IsCompleted)
            {
                Get.Achievement_BuyPrivateRoom.TryComplete();
            }
        }

        public void RemoveFromDirectlyUnlocked(UnlockableSpec unlockableSpec)
        {
            if (unlockableSpec == null)
            {
                Log.Error("Tried to remove null unlockable.", false);
                return;
            }
            if (!this.IsDirectlyUnlocked(unlockableSpec))
            {
                Log.Error("Tried to remove unlockable which isn't directly unlocked.", false);
                return;
            }
            this.unlocked.Remove(unlockableSpec);
        }

        public void Update()
        {
            if (Get.InLobby)
            {
                this.CheckGiveRewards();
            }
            if (Clock.Frame % 3 == 1)
            {
                this.TryShowUnlockedWindow();
            }
        }

        private void CheckGiveRewards()
        {
            List<UnlockableSpec> all = Get.Specs.GetAll<UnlockableSpec>();
            for (int i = 0; i < all.Count; i++)
            {
                this.CheckGiveRewards(all[i]);
            }
        }

        private void CheckGiveRewards(UnlockableSpec unlockableSpec)
        {
            if (!this.IsUnlocked(unlockableSpec, null))
            {
                return;
            }
            if (unlockableSpec.PrivateRoomStructureReward == null)
            {
                return;
            }
            if (this.rewardsGiven.Contains(unlockableSpec))
            {
                return;
            }
            Get.PrivateRoom.ChangeInventoryCount(unlockableSpec.PrivateRoomStructureReward, 1);
            this.rewardsGiven.Add(unlockableSpec);
        }

        private void TryShowUnlockedWindow()
        {
            if (Get.UI.IsEscapeMenuOpen || Get.UI.InventoryOpen || Get.WindowManager.AnyWindowOpen || Get.WheelSelector != null || Get.TextSequenceDrawer.Showing || Get.ScreenFader.Alpha > 0.25f || Root.ChangingScene || Get.ScreenFader.AnyActionQueued || Get.DungeonMapDrawer.Showing)
            {
                return;
            }
            if (Get.RunConfig.IsDailyChallenge)
            {
                return;
            }
            if (!this.notifiedAboutFinishingGame && Get.Progress.FinishedGame)
            {
                this.notifiedAboutFinishingGame = true;
                Get.WindowManager.OpenMessageWindow("FinishedGameText".Translate(Get.Progress.Score.ToStringCached(), MainMenuUI.DiscordServerAddress), "FinishedGameTitle".Translate());
                if (!Get.Achievement_FinishGame.IsCompleted)
                {
                    Get.Achievement_FinishGame.TryComplete();
                }
                return;
            }
            if (!this.notifiedAboutPetRat && Get.RunConfig.HasPetRat)
            {
                this.notifiedAboutPetRat = true;
                ((Window_TutorialPopup)Get.WindowManager.Open(Get.Window_TutorialPopup, true)).SetText(null, "PetRatInfo".Translate());
                return;
            }
            if (!this.notifiedAboutParty && Get.Player.Party.Count >= 2)
            {
                this.notifiedAboutParty = true;
                ((Window_TutorialPopup)Get.WindowManager.Open(Get.Window_TutorialPopup, true)).SetText(null, "PartyInfo".Translate());
                return;
            }
            int num = (Get.DeathScreenDrawer.ShouldShow ? Get.DeathScreenDrawer.ProgressBar.CurAnimationLevel : Get.Progress.ScoreLevel);
            UnlockableSpec unlockableSpec = null;
            List<UnlockableSpec> all = Get.Specs.GetAll<UnlockableSpec>();
            for (int i = 0; i < all.Count; i++)
            {
                if (!all[i].UnlockedText.NullOrEmpty() && !this.notifiedAboutUnlocking.Contains(all[i]) && this.IsUnlocked(all[i], new int?(num)))
                {
                    if (unlockableSpec != null && unlockableSpec.AutoUnlockAtScoreLevel != null && all[i].AutoUnlockAtScoreLevel != null)
                    {
                        int? autoUnlockAtScoreLevel = unlockableSpec.AutoUnlockAtScoreLevel;
                        int? autoUnlockAtScoreLevel2 = all[i].AutoUnlockAtScoreLevel;
                        if ((autoUnlockAtScoreLevel.GetValueOrDefault() < autoUnlockAtScoreLevel2.GetValueOrDefault()) & ((autoUnlockAtScoreLevel != null) & (autoUnlockAtScoreLevel2 != null)))
                        {
                            goto IL_024B;
                        }
                    }
                    unlockableSpec = all[i];
                }
            IL_024B:;
            }
            if (unlockableSpec == null)
            {
                return;
            }
            ((Window_UnlockedUnlockable)Get.WindowManager.Open(Get.Window_UnlockedUnlockable, true)).SetUnlockable(unlockableSpec);
            this.notifiedAboutUnlocking.Add(unlockableSpec);
        }

        [Saved(Default.New, true)]
        private List<UnlockableSpec> unlocked = new List<UnlockableSpec>();

        [Saved(Default.New, true)]
        private HashSet<UnlockableSpec> notifiedAboutUnlocking = new HashSet<UnlockableSpec>();

        [Saved(Default.New, true)]
        private HashSet<UnlockableSpec> rewardsGiven = new HashSet<UnlockableSpec>();

        [Saved]
        private bool notifiedAboutFinishingGame;

        [Saved]
        private bool notifiedAboutPetRat;

        [Saved]
        private bool notifiedAboutParty;
    }
}