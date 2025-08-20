using System;
using System.Collections.Generic;
using Ricave.UI;

namespace Ricave.Core
{
    public class TotalQuestsState
    {
        public int Get(string key)
        {
            if (key.NullOrEmpty())
            {
                return 0;
            }
            return this.state.GetOrDefault(key, 0);
        }

        public void Apply(ThisRunQuestsState questsState)
        {
            foreach (KeyValuePair<string, int> keyValuePair in questsState.State)
            {
                this.Change(keyValuePair.Key, keyValuePair.Value);
            }
        }

        public void Change(string key, int offset)
        {
            if (key.NullOrEmpty())
            {
                Log.Error("Tried to change quest state for null key.", false);
                return;
            }
            this.state.SetOrIncrement(key, offset);
            StringWithQuestsStateUtility.OnQuestsStateChanged();
        }

        [Saved(Default.New, false)]
        private Dictionary<string, int> state = new Dictionary<string, int>();
    }
}