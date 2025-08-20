using System;

namespace Ricave.Core
{
    public class UniqueIDsManager
    {
        public int NextEntityID()
        {
            int num = this.nextEntityID;
            this.nextEntityID = num + 1;
            return num;
        }

        public int NextPlayLogEntryID()
        {
            int num = this.nextPlayLogEntryID;
            this.nextPlayLogEntryID = num + 1;
            return num;
        }

        public int NextPlaceID()
        {
            int num = this.nextPlaceID;
            this.nextPlaceID = num + 1;
            return num;
        }

        [Saved(1, false)]
        private int nextEntityID = 1;

        [Saved(1, false)]
        private int nextPlayLogEntryID = 1;

        [Saved(1, false)]
        private int nextPlaceID = 1;
    }
}