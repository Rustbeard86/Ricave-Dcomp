using System;

namespace Ricave.Core
{
    public class StringSlot
    {
        public string String
        {
            get
            {
                return this.str;
            }
        }

        public bool Has
        {
            get
            {
                return this.str != null;
            }
        }

        public void Set(string str)
        {
            if (str == null)
            {
                this.Clear();
                return;
            }
            if (this.str != null)
            {
                Log.Warning("Tried add string to StringSlot twice.", false);
                return;
            }
            this.str = str;
        }

        public void Clear()
        {
            this.str = null;
        }

        private string str;
    }
}