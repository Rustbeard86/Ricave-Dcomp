using System;
using System.Collections.Generic;
using Ricave.UI;

namespace Ricave.Core
{
    public class ThisRunQuestsState
    {
        public Dictionary<string, int> State
        {
            get
            {
                return this.state;
            }
        }

        public int Get(string key)
        {
            if (key.NullOrEmpty())
            {
                return 0;
            }
            return this.state.GetOrDefault(key, 0);
        }

        public void Change(string key, int offset)
        {
            Instruction.ThrowIfNotExecuting();
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