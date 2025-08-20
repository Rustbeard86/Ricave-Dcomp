using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class TotalLobbyItems
    {
        public Dictionary<EntitySpec, int> LobbyItems
        {
            get
            {
                return this.lobbyItems;
            }
        }

        public int Stardust
        {
            get
            {
                return this.GetCount(Get.Entity_Stardust);
            }
        }

        public int Diamonds
        {
            get
            {
                return this.GetCount(Get.Entity_Diamond);
            }
        }

        public List<ValueTuple<EntitySpec, int>> LastRunLobbyItemsForAnimation
        {
            get
            {
                return this.lastRunLobbyItemsForAnimation;
            }
        }

        public Dictionary<EntitySpec, int> LastRunLobbyItemsAnimationSoundsPlayed
        {
            get
            {
                return this.lastRunLobbyItemsAnimationSoundsPlayed;
            }
        }

        public void ChangeCount(EntitySpec itemSpec, int offset)
        {
            if (itemSpec == null)
            {
                Log.Error("Tried to set null lobby item.", false);
                return;
            }
            if (!itemSpec.IsItem || !itemSpec.Item.LobbyItem)
            {
                Log.Error("Tried to offset lobby item count for an item which is not a lobby item.", false);
                return;
            }
            if (offset == 0)
            {
                return;
            }
            this.lobbyItems.SetOrIncrement(itemSpec, offset);
            if (this.lobbyItems[itemSpec] == 0)
            {
                this.lobbyItems.Remove(itemSpec);
            }
            this.lastCountChangedTime[itemSpec] = Clock.UnscaledTime;
        }

        public int GetCount(EntitySpec itemSpec)
        {
            if (itemSpec == null)
            {
                return 0;
            }
            return this.lobbyItems.GetOrDefault(itemSpec, 0);
        }

        public bool Any(EntitySpec itemSpec)
        {
            return this.GetCount(itemSpec) >= 1;
        }

        public void Apply(ThisRunLobbyItems thisRunLobbyItems)
        {
            this.lastRunLobbyItemsForAnimation.Clear();
            this.lastRunLobbyItemsAnimationSoundsPlayed.Clear();
            foreach (KeyValuePair<EntitySpec, int> keyValuePair in thisRunLobbyItems.LobbyItems)
            {
                this.ChangeCount(keyValuePair.Key, keyValuePair.Value);
                this.lastRunLobbyItemsForAnimation.Add(new ValueTuple<EntitySpec, int>(keyValuePair.Key, keyValuePair.Value));
            }
        }

        public float GetLastCountChangedTime(EntitySpec itemSpec)
        {
            float num;
            if (this.lastCountChangedTime.TryGetValue(itemSpec, out num))
            {
                return num;
            }
            return -99999f;
        }

        public void OnLastRunLobbyItemsAnimationPlayed()
        {
            this.lastRunLobbyItemsForAnimation.Clear();
            this.lastRunLobbyItemsAnimationSoundsPlayed.Clear();
        }

        [Saved(Default.New, false)]
        private Dictionary<EntitySpec, int> lobbyItems = new Dictionary<EntitySpec, int>();

        private Dictionary<EntitySpec, float> lastCountChangedTime = new Dictionary<EntitySpec, float>();

        private List<ValueTuple<EntitySpec, int>> lastRunLobbyItemsForAnimation = new List<ValueTuple<EntitySpec, int>>();

        private Dictionary<EntitySpec, int> lastRunLobbyItemsAnimationSoundsPlayed = new Dictionary<EntitySpec, int>();
    }
}