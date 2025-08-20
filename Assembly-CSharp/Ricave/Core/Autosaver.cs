using System;

namespace Ricave.Core
{
    public class Autosaver
    {
        public void Init()
        {
            this.lastTimeSaved = Clock.UnscaledTime;
        }

        public void Update()
        {
            if (Clock.Frame % 10 != 0)
            {
                return;
            }
            if (Root.ChangingScene || Get.ScreenFader.AnyActionQueued || Get.TextSequenceDrawer.Showing || Get.DungeonMapDrawer.Showing || Get.DeathScreenDrawer.ShouldShow)
            {
                return;
            }
            if (((Clock.UnscaledTime - this.lastTimeSaved >= 300f && Get.UI.IsEscapeMenuOpen) || (Clock.UnscaledTime - this.lastTimeSaved >= 720f && Get.UI.InventoryOpen) || Clock.UnscaledTime - this.lastTimeSaved >= 1500f) && Get.TurnManager.IsPlayerTurn_CanChooseNextAction)
            {
                this.Autosave();
            }
        }

        private void Autosave()
        {
            this.lastTimeSaved = Clock.UnscaledTime;
            Get.Run.Save();
            Get.Progress.Save();
        }

        private float lastTimeSaved;

        private const float AutosaveIntervalMinutes_EscapeMenu = 5f;

        private const float AutosaveIntervalMinutes_Inventory = 12f;

        private const float AutosaveIntervalMinutes = 25f;
    }
}