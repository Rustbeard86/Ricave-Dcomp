using System;
using System.Collections.Generic;

namespace Ricave.Core
{
    public class ThisRunLobbyItems
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
                return this.lobbyItems.GetOrDefault(Get.Entity_Stardust, 0);
            }
        }

        public int Diamonds
        {
            get
            {
                return this.lobbyItems.GetOrDefault(Get.Entity_Diamond, 0);
            }
        }

        public void ChangeCount(EntitySpec itemSpec, int offset)
        {
            Instruction.ThrowIfNotExecuting();
            if (itemSpec == null)
            {
                Log.Error("Tried to set null collected lobby item.", false);
                return;
            }
            if (!itemSpec.IsItem || !itemSpec.Item.LobbyItem)
            {
                Log.Error("Tried to add collected lobby item which is not a lobby item.", false);
                return;
            }
            this.lobbyItems.SetOrIncrement(itemSpec, offset);
            if (this.lobbyItems[itemSpec] == 0)
            {
                this.lobbyItems.Remove(itemSpec);
            }
        }

        public int GetCount(EntitySpec itemSpec)
        {
            if (itemSpec == null)
            {
                return 0;
            }
            return this.lobbyItems.GetOrDefault(itemSpec, 0);
        }

        [Saved(Default.New, false)]
        private Dictionary<EntitySpec, int> lobbyItems = new Dictionary<EntitySpec, int>();
    }
}